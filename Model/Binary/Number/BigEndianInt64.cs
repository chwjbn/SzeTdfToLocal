using BitConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SzeTdfToLocal.Model.Binary.Number
{

    [StructLayout(LayoutKind.Sequential)]
    public struct BigEndianInt64
    {
        const int size = 8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = size)]
        byte[] data;

        public static implicit operator Int64(BigEndianInt64 d) { if (d.data == null) { d.data = new byte[size]; } return EndianBitConverter.BigEndian.ToInt64(d.data,0); }
        public static implicit operator BigEndianInt64(UInt16 d) { return new BigEndianInt64 { data = EndianBitConverter.BigEndian.GetBytes(d) }; }
        public override string ToString() { return ((Int64)this).ToString(); }

    }
}
