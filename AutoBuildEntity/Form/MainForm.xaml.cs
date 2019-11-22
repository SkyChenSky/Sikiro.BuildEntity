using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private List<string> _hadAddCheckSelectList = new List<string>();

        private List<string> _noAddCheckSelectList = new List<string>();

        private List<string> _noExistCheckSelectList = new List<string>();

        private IEnumerable<ListViewItem> _noAddList;

        private IEnumerable<ListViewItem> _hadAddList;

        private IEnumerable<ListViewItem> _noExistList;

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
            _noAddList =
                _autoBuildEntityContent.TablesName.Where(
                    a => !_autoBuildEntityContent.SelectedProject.CsFilesName.Contains(a.ToCaseCamelName()))
                    .Select(a => new ListViewItem(a)).ToList();

            _hadAddList =
                _autoBuildEntityContent.TablesName.Where(
                    a => _autoBuildEntityContent.SelectedProject.CsFilesName.Contains(a.ToCaseCamelName()))
                    .Select(a => new ListViewItem(a)).ToList();

            var classList = _autoBuildEntityContent.TablesName.Select(a => a.ToCaseCamelName()).ToList();
            _noExistList =
                _autoBuildEntityContent.SelectedProject.CsFilesName.Where(
                    a => !classList.Contains(a))
                    .Select(a => new ListViewItem(a)).ToList();

            NoAddListView.ItemsSource = _noAddList;
            HadAddListView.ItemsSource = _hadAddList;
            NoExistListView.ItemsSource = _noExistList;
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
                var removeFiles = _noExistCheckSelectList.Select(a => a.ToCaseCamelName() + ".cs").ToList();

                //查询出表结构
                var dbTable = new DbTable(_autoBuildEntityContent.EntityXml.ConnString);
                var dbtables = dbTable.GetTables(addAndUpdateList);

                //根据模版输出
                var templateModel =
                    dbtables.Select(
                        a =>
                            new TemplateModel(a.TableName, a.Columns,
                                theSelectedProject.ProjectName)).ToList();

                var templateDic = templateModel.ToDictionary(a => a.ClassName,
                    item =>
                        NVelocityHelper.ProcessTemplate(_autoBuildEntityContent.EntityXml.EntityTemplate,
                            new Dictionary<string, object> { { "entity", item } }));

                //保存文件
                foreach (var templateData in templateDic)
                {
                    var path = FilesHelper.WriteAndSave(theSelectedProject.ProjectDirectoryName,
                        templateData.Key, templateData.Value);

                    if (_noAddCheckSelectList.Contains(templateData.Key))
                        theSelectedProject.ProjectDte.ProjectItems.AddFromFile(path);
                }

                //添加项目项和排除项目项
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

            var hlv = (List<ListViewItem>)HadAddListView.ItemsSource;
            hlv.ForEach(item =>
            {
                item.IsChecked = cb?.IsChecked ?? false;
            });

            _hadAddCheckSelectList = hlv.Where(a => !string.IsNullOrEmpty(a.Name) && a.IsChecked).Select(a => a.Name?.ToString()).ToList();
        }

        private void NoAddSelectAll_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            var hlv = (List<ListViewItem>)NoAddListView.ItemsSource;
            hlv.ForEach(item =>
            {
                item.IsChecked = cb?.IsChecked ?? false;
            });

            _noAddCheckSelectList = hlv.Where(a => !string.IsNullOrEmpty(a.Name) && a.IsChecked).Select(a => a.Name?.ToString()).ToList();
        }

        private void NoExistSelectAll_ClickEvent(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            var hlv = (List<ListViewItem>)NoExistListView.ItemsSource;
            hlv.ForEach(item =>
            {
                item.IsChecked = cb?.IsChecked ?? false;
            });

            _noExistCheckSelectList = hlv.Where(a => !string.IsNullOrEmpty(a.Name) && a.IsChecked).Select(a => a.Name?.ToString()).ToList();
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

        #region 搜索过滤
        private void addedSearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            FilterList(addedSearchBox, HadAddListView, _hadAddList);
        }

        private void unAddSearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            FilterList(unAddSearchBox, NoAddListView, _noAddList);
        }

        private void unExistSearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            FilterList(unExistSearchBox, NoExistListView, _noExistList);
        }

        private void FilterList(TextBox tb, ItemsControl clb, IEnumerable<ListViewItem> data)
        {
            var selectInput = tb.Text.ToLower();
            var resultList = data.Where(a => a.Name.ToLower().StartsWith(selectInput));
            clb.ItemsSource = resultList;
        }
        #endregion
    }


    public class ListViewItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string _name;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public ListViewItem(string name)
        {
            Name = name;
            IsChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propName)
        {
            var eh = PropertyChanged;
            eh?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
