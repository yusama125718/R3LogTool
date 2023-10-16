using System;
using System.IO;
using static R3LogTool.GetLog;

namespace R3LogTool
{
    internal class Start
    {
        static void Main(string[] args)
        {

            string locallow = (System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "Low/VRChat/VRChat");
            string[] texts =  Directory.GetFiles(locallow, "*txt");

            string log = "";

            foreach (string s in texts)
            {
                if (s.Contains("output_log") && String.Compare(s,log) == 1)
                {
                    log = s;
                }
            }

            if (log == "")
            {
                Console.WriteLine("File Not Found");
                Console.ReadKey();
            }
            else
            {
                GetLog getLog = new GetLog();
                // getLog.TimerStart(log);
                string line = getLog.GetLine(log);
                Console.WriteLine(line);
                string data = getLog.ConvertLog(line);
                Console.ReadKey();
            }
        }
    }
}
