using System;
using System.ComponentModel;
using System.Management.Automation.Host;
using System.Threading;

namespace de.mastersign.shell
{
    public partial class ConsoleShell : BaseShell
    {
        private ConsoleUI consoleUI;

        private ConsoleUI ConsoleUI
        {
            get
            {
                if (consoleUI == null)
                {
                    consoleUI = new ConsoleUI(Buffer);
                }
                return consoleUI;
            }
        }

        public override BaseUI UI
        {
            get { return ConsoleUI; }
        }

        public ConsoleShell()
        {
        }

        public ConsoleShell(IContainer container)
            : base(container)
        {
        }

        public override void StartShell()
        {
            base.StartShell();
            new Thread(MainLoop).Start();
        }

        private void MainLoop()
        {
            while (!exitFlag)
            {
                PromptForCommand();
            }
        }

        private void PromptForCommand()
        {
            string cmd;
            try
            {
                UI.Write(GetPrompt());
                cmd = UI.ReadLine();
            }
            catch (OperationCanceledException)
            {
                exitFlag = true;
                return;
            }

            InvokeAsync(new CommandInfo(cmd));
        }
    }
}