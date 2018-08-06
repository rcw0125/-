using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Data;
using Talent.Measure.DomainModel;
using System.Windows;
using Talent.Measure.DomainModel.CommonModel;
using Newtonsoft.Json;


namespace Talent.CarMeasureClient.Common
{
    /// <summary>
    /// 界面UI设置
    /// </summary>
    public class RenderMainUI
    {
        #region 属性
        private decimal _gridHeight;
        /// <summary>
        /// grid高度
        /// </summary>
        public decimal gridHeight
        {
            set { _gridHeight = value; }
            get { return _gridHeight; }
        }
        private Grid _gridReader;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridReader
        {
            set { _gridReader = value; }
            get { return _gridReader; }
        }
        private Grid _gridSupplier;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridSupplier
        {
            set { _gridSupplier = value; }
            get { return _gridSupplier; }
        }
        private Grid _gridMeasureWeight;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridMeasureWeight
        {
            set { _gridMeasureWeight = value; }
            get { return _gridMeasureWeight; }
        }
        private Grid _gridMeasure;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridMeasure
        {
            set { _gridMeasure = value; }
            get { return _gridMeasure; }
        }
        private List<RenderUI> _readerInfoList;
        /// <summary>
        /// 页面实例化字段
        /// </summary>
        public List<RenderUI> ReaderInfoList
        {
            set { _readerInfoList = value; }
            get { return _readerInfoList; }
        }
        private object _bindObject;
        /// <summary>
        /// 绑定数据源
        /// </summary>
        public object BindObject
        {
            set { _bindObject = value; }
            get { return _bindObject; }
        }
        #endregion
        public RenderMainUI(decimal gridHeight, Grid gridReader, Grid gridSupplier, Grid gridMeasure, Grid gridMeasureWeight, object bindObject, List<RenderUI> readerInfoList)
        {
            this.gridHeight = gridHeight;
            this.gridReader = gridReader;
            this.BindObject = bindObject;
            this.ReaderInfoList = readerInfoList;
            this.gridSupplier = gridSupplier;
            this.gridMeasure = gridMeasure;
            this.gridMeasureWeight = gridMeasureWeight;
        }
        private TextBlock getTB1(int i)
        {
            Color color1 = (Color)ColorConverter.ConvertFromString("#FFCF5C");
            TextBlock tb1 = new TextBlock();
            tb1.Foreground = new SolidColorBrush(color1);
            if (i == 0)
            {
                tb1.FontSize = 21;//28变为21 2016-3-10 11:09:38……
            }
            else
            {
                tb1.FontSize = 21;
            }
            tb1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            tb1.Margin = new System.Windows.Thickness(0, 0, 0, 10);
            return tb1;
        }
        private TextBlock getTB2(int i)
        {
            TextBlock tb2 = new TextBlock();
            Color color = (Color)ColorConverter.ConvertFromString("#FFFFFF");
            tb2.Foreground = new SolidColorBrush(color);
            if (i == 0)
            {
                tb2.FontSize = 21;//28变为21 2016-3-10 11:09:38……
            }
            else
            {
                tb2.FontSize = 21;
            }
            tb2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tb2.Margin = new System.Windows.Thickness(10, 0, 0, 0);

            return tb2;
        }
        public bool SetRenderMainUI()
        {
            try
            {
                #region 计量业务信息控制
                gridReader.Children.Clear();
                for (int i = gridReader.RowDefinitions.Count - 1; i > -1; i--)
                {
                    gridReader.RowDefinitions.RemoveAt(i);
                }
                //List<RenderUI> getBullInfo = ReaderInfoList.Where(p => p.aboutweight == 0 && p.isdisplay == 1).OrderBy(o => o.orderno).ToList<RenderUI>();
                List<RenderUI> getBullInfo = ReaderInfoList.Where(p => p.aboutweight == 0 && p.isdisplay == 1
                    && p.fieldname != "tareb" && p.fieldname != "grossb" && p.fieldname != "suttleb" && p.fieldname != "deduction").OrderBy(o => o.orderno).ToList<RenderUI>();
                int rowNum = 0;
                for (int i = 0; i < getBullInfo.Count; i += 2)
                {
                    RowDefinition rd = new RowDefinition();
                    if (getBullInfo.Count > 12)
                    {
                        rd.Height = new System.Windows.GridLength(45);
                    }
                    else
                    {
                        gridReader.Margin = new System.Windows.Thickness(0, 50, 0, 0);
                        rd.Height = new System.Windows.GridLength(50);
                    }
                    Border bd = new Border();
                    Color color = (Color)ColorConverter.ConvertFromString("#335365");
                    bd.BorderThickness = new System.Windows.Thickness(0, 0, 0, 1);
                    bd.BorderBrush = new SolidColorBrush(color);
                    bd.Margin = new System.Windows.Thickness(-24, 0, 0, 0);
                    Grid itemGD = new Grid();
                    itemGD.Margin = new System.Windows.Thickness(24, 0, 0, 0);
                    ColumnDefinition cd1 = new ColumnDefinition();
                    cd1.Width = new System.Windows.GridLength(85);
                    ColumnDefinition cd2 = new ColumnDefinition();
                    ColumnDefinition cd3 = new ColumnDefinition();
                    cd3.Width = new System.Windows.GridLength(85);
                    ColumnDefinition cd4 = new ColumnDefinition();
                    itemGD.ColumnDefinitions.Add(cd1);
                    itemGD.ColumnDefinitions.Add(cd2);
                    itemGD.ColumnDefinitions.Add(cd3);
                    itemGD.ColumnDefinitions.Add(cd4);
                    bd.Child = itemGD;


                    TextBlock tb1 = getTB1(i);
                    tb1.Name = "lb_" + i;//新增name 以后清除信息使用…lt…2016-2-17 09:58:01…… 
                    tb1.Text = getBullInfo[i].displayname;
                    tb1.FontWeight = FontWeights.Bold;
                    itemGD.Children.Add(tb1);
                    Grid.SetRow(tb1, 0);
                    Grid.SetColumn(tb1, 0);

                    TextBlock tb2 = getTB2(i);
                    tb2.Name = "tx_" + i;
                    tb2.TextWrapping = System.Windows.TextWrapping.Wrap;
                    tb2.SetBinding(TextBlock.TextProperty, new Binding(getBullInfo[i].fieldname) { Source = BindObject, Mode = BindingMode.TwoWay });
                    //tb2.Text = "测试测试测试测试测试测试";
                    itemGD.Children.Add(tb2);
                    Grid.SetRow(tb2, 1);
                    Grid.SetColumn(tb2, 1);
                    bool checkIsOneRow = CheckIsOneRow(getBullInfo[i].fieldname);//文本框 是不是一行显示出来……lt 2016-2-3 09:17:49……
                    if (checkIsOneRow)
                    {
                        Grid.SetColumnSpan(tb2, 3);
                        i = i - 1;
                        gridReader.RowDefinitions.Add(rd);
                        gridReader.Children.Add(bd);
                        Grid.SetRow(bd, rowNum);
                        Grid.SetColumn(bd, 0);
                        rowNum = rowNum + 1;
                        continue;
                    }
                    if ((i + 1) < getBullInfo.Count)
                    {
                        checkIsOneRow = CheckIsOneRow(getBullInfo[i + 1].fieldname);//如果第一个不是 第二个是 则直接换行…… 2016-2-3 09:57:12
                        if (checkIsOneRow)
                        {
                            i = i - 1;
                            gridReader.RowDefinitions.Add(rd);
                            gridReader.Children.Add(bd);
                            Grid.SetRow(bd, rowNum);
                            Grid.SetColumn(bd, 0);
                            rowNum = rowNum + 1;
                            continue;
                        }
                        TextBlock tb3 = getTB1(i);
                        tb3.Name = "lb_" + i + 1;
                        tb3.Text = getBullInfo[i + 1].displayname;
                        tb3.FontWeight = FontWeights.Bold;//字体加粗 2016-3-10 11:18:18……
                        itemGD.Children.Add(tb3);
                        Grid.SetRow(tb3, 2);
                        Grid.SetColumn(tb3, 2);

                        TextBlock tb4 = getTB2(i);
                        tb4.Name = "tx_" + i + 1;
                        tb2.TextWrapping = System.Windows.TextWrapping.Wrap;
                        tb4.SetBinding(TextBlock.TextProperty, new Binding(getBullInfo[i + 1].fieldname) { Source = BindObject, Mode = BindingMode.TwoWay });
                        //tb4.Text = "测试测试测试测试测试测试";
                        itemGD.Children.Add(tb4);
                        Grid.SetRow(tb4, 3);
                        Grid.SetColumn(tb4, 3);
                    }

                    gridReader.RowDefinitions.Add(rd);
                    gridReader.Children.Add(bd);
                    Grid.SetRow(bd, rowNum);
                    Grid.SetColumn(bd, 0);
                    rowNum = rowNum + 1;
                    //i++;
                }
                #endregion
                #region 供方信息控制
                int isDisplay = 0;
                try
                {
                    isDisplay = ReaderInfoList.Where(p => p.fieldname == "tareb").FirstOrDefault().isdisplay * ReaderInfoList.Where(p => p.fieldname == "grossb").FirstOrDefault().isdisplay * ReaderInfoList.Where(p => p.fieldname == "suttleb").FirstOrDefault().isdisplay;
                }
                catch //(Exception ex)
                {

                }

                foreach (UIElement element in gridSupplier.Children)
                {
                    if (isDisplay == 0)
                    {
                        element.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        element.Visibility = Visibility.Visible;
                    }
                }

                #endregion
                #region 称量信息控制
                try
                {
                    isDisplay = ReaderInfoList.Where(p => p.fieldname == "deduction").FirstOrDefault().isdisplay;//扣重是否显示   
                }
                catch //(Exception ex)
                {

                }
                var getMinusDeduction = gridMeasureWeight.FindName("txtMinusDeduction") as TextBlock;
                var getTxtDeduction = gridMeasureWeight.FindName("txtDeduction") as TextBox;
                var getlblDeduction = gridMeasure.FindName("lblDeduction") as TextBlock;
                var getEqualSuttle = gridMeasure.FindName("txtEqualSuttle") as TextBlock;
                var getSuttle = gridMeasure.FindName("txtSuttle") as TextBox;
                var getlblSuttle = gridMeasure.FindName("lblSuttle") as TextBlock;
                //设置当前计量重量的边框颜色
                if (this.BindObject != null)
                {
                    BullInfo bi = this.BindObject as BullInfo;
                    if (!string.IsNullOrEmpty(bi.measurestate))
                    {
                        if (bi.measurestate == "G")//计毛
                        {
                            var getTxtGrossWeight = gridMeasureWeight.FindName("txtGrossWeight") as TextBox;
                            //getTxtGrossWeight.SetValue(TextBox.BorderBrushProperty, Colors.Red);
                            //解决 “#FF0000”#FFFF0000”不是属性“BorderBrush”的有效值 lt 2016-2-16 17:17:33……
                            getTxtGrossWeight.BorderBrush = new SolidColorBrush(Colors.Red);

                            var getTxtTaireWeight = gridMeasureWeight.FindName("txtTaireWeight") as TextBox;
                            getTxtTaireWeight.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 56, 69, 86));//变为初始化 2016-2-25 10:52:58……
                        }
                        else if (bi.measurestate == "T")//计皮
                        {
                            var getTxtTaireWeight = gridMeasureWeight.FindName("txtTaireWeight") as TextBox;
                            //getTxtTaireWeight.SetValue(TextBox.BorderBrushProperty, Colors.Red);
                            //解决 “#FF0000”#FFFF0000”不是属性“BorderBrush”的有效值 lt 2016-2-16 17:17:33……
                            getTxtTaireWeight.BorderBrush = new SolidColorBrush(Colors.Red);
                            var getTxtGrossWeight = gridMeasureWeight.FindName("txtGrossWeight") as TextBox;
                            getTxtGrossWeight.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 56, 69, 86));//变为初始化  2016-2-25 10:52:58……


                        }
                    }
                }
                if (isDisplay == 0)
                {
                    if (getMinusDeduction != null)
                    {
                        getMinusDeduction.Visibility = Visibility.Hidden;
                    }
                    if (getTxtDeduction != null)
                    {
                        getTxtDeduction.Visibility = Visibility.Hidden;
                    }
                    //if (getMinusDeduction != null)
                    //{
                    //    getMinusDeduction.Visibility = Visibility.Hidden;
                    //}
                    if (getlblDeduction != null)
                    {
                        getlblDeduction.Visibility = Visibility.Hidden;
                    }
                    //去掉合并……2016-2-3 11:12:06
                    //if (getEqualSuttle != null)
                    //{
                    //    getEqualSuttle.SetValue(Grid.ColumnProperty, 3);
                    //}
                    //if (getSuttle!=null)
                    //{
                    //     getSuttle.SetValue(Grid.ColumnProperty, 4);                   
                    //}
                    //if (getlblSuttle!=null)
                    //{
                    //     getlblSuttle.SetValue(Grid.ColumnProperty, 4);
                    //}
                    //gridMeasureWeight.ColumnDefinitions.RemoveAt(6);
                    //gridMeasureWeight.ColumnDefinitions.RemoveAt(5);

                    //gridMeasure.ColumnDefinitions.RemoveAt(6);
                    //gridMeasure.ColumnDefinitions.RemoveAt(5);

                }
                else
                {
                    if (getMinusDeduction != null)
                    {
                        getMinusDeduction.Visibility = Visibility.Visible;
                    }
                    if (getTxtDeduction != null)
                    {
                        getTxtDeduction.Visibility = Visibility.Visible;
                    }
                    //if (getMinusDeduction != null)
                    //{
                    //    getMinusDeduction.Visibility = Visibility.Visible;
                    //}
                    if (getlblDeduction != null)
                    {
                        getlblDeduction.Visibility = Visibility.Visible;
                    }
                    //去掉 合并……
                    //gridMeasureWeight.ColumnDefinitions.Add(new ColumnDefinition());
                    //gridMeasureWeight.ColumnDefinitions.Add(new ColumnDefinition());

                    //gridMeasure.ColumnDefinitions.Add(new ColumnDefinition());
                    //gridMeasure.ColumnDefinitions.Add(new ColumnDefinition());

                    //gridMeasureWeight.ColumnDefinitions[5].Width = new System.Windows.GridLength(24);
                    //gridMeasure.ColumnDefinitions[5].Width = new System.Windows.GridLength(24);
                    //if (getEqualSuttle != null)
                    //{
                    //    getEqualSuttle.SetValue(Grid.ColumnProperty, 5);
                    //}
                    //if (getSuttle != null)
                    //{
                    //    getSuttle.SetValue(Grid.ColumnProperty, 6);
                    //}
                    //if (getlblSuttle != null)
                    //{
                    //    getlblSuttle.SetValue(Grid.ColumnProperty, 6);
                    //}
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "RenderMainUI出错：" + ex.StackTrace,
                    FunctionName = "称点主窗体_SetRenderMainUI",
                    Origin = "汽车衡_" + ClientInfo.Name,                 
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            
            }
            return true;
        }

        /// <summary>
        /// 判断是不是显示一行信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool CheckIsOneRow(string fileName)
        {
            bool rtB = false;
            switch (fileName.ToUpper())
            {
                case "TARGETNAME":
                    rtB = true;
                    break;
                case "SOURCENAME":
                    rtB = true;
                    break;
                case "USERMEMO":
                    rtB = true;
                    break;
            }
            return rtB;
        }
    }

}
