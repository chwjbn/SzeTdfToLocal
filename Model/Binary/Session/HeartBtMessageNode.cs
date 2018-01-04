using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzeTdfToLocal.Model.Binary.Session
{
    public struct HeartBtMessageNode : IMessageBody
    {
        public byte[] GetBytes()
        {
            return YunLib.DataHelper.StructToBytes<HeartBtMessageNode>(this);
        }
    }
}
