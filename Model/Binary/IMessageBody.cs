using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzeTdfToLocal.Model.Binary
{
    public interface IMessageBody
    {
        byte[] GetBytes();
    }
}
