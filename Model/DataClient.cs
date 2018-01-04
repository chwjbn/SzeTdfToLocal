using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SzeTdfToLocal.Model.Binary;

namespace SzeTdfToLocal.Model
{
    public class DataClient
    {
        private string mHost = "172.25.10.112";
        private int mPort = 5016;
        private NetClient mNetClient;

        //最后保持登录的时间
        private DateTime mLastLoginTime = DateTime.Now.AddDays(-1);
        private object mLastLoginTimeLock = new object();

        private AutoResetEvent mLoginResetEvent = new AutoResetEvent(false);

        private ReaderWriterLockSlim mPeekLock = new ReaderWriterLockSlim();

        public event EventHandler<MessageModel> OnMessageRecv = null;

        public DataClient()
        {
            this.mHost= ConfigurationManager.AppSettings.Get("sdk_host");
            this.mPort= int.Parse(ConfigurationManager.AppSettings.Get("sdk_port"));
            this.mNetClient = new NetClient(this.mHost,this.mPort);
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doCheckConnectThreadFunc));
            ThreadPool.QueueUserWorkItem(new WaitCallback(loginThreadFunc));
            ThreadPool.QueueUserWorkItem(new WaitCallback(heartBtMessageThreadFunc));
            ThreadPool.QueueUserWorkItem(new WaitCallback(doRecvThreadFunc));
        }

        /// <summary>
        /// 是否登录
        /// </summary>
        /// <returns></returns>
        private bool IsLogin()
        {
            bool bRet = false;

            lock (this.mLastLoginTimeLock)
            {
                if (this.mNetClient.IsConnected())
                {
                    var timeSpan = DateTime.Now - this.mLastLoginTime;
                    if (timeSpan.TotalSeconds<5)
                    {
                        bRet = true;
                    }
                }
            }

            return bRet;
        }

        /// <summary>
        /// 更新登录时间
        /// </summary>
        private void UpdateLogin()
        {
            lock (this.mLastLoginTimeLock)
            {
                this.mLastLoginTime = DateTime.Now;
            }
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        private void sendData(byte[] data)
        {
            try
            {
                this.mNetClient.sendData(data);
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 从缓冲区获取一个消息，线程安全
        /// </summary>
        /// <returns></returns>
        private MessageModel peekMessage()
        {

            var msg = new MessageModel();

            this.mPeekLock.EnterWriteLock();

            try
            {

                //消息头
                var msgHeadSize = YunLib.DataHelper.StructSize<MessageHeaderNode>();
                var msgHeaderData = this.mNetClient.recvBuffer(msgHeadSize);
                var msgHeader = YunLib.DataHelper.BytesToStruct<MessageHeaderNode>(msgHeaderData);

                //消息体
                UInt32 bodyLen = msgHeader.bodyLength;
                var msgBodyData = this.mNetClient.recvBuffer((int)bodyLen);

                //消息尾
                var msgTailerSize= YunLib.DataHelper.StructSize<MessageTailerNode>();
                var msgTailerData = this.mNetClient.recvBuffer(msgTailerSize);

                //消息二进制数据
                var msgData = new byte[msgHeaderData.Length + msgBodyData.Length + msgTailerData.Length];
                msgHeaderData.CopyTo(msgData, 0);
                msgBodyData.CopyTo(msgData, msgHeaderData.Length);
                msgTailerData.CopyTo(msgData, msgHeaderData.Length + msgBodyData.Length);

                msg = MessageModel.BuildMessage(msgData);
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            this.mPeekLock.ExitWriteLock();
           
            return msg;

        }


        /// <summary>
        /// 登录
        /// </summary>
        private void doLogin()
        {
            try
            {
                if (!this.mNetClient.IsConnected())
                {
                    return;
                }

                YunLib.LogWriter.Log("DataClient.doLogin Begin.");
                Console.WriteLine("DataClient.doLogin Begin.");

                var loginMsg = MsgFactory.getLogonMessage();
                var sendData = loginMsg.ToBytes();
                this.sendData(sendData);

                YunLib.LogWriter.Log("DataClient.doLogin End.");
                Console.WriteLine("DataClient.doLogin End.");

            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }
        }

        

        /// <summary>
        /// 心跳消息
        /// </summary>
        private void doHeartBt()
        {
            try
            {
                if (!this.mNetClient.IsConnected())
                {
                    return;
                }

                var heartBtMsg = MsgFactory.getHeartBtMessage();
                var sendData = heartBtMsg.ToBytes();
                this.sendData(sendData);

            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 检查网络是否掉线线程
        /// </summary>
        /// <param name="ob"></param>
        private void doCheckConnectThreadFunc(object ob)
        {
            while (true)
            {
                try
                {
                    if (!this.mNetClient.IsConnected())
                    {
                        Console.WriteLine("DataClient CheckConnect Result=Offline.");
                        this.mNetClient.doConnect();
                        this.mLoginResetEvent.Set();
                        continue;
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    YunLib.LogWriter.Log(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 接收消息线程
        /// </summary>
        /// <param name="ob"></param>
        private void doRecvThreadFunc(object ob)
        {
            while (true)
            {
                try
                {

                    if (!this.mNetClient.IsConnected())
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    var msg = this.peekMessage();

                    //没有获取到消息
                    if (msg.mMsgHeader.msgType<1)
                    {
                        continue;
                    }

                    //更新登录时间
                    if (msg.mMsgHeader.msgType==1||msg.mMsgHeader.msgType>2)
                    {
                        this.UpdateLogin();
                    }

                    if (this.OnMessageRecv!=null)
                    {
                        this.OnMessageRecv(this, msg);
                    }

                }
                catch (Exception ex)
                {
                    YunLib.LogWriter.Log(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 心跳线程
        /// </summary>
        /// <param name="ob"></param>
        private void heartBtMessageThreadFunc(object ob)
        {
            while (true)
            {
                try
                {
                    if (this.IsLogin())
                    {
                        this.doHeartBt();
                        YunLib.LogWriter.Log("heartBtMessageThreadFunc IsLogin=true");
                    }
                    else
                    {
                        YunLib.LogWriter.Log("heartBtMessageThreadFunc IsLogin=false");
                    }
                                  
                }
                catch (Exception ex)
                {
                    YunLib.LogWriter.Log(ex.ToString());
                }

                Thread.Sleep(2800);
            }
        }

        /// <summary>
        /// 登录线程
        /// </summary>
        /// <param name="ob"></param>
        private void loginThreadFunc(object ob)
        {
            while (true)
            {
                try
                {
                    this.mLoginResetEvent.WaitOne();

                    if (!this.IsLogin())
                    {
                        this.doLogin();
                    }
                    
                }
                catch (Exception ex)
                {
                    YunLib.LogWriter.Log(ex.ToString());
                }
            }
        }

    }
}
