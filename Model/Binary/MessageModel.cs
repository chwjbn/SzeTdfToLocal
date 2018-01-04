using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Market;
using SzeTdfToLocal.Model.Binary.Session;

namespace SzeTdfToLocal.Model.Binary
{
    public class MessageModel
    {

        public static readonly UInt32 MSGTYPE_LOGON = 1;
        public static readonly UInt32 MSGTYPE_LOGOUT = 2;
        public static readonly UInt32 MSGTYPE_HEARTBEAT = 3;
        public static readonly UInt32 MSGTYPE_CHANNELHEARTBT = 390095;
        public static readonly UInt32 MSGTYPE_REBUILD = 390094;
        public static readonly UInt32 MSGTYPE_USERREPORT = 390093;
        public static readonly UInt32 MSGTYPE_MDCHANNELSTATISTIC = 390090;
        public static readonly UInt32 MSGTYPE_BUSINESSREJECT = 8;
        public static readonly UInt32 MSGTYPE_FINANCIALSTATUS = 390013;
        public static readonly UInt32 MSGTYPE_NEWS = 390012;
        public static readonly UInt32 MSGTYPE_AUCTIONMD = 300111;
        public static readonly UInt32 MSGTYPE_ENDTRPRICING = 300611;
        public static readonly UInt32 MSGTYPE_INDEXMD = 309011;
        public static readonly UInt32 MSGTYPE_STATISTICALINDICATORSMD = 309111;


        /// <summary>
        /// 消息头
        /// </summary>
        public MessageHeaderNode mMsgHeader;


        /// <summary>
        /// 消息体
        /// </summary>
        public IMessageBody mMsgBody;



        /// <summary>
        /// 消息尾
        /// </summary>
        public MessageTailerNode mMsgTailer;



        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <param name="msgBody">消息体</param>
        /// <returns></returns>
        public static MessageModel BuildMessage(UInt32 msgType,IMessageBody msgBody)
        {
            MessageModel data = new MessageModel();

            data.mMsgHeader = new MessageHeaderNode();
            data.mMsgBody = msgBody;
            data.mMsgTailer = new MessageTailerNode();

            data.mMsgHeader.msgType = msgType;
            data.mMsgHeader.bodyLength = (UInt32)data.mMsgBody.GetBytes().Length;


            data.mMsgTailer.checkSum=(UInt32)data.GetMessageCheckSum();
           
            return data;
        }

        /// <summary>
        /// 使用二进制转消息
        /// </summary>
        /// <param name="data">二进制数组</param>
        /// <returns></returns>
        public static MessageModel BuildMessage(byte[] data)
        {
            MessageModel msgData = new MessageModel();

            int headSize = YunLib.DataHelper.StructSize<MessageHeaderNode>();
            msgData.mMsgHeader = YunLib.DataHelper.BytesToStruct<MessageHeaderNode>(data.Take(headSize).ToArray());

            UInt32 bodySize = msgData.mMsgHeader.bodyLength;

            int tailerSize = YunLib.DataHelper.StructSize<MessageTailerNode>();
            int skipSize = headSize+(int)(bodySize);
            msgData.mMsgTailer = YunLib.DataHelper.BytesToStruct<MessageTailerNode>(data.Skip(skipSize).Take(tailerSize).ToArray());


            try
            {
                //登录消息
                if (msgData.mMsgHeader.msgType == MSGTYPE_LOGON)
                {
                    msgData.mMsgBody = new LogonMessageNode();
                    var bodyData = data.Skip(headSize).Take((int)bodySize).ToArray();
                    msgData.mMsgBody = YunLib.DataHelper.BytesToStruct<LogonMessageNode>(bodyData);
                    return msgData;
                }


                //心跳消息
                if (msgData.mMsgHeader.msgType == MSGTYPE_HEARTBEAT)
                {
                    msgData.mMsgBody = new HeartBtMessageNode();
                    var bodyData = data.Skip(headSize).Take((int)bodySize).ToArray();
                    if(bodyData.Length>0)
                    {
                        msgData.mMsgBody = YunLib.DataHelper.BytesToStruct<HeartBtMessageNode>(bodyData);
                    }          
                    return msgData;
                }


                //股票等
                if (msgData.mMsgHeader.msgType == MSGTYPE_AUCTIONMD)
                {

                    var bodyData = data.Skip(headSize).Take((int)bodySize).ToArray();


                    var marketMsg= new MarketMessageNode();
                    marketMsg= YunLib.DataHelper.BytesToStruct<MarketMessageNode>(bodyData);
                    msgData.mMsgBody = marketMsg;

                    if (marketMsg.MDStreamID=="01")
                    {
                        msgData.mMsgBody = MessageHelper.ParseStockMessageNode(bodyData);
                    }

                    return msgData;

                }


                //指数
                if (msgData.mMsgHeader.msgType == MSGTYPE_INDEXMD)
                {

                    var bodyData = data.Skip(headSize).Take((int)bodySize).ToArray();

                    var marketMsg = new MarketMessageNode();
                    marketMsg = YunLib.DataHelper.BytesToStruct<MarketMessageNode>(bodyData);
                    msgData.mMsgBody = marketMsg;

                    if (marketMsg.MDStreamID == "90")
                    {
                        msgData.mMsgBody = MessageHelper.ParseIndexMessageNode(bodyData);
                    }

                    return msgData;
                }

            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }


            return msgData;
        }
        


        /// <summary>
        /// 校验和算法
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <returns></returns>
        public int GenerateCheckSum(byte[] data)
        {
            int sum = 0;

            int cks = 0;

            for (int i = 0; i < data.Length; ++i)
            {
                cks += (data[i] & 0xFF);
            }

            sum = cks % 256;

            return sum;
        }


        /// <summary>
        /// 获取消息校验和
        /// </summary>
        /// <returns></returns>
        public int GetMessageCheckSum()
        {
            var checkData = this.ToCheckSumBytes();
            return this.GenerateCheckSum(checkData);
        }


        /// <summary>
        /// 转成校验和计算所需要的二进制数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToCheckSumBytes()
        {
            var headData = YunLib.DataHelper.StructToBytes<MessageHeaderNode>(this.mMsgHeader);
            var bodyData = this.mMsgBody.GetBytes();

            var data = new byte[headData.Length + bodyData.Length];
            headData.CopyTo(data, 0);
            bodyData.CopyTo(data, headData.Length);

            return data;
        }


        /// <summary>
        /// 转成二进制数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            var headData = YunLib.DataHelper.StructToBytes<MessageHeaderNode>(this.mMsgHeader);
            var bodyData = this.mMsgBody.GetBytes();
            var tailerData = YunLib.DataHelper.StructToBytes<MessageTailerNode>(this.mMsgTailer);

            var data = new byte[headData.Length + bodyData.Length + tailerData.Length];
            headData.CopyTo(data, 0);
            bodyData.CopyTo(data, headData.Length);
            tailerData.CopyTo(data, headData.Length + bodyData.Length);

            return data;

        }


    }
}
