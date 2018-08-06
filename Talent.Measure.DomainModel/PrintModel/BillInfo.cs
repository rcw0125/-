using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.PrintModel
{
    public class BillInfo
    {
        private string _code;
        public string billcode
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _script;
        /// <summary>
        /// 
        /// </summary>
        public string billscript
        {
            get { return _script; }
            set { _script = value; }
        }

        private string _xml;
        /// <summary>
        /// 
        /// </summary>
        public string billxml
        {
            get { return _xml; }
            set { _xml = value; }
        }
      
    }
}
