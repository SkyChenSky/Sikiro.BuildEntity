using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using 陈珙.AutoBuildEntity.Common.Extension;
using 陈珙.AutoBuildEntity.Common.Helper;
using 陈珙.AutoBuildEntity.Model;
using MessageBox = System.Windows.MessageBox;

namespace 陈珙.AutoBuildEntity.Form
{
    public partial class MainForm : Window
    {
        #region 窗体初始化
        private readonly AutoBuildEntityContent _autoBuildEntityContent;

        private readonly List<string> _hadAddCheckSelectList = new List<string>();

        private readonly List<string> _noAddCheckSelectList = new List<string>();

        private readonly List<string> _noExistCheckSelectList = new List<string>();
        public MainForm(AutoBuildEntityContent autoBuildEntityContent)
        {
            InitializeComponent();
            _autoBuildEntityContent = autoBuildEntityContent;
        }
        #endregion

        #region 加载列表
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //加载列表
            var noAddList =
                _autoBuildEntityContent.TablesName.Where(
                    a => !_autoBuildEntityContent.SelectedProject.CsFilesName.Contains(a))
                    .Select(a => new ListViewItem { Name = a });

            var hadAddList =
                _autoBuildEntityContent.TablesName.Where(
                    a => _autoBuildEntityContent.SelectedProject.CsFilesName.Contains(a))
                    .Select(a => new ListViewItem { Name = a });

            var noExistList =
                _autoBuildEntityContent.SelectedProject.CsFilesName.Where(
                    a => !_autoBuildEntityContent.TablesName.Contains(a))
                    .Select(a => new ListViewItem { Name = a });

            NoAddListView.ItemsSource = noAddList;
            HadAddListView.ItemsSource = hadAddList;
            NoExistListView.ItemsSource = noExistList;
        }
        #endregion

        #region 确认提交事件
        /// <summary>
        /// 确认提交事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitEvent(object sender, RoutedEventArgs e)
        {
            var theSelectedProject = _autoBuildEntityContent.SelectedProject;
            try
            {
                //获取物理表名
                var addAndUpdateList = _hadAddCheckSelectList.Union(_noAddCheckSelectList).ToList();
                var removeFiles = _noExistCheckSelectList.Select(a => a + ".cs").ToList();

                //查询出表结构
                var dbTable = new DbTable(_autoBuildEntityContent.EntityXml.ConnString);
                var dbtables = dbTable.GetTables(addAndUpdateList);

                //根据模版输出
                var templateModel =
                    dbtables.Select(
                        a =>
                            new TemplateModel(a.TableName, a.Columns,
                                theSelectedProject.ProjectName)).ToList();

                var templateDic = templateModel.ToDictionary(a => a.TableName,
                    item =>
                        NVelocityHelper.ProcessTemplate(_autoBuildEntityContent.EntityXml.EntityTemplate,
                            new Dictionary<string, object> { { "entity", item } }));

                //保存文件
                var addfiles =
                    templateDic.Select(
                        templateData =>
                            FilesHelper.Write(theSelectedProject.ProjectDirectoryName,
                                templateData.Key, templateData.Value)).ToList();

                //添加项目项和排除项目项、
                theSelectedProject.ProjectDte.AddFilesToProject(addfiles);
                theSelectedProject.ProjectDte.RemoveFilesFromProject(removeFiles);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 全选
        private void HadAddSelectAll_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var checkBoxList = FindVisualChild<CheckBox>(HadAddListView);
            checkBoxList.ForEach(item =>
            {
                item.IsChecked = cb.IsChecked;
            });

        }

        private void NoAddSelectAll_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var checkBoxList = FindVisualChild<CheckBox>(NoAddListView);
            checkBoxList.ForEach(item =>
            {
                item.IsChecked = cb.IsChecked;
            });

        }

        private void NoExistSelectAll_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var checkBoxList = FindVisualChild<CheckBox>(NoExistListView);
            checkBoxList.ForEach(item =>
            {
                item.IsChecked = cb.IsChecked;
            });

        }

        private List<T> FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            List<T> list = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var item = child as T;
                if (item != null)
                {
                    list.Add(item);
                }
                else
                {
                    var childOfChildren = FindVisualChild<T>(child);
                    if (childOfChildren != null)
                    {
                        list.AddRange(childOfChildren);
                    }
                }
            }
            return list;
        }
        #endregion

        #region CheckBox选中事件
        private void TemplateHadAddCheckBox_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var tbName = cb.Tag.ToString();
            if (cb.IsChecked == true)
            {
                _hadAddCheckSelectList.Add(tbName);
            }
            else
            {
                _hadAddCheckSelectList.Remove(tbName);
            }
        }

        private void TemplateNoAddCheckBox_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var tbName = cb.Tag.ToString();
            if (cb.IsChecked == true)
            {
                _noAddCheckSelectList.Add(tbName);
            }
            else
            {
                _noAddCheckSelectList.Remove(tbName);
            }
        }

        private void TemplateNoExistCheckBox_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var tbName = cb.Tag.ToString();
            if (cb.IsChecked == true)
            {
                _noExistCheckSelectList.Add(tbName);
            }
            else
            {
                _noExistCheckSelectList.Remove(tbName);
            }
        }
        #endregion
    }

    public class ListViewItem
    {
        public string Name { get; set; }
    }
}
