using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 图片模型
    /// </summary>
    public class PictureModel
    {
        private string _equcode;
        /// <summary>
        /// 称点编码
        /// </summary>
        public string equcode
        {
            get { return _equcode; }
            set
            {
                _equcode = value;
                this.SendPropertyChanged("equcode");
            }
        }
        private string _equname;
        /// <summary>
        /// 称点名称
        /// </summary>
        public string equname
        {
            get { return _equname; }
            set
            {
                _equname = value;
                this.SendPropertyChanged("equname");
            }
        }
        private string _id;
        /// <summary>
        /// 服务那里表主键(无用)
        /// </summary>
        public string id
        {
            get { return _id; }
            set
            {
                _id = value;
                this.SendPropertyChanged("id");
            }
        }
        private string _measuretype;
        /// <summary>
        /// 计量类型(G:计毛;S:净重;T:计皮)
        /// </summary>
        public string measuretype
        {
            get { return _measuretype; }
            set
            {
                _measuretype = value;
                this.SendPropertyChanged("measuretype");
            }
        }
        private string _matchid;
        /// <summary>
        /// 过磅单号
        /// </summary>
        public string matchid
        {
            get { return _matchid; }
            set
            {
                _matchid = value;
                this.SendPropertyChanged("matchid");
            }
        }

        private string _photo;
        /// <summary>
        /// 图片名称(带路径)
        /// </summary>
        public string photo
        {
            get { return _photo; }
            set
            {
                _photo = value;
                this.SendPropertyChanged("photo");
            }
        }

        private string ftpPhoto;
        /// <summary>
        /// ftp图片路径
        /// </summary>
        public string FtpPhoto
        {
            get { return ftpPhoto; }
            set
            {
                ftpPhoto = value;
                this.SendPropertyChanged("FtpPhoto");
            }
        }


        private string _createtime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public string createtime
        {
            get { return _createtime; }
            set
            {
                _createtime = value;
                this.SendPropertyChanged("createtime");
            }
        }

        private BitmapImage _image;
        /// <summary>
        /// 图片
        /// </summary>
        public BitmapImage image
        {
            get { return _image; }
            set
            {
                _image = value;
                this.SendPropertyChanged("image");
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
