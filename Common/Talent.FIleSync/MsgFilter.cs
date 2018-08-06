using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Talent.FIleSync
{
    class MsgFilter : IMessageFilter
    {
        public Action ClosedApp;
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x80F0)
            {
                if (ClosedApp != null)
                {
                    ClosedApp();
                }
                return true;
            }
            return false;
        }
    }
}
