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
        { //"C:\Users\jbudp\AppData\Local\Arduino15\packages\arduino\tools\avrdude\6.3.0-arduino17/bin/avrdude" "-CC:\Users\jbudp\AppData\Local\Arduino15\packages\arduino\tools\avrdude\6.3.0-arduino17/etc/avrdude.conf" -v  -patmega328p -carduino "-PCOM9" -b57600 -D "-Uflash:w:C:\Users\jbudp\AppData\Local\Temp\arduino-sketch-91D19F855BEE8DD1407C94EF462240BD/Blink.ino.hex:i"
            Process t = new Process();
            t.StartInfo.FileName = @".\avrdude.exe";
            t.StartInfo.Arguments = " -c arduino -p m328p -v -P " + port + " -b 57600 -e -D -U flash:w:" + input + ":i";
            t.StartInfo.CreateNoWindow = true;
            t.StartInfo.UseShellExecute = false;
            t.StartInfo.RedirectStandardOutput = true;
            t.StartInfo.RedirectStandardError = true;
            t.EnableRaisingEvents = true;
            System.Diagnostics.Debug.WriteLine("hi");
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
            return true;
        }
        private void p_Exited(object sender, EventArgs e)
        {
            done = true;
        }
        public exe(string p, string i)
        {
            port = p;
            input = i;
            success = launch();
        }
    }
}
