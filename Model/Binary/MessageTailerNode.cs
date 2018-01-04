using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Model.Binary.Number;

namespace SzeTdfToLocal.Model.Binary
{
    public struct MessageTailerNode
    {
        /// <summary>
        /// 校验和
        /// </summary>
        public BigEndianUInt32 checkSum;
    }
}
