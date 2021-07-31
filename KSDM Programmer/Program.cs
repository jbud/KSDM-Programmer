using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

//avrdude.exe -c arduino -p m328p -P COM1 -b 57600 -e -U flash:w:"C:\Users\jbudp\Documents\Arduino\ksdm3_final\ksdm3_final\ksdm3_final.ino.with_bootloader.eightanaloginputs.hex":a 


namespace KSDM_Programmer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

    }
}
