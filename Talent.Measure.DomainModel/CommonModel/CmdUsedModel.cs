using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel
{
    // 各命令使用的模型（命名规则:命令+model）

    /// <summary>
    /// sendReply命令对应的对象
    /// </summary>
    public class SendReplyModel
    {
        public string clientid { get; set; }
        public string matchid { get; set; }
        public object data { get; set; }//业务数据
        public int result { get; set; }
    }
}
