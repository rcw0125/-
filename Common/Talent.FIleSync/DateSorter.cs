using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Talent.FIleSync
{
    /// <summary>
    /// 文件排序规则
    /// </summary>
    internal class DateSorter : IComparer
    {
        #region IComparer Members
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            FileInfo xInfo = (FileInfo)x;
            FileInfo yInfo = (FileInfo)y;

            //按照日期排序    
            return xInfo.LastWriteTime.CompareTo(yInfo.LastWriteTime);  
            //return yInfo.LastWriteTime.CompareTo(xInfo.LastWriteTime);  
        }
        #endregion
    }  
}
