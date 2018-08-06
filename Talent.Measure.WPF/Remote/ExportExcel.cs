using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using Talent.ClientCommonLib;

namespace Talent.Measure.WPF
{
    public class ExportExcel
    {

        #region  wpf客户端 导出DataGrid数据到Excel
        /// <summary>
        /// CSV格式化
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>格式化数据</returns>
        private static string FormatCsvField(string data)
        {
            return String.Format("\"{0}\"", data.Replace("\"", "\"\"\"").Replace("\n", "").Replace("\r", ""));
        }
        /// <summary>
        /// 导出DataGrid数据到Excel
        /// </summary>
        /// <param name="withHeaders">是否需要表头</param>
        /// <param name="grid">DataGrid</param>
        /// <param name="dataBind"></param>
        /// <returns>Excel内容字符串</returns>
        public static string ExportDataGrid(bool withHeaders, System.Windows.Controls.DataGrid grid, bool dataBind)
        {
            try
            {
                var strBuilder = new System.Text.StringBuilder();
                var source = (grid.ItemsSource as System.Collections.IList);
                if (source == null) return "";
                var headers = new List<string>();
                List<string> bt = new List<string>();
                foreach (var hr in grid.Columns)
                {
                    //   DataGridTextColumn textcol = hr. as DataGridTextColumn;
                    headers.Add(hr.Header.ToString());
                    if (hr is DataGridTextColumn)//列绑定数据
                    {
                        DataGridTextColumn textcol = hr as DataGridTextColumn;
                        if (textcol != null)
                            bt.Add((textcol.Binding as Binding).Path.Path.ToString());		//获取绑定源	  

                    }
                    else if (hr is DataGridTemplateColumn)
                    {
                        if (hr.Header.Equals("操作"))
                            bt.Add("Id");
                    }
                    else
                    {
                    }
                }
                strBuilder.Append(String.Join(",", headers.ToArray())).Append("\r\n");
               
                foreach (var data in source)
                {
                    var csvRow = new List<string>();
                    foreach (var ab in bt)
                    {
                        object temp=ReflectionUtil.GetProperty(data, ab);
                        string s = temp==null?"":temp.ToString();
                        if (s != null)
                        {
                            csvRow.Add(FormatCsvField(s));
                        }
                        else
                        {
                            csvRow.Add("\t");
                        }
                    }
                    strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\r\n");
                    // strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\t");
                }
                return strBuilder.ToString();
            }
            catch //(Exception ex)
            {

                return "";
            }
        }
        /// <summary>
        /// 导出DataGrid数据到Excel为CVS文件
        /// 使用utf8编码 中文是乱码 改用Unicode编码
        /// 
        /// </summary>
        /// <param name="withHeaders">是否带列头</param>
        /// <param name="grid">DataGrid</param>
        public static void ExportDataGridSaveAs(bool withHeaders, System.Windows.Controls.DataGrid grid)
        {
            try
            {
                string data = ExportDataGrid(true, grid, true);
                var sfd = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = "csv",
                    Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                    FilterIndex = 1
                };
                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        using (var writer = new StreamWriter(stream, System.Text.Encoding.Unicode))
                        {
                            data = data.Replace(",", "\t");
                            writer.Write(data);
                            writer.Close();
                        }
                        stream.Close();
                    }
                }
            }
            catch //(Exception ex)
            {

            }
        }
        #endregion 导出DataGrid数据到Excel
    }
}
