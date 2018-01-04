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
    public struct BigEndianUInt16
    {
        const int size = 2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = size)]
        byte[] data;

        public static implicit operator UInt16(BigEndianUInt16 d) { if (d.data == null) { d.data = new byte[size]; } return EndianBitConverter.BigEndian.ToUInt16(d.data,0); }
        public static implicit operator BigEndianUInt16(UInt16 d) { return new BigEndianUInt16 { data = EndianBitConverter.BigEndian.GetBytes(d) }; }
        public override string ToString() { return ((UInt16)this).ToString(); }

    }
}
