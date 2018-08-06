using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.Measure.DomainModel
{
    /// <summary>
    /// 键盘模型
    /// </summary>
    public class KeyboardModel
    {
        private string keyName;
        /// <summary>
        /// 键名
        /// </summary>
        public string KeyName
        {
            get { return keyName; }
            set { keyName = value; }
        }
        private string keyValue;
        /// <summary>
        /// 键值
        /// </summary>
        public string KeyValue
        {
            get { return keyValue; }
            set { keyValue = value; }
        }
        private string keyCode;
        /// <summary>
        /// 标识码
        /// </summary>
        public string KeyCode
        {
            get { return keyCode; }
            set { keyCode = value; }
        }

        private string availableIn;
        /// <summary>
        /// 有效范围
        /// </summary>
        public string AvailableIn
        {
            get { return availableIn; }
            set { availableIn = value; }
        }

        private List<ComboxModel> availableInList;
        /// <summary>
        /// "有效范围"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> AvailableInList
        {
            get { return availableInList; }
            set { availableInList = value; }
        }
    }
}
