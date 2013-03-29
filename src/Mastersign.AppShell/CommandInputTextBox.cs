using System;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    internal class CommandInputTextBox : TextBox
    {
        public event EventHandler EnterPressed;

        public event EventHandler HistoryBackRequested;

        public event EventHandler HistoryForwardRequested;

        public event EventHandler TypingLineBreak;

        public event EventHandler TypingEscape;

        public event EventHandler TabPressed;

        public event MouseEventHandler ExternalMouseWheel;

        public event EventHandler CancelRequest;

        public CommandInputTextBox()
        {
            Multiline = true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (ClientRectangle.Contains(e.Location))
            {
                base.OnMouseWheel(e);    
            }
            else
            {
                OnExternalMouseWheel(e);
            }
        }

        protected virtual void OnExternalMouseWheel(MouseEventArgs e)
        {
            if (ExternalMouseWheel != null)
            {
                ExternalMouseWheel(this, e);
            }
        }

        public Runspace Runspace { get; set; }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    OnTypingEscape();
                    return true;
                case Keys.Enter:
                    OnEnterPressed();
                    return true;
                case Keys.Enter | Keys.Shift:
                    OnTypingLineBreak();
                    return true;
                case Keys.Tab:
                    OnTabPressed();
                    return true;
                case Keys.Up:
                    if (!HasMorThanOneLine)
                    {
                        OnHistoryBackRequested();
                        return true;
                    }
                    break;
                case Keys.Up | Keys.Control:
                    OnHistoryBackRequested();
                    return true;
                case Keys.Down:
                    if (!HasMorThanOneLine)
                    {
                        OnHistoryForwardRequested();
                        return true;
                    }
                    break;
                case Keys.Down | Keys.Control:
                    OnHistoryForwardRequested();
                    return true;
                case Keys.C | Keys.Control:
                    OnCancelRequest();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected virtual void OnTypingEscape()
        {
            Text = "";
            if (TypingEscape != null)
            {
                TypingEscape(this, EventArgs.Empty);
            }
        }

        protected virtual void OnTypingLineBreak()
        {
            SelectedText = Environment.NewLine;
            if (TypingLineBreak != null)
            {
                TypingLineBreak(this, EventArgs.Empty);
            }
        }

        protected virtual void OnEnterPressed()
        {
            if (EnterPressed != null)
            {
                EnterPressed(this, EventArgs.Empty);
            }
        }

        protected virtual void OnTabPressed()
        {
            if (TabPressed != null)
            {
                TabPressed(this, EventArgs.Empty);
            }
        }

        protected virtual void OnHistoryBackRequested()
        {
            if (HistoryBackRequested != null)
            {
                HistoryBackRequested(this, EventArgs.Empty);
            }
        }

        protected virtual void OnHistoryForwardRequested()
        {
            if (HistoryForwardRequested != null)
            {
                HistoryForwardRequested(this, EventArgs.Empty);
            }
        }

        protected virtual void OnCancelRequest()
        {
            if (CancelRequest != null)
            {
                CancelRequest(this, EventArgs.Empty);
            }
        }

        private bool HasMorThanOneLine
        {
            get { return Text.Contains(Environment.NewLine); }
        }
    }
}