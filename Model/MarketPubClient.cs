using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetMQ;
using System.Threading.Tasks;
using YunLib.StructData;
using SzeTdfToLocal.Model.Binary.Market;
using SzeTdfToLocal.Model.Binary;

namespace SzeTdfToLocal.Model
{
    public class MarketPubClient
    {

        private YunPublisherSocket mPubSocket = new YunPublisherSocket();
        private ReaderWriterLockSlim mPubLock = new ReaderWriterLockSlim();

        private IndexTickNode mLastIndexData = new IndexTickNode();
        private MarketTickNode mLastMarketData = new MarketTickNode();

        public MarketPubClient()
        {
            initClient();
        }


        private void initClient()
        {
            mPubSocket.Bind("tcp://*:19002");
        }

        public void showStatus()
        {
            Console.WriteLine("MarketPubClient.mLastIndexData={0}",this.mLastIndexData.PrintData());
            Console.WriteLine("MarketPubClient.mLastMarketData={0}", this.mLastMarketData.PrintData());
        }


        public void PubStockMessage(StockMessageNode data)
        {
            try
            {
                var xData = this.StockMessageMarketTick(data);
                var dataBuf = YunLib.DataHelper.StructToBytes<MarketTickNode>(xData);
                this.pubData("stock", dataBuf);
                this.mLastMarketData = xData;
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
                this.mLastIndexData = xData;
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

            nodeData.foxxcode = string.Format("{0}.SZ", MessageHelper.GetMessageString(data.marketInfo.SecurityID).Trim());
            nodeData.code = string.Format("{0}", MessageHelper.GetMessageString(data.marketInfo.SecurityID).Trim());
            nodeData.date = int.Parse(string.Format("{0}", data.marketInfo.OrigTime).Substring(0, 8));
            nodeData.time = int.Parse(string.Format("{0}", data.marketInfo.OrigTime).Substring(8));
            nodeData.status = MessageHelper.GetMessageString(data.marketInfo.TradingPhaseCode).Trim();
            nodeData.name = string.Empty;

            nodeData.preclose = (int)((UInt64)data.marketInfo.PrevClosePx);
            nodeData.vol = (long)((Int64)data.marketInfo.TotalVolumeTrade/100);
            nodeData.money = (long)((Int64)data.marketInfo.TotalValueTrade/10000);
            nodeData.transnum = (long)((Int64)data.marketInfo.NumTrades);


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

                var mdType = MessageHelper.GetMessageString(item.MDEntryType).Trim();

                //买五档
                if (mdType=="0")
                {
                    if (item.MDPriceLevel==1)
                    {
                        nodeData.buy1 = (long)((UInt64)item.MDEntryPx/100);
                        nodeData.buy1_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 2)
                    {
                        nodeData.buy2 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.buy2_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 3)
                    {
                        nodeData.buy3 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.buy3_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 4)
                    {
                        nodeData.buy4 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.buy4_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 5)
                    {
                        nodeData.buy5 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.buy5_count = (long)((UInt64)item.MDEntrySize / 100);
                    }
                }

                //卖五档
                if (mdType == "1")
                {
                    if (item.MDPriceLevel == 1)
                    {
                        nodeData.sell1 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.sell1_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 2)
                    {
                        nodeData.sell2 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.sell2_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 3)
                    {
                        nodeData.sell3 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.sell3_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 4)
                    {
                        nodeData.sell4 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.sell4_count = (long)((UInt64)item.MDEntrySize / 100);
                    }

                    if (item.MDPriceLevel == 5)
                    {
                        nodeData.sell5 = (long)((UInt64)item.MDEntryPx / 100);
                        nodeData.sell5_count = (long)((UInt64)item.MDEntrySize / 100);
                    }
                }

                //最新价格
                if (mdType=="2")
                {
                    nodeData.close = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                //开盘价格
                if (mdType == "4")
                {
                    nodeData.dopen = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                //最高价格
                if (mdType == "7")
                {
                    nodeData.dhigh = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                //最低价格
                if (mdType == "8")
                {
                    nodeData.dlow = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                //平均委托买,不知道level1有不有
                if (mdType == "x3")
                {
                    nodeData.buyorder = (long)((UInt64)item.MDEntrySize / 100);
                    nodeData.buyorder_aveprice= (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                //平均委托卖,不知道level1有不有
                if (mdType == "x4")
                {
                    nodeData.sellorder = (long)((UInt64)item.MDEntrySize / 100);
                    nodeData.sellorder_aveprice = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }


            }

            nodeData.status = this.getStatus(nodeData);

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

            nodeData.foxxcode = string.Format("{0}.SZ", MessageHelper.GetMessageString(data.marketInfo.SecurityID).Trim());
            nodeData.code = string.Format("{0}", MessageHelper.GetMessageString(data.marketInfo.SecurityID).Trim());
            nodeData.date = int.Parse(string.Format("{0}",data.marketInfo.OrigTime).Substring(0,8));
            nodeData.time = int.Parse(string.Format("{0}", data.marketInfo.OrigTime).Substring(8));
            nodeData.status = "idx";
            nodeData.name = string.Empty;

            //if (!nodeData.code.Contains("399001")){return nodeData;}

            nodeData.preclose = (int)((UInt64)data.marketInfo.PrevClosePx / 100);
            nodeData.vol = (long)((Int64)data.marketInfo.TotalVolumeTrade / 100);
            nodeData.money = (long)((Int64)data.marketInfo.TotalValueTrade/10000);
            nodeData.transnum = (long)((Int64)data.marketInfo.NumTrades);

            nodeData.buyorder = 0;
            nodeData.sellorder = 0;
            nodeData.buyorder_aveprice = 0;
            nodeData.sellorder_aveprice = 0;


            foreach (var item in data.MDEntry)
            {

                var mdType = MessageHelper.GetMessageString(item.MDEntryType).Trim();

                if (mdType == "3")
                {
                    nodeData.close = (int)((UInt64)item.MDEntryPx/100);
                    continue;
                }

                if (mdType == "xa")
                {
                    nodeData.preclose = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                if (mdType == "xb")
                {
                    nodeData.dopen = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                if (mdType == "xc")
                {
                    nodeData.dhigh = (int)((UInt64)item.MDEntryPx / 100);
                    continue;
                }

                if (mdType == "xd")
                {
                    nodeData.dlow = (int)((UInt64)item.MDEntryPx / 100);
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

        public string getStatus(MarketTickNode data)
        {
            string sRet = "0";

            try
            {

                string firstCode = string.Empty;
                string secondCode = string.Empty;

                if (data.status.Length>0)
                {
                    firstCode = data.status.Substring(0, 1);
                }

                if (data.status.Length > 1)
                {
                    secondCode = data.status.Substring(1, 1);
                }

                //未开盘
                if(firstCode=="S")
                {
                    sRet = "3";
                    return sRet;
                }

                //已经休市
                if (firstCode == "B")
                {
                    sRet = "4";
                    return sRet;
                }

                //已经闭市
                if (firstCode == "E")
                {
                    sRet = "5";
                    return sRet;
                }

                //临时停牌
                if (firstCode=="H")
                {
                    sRet = "6";
                    return sRet;
                }

                //全天停牌
                if (secondCode == "1")
                {
                    sRet = "7";
                    return sRet;
                }

                //暂停交易,如熔断等特殊情况
                if (firstCode == "V")
                {
                    sRet = "8";
                    return sRet;
                }


                //正常交易
                if (secondCode == "0")
                {

                    //涨停
                    if (data.sell1 == 0 && data.sell1_count == 0)
                    {
                        if (data.buy1 > 0 && data.buy1_count > 0)
                        {
                            sRet = "1";
                            return sRet;
                        }
                    }


                    //跌停
                    if (data.buy1 == 0 && data.buy1_count == 0)
                    {
                        if (data.sell1 > 0 && data.sell1_count > 0)
                        {
                            sRet = "2";
                            return sRet;
                        }
                    }


                }



            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            return sRet;
        }

    }
}
