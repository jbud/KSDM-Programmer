using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KSDM_Programmer
{
    class exe
    {
        private string port;
        private string input;
        public bool success;
        public bool done;
        public string output;
        private string error;
        private bool launch()
        { 
            Process t = new Process();
            t.StartInfo.FileName = @".\includes\avrdude.exe";
            t.StartInfo.Arguments = " -c arduino -p m328p -P " + port + " -b 57600 -e -D -U flash:w:" + input + ":i";
            t.StartInfo.CreateNoWindow = true;
            t.StartInfo.UseShellExecute = false;
            t.StartInfo.RedirectStandardOutput = true;
            t.StartInfo.RedirectStandardError = true;
            t.EnableRaisingEvents = true;
            System.Diagnostics.Debug.WriteLine("process spawned!");
            t.Exited += new EventHandler(p_Exited);
            try
            {
                t.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            output = t.StandardError.ReadToEnd();
            System.Diagnostics.Debug.WriteLine(output);
            if (output.Contains("verified"))
                return true;
            else
                return false;
        }

        private bool testc()
        {
            Process t = new Process();
            t.StartInfo.FileName = @".\includes\avrdude.exe";
            t.StartInfo.Arguments = "-c arduino -p m8 -P" + port + "-b 57600 -u -q";
            t.StartInfo.CreateNoWindow = true;
            t.StartInfo.UseShellExecute = false;
            t.StartInfo.RedirectStandardOutput = true;
            t.StartInfo.RedirectStandardError = true;
            t.EnableRaisingEvents = true;
            try
            {
                t.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            output = t.StandardError.ReadToEnd();
            System.Diagnostics.Debug.WriteLine(output);

            if (output.Contains("m328p"))
                return true;
            else
                return false;
        }

        private void p_Exited(object sender, EventArgs e)
        {
            done = true;
        }
        public exe(string p, string i, bool test = false)
        {
            port = p;
            input = i;
            if (!test)
                success = launch();
            else
                success = testc();
        }
    }
}
