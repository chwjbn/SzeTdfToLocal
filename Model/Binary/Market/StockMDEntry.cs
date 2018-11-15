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
    public struct StockMDEntry
    {

        /// <summary>
        /// 行情条目类别,char[2]
        /// 0=买入
        /// 1=卖出
        /// 2=最近价
        /// 4=开盘价
        /// 7=最高价
        /// 8=最低价
        /// x1=升跌一
        /// x2=升跌二
        /// x3=买入汇总（总量及加权平均价）
        /// x4=卖出汇总（总量及加权平均价）
        /// x5=股票市盈率一
        /// x6=股票市盈率二
        /// x7=基金 T-1 日净值
        /// x8=基金实时参考净值（包括 ETF的 IOPV）
        /// x9=权证溢价率
        /// xe=涨停价
        /// xf=跌停价
        /// xg=合约持仓量
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] MDEntryType;


        /// <summary>
        /// 价格
        /// </summary>
        public BigEndianUInt64 MDEntryPx;


        /// <summary>
        /// 数量
        /// </summary>
        public BigEndianUInt64 MDEntrySize;

        /// <summary>
        /// 买卖盘档位
        /// </summary>
        public BigEndianUInt16 MDPriceLevel;



        /// <summary>
        /// 价位总委托笔数为 0 表示不揭示
        /// </summary>
        public BigEndianUInt64 NumberOfOrders;


        /// <summary>
        /// 价位揭示委托笔数为 0 表示不揭示
        /// </summary>
        public BigEndianUInt32 NoOrders;

        /// <summary>
        /// 委托数量,Level1不用关心
        /// </summary>
        //public BigEndianUInt64[][] OrderQty;
    }
}
