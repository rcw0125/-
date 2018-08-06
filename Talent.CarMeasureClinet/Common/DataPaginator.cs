using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Talent.Measure.DomainModel.PrintModel;

namespace Talent.CarMeasureClient
{
    /// <summary>
    /// 打印设置
    /// </summary>
    public class DataPaginator : DocumentPaginator
    {
        #region  属性及字段
        private Bill _bill;
        private Dictionary<string, string> _printData;
        private Typeface typeFace;
        private double fontSize;
        private double margin;
        private int rowsPerPage;
        private int pageCount;
        private Size _pageSize;

        public override Size PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
                PaginateData();
            }
        }
        public override bool IsPageCountValid
        {
            get { return true; }
        }
        public override int PageCount
        {
            get { return pageCount; }
        }
        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }
        #endregion

        #region  构造函数相关方法
        //构造函数
        public DataPaginator(Bill pBill, Typeface typeface, int fontsize, double margin, Size pagesize,Dictionary<string, string> pPrintdata)
        {
            this._bill = pBill;
            this.typeFace = typeface;
            this.fontSize = fontsize;
            this.margin = margin;
            this._pageSize = pagesize;
            _printData = pPrintdata;
         
            PaginateData();
        }
        /// <summary>
        /// 计算页数pageCount
        /// </summary>
        private void PaginateData()
        {
            //字符大小度量标准
            FormattedText ft = GetFormattedText("A");  //取"A"的大小计算行高等；
            pageCount = 1;
            //pageCount = (int)Math.Ceiling((double)dataTable.Rows.Count / rowsPerPage);
        }
        /// <summary>
        /// 格式化字符
        /// </summary>
        private FormattedText GetFormattedText(string text)
        {
            return GetFormattedText(text, typeFace);
        }
        /// <summary>
        /// 按指定样式格式化字符
        /// </summary>
        private FormattedText GetFormattedText(string text, Typeface typeFace)
        {
            return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, fontSize, Brushes.Black);
        }
        /// <summary>
        /// 获取对应页面数据并进行相应的打印设置
        /// </summary>
        public override DocumentPage GetPage(int pageNumber)
        {
            ////绘制打印内容
            FormattedText ft = GetFormattedText("A");
            DrawingVisual visual = new DrawingVisual();
            Point point = new Point(margin, margin);
            using (DrawingContext dc = visual.RenderOpen())
            {
                foreach (Row row in _bill.RowList)
                {
                    string drawData = row.Value;
                    if (row.Params != null)
                    {
                        foreach (Param p in row.Params)
                        {
                            drawData = drawData.Replace(p.Value, _printData[p.Value]);
                        }
                    }
                    ft = GetFormattedText(drawData);
                    dc.DrawText(ft, point);
                    System.Diagnostics.Debug.WriteLine(drawData);
                    point.Y += 1.5 * ft.Height;
                }
            }
            return new DocumentPage(visual, _pageSize, new Rect(_pageSize), new Rect(_pageSize));
        }
        #endregion
    }
}
