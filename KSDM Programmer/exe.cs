using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Management;

namespace KSDM_Programmer
{
    class exe
    {
        private string port;
        private string input;
        private List<string> tempFiles = new List<string>();
        private string exePath = @"c:\temp\";
        private string avrdude;
        
        // public static stuff
        public static string type;
        public static string fport;
        
        // public object stuff
        public bool success;
        public bool done;
        public string output;
        
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
                Debug.WriteLine(ex.Message);
                return false;
            }
            Debug.WriteLine("process spawned!");
            if (readFromProc) 
            { 
                output = t.StandardError.ReadToEnd();
                Debug.WriteLine(output);
            }
            return true;
        }

        private bool flashRP2040()
        {

            bool procStatus = spawnProc("cmd.exe", "/K Mode " + port + " baud=1200", false, false);
            if (procStatus)
            {
                Thread.Sleep(2000);                            // wait for windows to discover the Drive

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
                    File.Copy(input, path + "ksdm3.uf2");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
                done = true;
                return true;
            }
            else 
                return false;
        }
        private bool flashAVR()
        {
            extractIncludes();
            bool procStatus = spawnProc(avrdude, " -c arduino -p m328p -P " + port + " -b 57600 -e -u -D -U flash:w:" + input + ":i", true, true);

            if (procStatus)
            {
                deleteExtractedFiles();
                if (output.Contains("verified"))
                    return true;
                else
                    return false;
            }
            else
            {
                deleteExtractedFiles();
                return false;
            }
        }
                
        private void p_Exited(object sender, EventArgs e)
        {
            done = true;
        }
        private void extractIncludes()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resources = asm.GetManifestResourceNames();

            string strip = "KSDM_Programmer.includes.";

            avrdude = exePath + "avrdude.exe";

            foreach (string r in resources)
            {
                if (r.Contains("includes"))
                {
                    string filename = r.Replace(strip, "");

                    Stream stream = GetType().Assembly.GetManifestResourceStream(r);
                    byte[] bytes = new byte[(int)stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    File.WriteAllBytes(exePath + filename, bytes);
                    tempFiles.Add(filename);
                }
            }
        }
        private void deleteExtractedFiles()
        {
            foreach (string f in tempFiles)
            {
                File.Delete(exePath + f);
            }
        }
       
        public static bool findKSDM()
        {
            bool found = false;

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity where DeviceID Like ""USB%"""))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                if (device.GetPropertyValue("DeviceID").ToString().Contains("VID_1A86&PID_7523")) //USB-SERIAL CH340 (COM*)
                {
                    fport = device.GetPropertyValue("Name").ToString().Replace("USB-SERIAL CH340 (", "").Replace(")", "");
                    type = "avr";
                    found = true;
                    break;
                }
                if (device.GetPropertyValue("DeviceID").ToString().Contains("VID_2E8A&PID_000A")) //USB Serial Device (COM*)
                {
                    fport = device.GetPropertyValue("Name").ToString().Replace("USB Serial Device (", "").Replace(")", "");
                    type = "rp2040";
                    found = true;
                    break;
                }
            }
            collection.Dispose();

            return found;
        }

        public exe(string p, string i)
        {
            port = p;
            input = i;
            if (input.Contains(".uf2"))
                success = flashRP2040();
            else
                success = flashAVR();
        }
    }
}
