using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 设备配置模型
    /// </summary>
    public class EquConfigModel
    {
        private string equName;

        public string EquName
        {
            get { return equName; }
            set { equName = value; }
        }

        private string port;

        public string Port
        {
            get { return port; }
            set { port = value; }
        }
        
        private bool isUse;

        public bool IsUse
        {
            get { return isUse; }
            set { isUse = value; }
        }

        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private string portType;
        /// <summary>
        /// 端口类型
        /// </summary>
        public string PortType
        {
            get { return portType; }
            set { portType = value; }
        }
        private string equTypeCode;
        /// <summary>
        /// 设备类型编码
        /// </summary>
        public string EquTypeCode
        {
            get { return equTypeCode; }
            set { equTypeCode = value; }
        }
        private string alwaysLight;
        /// <summary>
        /// 常亮设置
        /// </summary>
        public string AlwaysLight
        {
            get { return alwaysLight; }
            set { alwaysLight = value; }
        }
        private List<SomeHierarchyViewModel> comboBoxTreeList;
        /// <summary>
        /// 下拉树集合
        /// </summary>
        public List<SomeHierarchyViewModel> ComboBoxTreeList
        {
            get { return comboBoxTreeList; }
            set { comboBoxTreeList = value; }
        }        
        private List<SomeHierarchyViewModel> comboBoxItemSource;
        /// <summary>
        /// 下拉树数据源集合
        /// </summary>
        public List<SomeHierarchyViewModel> ComboBoxItemSource
        {
            get { return comboBoxItemSource; }
            set { comboBoxItemSource = value; }
        }

        private SomeHierarchyViewModel selectedEqu;
        /// <summary>
        /// 选择的设备
        /// </summary>
        public SomeHierarchyViewModel SelectedEqu
        {
            get { return selectedEqu; }
            set
            {
                selectedEqu = value;
                if (value != null)
                {
                    EquName = value.Title;
                }
            }
        }

        private List<ComboxModel> portTypeList;
        /// <summary>
        /// "端口类型"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> PortTypeList
        {
            get { return portTypeList; }
            set { portTypeList = value; }
        }

        //private IList<SomeHierarchyViewModel> equList;
        ///// <summary>
        ///// 设备集合
        ///// </summary>
        //public IList<SomeHierarchyViewModel> EquList
        //{
        //    get { return equList; }
        //    set { equList = value; }
        //}
    }
}
