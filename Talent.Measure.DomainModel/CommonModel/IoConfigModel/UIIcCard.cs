using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.Measure.DomainModel
{
    /// <summary>
    /// UI层使用的IC卡信息对象
    /// </summary>
    public class UIIcCard : ICCard
    {
        private List<ComboxModel> conTypeList;
        /// <summary>
        /// "连接方式"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ConTypeList
        {
            get { return conTypeList; }
            set { conTypeList = value; }
        }

        private List<ComboxModel> comportList;
        /// <summary>
        /// "串口"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ComportList
        {
            get { return comportList; }
            set { comportList = value; }
        }

        private List<ComboxModel> iCReadTypeList;
        /// <summary>
        /// "读卡器类型"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ICReadTypeList
        {
            get { return iCReadTypeList; }
            set { iCReadTypeList = value; }
        }

        private List<ComboxModel> iCWriteTempList;
        /// <summary>
        /// "缓存模式"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ICWriteTempList
        {
            get { return iCWriteTempList; }
            set { iCWriteTempList = value; }
        }

        private List<ComboxModel> isUseList;
        /// <summary>
        /// "是否启用"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> IsUseList
        {
            get { return isUseList; }
            set { isUseList = value; }
        }

        private List<ComboxModel> baudrateList;
        /// <summary>
        ///"波特率"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> BaudrateList
        {
            get { return baudrateList; }
            set { baudrateList = value; }
        }
    }
}
