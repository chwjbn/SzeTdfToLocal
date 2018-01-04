using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SzeTdfToLocal.Service;

namespace SzeTdfToLocal
{
    class Program
    {
        static void Main(string[] args)
        {
            var iTask = new TaskService();
            iTask.startService();

            while (true)
            {
                String cmdStr = Console.ReadLine();
                cmdStr = cmdStr.Trim();

                if (cmdStr == "cache_status")
                {
                    iTask.showStatus();
                }

                if (cmdStr == "clear")
                {
                    Console.Clear();
                }

                if (cmdStr == "quit")
                {
                    break;
                }
            }
        }
    }
}
