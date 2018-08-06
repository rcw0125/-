using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// KeyboardConfig.xaml 的交互逻辑
    /// </summary>
    public partial class KeyboardConfig : UserControl
    {
        #region 属性
        /// <summary>
        /// 当前配置文件名称
        /// </summary>
        public string curConfigFileName { get; set; }
        /// <summary>
        /// 当前配置文件对象
        /// </summary>
        private configlist curConfig;
        /// <summary>
        /// 当前操作的子模块对象
        /// </summary>
        private submodule curSubModule;

        private List<ComboxModel> availableInList;
        /// <summary>
        /// "有效范围"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> AvailableInList
        {
            get { return availableInList; }
            set { availableInList = value; }
        }

        #endregion

        /// <summary>
        /// 窗体构造函数
        /// </summary>
        /// <param name="curConfig">当前配置文件对象</param>
        /// <param name="curSubModule">当前配置文件的子模块对象</param>
        /// <param name="configFileName">配置文件名称</param>
        public KeyboardConfig(configlist curConfig, submodule curSubModule, string configFileName)
        {
            InitializeComponent();
            this.curConfig = curConfig;
            this.curSubModule = curSubModule;
            this.curConfigFileName = configFileName;
        }

        /// <summary>
        /// 窗体加载时触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Window_Loaded;
            SetFormControls();
        }

        /// <summary>
        /// 设置窗体中的各控件值
        /// </summary>
        private void SetFormControls()
        {
            if (curSubModule != null)
            {
                SetHostComport();
                SetHostBaudrate();
                SetHostIsUsed();
                SetAuxiliaryComport();
                SetAuxiliaryBaudrate();
                SetAuxiliaryIsUsed();
                SetIsStandardBoard();
                SetGridView();
            }
            else
            {
                this.HostComportComBox.SelectedIndex = 0;
                this.HostBaudrateComBox.SelectedIndex = 0;
                this.HostIsUsedComBox.SelectedIndex = 0;
                this.AuxiliaryComportComBox.SelectedIndex = 0;
                this.AuxiliaryBaudrateComBox.SelectedIndex = 0;
                this.AuxiliaryIsUsedComBox.SelectedIndex = 0;
                this.IsStandardBoardComBox.SelectedIndex = 0;
                this.KeyboardConfigDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// 设置gridview值
        /// </summary>
        private void SetGridView()
        {
            IList<KeyboardModel> models = new List<KeyboardModel>();
            if (curSubModule.GridRow != null)
            {
                foreach (var row in curSubModule.GridRow.RowList)
                {
                    KeyboardModel model = new KeyboardModel();
                    foreach (var item in row.Params)
                    {
                        do
                        {
                            if (KeyboardConfigParam.Row_AvailableIn.Equals(item.Name))
                            {
                                model.AvailableIn = item.Value;
                                model.AvailableInList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                if (this.AvailableInList == null || this.AvailableInList.Count == 0)//设置行中"有效范围"数据源
                                {
                                    this.AvailableInList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                }
                                break;
                            }
                            if (KeyboardConfigParam.Row_KeyName.Equals(item.Name))
                            {
                                model.KeyName = item.Value;
                                break;
                            }
                            if (KeyboardConfigParam.Row_KeyValue.Equals(item.Name))
                            {
                                model.KeyValue = item.Value;
                                break;
                            }
                            if (KeyboardConfigParam.Row_KeyCode.Equals(item.Name))
                            {
                                model.KeyCode = item.Value;
                                break;
                            }
                        } while (false);
                    }
                    models.Add(model);
                }
            }
            this.KeyboardConfigDataGrid.ItemsSource = models;
            this.KeyboardConfigDataGrid.Items.Refresh();
        }
        /// <summary>
        /// 设置是否标准键盘
        /// </summary>
        private void SetIsStandardBoard()
        {
            SetComboxDs(this.IsStandardBoardComBox, KeyboardConfigParam.IsStandardBoard);
        }

        /// <summary>
        /// 设置辅机是否启用
        /// </summary>
        private void SetAuxiliaryIsUsed()
        {
            SetComboxDs(this.AuxiliaryIsUsedComBox, KeyboardConfigParam.AuxiliaryIsUse);
        }
        /// <summary>
        ///设置辅机波特率
        /// </summary>
        private void SetAuxiliaryBaudrate()
        {
            SetComboxDs(this.AuxiliaryBaudrateComBox, KeyboardConfigParam.AuxiliaryBaudrate);
        }
        /// <summary>
        /// 设置辅机串口
        /// </summary>
        private void SetAuxiliaryComport()
        {
            SetComboxDs(this.AuxiliaryComportComBox, KeyboardConfigParam.AuxiliaryComport);
        }

        /// <summary>
        /// 设置主机是否启用
        /// </summary>
        private void SetHostIsUsed()
        {
            SetComboxDs(this.HostIsUsedComBox, KeyboardConfigParam.HostIsUse);
        }

        /// <summary>
        /// 设置主机波特率
        /// </summary>
        private void SetHostBaudrate()
        {
            SetComboxDs(this.HostBaudrateComBox, KeyboardConfigParam.HostBaudrate);
        }

        /// <summary>
        /// 设置主机串口
        /// </summary>
        private void SetHostComport()
        {
            SetComboxDs(this.HostComportComBox, KeyboardConfigParam.HostComport);
        }

        /// <summary>
        /// 设置comboBox数据源及选中的值
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="xmlNodeName"></param>
        private void SetComboxDs(ComboBox cb, string xmlNodeName)
        {
            cb.Items.Clear();
            var list = (from r in curSubModule.Params where r.Name == xmlNodeName select r).ToList();
            if (list.Count > 0)
            {
                foreach (var item in list.First().List)
                {
                    ComboBoxItem item1 = new ComboBoxItem() { Content = item };
                    cb.Items.Add(item1);
                }
                for (int i = 0; i < list.First().List.Count; i++)
                {
                    if (list.First().List[i].Contains(list.First().Value))
                    {
                        cb.SelectedItem = cb.Items[i];
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 新增按钮点击事件
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //获取配置
            IList<KeyboardModel> models = this.KeyboardConfigDataGrid.ItemsSource as IList<KeyboardModel>;
            KeyboardModel model = new KeyboardModel()
            {
                AvailableInList = this.AvailableInList
            };
            model.AvailableIn = (model.AvailableInList != null && model.AvailableInList.Count > 0) ? model.AvailableInList.First().Name : string.Empty;
            models.Add(model);
            this.KeyboardConfigDataGrid.ItemsSource = models;
            this.KeyboardConfigDataGrid.Items.Refresh();
        }

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void Detete_Click(object sender, RoutedEventArgs e)
        {
            if (this.KeyboardConfigDataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条按键配置信息");
            }
            else
            {
                List<KeyboardModel> list = this.KeyboardConfigDataGrid.ItemsSource as List<KeyboardModel>;
                KeyboardModel removeModel = this.KeyboardConfigDataGrid.SelectedItem as KeyboardModel;
                list.Remove(removeModel);
                this.KeyboardConfigDataGrid.ItemsSource = list;
                this.KeyboardConfigDataGrid.Items.Refresh();
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (curSubModule == null)
            {
                return;
            }
            //获取主机串口
            var hostComportList = (from r in curSubModule.Params where r.Name == KeyboardConfigParam.HostComport select r).ToList();
            if (hostComportList.Count > 0)
            {
                var hostComport = this.HostComportComBox.SelectedItem as ComboBoxItem;
                hostComportList.First().Value = hostComport.Content.ToString();
            }
            //获取主机波特率
            var hostBaudrateList = (from r in curSubModule.Params where r.Name == KeyboardConfigParam.HostBaudrate select r).ToList();
            if (hostBaudrateList.Count > 0)
            {
                var hostBaudrate = this.HostBaudrateComBox.SelectedItem as ComboBoxItem;
                hostBaudrateList.First().Value = hostBaudrate.Content.ToString();
            }
            //获取主机是否启用
            var hostIsUsedList = (from r in curSubModule.Params where r.Name == KeyboardConfigParam.HostIsUse select r).ToList();
            if (hostIsUsedList.Count > 0)
            {
                var hostIsUsed = this.HostIsUsedComBox.SelectedItem as ComboBoxItem;
                hostIsUsedList.First().Value = hostIsUsed.Content.ToString();
            }
            //获取辅机串口
            var auxiliaryComportList = (from r in curSubModule.Params where r.Name == KeyboardConfigParam.AuxiliaryComport select r).ToList();
            if (auxiliaryComportList.Count > 0)
            {
                var auxiliaryComport = this.AuxiliaryComportComBox.SelectedItem as ComboBoxItem;
                auxiliaryComportList.First().Value = auxiliaryComport.Content.ToString();
            }
            //获取辅机波特率
            var auxiliaryBaudrateList = (from r in curSubModule.Params where r.Name == KeyboardConfigParam.AuxiliaryBaudrate select r).ToList();
            if (auxiliaryBaudrateList.Count > 0)
            {
                var auxiliaryBaudrate = this.AuxiliaryBaudrateComBox.SelectedItem as ComboBoxItem;
                auxiliaryBaudrateList.First().Value = auxiliaryBaudrate.Content.ToString();
            }
            //获取辅机是否启用
            var auxiliaryIsUsedList = (from r in curSubModule.Params where r.Name == KeyboardConfigParam.AuxiliaryIsUse select r).ToList();
            if (auxiliaryIsUsedList.Count > 0)
            {
                var auxiliaryIsUsed = this.AuxiliaryIsUsedComBox.SelectedItem as ComboBoxItem;
                auxiliaryIsUsedList.First().Value = auxiliaryIsUsed.Content.ToString();
            }
            //获取是否标准键盘
            var isStandardBoardList = (from r in curSubModule.Params where r.Name == KeyboardConfigParam.IsStandardBoard select r).ToList();
            if (isStandardBoardList.Count > 0)
            {
                var isStandardBoard = this.IsStandardBoardComBox.SelectedItem as ComboBoxItem;
                isStandardBoardList.First().Value = isStandardBoard.Content.ToString();
            }
            //获取按键配置
            IList<KeyboardModel> models = this.KeyboardConfigDataGrid.ItemsSource as IList<KeyboardModel>;
            //键标识不可重复验证
            foreach (var item in models)
            {
                var keyCodes = (from r in models where r.KeyCode.Trim() == item.KeyCode.Trim() select r).ToList();
                if (keyCodes != null && keyCodes.Count > 1)
                {
                    int firstKey = models.IndexOf(keyCodes.First());
                    int secondKey = models.IndexOf(keyCodes[1]);
                    MessageBox.Show("第" + (firstKey + 1) + "条的标识码与第" + (secondKey + 1) + "条的标识码重复,保存失败!");
                    return;
                }
            }
            curSubModule.GridRow.RowList.Clear();
            foreach (var item in models)
            {
                Row row = new Row() { Params = new List<Param>() };
                Param availableIn = new Param()
                {
                    Name = KeyboardConfigParam.Row_AvailableIn,
                    Value = item.AvailableIn,
                    List = (from r in item.AvailableInList select r.Name).ToList()
                };
                Param keyName = new Param()
                {
                    Name = KeyboardConfigParam.Row_KeyName,
                    Value = item.KeyName
                };
                Param keyValue = new Param()
                {
                    Name = KeyboardConfigParam.Row_KeyValue,
                    Value = item.KeyValue
                };
                Param keyCode = new Param()
                {
                    Name = KeyboardConfigParam.Row_KeyCode,
                    Value = item.KeyCode
                };
                row.Params.Add(keyName);
                row.Params.Add(keyValue);
                row.Params.Add(keyCode);
                row.Params.Add(availableIn);
                curSubModule.GridRow.RowList.Add(row);
            }
            if (XmlHelper.WriteXmlFile<configlist>(curConfigFileName, curConfig))
            {
                MessageBox.Show("保存成功");
               // ConfigReader.ReLoadConfig();
            }
        }

        /// <summary>
        /// 测试按钮点击事件
        /// </summary>
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.KeyboardConfigDataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条按键配置信息");
            }
            else
            {
                
            }
        }
    }
}
