using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Talent.ClientCommonLib;
using Talent.RemoteCarMeasure.Model;

namespace Talent.RemoteCarMeasure.ViewModel
{
    /// <summary>
    /// 远程计量终端viewModel
    /// </summary>
    public class TestClientInfoViewModel : Only_ViewModelBase
    {
        private List<ClientInfo> models;
        /// <summary>
        /// 
        /// </summary>
        public List<ClientInfo> Models
        {
            get { return models; }
            set { models = value; this.RaisePropertyChanged("Models"); }
        }
        private Timer timer;
        private Random rd = new Random();

        public TestClientInfoViewModel()
        {
            if (IsInDesignMode)
            {
                return;
            }
            models = new List<ClientInfo>();
            InitForm();
            timer = new Timer();
            timer.Interval = 1000;
            timer.Start();
            timer.Tick += timer_Tick;
        }

        /// <summary>
        /// 窗体构造函数
        /// </summary>
        private void InitForm()
        {
            bool first = true;
            for (int i = 0; i < 9; i++)
            {
                ClientInfo model = new ClientInfo()
                {
                    ClientId = "00" + (i + 1).ToString(),
                    ClientName = (i + 1) + "号衡器",
                    Weight = rd.Next(27500, 30000).ToString(),
                    State = first ? "1" : "2",
                    IsRedLight = first,
                    IsGreenLight = !first
                };
                models.Add(model);
                first = !first;
            }
        }

        /// <summary>
        /// 指定的计时器间隔已过去而且计时器处于启用状态时发生
        /// </summary>
        void timer_Tick(object sender, EventArgs e)
        {
            if (models != null && models.Count > 0)
            {
                string clientId="00" + rd.Next(1, 9);
                var ls = (from r in models where r.ClientId == clientId select r).ToList();
                if (ls != null && ls.Count > 0)
                {
                    ls.First().IsGreenLight = !ls.First().IsGreenLight;
                    ls.First().IsRedLight = !ls.First().IsRedLight;
                    ls.First().State = ls.First().IsRedLight ? "1" : "2";
                    ls.First().Weight = rd.Next(27500, 30000).ToString();
                }
            }
        }
    }
}
