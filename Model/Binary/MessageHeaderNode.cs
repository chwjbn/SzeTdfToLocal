using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary
{
    public struct MessageHeaderNode
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public BigEndianUInt32 msgType;


        /// <summary>
        /// 消息体长度
        /// </summary>
        public BigEndianUInt32 bodyLength;
    }
}
