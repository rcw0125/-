using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 设备模型
    /// </summary>
    public class Equ
    {
        private string id;
        /// <summary>
        /// id
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string code;
        /// <summary>
        /// 编码
        /// </summary>
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string equTypeId;
        /// <summary>
        /// 设备类型ID
        /// </summary>
        public string EquTypeId
        {
            get { return equTypeId; }
            set { equTypeId = value; }
        }
    }
}
