using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary.Market
{

    [StructLayout(LayoutKind.Sequential)]
    public struct StockMessageNode:IMessageBody
    {
        public MarketMessageNode marketInfo;


        /// <summary>
        /// 行情条目个数
        /// </summary>
        public BigEndianUInt32 NoMDEntries;
        public StockMDEntry[] MDEntry;

        public byte[] GetBytes()
        {
            return YunLib.DataHelper.StructToBytes<StockMessageNode>(this);
        }
    }
}
