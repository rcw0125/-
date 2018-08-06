using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel
{
    /// <summary>
    /// 命令参数对象
    /// </summary>
    public class CommandParam
    {
        public string clientid { get; set; }
        public string type { get; set; }
        public commandChildParam msg { get; set; }
    }

    /// <summary>
    /// 命令参数对象子对象
    /// </summary>
    public class commandChildParam
    {
        public string clientid { get; set; }

        public string cmd { get; set; }

        public object msg { get; set; }

        public string msgid { get; set; }
    }
}
