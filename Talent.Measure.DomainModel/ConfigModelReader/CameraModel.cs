using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.Measure.DomainModel
{
    /// <summary>
    /// 视频模型
    /// </summary>
    public class CameraModel : Camera
    {
        private List<ComboxModel> isUseList;
        /// <summary>
        /// "是否启用"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> IsUseList
        {
            get { return isUseList; }
            set { isUseList = value; }
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

        private List<ComboxModel> photographList;
        /// <summary>
        /// "是否拍照"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> PhotographList
        {
            get { return photographList; }
            set { photographList = value; }
        }
    }
}
