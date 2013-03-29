using System;
using System.ComponentModel;
using System.Management.Automation.Host;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    public partial class GraphicalShell : BaseShell
    {
        private GraphicalUI graphicalUI;

        private GraphicalUI GraphicalUI
        {
            get
            {
                if (graphicalUI == null)
                {
                    graphicalUI = new GraphicalUI(Buffer);
                }
                return graphicalUI;
            }
        }

        public override BaseUI UI
        {
            get { return GraphicalUI; }
        }

        private GraphicalShellControl shellControl;

        public GraphicalShellControl GraphicalShellControl
        {
            get { return shellControl; }
            set
            {
                if (shellControl == value)
                {
                    return;
                }
                if (shellControl != null)
                {
                    shellControl.InputTyped -= NotifyCommandInput;
                    shellControl.CancelRequest -= NotifyCancelRequest;
                }
                shellControl = value;
                GuiSynchronized = shellControl != null;
                if (shellControl != null)
                {
                    shellControl.InputTyped += NotifyCommandInput;
                    shellControl.CancelRequest += NotifyCancelRequest;
                    UpdatePrompt();
                }
            }
        }

        public GraphicalShell()
        {
        }

        public GraphicalShell(IContainer container)
            : base(container)
        {
        }

        public override void StartShell()
        {
            base.StartShell();
            if (GraphicalShellControl != null)
            {
                GraphicalShellControl.PromptText = GetPrompt();
            }
        }

        private void NotifyCommandInput(object sender, CommandInputEventArgs e)
        {
            InvokeAsync(new CommandInfo(e.CommandText) { WriteDecorations = true });
        }

        private void NotifyCancelRequest(object sender, EventArgs e)
        {
            CancelProcessing();
        }

        protected override void StartInvocation(CommandInfo info)
        {
            if (info.WriteDecorations)
            {
                UI.WriteLine(BaseUI.debugColor, UI.RawUI.BackgroundColor,
                    GetPrompt() + info.Source);
            }
            base.StartInvocation(info);
        }

        protected override void EndInvocation(CommandInfo info)
        {
            base.EndInvocation(info);
            if (GraphicalShellControl == null ||
                GraphicalShellControl.IsDisposed)
            {
                return;
            }
            if (info.WriteDecorations)
            {
                UI.WriteLine(BaseUI.verboseColor, UI.RawUI.BackgroundColor,
                    new string('_', Buffer.BufferSize.Width - 1));
            }
            UpdatePrompt();
        }

        protected override void ProcessingStarts()
        {
            if (shellControl == null || !shellControl.IsHandleCreated) { return; }
            shellControl.Invoke((MethodInvoker)(() => { shellControl.CancelButtonEnabled = true; }));
        }

        protected override void ProcessingEnds()
        {
            if (shellControl == null || !shellControl.IsHandleCreated) { return; }
            shellControl.Invoke((MethodInvoker)(() => { shellControl.CancelButtonEnabled = false; }));
        }

        private void UpdatePrompt()
        {
            if (GraphicalShellControl == null) return;

            if (GraphicalShellControl.InvokeRequired)
            {
                GraphicalShellControl.Invoke(
                    (MethodInvoker)
                    (() => { GraphicalShellControl.PromptText = GetPrompt(); }));
            }
            else
            {
                GraphicalShellControl.PromptText = GetPrompt();
            }
        }
    }
}