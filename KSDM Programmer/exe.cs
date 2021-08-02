using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

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

        private bool spawnProc(string filename, string arguments, bool events, bool readFromProc = true)
        {
            Process t = new Process();
            t.StartInfo.FileName = filename;
            t.StartInfo.Arguments = arguments;
            t.StartInfo.CreateNoWindow = true;
            t.StartInfo.UseShellExecute = false;
            t.StartInfo.RedirectStandardOutput = true;
            t.StartInfo.RedirectStandardError = true;
            t.EnableRaisingEvents = events;
            if (events)
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
            System.Diagnostics.Debug.WriteLine("process spawned!");
            if (readFromProc) 
            { 
                output = t.StandardError.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(output);
            }
            return true;
        }

        private bool flashRP2040()
        {

            bool procStatus = spawnProc("cmd.exe", "/K Mode " + port + " baud=1200", false, false);
            if (procStatus)
            {
                System.Threading.Thread.Sleep(2000);
                DriveInfo[] drives = DriveInfo.GetDrives();
                string path = "";
                foreach (DriveInfo d in drives)
                {
                    if (d.VolumeLabel == "RPI-RP2")
                    {
                        path = d.Name;
                        break;
                    }
                }

                try
                {
                    System.IO.File.Copy(input, path + "ksdm3.uf2");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
                done = true;
                return true;


            }
            else 
                return false;
        }
        private bool launch()
        {
            bool procStatus = spawnProc(@".\includes\avrdude.exe", " -c arduino -p m328p -P " + port + " -b 57600 -e -u -D -U flash:w:" + input + ":i", true, true);

            if (procStatus)
            {
                if (output.Contains("verified"))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private bool testc()
        {
            bool procStatus = spawnProc(@".\includes\avrdude.exe", "-c arduino -p m8 -P " + port + " -b 57600 -u -q", false, true);

            if (procStatus)
            {
                if (output.Contains("m328p"))
                    return true;
                else
                    return false;
            }
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
            if (input.Contains(".uf2"))
                success = flashRP2040();
            else if (!test)
                success = launch();
            else
                success = testc();
        }
    }
}
