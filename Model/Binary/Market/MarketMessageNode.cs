using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary.Market
{
    public struct MarketMessageNode : IMessageBody
    {
        /// <summary>
        /// 数据生成时间,LocalTimestamp,YYYYMMDDHHMMSSsss
        /// </summary>
        public BigEndianUInt64 OrigTime;

        /// <summary>
        /// 频道代码
        /// </summary>
        public BigEndianUInt16 ChannelNo;

        /// <summary>
        /// 行情类别,char[3]
        /// 300111,010,现货（股票，基金，债券等）集中竞价交易快照行情
        /// 309011,900,指数快照行情
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public string MDStreamID ;

        /// <summary>
        /// 证券代码,char[8]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string SecurityID ;

        /// <summary>
        /// 证券代码源,char[4]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string SecurityIDSource ;

        /// <summary>
        /// 产品所处的交易阶段代码,char[8]
        /// 第 0 位：
        /// S=启动（开市前）
        /// O=开盘集合竞价
        /// T = 连续竞价
        /// B=休市
        /// C = 收盘集合竞价
        /// E=已闭市
        /// H = 临时停牌
        /// A=盘后交易
        /// V = 波动性中断
        /// 第 1 位：
        /// 0=正常状态
        /// 1=全天停牌
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string TradingPhaseCode ;

        /// <summary>
        /// 昨收价
        /// </summary>
        public BigEndianUInt64 PrevClosePx ;

        /// <summary>
        /// 成交笔数
        /// </summary>
        public BigEndianUInt64 NumTrades ;

        /// <summary>
        /// 成交总量
        /// </summary>
        public BigEndianUInt64 TotalVolumeTrade ;

        /// <summary>
        /// 成交总金额
        /// </summary>
        public BigEndianUInt64 TotalValueTrade ;

        public byte[] GetBytes()
        {
            return YunLib.DataHelper.StructToBytes<MarketMessageNode>(this);
        }
    }
}
