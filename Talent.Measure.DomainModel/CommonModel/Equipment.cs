using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class Equipment
    {
        private int id = 0;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string equcode = ""; //衡器编码

        public string Equcode
        {
            get { return equcode; }
            set { equcode = value; }
        }

        private string equname = ""; //衡器名称

        public string Equname
        {
            get { return equname; }
            set { equname = value; }
        }

        private int validflag = 1;

        public int Validflag
        {
            get { return validflag; }
            set { validflag = value; }
        }

        private string equclass = ""; //1：模版  2：设备

        public string Equclass
        {
            get { return equclass; }
            set { equclass = value; }
        }

        private string createman = "";

        public string Createman
        {
            get { return createman; }
            set { createman = value; }
        }

        private string equunit = "";

        public string Equunit
        {
            get { return equunit; }
            set { equunit = value; }
        }

        private string equpostion = "";

        public string Equpostion
        {
            get { return equpostion; }
            set { equpostion = value; }
        }

        private string equtype = "";

        public string Equtype
        {
            get { return equtype; }
            set { equtype = value; }
        }

        //private DateTime createdate = DateTime.Now;

        //public DateTime Createdate
        //{
        //    get { return createdate; }
        //    set { createdate = value; }
        //}

        //private DateTime uptime = DateTime.Now;

        //public DateTime Uptime
        //{
        //    get { return uptime; }
        //    set { uptime = value; }
        //}

        private string paramodel = "";

        public string Paramodel
        {
            get { return paramodel; }
            set { paramodel = value; }
        }

        private string ip = "";  //唯一

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

    }
}
