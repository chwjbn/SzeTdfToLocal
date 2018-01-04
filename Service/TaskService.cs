using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzeTdfToLocal.Service
{
    public class TaskService : YunLib.YunTaskService
    {

        private SzeLevel1Service mSzeLevel1Service = new SzeLevel1Service();

        public override void showStatus()
        {
            this.mSzeLevel1Service.showStatus();
        }

        public override void startService()
        {
            this.mSzeLevel1Service.startService();
        }
    }
}
