using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SzeTdfToLocal.Model
{
    public class NetClient:IDisposable
    {
        private string mHost = "172.25.10.112";
        private int mPort = 6666;
        private Socket mNetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private ReaderWriterLockSlim mNetReadWriteLock = new ReaderWriterLockSlim();

        public NetClient(string host, int port)
        {
            this.mHost = host;
            this.mPort = port;
        }

        /// <summary>
        /// 远程连接
        /// </summary>
        public void doConnect()
        {

            this.mNetReadWriteLock.EnterWriteLock();

            try
            {

                YunLib.LogWriter.Log("NetClient doConnet to {0}:{1} Begin", this.mHost, this.mPort);
                Console.WriteLine("NetClient doConnet to {0}:{1} Begin", this.mHost, this.mPort);

                this.mNetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var keepAliveData = this.keepAliveData(1, 3000, 500);
                this.mNetSocket.IOControl(IOControlCode.KeepAliveValues, keepAliveData, null);

                this.mNetSocket.Connect(this.mHost, this.mPort);

                YunLib.LogWriter.Log("NetClient doConnet to {0}:{1} End",this.mHost,this.mPort);
                Console.WriteLine("NetClient doConnet to {0}:{1} End", this.mHost, this.mPort);
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            this.mNetReadWriteLock.ExitWriteLock();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void sendData(byte[] data)
        {
            this.mNetReadWriteLock.EnterReadLock();

            try
            {
                if (!this.IsConnected())
                {
                    return;
                }

                this.mNetSocket.Send(data);
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            this.mNetReadWriteLock.ExitReadLock();
        }

        /// <summary>
        /// 接收缓存数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int receiveData(byte[] data)
        {
            int nRet = 0;

            this.mNetReadWriteLock.EnterReadLock();

            try
            {
                nRet = this.mNetSocket.Receive(data);
            }
            catch (Exception ex)
            {
                nRet = -1;
                YunLib.LogWriter.Log(ex.ToString());
            }

            this.mNetReadWriteLock.ExitReadLock();

            return nRet;
        }

        /// <summary>
        /// 接收缓存
        /// </summary>
        /// <param name="bufferLen">要接收的长度</param>
        /// <returns></returns>
        public byte[] recvBuffer(int bufferLen)
        {
            byte[] outData = new byte[bufferLen];


            int recvLen = 0;

            while (true)
            {
                if (!this.IsConnected())
                {
                    break;
                }

                if (recvLen >= bufferLen)
                {
                    break;
                }

                var dataBuffer = new byte[bufferLen - recvLen];

                int tempLen = this.receiveData(dataBuffer);

                if (tempLen<0)
                {
                    throw new SocketException(10052);
                }

                if (tempLen < 1)
                {
                    continue;
                }

                dataBuffer.Take(tempLen).ToArray().CopyTo(outData, recvLen);

                recvLen += tempLen;
            }

            return outData;

        }

        /// <summary>
        /// 判断网络状态是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            bool bRet = false;

            try
            {

                if (this.mNetSocket==null)
                {
                    return bRet;
                }

                if (!this.mNetSocket.Connected)
                {
                    return bRet;
                }

                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

                foreach (TcpConnectionInformation c in tcpConnections)
                {
                    TcpState stateOfConnection = c.State;

                    if (!c.LocalEndPoint.Equals(this.mNetSocket.LocalEndPoint))
                    {
                        continue;
                    }

                    if (!c.RemoteEndPoint.Equals(this.mNetSocket.RemoteEndPoint))
                    {
                        continue;
                    }

                    if (stateOfConnection == TcpState.Established)
                    {
                        bRet = true;
                        break;
                    }
                                         
                }

            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }

            return bRet;
        }


        /// <summary>
        /// keepAlive 数据
        /// </summary>
        /// <param name="onOff">是否开启KeepAlive</param>
        /// <param name="keepAliveTime">开始首次KeepAlive探测前的TCP空闭时间</param>
        /// <param name="keepAliveInterval">两次KeepAlive探测间的时间间隔</param>
        /// <returns></returns>
        private byte[] keepAliveData(int onOff, int keepAliveTime, int keepAliveInterval)
        {
            byte[] buffer = new byte[12];
            System.BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
            System.BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
            System.BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
            return buffer;
        }

        public void Dispose()
        {
            try
            {
                this.mNetSocket.Dispose();
            }
            catch (Exception ex)
            {
                YunLib.LogWriter.Log(ex.ToString());
            }
        }
    }
}
