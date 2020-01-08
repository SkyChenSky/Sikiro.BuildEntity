using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using 陈珙.AutoBuildEntity.Common.Extension;
using 陈珙.AutoBuildEntity.Form;
using 陈珙.AutoBuildEntity.Model;

namespace 陈珙.AutoBuildEntity
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidAutoBuildEntityPkgString)]
    public sealed class AutoBuildEntityPackage : Package
    {
        #region 初始化
        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                var menuCommandId = new CommandID(GuidList.guidAutoBuildEntityCmdSet, (int)PkgCmdIDList.AutoBuildEntityCommandId);

                var menuItem = new OleMenuCommand(AutoBuildEntityEvent, menuCommandId);
                mcs.AddCommand(menuItem);
            }
        }
        #endregion

        /// <summary>
        /// 按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoBuildEntityEvent(object sender, EventArgs e)
        {
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));

            //获取选中项目信息
            var autoBuildEntityContent = new AutoBuildEntityContent { SelectedProject = GetSelectedProject() };
            if (autoBuildEntityContent.SelectedProject == null)
            {
                uiShell.ShowMessageBox("获取项目信息失败");
                return;
            }

            //读取选中项目下的配置信息
            var entityXmlModel = new EntityXml(autoBuildEntityContent.SelectedProject.EntityXmlPath);
            entityXmlModel.Load();
            autoBuildEntityContent.EntityXml = entityXmlModel;

            try
            {
                //读取表集合
                autoBuildEntityContent.TablesName = GetTables(entityXmlModel.ConnString, entityXmlModel.Type);
            }
            catch (Exception ex)
            {
                uiShell.ShowMessageBox($"数据库访问异常:{ex.Message}");
                return;
            }

            new MainForm(autoBuildEntityContent, entityXmlModel.Type).ShowDialog();
        }

        /// <summary>
        /// 获取选中的项目所有信息
        /// </summary>
        /// <returns></returns>
        private SelectedProject GetSelectedProject()
        {
            var dte = (DTE)GetService(typeof(SDTE));

            //获取选中项目信息
            var projectInfo = dte.GetSelectedProjectInfo();

            return projectInfo;
        }

        /// <summary>
        /// 获取物理表
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        private List<string> GetTables(string sqlstr, string sqlType)
        {
            var dbTable = new DbTable(sqlstr);

            switch (sqlType)
            {
                case "mysql":
                    return dbTable.QueryMysqlTablesName();
                case "mssql":
                    return dbTable.QueryMssqlTablesName();
                default: throw new Exception("未知类型");
            }
        }
    }
}
