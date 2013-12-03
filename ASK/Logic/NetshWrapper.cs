using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ASK.Logic
{
    static class NetshWrapper
    {
        public static string invoke(string args)
        {
            Process proc = new Process();

            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "netsh";
            proc.StartInfo.Arguments = args;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(852);

            proc.Start();

            return proc.StandardOutput.ReadToEnd();
        }
    }
}
