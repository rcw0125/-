using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.CommonMethod
{
    public class BinaryHelper
    {
        /// <summary>
        /// byte对应位置的位数是否为1
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pPosion">0-7</param>
        /// <returns></returns>
        public static bool IsOne(byte pData, byte pPosion)
        {
            bool rtn = false;
            if (((pData >> pPosion) & 0x01) == 1)
            {
                rtn = true;
            }
            return rtn;
        }
    }
}
