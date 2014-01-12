using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace NetworkManager
{
    public class Netsh
    {
        public static string invoke(string parameters)
        {
            Process proc = new Process();

            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "netsh";
            proc.StartInfo.Arguments = parameters;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(852);

            proc.Start();

            return proc.StandardOutput.ReadToEnd();
        }

        public static IList<string> GetLines(string str)
        {
            return str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        public static Tuple<string, string> GetTuple(string line, char c = ':')
        {
            var strings = line.Split(c);
            return new Tuple<string, string>(strings[0].Trim(), strings[1].Trim());
        }
    }
}
