using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary.Market
{
    public struct IndexMDEntry
    {

        /// <summary>
        /// 行情条目类别
        /// 3=当前指数
        /// xa=昨日收盘指数
        /// xb = 开盘指数
        /// xc=最高指数
        /// xd=最低指数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string MDEntryType;


        /// <summary>
        /// 指数点位
        /// </summary>
        public BigEndianUInt64 MDEntryPx;
    }
}
