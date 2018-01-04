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
    public struct BigEndianUInt32
    {
        const int size = 4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = size)]
        byte[] data;

        public static implicit operator UInt32(BigEndianUInt32 d) { if (d.data == null) { d.data = new byte[size]; } return EndianBitConverter.BigEndian.ToUInt32(d.data,0); }
        public static implicit operator BigEndianUInt32(UInt32 d) { return new BigEndianUInt32{ data = EndianBitConverter.BigEndian.GetBytes(d) }; }
        public override string ToString() { return ((UInt32)this).ToString(); }

    }
}
