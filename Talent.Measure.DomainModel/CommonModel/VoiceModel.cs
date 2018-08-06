using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 声音模型
    /// added by wangc on 20151110
    /// </summary>
    public class VoiceModel : INotifyPropertyChanged
    {
        private string id;
        /// <summary>
        /// ID
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                this.SendPropertyChanged("Id");
            }
        }

        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.SendPropertyChanged("Name");
            }
        }

        private string content;
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                this.SendPropertyChanged("Content");
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(System.String propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
