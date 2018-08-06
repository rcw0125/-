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
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.CommonMethod;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// 视频配置的交互逻辑
    /// </summary>
    public partial class VideoConfig : UserControl
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
        /// <summary>
        /// 选择的设备对应的驱动名称
        /// </summary>
        private string selectedVideoDriverName = string.Empty;
        /// <summary>
        /// 视频类型数据集
        /// </summary>
        private List<string> videoTypeNameList = new List<string>();

        private List<ComboxModel> isUseList;
        /// <summary>
        /// "是否启用"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> IsUseList
        {
            get { return isUseList; }
            set { isUseList = value; }
        }

        private List<ComboxModel> photographList;
        /// <summary>
        /// "是否拍照"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> PhotographList
        {
            get { return photographList; }
            set { photographList = value; }
        }

        private List<ComboxModel> controlList;
        /// <summary>
        /// "云台控制"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ControlList
        {
            get { return controlList; }
            set { controlList = value; }
        }

        private List<ComboxModel> dialogList;
        /// <summary>
        /// "远程对讲"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> DialogList
        {
            get { return dialogList; }
            set { dialogList = value; }
        }

        #endregion

        /// <summary>
        /// 窗体构造函数
        /// </summary>
        /// <param name="curConfig">当前配置文件对象</param>
        /// <param name="curSubModule">当前配置文件的子模块对象</param>
        /// <param name="configFileName">配置文件名称</param>
        public VideoConfig(configlist curConfig, submodule curSubModule, string configFileName)
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
                SetVideoType();
                SetDialogNum();
                SetIONum();
                SetGridView();
            }
            else
            {
                this.VideoTypeComBox.SelectedIndex = 0;
                this.DialogNumComBox.SelectedIndex = 0;
                this.IONumComBox.SelectedIndex = 0;
                this.VideoDriverTextBox.Text = string.Empty;
                this.VideoConfigDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// 设置gridview
        /// </summary>
        private void SetGridView()
        {
            IList<CameraModel> models = new List<CameraModel>();
            if (curSubModule.GridRow != null)
            {
                foreach (var row in curSubModule.GridRow.RowList)
                {
                    CameraModel model = new CameraModel();
                    foreach (var item in row.Params)
                    {
                        do
                        {
                            if (VideoConfigParam.Row_IsUse.Equals(item.Name))
                            {
                                model.IsUse = item.Value;
                                model.IsUseList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                if (this.IsUseList == null || this.IsUseList.Count == 0)//设置行中"是否启用"数据源
                                {
                                    this.IsUseList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                }
                                break;
                            }
                            if (VideoConfigParam.Row_Photograph.Equals(item.Name))
                            {
                                model.Photograph = item.Value;
                                model.PhotographList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                if (this.PhotographList == null || this.PhotographList.Count == 0)//设置行中"是否拍照"数据源
                                {
                                    this.PhotographList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                }
                                break;
                            }
                            if (VideoConfigParam.Row_VideoName.Equals(item.Name))
                            {
                                model.VideoName = item.Value;
                                break;
                            }
                            if (VideoConfigParam.Row_Position.Equals(item.Name))
                            {
                                model.Position = item.Value;
                                break;
                            }
                            if (VideoConfigParam.Row_Control.Equals(item.Name))
                            {
                                model.Control = item.Value;
                                model.ControlList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                if (this.ControlList == null || this.ControlList.Count == 0)//设置行中"云台控制"数据源
                                {
                                    this.ControlList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                }
                                break;
                            }
                            if (VideoConfigParam.Row_Ip.Equals(item.Name))
                            {
                                model.Ip = item.Value;
                                break;
                            }
                            if (VideoConfigParam.Row_Port.Equals(item.Name))
                            {
                                model.Port = item.Value;
                                break;
                            }
                            if (VideoConfigParam.Row_UserName.Equals(item.Name))
                            {
                                model.UserName = item.Value;
                                break;
                            }
                            if (VideoConfigParam.Row_PassWord.Equals(item.Name))
                            {
                                model.PassWord = item.Value;
                                break;
                            }
                            if (VideoConfigParam.Row_Channel.Equals(item.Name))
                            {
                                model.Channel = item.Value;
                                break;
                            }
                            if (VideoConfigParam.Row_Dialog.Equals(item.Name))
                            {
                                model.Dialog = item.Value;
                                model.DialogList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                if (this.DialogList == null || this.DialogList.Count == 0)//设置行中"远程对讲"数据源
                                {
                                    this.DialogList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                }
                                break;
                            }
                        } while (false);
                    }
                    models.Add(model);
                }
            }
            this.VideoConfigDataGrid.ItemsSource = models;
            this.VideoConfigDataGrid.Items.Refresh();
        }

        /// <summary>
        /// 设置视频类型
        /// </summary>
        private void SetVideoType()
        {
            var videoList = (from r in curSubModule.Params where r.Name == VideoConfigParam.VideoType select r).ToList();
            if (videoList.Count > 0)
            {
                foreach (var item in videoList.First().List)
                {
                    videoTypeNameList.Add(item);
                    ComboBoxItem item1 = new ComboBoxItem() { Content = item.Split('@')[0] };
                    this.VideoTypeComBox.Items.Add(item1);
                }
                for (int i = 0; i < videoList.First().List.Count; i++)
                {
                    if (videoList.First().List[i].Contains(videoList.First().Value))
                    {
                        this.VideoTypeComBox.SelectedItem = this.VideoTypeComBox.Items[i];
                        this.selectedVideoDriverName = videoList.First().List[i].Split('@')[1];
                        break;
                    }
                }
            }
            else
            {
                this.VideoTypeComBox.Items.Clear();
            }
        }

        /// <summary>
        /// 设置对讲设备
        /// </summary>
        private void SetDialogNum()
        {
            SetComboxDs(this.DialogNumComBox, VideoConfigParam.DialogNum);
        }

        /// <summary>
        /// 设置串口
        /// </summary>
        private void SetIONum()
        {
            SetComboxDs(this.IONumComBox, VideoConfigParam.IONum);
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
        /// 厂家选择改变触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoTypeComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.VideoTypeComBox.SelectedItem != null)
            {
                var equName = (this.VideoTypeComBox.SelectedItem as ComboBoxItem).Content.ToString();
                var drivers = (from r in videoTypeNameList where r.Contains(equName) select r.Split('@')[1]).ToList();
                this.VideoDriverTextBox.Text = drivers.First();
            }
            else
            {
                this.VideoDriverTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// 测试按钮点击事件
        /// </summary>
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.VideoConfigDataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条视频配置信息");
            }
            else
            {
                Camera camera = this.VideoConfigDataGrid.SelectedItem as Camera;
                ShowVideoForm showVideoFrom = new ShowVideoForm(camera,this.curConfigFileName);
                showVideoFrom.ShowDialog();
            }
        }

        /// <summary>
        /// 新增按钮点击事件
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //获取配置
            IList<CameraModel> models = this.VideoConfigDataGrid.ItemsSource as IList<CameraModel>;
            CameraModel model = new CameraModel()
            {
                ControlList = this.ControlList,
                DialogList = this.DialogList,
                IsUseList = this.IsUseList,
                PhotographList = this.PhotographList
            };
            model.Control = (model.ControlList != null && model.ControlList.Count > 0) ? model.ControlList.First().Name : string.Empty;
            model.Dialog = (model.DialogList != null && model.DialogList.Count > 0) ? model.DialogList.First().Name : string.Empty;
            model.IsUse = (model.IsUseList != null && model.IsUseList.Count > 0) ? model.IsUseList.First().Name : string.Empty;
            model.Photograph = (model.PhotographList != null && model.PhotographList.Count > 0) ? model.PhotographList.First().Name : string.Empty;
            models.Add(model);
            this.VideoConfigDataGrid.ItemsSource = models;
            this.VideoConfigDataGrid.Items.Refresh();
        }

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void Detete_Click(object sender, RoutedEventArgs e)
        {
            if (this.VideoConfigDataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条视频配置信息");
            }
            else
            {
                List<CameraModel> list = this.VideoConfigDataGrid.ItemsSource as List<CameraModel>;
                CameraModel removeModel = this.VideoConfigDataGrid.SelectedItem as CameraModel;
                list.Remove(removeModel);
                this.VideoConfigDataGrid.ItemsSource = list;
                this.VideoConfigDataGrid.Items.Refresh();
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
            //获取视频类型
            var videoTypeList = (from r in curSubModule.Params where r.Name == VideoConfigParam.VideoType select r).ToList();
            if (videoTypeList.Count > 0)
            {
                var videoType = this.VideoTypeComBox.SelectedItem as ComboBoxItem;
                videoTypeList.First().Value = videoType.Content.ToString();
            }
            //获取对讲设备
            var list = (from r in curSubModule.Params where r.Name == VideoConfigParam.DialogNum select r).ToList();
            if (list.Count > 0)
            {
                var dialogNum = this.DialogNumComBox.SelectedItem as ComboBoxItem;
                list.First().Value = dialogNum.Content.ToString();
            }
            //获取串口
            var ioNumList = (from r in curSubModule.Params where r.Name == VideoConfigParam.IONum select r).ToList();
            if (ioNumList.Count > 0)
            {
                var ioNum = this.IONumComBox.SelectedItem as ComboBoxItem;
                ioNumList.First().Value = ioNum.Content.ToString();
            }
            //获取视频驱动
            var videoDriverList = (from r in curSubModule.Params where r.Name == VideoConfigParam.VideoDriver select r).ToList();
            if (videoDriverList.Count > 0)
            {
                videoDriverList.First().Value = this.VideoDriverTextBox.Text.Trim();
            }
            //获取视频配置
            IList<CameraModel> models = this.VideoConfigDataGrid.ItemsSource as IList<CameraModel>;
            //摄像头名称不可重复验证
            foreach (var item in models)
            {
                var videoNames = (from r in models where r.VideoName == item.VideoName select r).ToList();
                if (videoNames != null && videoNames.Count > 1)
                {
                    int firstVideo = models.IndexOf(videoNames.First());
                    int secondVideo = models.IndexOf(videoNames[1]);
                    MessageBox.Show("第" + (firstVideo + 1) + "个视频名称与第" + (secondVideo + 1) + "个视频名称重复,保存失败!");
                    return;
                }
            }
            curSubModule.GridRow.RowList.Clear();
            foreach (var item in models)
            {
                Row row = new Row() { Params = new List<Param>() };
                Param isUse = new Param()
                {
                    Name = VideoConfigParam.Row_IsUse,
                    Value = item.IsUse,
                    List = (from r in item.IsUseList select r.Name).ToList()
                };
                Param videoName = new Param()
                {
                    Name = VideoConfigParam.Row_VideoName,
                    Value = item.VideoName
                };
                Param position = new Param()
                {
                    Name = VideoConfigParam.Row_Position,
                    Value = item.Position
                };
                Param control = new Param()
                {
                    Name = VideoConfigParam.Row_Control,
                    Value = item.Control,
                    List = (from r in item.ControlList select r.Name).ToList()
                };
                Param ip = new Param()
                {
                    Name = VideoConfigParam.Row_Ip,
                    Value = item.Ip
                };
                Param Port = new Param()
                {
                    Name = VideoConfigParam.Row_Port,
                    Value = item.Port
                };
                Param userName = new Param()
                {
                    Name = VideoConfigParam.Row_UserName,
                    Value = item.UserName
                };
                Param passWord = new Param()
                {
                    Name = VideoConfigParam.Row_PassWord,
                    Value = item.PassWord
                };
                Param channel = new Param()
                {
                    Name = VideoConfigParam.Row_Channel,
                    Value = item.Channel
                };
                Param dialog = new Param()
                {
                    Name = VideoConfigParam.Row_Dialog,
                    Value = item.Dialog,
                    List = (from r in item.DialogList select r.Name).ToList()
                };
                Param photograph = new Param()
                {
                    Name = VideoConfigParam.Row_Photograph,
                    Value = item.Photograph,
                    List = (from r in item.PhotographList select r.Name).ToList()
                };
                row.Params.Add(isUse);
                row.Params.Add(videoName);
                row.Params.Add(position);
                row.Params.Add(control);
                row.Params.Add(ip);
                row.Params.Add(Port);
                row.Params.Add(userName);
                row.Params.Add(passWord);
                row.Params.Add(channel);
                row.Params.Add(dialog);
                row.Params.Add(photograph);
                curSubModule.GridRow.RowList.Add(row);
            }
            if (XmlHelper.WriteXmlFile<configlist>(curConfigFileName, curConfig))
            {
                MessageBox.Show("保存成功");
               // ConfigReader.ReLoadConfig();
            }
        }
    }
}
