using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetMQ;
using System.Threading.Tasks;
using YunLib.StructData;
using SzeTdfToLocal.Model.Binary.Market;

namespace SzeTdfToLocal.Model
{
    public class MarketPubClient
    {

        private YunPublisherSocket mPubSocket = new YunPublisherSocket();
        private ReaderWriterLockSlim mPubLock = new ReaderWriterLockSlim();

        public MarketPubClient()
        {
            initClient();
        }


        private void initClient()
        {
            mPubSocket.Bind("tcp://*:19002");
        }


        public void PubStockMessage(StockMessageNode data)
        {
            try
            {
                var xData = this.StockMessageMarketTick(data);
                var dataBuf = YunLib.DataHelper.StructToBytes<MarketTickNode>(xData);
                this.pubData("stock", dataBuf);
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }
        }

        public void PubIndexMessage(IndexMessageNode data)
        {
            try
            {
                var xData = this.IndexMessageToIndexTick(data);
                var dataBuf = YunLib.DataHelper.StructToBytes<IndexTickNode>(xData);
                this.pubData("index", dataBuf);
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }
        }


        /// <summary>
        /// 深交所股票消息转云财经统一数据格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public MarketTickNode StockMessageMarketTick(StockMessageNode data)
        {
            MarketTickNode nodeData = new MarketTickNode();

            nodeData.foxxcode = string.Format("{0}.SZ", data.marketInfo.SecurityID);
            nodeData.code = string.Format("{0}", data.marketInfo.SecurityID);
            nodeData.date = int.Parse(string.Format("{0}", data.marketInfo.OrigTime).Substring(0, 8));
            nodeData.time = int.Parse(string.Format("{0}", data.marketInfo.OrigTime).Substring(8));
            nodeData.status = data.marketInfo.TradingPhaseCode.Trim();
            nodeData.name = string.Empty;

            nodeData.preclose = (int)((UInt64)data.marketInfo.PrevClosePx);
            nodeData.vol = (long)((UInt64)data.marketInfo.TotalVolumeTrade);
            nodeData.money = (long)((UInt64)data.marketInfo.TotalValueTrade);
            nodeData.transnum = (long)((UInt64)data.marketInfo.NumTrades);


            nodeData.buyorder = 0;
            nodeData.sellorder = 0;
            nodeData.buyorder_aveprice = 0;
            nodeData.sellorder_aveprice = 0;

            nodeData.buy1 = 0;
            nodeData.buy1_count = 0;
            nodeData.sell1 = 0;
            nodeData.sell1_count = 0;

            nodeData.buy2 = 0;
            nodeData.buy2_count = 0;
            nodeData.sell2 = 0;
            nodeData.sell2_count = 0;

            nodeData.buy3 = 0;
            nodeData.buy3_count = 0;
            nodeData.sell3 = 0;
            nodeData.sell3_count = 0;

            nodeData.buy4 = 0;
            nodeData.buy4_count = 0;
            nodeData.sell4 = 0;
            nodeData.sell4_count = 0;

            nodeData.buy5 = 0;
            nodeData.buy5_count = 0;
            nodeData.sell5 = 0;
            nodeData.sell5_count = 0;

            nodeData.buy6 = 0;
            nodeData.buy6_count = 0;
            nodeData.sell6 = 0;
            nodeData.sell6_count = 0;

            nodeData.buy7 = 0;
            nodeData.buy7_count = 0;
            nodeData.sell7 = 0;
            nodeData.sell7_count = 0;

            nodeData.buy8 = 0;
            nodeData.buy8_count = 0;
            nodeData.sell8 = 0;
            nodeData.sell8_count = 0;

            nodeData.buy9 = 0;
            nodeData.buy9_count = 0;
            nodeData.sell9 = 0;
            nodeData.sell9_count = 0;

            nodeData.buy10 = 0;
            nodeData.buy10_count = 0;
            nodeData.sell10 = 0;
            nodeData.sell10_count = 0;

            foreach (var item in data.MDEntry)
            {
                //买五档
                if (item.MDEntryType=="0")
                {
                    if (item.MDPriceLevel==1)
                    {
                        nodeData.buy1 = (long)((UInt64)item.MDEntryPx);
                        nodeData.buy1_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 2)
                    {
                        nodeData.buy2 = (long)((UInt64)item.MDEntryPx);
                        nodeData.buy2_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 3)
                    {
                        nodeData.buy3 = (long)((UInt64)item.MDEntryPx);
                        nodeData.buy3_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 4)
                    {
                        nodeData.buy4 = (long)((UInt64)item.MDEntryPx);
                        nodeData.buy4_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 5)
                    {
                        nodeData.buy5 = (long)((UInt64)item.MDEntryPx);
                        nodeData.buy5_count = (long)((UInt64)item.MDEntrySize);
                    }
                }

                //卖五档
                if (item.MDEntryType == "1")
                {
                    if (item.MDPriceLevel == 1)
                    {
                        nodeData.sell1 = (long)((UInt64)item.MDEntryPx);
                        nodeData.sell1_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 2)
                    {
                        nodeData.sell2 = (long)((UInt64)item.MDEntryPx);
                        nodeData.sell2_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 3)
                    {
                        nodeData.sell3 = (long)((UInt64)item.MDEntryPx);
                        nodeData.sell3_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 4)
                    {
                        nodeData.sell4 = (long)((UInt64)item.MDEntryPx);
                        nodeData.sell4_count = (long)((UInt64)item.MDEntrySize);
                    }

                    if (item.MDPriceLevel == 5)
                    {
                        nodeData.sell5 = (long)((UInt64)item.MDEntryPx);
                        nodeData.sell5_count = (long)((UInt64)item.MDEntrySize);
                    }
                }

                //最新价格
                if (item.MDEntryType=="2")
                {
                    nodeData.close = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                //开盘价格
                if (item.MDEntryType == "4")
                {
                    nodeData.dopen = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                //最高价格
                if (item.MDEntryType == "7")
                {
                    nodeData.dhigh = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                //最低价格
                if (item.MDEntryType == "8")
                {
                    nodeData.dlow = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                //平均委托买,不知道level1有不有
                if (item.MDEntryType == "x3")
                {
                    nodeData.buyorder = (long)((UInt64)item.MDEntrySize);
                    nodeData.buyorder_aveprice= (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                //平均委托卖,不知道level1有不有
                if (item.MDEntryType == "x4")
                {
                    nodeData.sellorder = (long)((UInt64)item.MDEntrySize);
                    nodeData.sellorder_aveprice = (int)((UInt64)item.MDEntryPx);
                    continue;
                }


            }


                return nodeData;
        }


        /// <summary>
        /// 深交所指数消息转云财经统一数据格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IndexTickNode IndexMessageToIndexTick(IndexMessageNode data)
        {
            IndexTickNode nodeData = new IndexTickNode();

            nodeData.foxxcode = string.Format("{0}.SZ", data.marketInfo.SecurityID);
            nodeData.code = string.Format("{0}", data.marketInfo.SecurityID);
            nodeData.date = int.Parse(string.Format("{0}",data.marketInfo.OrigTime).Substring(0,8));
            nodeData.time = int.Parse(string.Format("{0}", data.marketInfo.OrigTime).Substring(8));
            nodeData.status = "idx";
            nodeData.name = string.Empty;

            nodeData.preclose = (int)((UInt64)data.marketInfo.PrevClosePx);
            nodeData.vol = (long)((UInt64)data.marketInfo.TotalVolumeTrade);
            nodeData.money = (long)((UInt64)data.marketInfo.TotalValueTrade);
            nodeData.transnum = (long)((UInt64)data.marketInfo.NumTrades);

            nodeData.buyorder = 0;
            nodeData.sellorder = 0;
            nodeData.buyorder_aveprice = 0;
            nodeData.sellorder_aveprice = 0;

            foreach (var item in data.MDEntry)
            {
                if (item.MDEntryType=="3")
                {
                    nodeData.close = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                if (item.MDEntryType == "xa")
                {
                    nodeData.preclose = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                if (item.MDEntryType == "xb")
                {
                    nodeData.dopen = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                if (item.MDEntryType == "xc")
                {
                    nodeData.dhigh = (int)((UInt64)item.MDEntryPx);
                    continue;
                }

                if (item.MDEntryType == "xd")
                {
                    nodeData.dlow = (int)((UInt64)item.MDEntryPx);
                    continue;
                }
            }


            return nodeData;

        }


        private void pubData(string channelName, byte[] data)
        {

            this.mPubLock.EnterWriteLock();

            try
            {
                string sendData = string.Format("{0}|{1}", channelName, Convert.ToBase64String(data));
                this.mPubSocket.SendFrame(sendData);
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            this.mPubLock.ExitWriteLock();
        }

    }
}
