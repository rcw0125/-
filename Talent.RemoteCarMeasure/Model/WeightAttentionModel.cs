using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.RemoteCarMeasure.Model
{
    public class WeightAttentionModel : WeighterClientModel
    {
        private bool isChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                this.SendPropertyChanged("IsChecked");
            }
        }
    }

    /// <summary>
    /// 关注点类型
    /// </summary>
    public enum AttentionTypes
    {
        /// <summary>
        /// 汽车衡
        /// </summary>
        CarMeasure,
        /// <summary>
        /// 火车衡
        /// </summary>
        TrainMeasure,
        /// <summary>
        /// 铁水衡
        /// </summary>
        MoltenIron
    }
}
