using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IDK_API_IMAGE.Services
{
    public class Log
    {
        public static void ILog(string r)
        {
            using (StreamWriter w = File.AppendText("/efs/image_test/log/log.txt"))
            {
                Wrirte(r,w);
            }
        }
        public static void Wrirte(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }
    }
}
