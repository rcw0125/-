using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// UI层使用的打印配置模型
    /// </summary>
    public class UIPrintConfigModel
    {
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrinterName { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string ComPort { get; set; }
        /// <summary>
        ///波特率
        /// </summary>
        public string Baudrate { get; set; }
        /// <summary>
        /// 缺纸时最大票数
        /// </summary>
        public int PageMaxCount { get; set; }
        /// <summary>
        /// 启用黑标
        /// </summary>
        public bool Notch { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 驱动
        /// </summary>
        public string Driver { get; set; }

        private List<ComboxModel> comportList;
        /// <summary>
        /// "串口"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ComportList
        {
            get { return comportList; }
            set { comportList = value; }
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
        private List<ComboxModel> notchList;
        /// <summary>
        /// "黑标"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> NotchList
        {
            get { return notchList; }
            set { notchList = value; }
        }

        private List<ComboxModel> brandList;
        /// <summary>
        /// 品牌
        /// </summary>
        public List<ComboxModel> BrandList
        {
            get { return brandList; }
            set { brandList = value; }
        }
    }
}
