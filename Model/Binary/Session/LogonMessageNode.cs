using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary.Session
{

    [StructLayout(LayoutKind.Sequential)]
    public struct LogonMessageNode:IMessageBody
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string senderCompID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string targetCompID;


        public BigEndianUInt32 heartBtInt;


        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string passwd;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string defaultApplVerID;

        public byte[] GetBytes()
        {
            return YunLib.DataHelper.StructToBytes<LogonMessageNode>(this);
        }
    }
}
