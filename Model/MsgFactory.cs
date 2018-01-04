using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using SzeTdfToLocal.Model.Binary;
using SzeTdfToLocal.Model.Binary.Session;

namespace SzeTdfToLocal.Model
{
    public class MsgFactory
    {
        public static MessageModel getLogonMessage()
        {
            var msgBody = new LogonMessageNode();
            msgBody.senderCompID = ConfigurationManager.AppSettings.Get("sdk_sender");
            msgBody.targetCompID = ConfigurationManager.AppSettings.Get("sdk_recver");
            msgBody.heartBtInt = 3;
            msgBody.passwd = ConfigurationManager.AppSettings.Get("sdk_pass");
            msgBody.defaultApplVerID = "1.02";

            var msg = MessageModel.BuildMessage(MessageModel.MSGTYPE_LOGON,msgBody);

            return msg;
        }

        public static MessageModel getHeartBtMessage()
        {
            var msgBody = new HeartBtMessageNode();
            var msg = MessageModel.BuildMessage(MessageModel.MSGTYPE_HEARTBEAT, msgBody);
            return msg;
        }


    }
}
