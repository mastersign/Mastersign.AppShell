using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Reflection;
using System.Threading;

namespace de.mastersign.shell
{
    public class ShellHost : PSHost
    {
        private readonly Guid guid = Guid.NewGuid();
        private readonly PSHostUserInterface ui;

        public event EventHandler<ExitEventArgs> ShouldExit;

        public ShellHost(PSHostUserInterface ui)
        {
            this.ui = ui;
        }

        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        public override void EnterNestedPrompt() { }

        public override void ExitNestedPrompt() { }

        public override Guid InstanceId
        {
            get { return guid; }
        }

        public override string Name
        {
            get { return "Mastersign ApplicationCoreShell Host"; }
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override void SetShouldExit(int exitCode)
        {
            if (ShouldExit != null)
            {
                ShouldExit(this, new ExitEventArgs(exitCode));
            }
        }

        public override PSHostUserInterface UI
        {
            get { return ui; }
        }

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
    }
}