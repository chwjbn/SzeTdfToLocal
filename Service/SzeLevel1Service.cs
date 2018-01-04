using System;
using System.Linq;
using System.Net.Sockets;
using SzeTdfToLocal.Model;
using SzeTdfToLocal.Model.Binary;
using SzeTdfToLocal.Model.Binary.Market;

namespace SzeTdfToLocal.Service
{
    public class SzeLevel1Service
    {
        DataClient mDataClient = new DataClient();
        MarketPubClient mPubClient = new MarketPubClient();

        public SzeLevel1Service()
        {
            mDataClient.OnMessageRecv += MDataClient_OnMessageRecv;
        }

        public  void startService()
        {
            mDataClient.Start();
        }

        private void MDataClient_OnMessageRecv(object sender, MessageModel e)
        {

           //Console.WriteLine("[{0}]msgType={1},msgBodyLen={2},msgTailer.Sum={3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.mMsgHeader.msgType, e.mMsgHeader.bodyLength, e.mMsgTailer.checkSum);

            if (e.mMsgBody==null)
            {
                return;
            }

            var msgBodyName = e.mMsgBody.GetType().Name;

            if (msgBodyName.Contains("IndexMessageNode"))
            {
                var msgBody = (IndexMessageNode)e.mMsgBody;
                mPubClient.PubIndexMessage(msgBody);
            }

            if (msgBodyName.Contains("StockMessageNode"))
            {
                var msgBody = (StockMessageNode)e.mMsgBody;
                mPubClient.PubStockMessage(msgBody);
            }

            Console.WriteLine("[{0}]msgType={1},msgBodyLen={2},msgBody Type ={3}, msgTailer.Sum={4}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.mMsgHeader.msgType, e.mMsgHeader.bodyLength, msgBodyName,e.mMsgTailer.checkSum);

        }

        public  void showStatus()
        {
            
        }
    }
}
