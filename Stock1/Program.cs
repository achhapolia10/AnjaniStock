using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock1
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
            string text = @"Provider = Microsoft.ACE.OLEDB.12.0;Data Source = " + '"';
            string location = System.IO.File.ReadAllText(Application.StartupPath+"\\conn.txt");
            text = text + location + '"' ;
            Properties.Settings.Default.Database1ConnectionString1 =
                text;
            Application.Run(new Form1());
            
        }
    }
}
