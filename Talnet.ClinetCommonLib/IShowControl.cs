using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.ClientCommonLib
{
    public interface IShowControl : IDisposable
    {
        string MenuReamk
        { get; set; }

        void Run(string menuId, string menuName);
    }
}
