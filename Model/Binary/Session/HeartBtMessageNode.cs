using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SzeTdfToLocal.Model.Binary.Session
{

    [StructLayout(LayoutKind.Sequential)]
    public struct HeartBtMessageNode : IMessageBody
    {
        public byte[] GetBytes()
        {
            return YunLib.DataHelper.StructToBytes<HeartBtMessageNode>(this);
        }
    }
}
