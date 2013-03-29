using System;
using System.Windows.Forms;

namespace de.mastersign.shell.demo
{
    internal static class Program
    {
        /// <summary>
        ///  Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ConsoleShellForm());
            Application.Run(new GraphicalShellForm());
        }
    }
}