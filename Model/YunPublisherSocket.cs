using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzeTdfToLocal.Model
{
    public class YunPublisherSocket:NetMQ.Sockets.PublisherSocket
    {
        public YunPublisherSocket(string connectionString = null) : base(connectionString)
        {
            initSocket();
        }

        private void initSocket()
        {
            this.Options.SendHighWatermark = 1000 * 10000;
        }
    }
}
