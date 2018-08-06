using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Talent.Ic.MH
{
    public class Rf32Controller
    {
        #region 函数错误代码定义
        /// <summary>
        /// 正确
        /// </summary>
        public const int _OK = 0;
        /// <summary>
        /// 无卡
        /// </summary>
        public const int _NO_CARD = 1;
        /// <summary>
        /// CRC校验错
        /// </summary>
        public const int _CRC_ERR = 2;
        /// <summary>
        /// 值溢出
        /// </summary>
        public const int _VALUE_OVERFLOW = 3;
        /// <summary>
        /// 未验证密码
        /// </summary>
        public const int _NO_PWD_AUTH = 4;
        /// <summary>
        /// 奇偶校验错
        /// </summary>
        public const int _ODD_EVEN_ERR = 5;
        /// <summary>
        /// 通讯出错
        /// </summary>
        public const int _COMM_ERR = 6;
        /// <summary>
        /// 错误的序列号
        /// </summary>
        public const int _SERAL_ERR = 8;
        /// <summary>
        /// 验证密码失败
        /// </summary>
        public const int _PWD_AUTH_ERR = 10;
        /// <summary>
        /// 接收的数据位错误
        /// </summary>
        public const int _RECV_BIT_ERR = 11;
        /// <summary>
        /// 接收的数据字节错误
        /// </summary>
        public const int _RECV_BYTE_ERR = 12;
        /// <summary>
        /// Transfer错误
        /// </summary>
        public const int _TRANSFER_ERR = 14;
        /// <summary>
        /// 写失败
        /// </summary>
        public const int _WRITE_ERR = 15;
        /// <summary>
        /// 加值失败
        /// </summary>
        public const int _INC_ERR = 16;
        /// <summary>
        /// 减值失败
        /// </summary>
        public const int _DEC_ERR = 17;
        /// <summary>
        /// 读失败
        /// </summary>
        public const int _READ_ERR = 18;
        /// <summary>
        /// PC与读写器通讯错误
        /// </summary>
        public const int _PC_COMM_ERR = -0x10;
        /// <summary>
        /// 通讯超时
        /// </summary>
        public const int _COMM_TIMEOUT = -0x11;
        /// <summary>
        /// 打开通信口失败
        /// </summary>
        public const int _OPEN_PORT_ERR = -0x20;
        /// <summary>
        /// 串口已被占用
        /// </summary>
        public const int _PORT_USED = -0x24;
        /// <summary>
        /// 地址格式错误
        /// </summary>
        public const int _ADDR_ERR = -0x30;
        /// <summary>
        /// 该块数据不是值格式
        /// </summary>
        public const int _BLOCK_ERR = -0x31;
        /// <summary>
        /// 长度错误
        /// </summary>
        public const int _LENGTH_ERR = -0x32;
        /// <summary>
        /// 值操作失败
        /// </summary>
        public const int _VALUE_OPERA_ERR = -0x40;
        /// <summary>
        /// 卡中的值不够减
        /// </summary>
        public const int _VALUE_DEC_ERR = -0x50;

        #endregion

        #region 设备操作函数

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="port">串口号，取值为0～8</param>
        /// <param name="baud">通讯波特率9600～115200</param>
        /// <returns>成功则返回串口标识符>0，失败返回负值</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_init", SetLastError = true,
         CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_init(Int16 port, int baud);

        /// <summary>
        /// 关闭读卡器
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_exit", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_exit(int icdev);

        /// <summary>
        /// 取得读写器硬件版本号
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="state">返回版本信息</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_get_status", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_get_status(int icdev, [MarshalAs(UnmanagedType.LPArray)]byte[] state);

        /// <summary>
        /// 蜂鸣
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="msec">蜂鸣时间，单位是10毫秒</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_beep", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_beep(int icdev, int msec);

        /// <summary>
        /// 将密码装入读写模块RAM中
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="mode">装入密码模式</param>
        /// <param name="secnr">扇区号（M1卡：0～15；ML卡：0）</param>
        /// <param name="keybuff">写入读写器中的卡密码</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_load_key", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_load_key(int icdev, int mode, int secnr, [MarshalAs(UnmanagedType.LPArray)]byte[] keybuff);

        /**************************************************************************************
         * 对于M1卡的每个扇区，在读写器中只有一套密码(密码A和密码B)，动态库为了和RF-25兼容，
         * 仍对应三套密码（KEYSET0、KEYSET1、KEYSET2），每套密码包括A密码（KEYA）和B密码（KEYB），
         * 共六个密码，用0～2、4～6来表示这六个密码：
         * 0——KEYSET0的KEYA
         * 1——KEYSET1的KEYA
         * 2——KEYSET2的KEYA
         * 4——KEYSET0的KEYB
         * 5——KEYSET1的KEYB
         * 6——KEYSET2的KEYB
         * 注意：RF-35中只区分密码A或密码B, 0~2中任何一个值表示密码A，4～6中任何一个值表示密码B。                 
         **************************************************************************************/

        /// <summary>
        /// 向读写器中装入十六进制密码
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="mode">密码验证模式</param>
        /// <param name="secnr">扇区号（M1卡：0～15）</param>
        /// <param name="keybuff">写入读写器中的卡密码</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_load_key_hex", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_load_key_hex(int icdev, int mode, int secnr, [MarshalAs(UnmanagedType.LPArray)]byte[] keybuff);

        /// <summary>
        /// 将asc转换为hex
        /// </summary>
        /// <param name="asc">要转换的asc数组</param>
        /// <param name="hex">转换后的hex数组</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "a_hex", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 a_hex([MarshalAs(UnmanagedType.LPArray)]byte[] asc, [MarshalAs(UnmanagedType.LPArray)]byte[] hex, int len);

        /// <summary>
        /// 将hex转换为asc
        /// </summary>
        /// <param name="hex">要转换的hex数组</param>
        /// <param name="asc">转换后的asc数组</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "hex_a", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 hex_a([MarshalAs(UnmanagedType.LPArray)]byte[] hex, [MarshalAs(UnmanagedType.LPArray)]byte[] asc, int len);

        /// <summary>
        /// 射频读写模块复位
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="msec">复位时间，0～500毫秒有效</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_reset", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_reset(int icdev, int msec);

        /// <summary>
        /// 清除射频模块内控制寄存器中的一个二进制位
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="_b">要清除的位</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_clr_control_bit", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_clr_control_bit(int icdev, int _b);

        /// <summary>
        /// 设置射频模块控制寄存器中的一个二进制位
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="_b">要设置的位，取值同rf_clr_control_bit()</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_set_control_bit", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_set_control_bit(int icdev, int _b);

        /// <summary>
        /// 在读写器数码管上显示数字
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="len">显示字符串的长度，最长为8</param>
        /// <param name="disp">要显示的数据</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_disp8", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_disp8(int icdev, short len, [MarshalAs(UnmanagedType.LPArray)]byte[] disp);
        /*
         * 受读写器控制时，显示的日期/时间请参照rf_disp_mode()rf_disp_mode中定义的格式；
         * 受计算机控制时，显示方式由显示数据决定；每个字节的最高位为1表示本位数后的小数点亮，为0表示小数点灭。
        */

        /// <summary>
        /// 在读写器的数码管上显示数字（为低版本兼容函数，V3.0及以上版本不能使用）
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode"></param>
        /// <param name="digit"></param>
        /// <returns>成功返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_disp", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_disp(int icdev, short mode, int digit);

        /// <summary>
        /// DES算法加密函数
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="ptrsource">要加密码的原文</param>
        /// <param name="len">原文长度，必需为8的倍数</param>
        /// <param name="ptrdest">加密后的密文</param>
        /// <returns>成功返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_encrypt", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_encrypt([MarshalAs(UnmanagedType.LPArray)]byte[] key, [MarshalAs(UnmanagedType.LPArray)]byte[] ptrsource, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] ptrdest);

        /// <summary>
        /// DES算法解密函数
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="ptrsource">要解密的密文</param>
        /// <param name="len">原文长度必需为8的倍数</param>
        /// <param name="ptrdest">解密后的原文</param>
        /// <returns>成功返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_decrypt", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_decrypt([MarshalAs(UnmanagedType.LPArray)]byte[] key, [MarshalAs(UnmanagedType.LPArray)]byte[] ptrsource, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] ptrdest);

        /// <summary>
        /// 读取读写器备注信息
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="offset">偏移地址（0～383）</param>
        /// <param name="len">读取信息长度（1～384）</param>
        /// <param name="databuff">读取到的信息</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_srd_eeprom", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_srd_eeprom(int icdev, int offset, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] databuff);

        /// <summary>
        /// 向读写器备注区中写入信息
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="offset">偏移地址（0～383）</param>
        /// <param name="len">读取信息长度（1～384）</param>
        /// <param name="databuff">要写入的信息</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_swr_eeprom", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_swr_eeprom(int icdev, int offset, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] databuff);

        /// <summary>
        /// 向读写器端口输出控制字，此信号可用于控制用户的外设。
        /// </summary>
        /// <param name="icdev">设备句柄</param>
        /// <param name="_byte">控制字，该字节低5位每一位控制一个输出</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_setport", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_setport(int icdev, byte _byte);

        /// <summary>
        /// 读取读写器端口输入的值
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="_byte">端口输入值，1个字节,低5位有效。</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_getport", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_getport(int icdev, out byte _byte);

        /// <summary>
        /// 读取读写器日期、星期、时间
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="time">返回数据，长度为7个字节，格式为“年、星期、月、日、时、分、秒”</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_gettime", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_gettime(int icdev, [MarshalAs(UnmanagedType.LPArray)]byte[] time);

        /// <summary>
        /// 同rf_gettime(),用十六进制表示
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="time">长度为14个字节,均为数字</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_gettimehex", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_gettime_hex(int icdev, [MarshalAs(UnmanagedType.LPArray)]byte[] time);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_settime_hex", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_settime_hex(int icdev, [MarshalAs(UnmanagedType.LPArray)]byte[] time);

        /// <summary>
        /// 设置读写器日期、星期、时间
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="time">长度为7个字节，格式为“年、星期、月、日、时、分、秒”</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_settime", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_settime(int icdev, [MarshalAs(UnmanagedType.LPArray)]byte[] time);

        /// <summary>
        /// 设置数码管显示亮度
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="bright">亮度值，0～15有效，0表示最暗，15表示最亮</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = " rf_setbright", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_setbright(int icdev, byte bright);

        /// <summary>
        /// 设置读写器数码管受控方式，关机后可保存设置值
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode">受控方式 
        /// 0数码管显示受计算机控制 
        /// 1数码管显示受读写器控制（出厂设置）
        /// </param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_ctl_mode", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_ctl_mode(int icdev, int mode);

        /// <summary>
        /// 设置读写器数码管显示模式，关机后可保存设置值
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode">
        /// 0——日期，格式为“年-月-日（yy-mm-dd）”，BCD码
        /// 1——时间，格式为“时-分-秒（hh-nn-ss）”
        /// </param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_disp_mode", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_disp_mode(int icdev, int mode);

        /// <summary>
        /// 读取软件版本号
        /// </summary>
        /// <param name="ver">存放版本号的缓冲区，长度18字节(包括结束字符’\0’)。</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "lib_ver", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 lib_ver([MarshalAs(UnmanagedType.LPArray)]byte[] ver);

        #endregion

        #region 通用函数
        /// <summary>
        /// 寻卡，能返回在工作区域内某张卡的序列号
        /// 选择IDLE模式，在对卡进行读写操作，执行rf_halt()rf_halt指令中止卡操作后
        /// 只有当该卡离开并再次进入操作区时，读写器才能够再次对它进行操作。
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode">寻卡模式</param>
        /// <param name="snr">返回的卡序列号</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_card", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_card(int icdev, int mode, out uint snr);

        /// <summary>
        /// 寻卡
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode">寻卡模式分三种情况：IDLE模式、ALL模式及指定卡模式。
        /// 0 表示IDLE模式，一次只对一张卡操作
        /// 1 表示ALL模式，一次可对多张卡操作
        /// 2 表示指定卡模式，只对序列号等于snr的卡操作（高级函数才有）
        /// </param>
        /// <param name="tagtype">卡类型值，0x0004为M1卡，0x0010为ML卡</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_request", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_request(int icdev, int mode, out UInt16 tagtype);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode"></param>
        /// <param name="tagtype"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_request_std", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_request_std(int icdev, int mode, out UInt16 tagtype);

        /// <summary>
        /// 卡防冲突，返回卡的序列号
        /// request指令之后应立即调用anticoll，除非卡的序列号已知。
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="bcnt">设为0</param>
        /// <param name="snr">返回的卡号</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_anticoll", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_anticoll(int icdev, int bcnt, out uint snr);

        /// <summary>
        /// 从多个卡中选取一个给定序列号的卡
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="snr">卡号</param>
        /// <param name="size">指向返回的卡容量的数据</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_select", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_select(int icdev, uint snr, out byte size);

        /// <summary>
        /// 验证某一扇区密码
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode">密码验证模式</param>
        /// <param name="secnr">要验证密码的扇区号（0～15）</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_authentication", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_authentication(int icdev, int mode, int secnr);

        /// <summary>
        /// ML卡验证密码
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode">密码验证模式</param>
        /// <param name="keynr">0</param>
        /// <param name="blocknr">0</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_authentication_2", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_authentication_2(int icdev, int mode, int keynr, int blocknr);

        /// <summary>
        /// 读取卡中数据
        /// 对于M1卡，一次读一个块的数据，为16个字节；
        /// 对于ML卡，一次读出相同属性的两页（0和1，2和3，...），为8个字节
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">
        /// M1卡——块地址（0～63）；
        /// ML卡——页地址（0～11）
        /// </param>
        /// <param name="databuff">读出数据</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_read", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_read(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)]byte[] databuff);

        /// <summary>
        /// 同rf_read() 读取卡中数据
        /// 对于M1卡，一次读一个块的数据，为16个字节；
        /// 对于ML卡，一次读出相同属性的两页（0和1，2和3，...），为8个字节        
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">
        /// M1卡——块地址（0～63）；
        /// ML卡——页地址（0～11）
        /// </param>
        /// <param name="databuff">读出数据，数据以十六进制形式表示</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_read_hex", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_read_hex(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)]byte[] databuff);

        /// <summary>
        /// 向卡中写入数据
        /// 对于M1卡，一次必须写一个块，为16个字节
        /// 对于ML卡，一次必须写一页，为4个字节
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">
        /// M1卡——块地址（1～63）；
        /// ML卡——页地址（2～11）
        /// </param>
        /// <param name="databuff">要写入的数据</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_write", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_write(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)]byte[] databuff);

        /// <summary>
        /// 同rf_write() 向卡中写入数据
        /// 对于M1卡，一次必须写一个块，为16个字节
        /// 对于ML卡，一次必须写一页，为4个字节
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">
        /// M1卡——块地址（1～63）；
        /// ML卡——页地址（2～11）
        /// </param>
        /// <param name="databuff">要写入的数据</param>
        /// <returns>成功则返回 0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_write_hex", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_write_hex(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)]byte[] databuff);

        /// <summary>
        /// 中止对该卡操作
        /// 说明：执行该命令后如果是ALL寻卡模式则必须重新寻卡才能够对该卡操作，如果是IDLE模式则必须把卡移开感应区再进来才能寻得这张卡。
        /// </summary>
        /// <param name="icdev"></param>
        /// <returns>成功则返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_halt", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_halt(int icdev);

        /// <summary>
        /// 初始化块值
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">块地址（1～63）</param>
        /// <param name="val">初始值</param>
        /// <returns>成功则返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_initval", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_initval(int icdev, int blocknr, uint val);

        /// <summary>
        /// 读块值
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">块地址（1～63）</param>
        /// <param name="val">读出值的地址</param>
        /// <returns>成功则返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_readval", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_readval(int icdev, int blocknr, out uint val);

        /// <summary>
        /// 块加值
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">块地址（1～63）</param>
        /// <param name="val">要增加的值</param>
        /// <returns>成功则返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_increment", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_increment(int icdev, int blocknr, uint val);

        /// <summary>
        /// 块减值
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">块地址1～63,4n+3除外</param>
        /// <param name="val">要减的值</param>
        /// <returns>成功则返回0</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_decrement", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_decrement(int icdev, int blocknr, uint val);

        /// <summary>
        /// 回传函数，将EEPROM中的内容传入卡的内部寄存器
        /// 用此函数将某一块中的数值传入内部寄存器，然后用rf_transfer()函数将寄存器中数据再传送到另一块中去，实现块与块之间数值传送。该函数只用于值块。
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">要进行回传的块地址（1～63）</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_restore", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_restore(int icdev, int blocknr);

        /// <summary>
        /// 传送，将寄存器的内容传送到EEPROM中
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="blocknr">要传送的地址（1～63）</param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_transfer", SetLastError = true,
        CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_transfer(int icdev, int blocknr);

        #endregion

    }
}
