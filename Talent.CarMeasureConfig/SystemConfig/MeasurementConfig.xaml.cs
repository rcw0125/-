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
using System.Windows.Shapes;
using Talent.Audio.Controller;
using Talent.Measure.DomainModel.CommonModel;
using Talent.Weight.Controller;
using Talent.Weight.Interface;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// MeasurementConfig.xaml 的交互逻辑
    /// </summary>
    public partial class MeasurementConfig : UserControl
    {
        AudioController curAudioController;
        /// <summary>
        /// 当前配置文件序列化后的对象
        /// </summary>
        /// <summary>
        /// 衡器封装实例
        /// </summary>
        WeightManager curWeightManager;
        private configlist curConfig;
        /// <summary>
        /// 当前模块节点对象
        /// </summary>
        private submodule curmodel;
        /// <summary>
        /// 保存按钮控件的名称
        /// </summary>
        private string SaveButtonName = "saveButton";
        /// <summary>
        /// xml文件名称
        /// </summary>
        private string xmlFileName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curConfig">当前配置文件序列化后的对象</param>
        /// <param name="subModel">当前模块节点对象</param>
        /// <param name="fileName">xml文件名称</param>
        public MeasurementConfig(configlist curConfig, submodule subModel, string fileName)
        {
            InitializeComponent();
            this.curConfig = curConfig;
            this.curmodel = subModel;
            xmlFileName = fileName;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Window_Loaded;
            if (this.curmodel.Params.Count > 13)
            {
                InitDoubleColumnForm();
            }
            else
            {
                InitSingleColumnForm();
            }
            InitTestPartControls();
        }

        #region 方法
        /// <summary>
        /// 构造布局为单列的窗体控件
        /// </summary>
        private void InitSingleColumnForm()
        {
            foreach (var param in this.curmodel.Params)
            {
                int index = this.curmodel.Params.IndexOf(param);
                //新增Label
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
                Label lable1 = new Label() { Name = "lable" + index, Content = param.Lab + ":" };
                //lable1.Width = 120;
                lable1.Height = 25;
                lable1.HorizontalAlignment = HorizontalAlignment.Right;
                lable1.VerticalAlignment = VerticalAlignment.Stretch;
                lable1.HorizontalContentAlignment = HorizontalAlignment.Right;
                lable1.SetValue(Grid.RowProperty, index);
                lable1.SetValue(Grid.ColumnProperty, 0);
                MainGrid.Children.Add(lable1);

                //新增textbox或者comboBox
                Control control;
                if (param.List != null && param.List.Count > 0)
                {
                    control = new ComboBox()
                    {
                        Name = param.Name
                    };
                    foreach (var item in param.List)
                    {
                        ComboBoxItem item1 = new ComboBoxItem() { Content = item };
                        ((ComboBox)control).Items.Add(item1);
                    }
                    for (int i = 0; i < param.List.Count; i++)
                    {
                        if (param.List[i].Contains(param.Value))
                        {
                            ((ComboBox)control).SelectedItem = ((ComboBox)control).Items[i];
                            break;
                        }
                    }
                }
                else if (param.Lab.IndexOf("密码") >= 0)
                {
                    control = new PasswordBox() { Name = param.Name, Password = param.Value };
                    // ((PasswordBox)control).TextWrapping = TextWrapping.NoWrap;
                    ((PasswordBox)control).MaxLength = string.IsNullOrEmpty(param.Size) ? IoConfigParam.TextBox_MaxLenght : Int32.Parse(param.Size);
                }
                else
                {
                    control = new TextBox() { Name = param.Name, Text = param.Value };
                    ((TextBox)control).TextWrapping = TextWrapping.NoWrap;
                    ((TextBox)control).MaxLength = string.IsNullOrEmpty(param.Size) ? IoConfigParam.TextBox_MaxLenght : Int32.Parse(param.Size);
                }
                control.Width = 250;
                control.Height = 25;
                control.VerticalAlignment = VerticalAlignment.Stretch;
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.SetValue(Grid.RowProperty, index);
                control.SetValue(Grid.ColumnProperty, 1);
                MainGrid.Children.Add(control);
            }
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            Button saveButton = new Button() { Name = SaveButtonName, Content = "保存" };
            saveButton.Width = 50;
            saveButton.Height = 25;
            saveButton.HorizontalAlignment = HorizontalAlignment.Left;
            saveButton.VerticalAlignment = VerticalAlignment.Stretch;
            saveButton.SetValue(Grid.RowProperty, this.curmodel.Params.Count);
            saveButton.SetValue(Grid.ColumnProperty, 1);
            saveButton.Click += new RoutedEventHandler(SaveMethod);
            MainGrid.Children.Add(saveButton);
        }

        /// <summary>
        /// 构造布局为双列的窗体控件
        /// </summary>
        private void InitDoubleColumnForm()
        {
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
            MainGrid.ColumnDefinitions[0].Width = new GridLength(100, GridUnitType.Pixel);
            MainGrid.ColumnDefinitions[1].Width = new GridLength(200, GridUnitType.Pixel);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(100, GridUnitType.Pixel);
            MainGrid.ColumnDefinitions[3].Width = new GridLength(200, GridUnitType.Pixel);
            int columnNum = 0;
            foreach (var param in this.curmodel.Params)
            {
                int index = this.curmodel.Params.IndexOf(param);
                if (index == 0)
                {
                    MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
                }
                if (index % 2 == 0 && index != 0)//行号为偶数时加行
                {
                    MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
                }
                //新增Label
                Label lable1 = new Label() { Name = "lable" + index, Content = param.Lab + ":" };
                //lable1.Width = 120;
                lable1.Height = 25;
                lable1.HorizontalAlignment = HorizontalAlignment.Right;
                lable1.VerticalAlignment = VerticalAlignment.Stretch;
                lable1.HorizontalContentAlignment = HorizontalAlignment.Right;
                lable1.SetValue(Grid.RowProperty, index / 2);
                lable1.SetValue(Grid.ColumnProperty, columnNum);
                MainGrid.Children.Add(lable1);
                columnNum = columnNum + 1;
                //新增textbox或者comboBox
                Control control;
                if (param.List != null && param.List.Count > 0)
                {
                    control = new ComboBox()
                    {
                        Name = param.Name
                    };
                    foreach (var item in param.List)
                    {
                        ComboBoxItem item1 = new ComboBoxItem() { Content = item };
                        ((ComboBox)control).Items.Add(item1);
                    }
                    for (int i = 0; i < param.List.Count; i++)
                    {
                        if (param.List[i].Contains(param.Value))
                        {
                            ((ComboBox)control).SelectedItem = ((ComboBox)control).Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    control = new TextBox() { Name = param.Name, Text = param.Value };
                    ((TextBox)control).TextWrapping = TextWrapping.NoWrap;
                    ((TextBox)control).MaxLength = string.IsNullOrEmpty(param.Size) ? IoConfigParam.TextBox_MaxLenght : Int32.Parse(param.Size);
                }
                control.Width = 200;
                control.Height = 25;
                control.VerticalAlignment = VerticalAlignment.Stretch;
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.SetValue(Grid.RowProperty, index / 2);
                control.SetValue(Grid.ColumnProperty, columnNum);
                MainGrid.Children.Add(control);
                if (columnNum == 3)
                {
                    columnNum = 0;
                }
                else
                {
                    columnNum = columnNum + 1;
                }
            }
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            Button saveButton = new Button() { Name = SaveButtonName, Content = "保存" };
            saveButton.Width = 50;
            saveButton.Height = 25;
            saveButton.HorizontalAlignment = HorizontalAlignment.Left;
            saveButton.VerticalAlignment = VerticalAlignment.Stretch;
            saveButton.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);//this.curmodel.Params.Count + 1
            saveButton.SetValue(Grid.ColumnProperty, 1);
            saveButton.Click += new RoutedEventHandler(SaveMethod);
            MainGrid.Children.Add(saveButton);
        }

        /// <summary>
        /// 构造衡器测试控件
        /// </summary>
        private void InitWeighterConfigTestControls()
        {
            //添加测试按钮
            Button testButton = new Button() { Name = "TestButton", Content = "测试" };
            testButton.Margin = new Thickness() { Left = 60 };
            testButton.Width = 50;
            testButton.Height = 25;
            testButton.HorizontalAlignment = HorizontalAlignment.Left;
            testButton.VerticalAlignment = VerticalAlignment.Stretch;
            testButton.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);
            testButton.SetValue(Grid.ColumnSpanProperty, 3);
            testButton.SetValue(Grid.ColumnProperty, 1);
            testButton.Click += new RoutedEventHandler(TestMethod);
            MainGrid.Children.Add(testButton);

            Button closeButton = new Button() { Name = "CloseButton", Content = "关闭串口" };
            closeButton.Margin = new Thickness() { Left = 120 };
            closeButton.Width = 50;
            closeButton.Height = 25;
            closeButton.HorizontalAlignment = HorizontalAlignment.Left;
            closeButton.VerticalAlignment = VerticalAlignment.Stretch;
            closeButton.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);
            closeButton.SetValue(Grid.ColumnSpanProperty, 3);
            closeButton.SetValue(Grid.ColumnProperty, 1);
            closeButton.Click += new RoutedEventHandler(CloseMethod);
            MainGrid.Children.Add(closeButton);

            //添加重置称按钮
            Button resetButton = new Button() { Name = "ResetWeightButton", Content = "秤清零" };
            resetButton.Margin = new Thickness() { Left = 180 };
            resetButton.Width = 50;
            resetButton.Height = 25;
            resetButton.HorizontalAlignment = HorizontalAlignment.Left;
            resetButton.VerticalAlignment = VerticalAlignment.Stretch;
            resetButton.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);
            resetButton.SetValue(Grid.ColumnSpanProperty, 3);
            resetButton.SetValue(Grid.ColumnProperty, 1);
            resetButton.Click += new RoutedEventHandler(ResetWeighterMethod);
            MainGrid.Children.Add(resetButton);

            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            //新增原始值Label
            Label weightLable = CreateNewLablel(25, "WeightLable", "重量:", MainGrid.RowDefinitions.Count - 1, 0);
            TextBox weightTextBox = CreateTextBox(250, 100, "weightTextBox", "", MainGrid.RowDefinitions.Count - 1, 1);
            weightTextBox.IsReadOnly = true;
            weightTextBox.TextWrapping = TextWrapping.Wrap;
            weightTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            MainGrid.Children.Add(weightLable);
            MainGrid.Children.Add(weightTextBox);
        }

        /// <summary>
        /// 创建Label
        /// </summary>
        /// <param name="height">高度</param>
        /// <param name="name">名称</param>
        /// <param name="content">内容</param>
        /// <param name="rowIndex">行号</param>
        /// <param name="columnIndex">列号</param>
        /// <returns></returns>
        private Label CreateNewLablel(double height, string name, string content, int rowIndex, int columnIndex)
        {
            Label lable1 = new Label() { Name = name, Content = content };
            lable1.Height = height;
            lable1.HorizontalAlignment = HorizontalAlignment.Right;
            lable1.VerticalAlignment = VerticalAlignment.Stretch;
            lable1.HorizontalContentAlignment = HorizontalAlignment.Right;
            lable1.SetValue(Grid.RowProperty, rowIndex);
            lable1.SetValue(Grid.ColumnProperty, columnIndex);
            return lable1;
        }

        /// <summary>
        /// 创建textbox
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="name">名称</param>
        /// <param name="text">内容</param>
        /// <param name="rowIndex">行号</param>
        /// <param name="columnIndex">列号</param>
        /// <returns></returns>
        private TextBox CreateTextBox(double width, double height, string name, string text, int rowIndex, int columnIndex)
        {
            TextBox control = new TextBox() { Name = name, Text = text };
            control.TextWrapping = TextWrapping.NoWrap;
            control.Width = width;//200;
            control.Height = height;// 25;
            control.VerticalAlignment = VerticalAlignment.Stretch;
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.SetValue(Grid.RowProperty, rowIndex);
            control.SetValue(Grid.ColumnProperty, columnIndex);
            return control;
        }

        #region 功能点分流
        /// <summary>
        /// 构造测试部分控件
        /// </summary>
        private void InitTestPartControls()
        {
            if (this.curmodel != null)
            {
                do
                {
                    if (this.curmodel.Code.Equals(WeighterConfigParam.Model_Code_WeighterConfig))
                    {
                        InitWeighterConfigTestControls();
                        break;
                    }
                    if (this.curmodel.Code.Equals(AudioConfigParam.Model_Code_AudioConfig))
                    {
                        InitAudioConfigTestControls();
                        break;
                    }
                } while (false);
            }
        }
        #endregion

        #endregion

        #region 事件
        /// <summary>
        /// 保存按钮事件
        /// </summary>
        private void SaveMethod(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in MainGrid.Children)
            {
                do
                {
                    if ((Control)element is TextBox)
                    {
                        TextBox tb = (TextBox)element;
                        var list = (from r in curmodel.Params where r.Name == tb.Name select r).ToList();
                        if (list.Count > 0)
                        {
                            if (!CommonMethod.CommonMethod.IsDataTransformSuccess(string.IsNullOrEmpty(list.First().Type) ? "string" : list.First().Type, tb.Text.Trim()))
                            {
                                MessageBox.Show(string.Format(CommonParam.Info_Input_Msg_Exption, list.First().Lab));
                                tb.Focus();
                                return;
                            }
                            list.First().Value = tb.Text.Trim();
                        }
                        break;
                    }
                    else if ((Control)element is PasswordBox)
                    {
                        PasswordBox tb = (PasswordBox)element;
                        var list = (from r in curmodel.Params where r.Name == tb.Name select r).ToList();
                        if (list.Count > 0)
                        {
                            if (!CommonMethod.CommonMethod.IsDataTransformSuccess(string.IsNullOrEmpty(list.First().Type) ? "string" : list.First().Type, tb.Password.Trim()))
                            {
                                MessageBox.Show(string.Format(CommonParam.Info_Input_Msg_Exption, list.First().Lab));
                                tb.Focus();
                                return;
                            }
                            list.First().Value = tb.Password.Trim();
                        }
                        break;
                    }
                    if ((Control)element is ComboBox)
                    {
                        ComboBox cb = (ComboBox)element;
                        var list = (from r in curmodel.Params where r.Name == cb.Name select r).ToList();
                        if (list.Count > 0)
                        {
                            list.First().Value = cb.Text;
                        }
                        break;
                    }
                } while (false);
            }
            foreach (var model in this.curConfig.Modules)
            {
                var subModels = (from r in model.SubModules where r.Name == this.curmodel.Name select r).ToList();
                if (subModels.Count > 0)
                {
                    subModels[0] = this.curmodel;
                }
            }
            if (XmlHelper.WriteXmlFile<configlist>(this.xmlFileName, this.curConfig))
            {
                MessageBox.Show("保存成功");
                //ConfigReader.ReLoadConfig();
            }
        }

        #region 衡器部分

        /// <summary>
        /// 测试按钮事件(衡器功能)
        /// </summary>
        private void TestMethod(object sender, RoutedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig";
            curWeightManager = new WeightManager(System.IO.Path.Combine(path, xmlFileName));
            curWeightManager.OnReceivedWeightData += new ReceivedWeightData(SetWeightData);
            curWeightManager.OnShowErrorMsg += curWeightController_OnShowErrorMsg;
            curWeightManager.Open();
            curWeightManager.Start();
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="msg"></param>
        void curWeightController_OnShowErrorMsg(ErrorType pErrorType, string msg)
        {
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 重置称按钮事件(衡器功能)
        /// </summary>
        private void ResetWeighterMethod(object sender, RoutedEventArgs e)
        {
            if (curWeightManager != null)
            {
                curWeightManager.ClearZero();
            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseMethod(object sender, RoutedEventArgs e)
        {
            if (curWeightManager != null)
            {
                curWeightManager.Stop();
                curWeightManager.Close();
            }
        }

        /// <summary>
        /// 界面设置重量
        /// </summary>
        /// <param name="pWeight"></param>
        /// <param name="pRawData"></param>
        private void SetWeightData(string pTag, string pWeight, string pRawData)
        {
            MainGrid.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var item in MainGrid.Children)
                {
                    if (item is TextBox)
                    {
                        TextBox tb = item as TextBox;
                        if (tb.Text.Length > 1000) tb.Clear();
                        if (tb.Name.Equals("weightTextBox"))
                        {
                            if (!string.IsNullOrEmpty(pWeight))
                            {
                                tb.Text = string.Format("{0} {1}", System.DateTime.Now.ToString("HH:mm:ss"), pWeight) + "\r\n" + tb.Text;
                            }
                        }
                    }
                }
            }));
        }
        #endregion

        #region 语音对讲部分

        private void InitAudioConfigTestControls()
        {
            //添加测试按钮
            Button startAudioButton = new Button() { Name = "StartAudioButton", Content = "启动对讲" };
            startAudioButton.Margin = new Thickness() { Left = 60 };
            startAudioButton.Width = 50;
            startAudioButton.Height = 25;
            startAudioButton.HorizontalAlignment = HorizontalAlignment.Left;
            startAudioButton.VerticalAlignment = VerticalAlignment.Stretch;
            startAudioButton.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);
            startAudioButton.SetValue(Grid.ColumnSpanProperty, 3);
            startAudioButton.SetValue(Grid.ColumnProperty, 1);
            startAudioButton.Click += new RoutedEventHandler(StartAudioMethod);
            MainGrid.Children.Add(startAudioButton);

            Button closeAudioButton = new Button() { Name = "CloseAudioButton", Content = "关闭对讲" };
            closeAudioButton.Margin = new Thickness() { Left = 120 };
            closeAudioButton.Width = 50;
            closeAudioButton.Height = 25;
            closeAudioButton.HorizontalAlignment = HorizontalAlignment.Left;
            closeAudioButton.VerticalAlignment = VerticalAlignment.Stretch;
            closeAudioButton.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);
            closeAudioButton.SetValue(Grid.ColumnSpanProperty, 3);
            closeAudioButton.SetValue(Grid.ColumnProperty, 1);
            closeAudioButton.Click += new RoutedEventHandler(CloseAudioMethod);
            MainGrid.Children.Add(closeAudioButton);
        }

        /// <summary>
        /// 启动对讲
        /// </summary>
        private void StartAudioMethod(object sender, RoutedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig";
            curAudioController = new AudioController(System.IO.Path.Combine(path, xmlFileName));
            curAudioController.OnShowErrMsg += curAudioController_OnShowErrMsg;
            curAudioController.Open();
            curAudioController.Start();
        }

        void curAudioController_OnShowErrMsg(string obj)
        {
            MessageBox.Show(obj);
        }

        /// <summary>
        /// 关闭对讲
        /// </summary>
        private void CloseAudioMethod(object sender, RoutedEventArgs e)
        {
            if (curAudioController != null)
            {
                curAudioController.Stop();
                curAudioController.Close();
            }
        }

        #endregion

        #endregion
    }
}
