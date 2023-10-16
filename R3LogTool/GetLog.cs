using System;
using System.IO;
using System.Text;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace R3LogTool
{
    internal class GetLog
    {
        string logpath = "";
        Timer timer = new Timer(1000);

        public void TimerStart(string log)
        {
            logpath = log;


            timer.Elapsed += (sender, e) =>
            {
                string line = GetLine(logpath);
                if (line == "") return;
                string data = ConvertLog(line);
            };

            timer.Start();
        }

        public void TimerStop()
        {
            timer.Stop();
            Console.ReadKey();
        }

        public string GetLine(String filepath)
        {
            int BUFFER_SIZE = 32; // バッファーサイズ(あえて小さく設定)
            int lineCountToWrite = 30; // 探索行数
            var buffer = new byte[BUFFER_SIZE];
            var foundCount = 0;

            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                // 検索ブロック位置の繰り返し
                for (var i = 0; ; i++)
                {
                    if (fs.Length <= i * BUFFER_SIZE)
                    {
                        // ファイルの先頭まで達した場合
                        Console.WriteLine("NOT FOUND");
                        string log = "";

                        using (var sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            log = sr.ReadToEnd();
                        }
                        Console.ReadKey();
                        return log;
                    }

                    // ブロック開始位置に移動
                    var offset = Math.Min((int)fs.Length, (i + 1) * BUFFER_SIZE);
                    fs.Seek(-offset, SeekOrigin.End);

                    // ブロックの読み込み
                    var readLength = offset - BUFFER_SIZE * i;
                    for (var j = 0; j < readLength; j += fs.Read(buffer, j, readLength - j)) ;

                    // ブロック内の改行コードの検索
                    for (var k = readLength - 1; k >= 0; k--)
                    {
                        if (buffer[k] == 0x0A)
                        {
                            var sr = new System.IO.StreamReader(fs, Encoding.UTF8);
                            fs.Seek(k + 3, SeekOrigin.Current);
                            string line = sr.ReadLine();
                            if (line != null &&  line.Contains("XDR3_DBG"))
                            {
                                // XrossDiscのログだった場合
                                Console.ReadKey();
                                return line;
                            }
                            foundCount++;
                            if (foundCount == lineCountToWrite)
                            {
                                // 所定の行数が見つかった場合
                                Console.ReadKey();
                                return "";
                            }
                        }
                    }
                }
            }
        }

        public string ConvertLog(string log)
        {
            JObject json = JObject.Parse(log);
            Console.WriteLine(json);
            foreach (var e in json)
            {
                Console.WriteLine(e);
            }
            return log;
        }
    }
}
