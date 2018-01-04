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
    public struct BigEndianUInt64
    {
        const int size = 8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = size)]
        byte[] data;

        public static implicit operator UInt64(BigEndianUInt64 d) { if (d.data == null) { d.data = new byte[size]; } return EndianBitConverter.BigEndian.ToUInt64(d.data,0); }
        public static implicit operator BigEndianUInt64(UInt16 d) { return new BigEndianUInt64 { data = EndianBitConverter.BigEndian.GetBytes(d) }; }
        public override string ToString() { return ((UInt64)this).ToString(); }

    }
}
