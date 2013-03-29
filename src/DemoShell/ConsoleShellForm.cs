using System;
using System.Windows.Forms;

namespace de.mastersign.shell.demo
{
    public partial class ConsoleShellForm : Form
    {
        public ConsoleShellForm()
        {
            InitializeComponent();
        }

        private void ShellForm_Load(object sender, EventArgs e)
        {
            shell.StartShell();
            shell.ClearBuffer();
            shellControl.UseShell(shell);
        }

        private void shell_Exit(object sender, ExitEventArgs e)
        {
            Environment.ExitCode = e.ExitCode;
            if (InvokeRequired)
            {
                Invoke((MethodInvoker) Close);
            }
            else
            {
                Close();
            }
        }
    }
}