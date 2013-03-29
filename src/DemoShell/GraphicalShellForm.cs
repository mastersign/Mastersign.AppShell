using System;
using System.Windows.Forms;
using System.IO;

namespace de.mastersign.shell.demo
{
    public partial class GraphicalShellForm : Form
    {
        public GraphicalShellForm()
        {
            InitializeComponent();
        }

        private void GraphicalShellForm_Load(object sender, EventArgs e)
        {
            shell.StartShell();
            shell.SetVariable("form", this);

            shell.GraphicalShellControl = shellControl;
            shellControl.UseShell(shell);
            shell.MoveBufferWindowPosition(0, 0);

            Show();
            Application.DoEvents();

            var path = Path.Combine(Application.StartupPath, "demo.ps1");
            var cmd = new CommandInfo(path,
                new object[] { "##input##", 77 }) { LocalScope = true, RetrieveOutput = true, Callback = Callback };
            shell.InvokeSync(cmd);
            shell.InvokeAsync(new CommandInfo("dir"));
            MessageBox.Show(this, "SyncCall finished:\n" + cmd.Output);
        }

        private void Callback(CommandInfo info)
        {
            MessageBox.Show(string.Format("Boot-Script '{0}' fertig.", info.Source));
        }

        private void shell_Exit(object sender, ExitEventArgs e)
        {
            Environment.ExitCode = e.ExitCode;
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)Close);
            }
            else
            {
                Close();
            }
        }
    }
}