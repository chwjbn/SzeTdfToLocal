using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary.Market
{
    public struct IndexMessageNode : IMessageBody
    {
        public MarketMessageNode marketInfo;


        /// <summary>
        /// 行情条目个数
        /// </summary>
        public BigEndianUInt32 NoMDEntries;


        public IndexMDEntry[] MDEntry;




        public byte[] GetBytes()
        {
            return YunLib.DataHelper.StructToBytes<IndexMessageNode>(this);
        }
    }
}
