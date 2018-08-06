using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.Measure.WPF
{
    /// <summary>
    /// 判断坐席权限
    /// </summary>
    public class CheckRemotePower
    {
        /// <summary>
        /// 坐席功能按钮
        /// </summary>
        public enum ButtonMemuEnum
        {
            /// <summary>
            /// 重量日志
            /// </summary>
            zlrz,
            /// <summary>
            /// 表头清零
            /// </summary>
            btql,
            /// <summary>
            /// 视频监控
            /// </summary>
            spjk,
            /// <summary>
            /// 发送通知
            /// </summary>
            fstz,
            /// <summary>
            /// 关注终端
            /// </summary>
            gzzd,
            /// <summary>
            /// 任务统计
            /// </summary>
            rwtj,
            /// <summary>
            /// 数据查询
            /// </summary>
            sjcx,
            /// <summary>
            /// 版本更新
            /// </summary>
            bbgx,
            /// <summary>
            /// 终端重启
            /// </summary>
            zdcq,
            /// <summary>
            /// 修改密码
            /// </summary>
            xgmm,
            /// <summary>
            /// 是否接收任务
            /// </summary>
            jsrw,
            /// <summary>
            /// 暂停计量
            /// </summary>
            ztjl,
            /// <summary>
            /// 抢任务
            /// </summary>
            qrw,
            /// <summary>
            /// 终端暂停计量
            /// </summary>
            zdztjl,
            /// <summary>
            /// 称点全屏
            /// </summary>
            cdqp

        }
        /// <summary>
        /// 判断是不是允许操作
        /// </summary>
        /// <param name="inValues"></param>
        /// <returns></returns>
        public bool CheckIsAllowUse(ButtonMemuEnum inValues)
        {
            bool isAllow = false;
            //for (int i = 0; i < LoginUser.Modules.Count;i++ )
            //{
            //    Module oneM = LoginUser.Modules[i];
            //    if(oneM.Resourcecode.ToUpper().Equals("JLZX"))
            //    {
            //        for (int j = 0; j < oneM.Children.Count;j++ )
            //        {
            //            ButtonMemu bM=oneM.Children[j];
            //            if (bM.Resourcecode.Equals(inValues.ToString()))
            //            {
            //                isAllow = true;
            //                break;
            //            }
            //        }
            //    }   
            //}
            isAllow = true;
            return isAllow;
        }

    }
}
