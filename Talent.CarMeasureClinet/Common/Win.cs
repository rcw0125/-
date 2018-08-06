using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Talent.CarMeasureClient.Common
{
    public class Win
    {
        public const int HWND_BROADCAST = 0xffff;
        public const string msgstr = "QUIT_FILESYNC";
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);
        
        //private uint msg;
        //private const int HWND_BROADCAST = 0xffff;
    }
}
