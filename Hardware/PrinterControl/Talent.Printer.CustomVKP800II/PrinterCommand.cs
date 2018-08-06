using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Printer.CustomVKP800II
{
    /// <summary>
    /// 打印机指令封装
    /// </summary>
    public class PrinterCommand
    {

        #region 打印机指令
        /// <summary>
        /// 换行
        /// 0X0A
        /// </summary>
        public readonly static Byte[] PRINT_NEWLINE = new Byte[] { 0X0A };
        /// <summary>
        /// 初始化打印机
        /// 0X1B, 0X40
        /// </summary>
        public readonly static Byte[] INIT_PRINTER = new Byte[] { 0X1B, 0X40 };

        /// <summary>
        /// 0X1B, 0X69 
        /// </summary>
        public readonly static Byte[] PRINTER_TOTAL_CUT = new Byte[] { 0X1B, 0X69 };
        /// <summary>
        /// 清除缓存
        /// 0X18
        /// </summary>
        public readonly static Byte[] CLEAR_PRINTER_BUFFER = new Byte[] { 0X18 };
        /// <summary>
        /// 设置打印字符的宽度，高度
        /// 注意：宽度高度要自己设置
        /// 0-3位表示 高度
        /// 4-7位表示 宽度
        /// 宽度高度来源于配置文件
        /// 0X1D, 0X21
        /// </summary>
        public readonly static Byte[] SELECT_PRINTER_CHRARACTER_SIZE = new Byte[] { 0X1D, 0X21 };
        /// <summary>
        /// 发送检测命令
        /// 0X10, 0X04
        /// </summary>
        public readonly static Byte[] DETECTION_PRINTER = new Byte[] { 0X10, 0X04 };
        /// <summary>
        /// 设置黑标检测距离
        /// 0X1D, 0XE7, 0X00, 0X00
        /// </summary>
        public readonly static Byte[] SET_PRINTER_NOTCH_DISTANCE = new Byte[] { 0X1D, 0XE7, 0X00, 0X00 };
        /// <summary>
        /// 设置打印头与黑标对齐
        /// </summary>
        public readonly static Byte[] SET_PRINTER_HERDER_NOTCH_ALIGNMENT = new Byte[] { 0X1D, 0XF6 };
        /// <summary>
        /// 切纸时与黑标对齐
        /// 0X1D, 0XF8
        /// </summary>
        public readonly static Byte[] SET_PRINTER_CUTPAPER_NOTCH_ALIGNMENT = new Byte[] { 0X1D, 0XF8 };

        /// <summary>
        /// 出票
        /// 0X1D, 0X65, 0X05 
        /// </summary>
        public readonly static Byte[] PRINTER_EJECTOR_TICKET = new Byte[] { 0X1D, 0X65, 0X05 };
        /// <summary>
        /// 禁止持续出票
        /// 0X1D, 0X65, 0X05 
        /// </summary>
        public readonly static Byte[] DISABLE_DISPENSER_CONTINUOUS = new Byte[] { 0X1D, 0X65, 0X12 };
        /// <summary>
        /// 提票
        /// 0X1D, 0X65, 0X03, 0X0C
        /// </summary>
        public readonly static Byte[] PRINTER_PRESENTS_TICKET = new Byte[] { 0X1D, 0X65, 0X03, 0X0C };

        /// <summary>
        /// 切纸
        /// 0X1B, 0X69
        /// </summary>
        public readonly static Byte[] PRINTER_CUT_PAPER = new Byte[] { 0X1B, 0X69 };

        /// <summary>
        /// 合并命令
        /// </summary>
        /// <param name="pOriginalCommand">原始命令</param>
        /// <param name="pAddByte">原始命令需要新增的字节</param>
        /// <returns></returns>
        public static Byte[] JoinCommand(Byte[] pOriginalCommand, byte[] pAddByte)
        {
            Byte[] rtn = new Byte[pOriginalCommand.Length + pAddByte.Length];
            pOriginalCommand.CopyTo(rtn, 0);
            pAddByte.CopyTo(rtn, pOriginalCommand.Length);
            return rtn;
        }
        /// <summary>
        /// 命令合并
        /// </summary>
        /// <param name="pOriginalCommand"></param>
        /// <param name="pAddByte"></param>
        /// <returns></returns>
        public static Byte[] JoinCommand(Byte[] pOriginalCommand, byte pAddByte)
        {
            Byte[] temp = new Byte[] { pAddByte };
            return JoinCommand(pOriginalCommand, temp);
        }


        #endregion

    }
}
