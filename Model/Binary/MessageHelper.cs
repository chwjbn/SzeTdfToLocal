using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Market;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary
{
    public static class MessageHelper
    {


        /// <summary>
        /// 解析指数消息数据包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IndexMessageNode ParseIndexMessageNode(byte[] data)
        {
            var msg = new IndexMessageNode();


            try
            {
                var marketInfoSize = YunLib.DataHelper.StructSize<MarketMessageNode>();
                msg.marketInfo = YunLib.DataHelper.BytesToStruct<MarketMessageNode>(data.Take(marketInfoSize).ToArray());

                var NoMDEntriesSize = YunLib.DataHelper.StructSize<BigEndianUInt32>();
                msg.NoMDEntries = YunLib.DataHelper.BytesToStruct<BigEndianUInt32>(data.Skip(marketInfoSize).Take(NoMDEntriesSize).ToArray());

                UInt32 dataCount = msg.NoMDEntries;

                var startIndex = marketInfoSize + NoMDEntriesSize;

                var MDEntrySize = YunLib.DataHelper.StructSize<IndexMDEntry>();
                msg.MDEntry = new IndexMDEntry[dataCount];
                for (int i=0;i<dataCount;++i)
                {
                    msg.MDEntry[i] = YunLib.DataHelper.BytesToStruct<IndexMDEntry>(data.Skip(startIndex+i*MDEntrySize).Take(MDEntrySize).ToArray());
                }

                
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            return msg;
        }

        /// <summary>
        /// 解析股票消息数据包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static StockMessageNode ParseStockMessageNode(byte[] data)
        {
            var msg = new StockMessageNode();

            try
            {
                var marketInfoSize = YunLib.DataHelper.StructSize<MarketMessageNode>();
                msg.marketInfo = YunLib.DataHelper.BytesToStruct<MarketMessageNode>(data.Take(marketInfoSize).ToArray());

                var NoMDEntriesSize = YunLib.DataHelper.StructSize<BigEndianUInt32>();
                msg.NoMDEntries = YunLib.DataHelper.BytesToStruct<BigEndianUInt32>(data.Skip(marketInfoSize).Take(NoMDEntriesSize).ToArray());

                UInt32 dataCount = msg.NoMDEntries;

                var startIndex = marketInfoSize + NoMDEntriesSize;


                var MDEntrySize = YunLib.DataHelper.StructSize<StockMDEntry>();
                msg.MDEntry = new StockMDEntry[dataCount];
                for (int i = 0; i < dataCount; ++i)
                {
                    msg.MDEntry[i] = YunLib.DataHelper.BytesToStruct<StockMDEntry>(data.Skip(startIndex + i * MDEntrySize).Take(MDEntrySize).ToArray());
                }


            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            return msg;
        }
    }
}
