using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using de.mastersign.shell.Properties;

namespace de.mastersign.shell
{
    public partial class GraphicalShellControl : UserControl, IShellControl
    {
        private const int LINE_HEIGHT = 13;
        private const int BORDER_HEIGHT = 4;
        private const int MULTILINE_LINES = 4;

        public History History { get; private set; }
        private History.Navigator HistoryNavigator { get; set; }

        private bool multilineModeActive;

        public bool MultilineModeActive
        {
            get { return multilineModeActive; }
            private set
            {
                if (multilineModeActive == value)
                {
                    return;
                }
                multilineModeActive = value;
                if (multilineModeActive)
                {
                    SuspendLayout();
                    commandInputTextBox.Font = new Font(commandInputTextBox.Font.FontFamily, 10f);
                    commandInputTextBox.Height =
                        BORDER_HEIGHT + LINE_HEIGHT * MULTILINE_LINES;
                    commandInputTextBox.ScrollBars = ScrollBars.Vertical;
                    commandInputTextBox.WordWrap = true;
                    commandInputTextBox.SelectionStart = commandInputTextBox.Text.Length;
                    panelInput.Height = commandInputTextBox.Height + 8;
                    ResumeLayout(true);
                }
                else
                {
                    SuspendLayout();
                    commandInputTextBox.Font = new Font(commandInputTextBox.Font.FontFamily, 11f);
                    commandInputTextBox.Height =
                        BORDER_HEIGHT + LINE_HEIGHT;
                    commandInputTextBox.ScrollBars = ScrollBars.None;
                    commandInputTextBox.WordWrap = false;
                    panelInput.Height = commandInputTextBox.Height + 10;
                    ResumeLayout(true);
                }
            }
        }

        public event EventHandler<CommandInputEventArgs> InputTyped;
        public event EventHandler CancelRequest;

        public GraphicalShellControl()
        {
            InitializeComponent();
            CancelButtonEnabled = false;
            History = new History();
            HistoryNavigator = History.GetNavigator();
        }

        protected virtual void OnCancelRequest()
        {
            if (CancelRequest != null)
            {
                CancelRequest(this, EventArgs.Empty);
            }
        }

        public string PromptText
        {
            get { return txtPrompt.Text; }
            set { txtPrompt.Text = value; }
        }

        private void commandInputTextBox_EnterPressed(object sender, EventArgs e)
        {
            string cmdText = commandInputTextBox.Text;
            if (InputTyped != null)
            {
                InputTyped(this, new CommandInputEventArgs(cmdText));
            }
            HistoryNavigator.CompleteOperationWith(cmdText);
            commandInputTextBox.Text = "";
            MultilineModeActive = false;
        }

        private void commandInputTextBox_HistoryDownRequested(object sender, EventArgs e)
        {
            if (!HistoryNavigator.CanMoveForward)
            {
                return;
            }
            commandInputTextBox.Text = HistoryNavigator.MoveForward();
            commandInputTextBox.SelectionStart = commandInputTextBox.Text.Length;
        }

        private void commandInputTextBox_HistoryUpRequested(object sender, EventArgs e)
        {
            if (!HistoryNavigator.CanMoveBack)
            {
                return;
            }
            commandInputTextBox.Text = HistoryNavigator.MoveBack();
            commandInputTextBox.SelectionStart = commandInputTextBox.Text.Length;
        }

        private void commandInputTextBox_TypingLineBreak(object sender, EventArgs e)
        {
            MultilineModeActive = true;
        }

        private void commandInputTextBox_TypingEscape(object sender, EventArgs e)
        {
            MultilineModeActive = false;
        }

        private void commandInputTextBox_CancelRequest(object sender, EventArgs e)
        {
            OnCancelRequest();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancelRequest();
        }

        public void UseShell(IShellComponent shell)
        {
            Buffer = shell.Buffer;
        }

        private void console_Enter(object sender, EventArgs e)
        {
            commandInputTextBox.Focus();
        }

        public bool CancelButtonEnabled
        {
            get { return btnCancel.Enabled; }
            set
            {
                btnCancel.Enabled = value;
                btnCancel.BackgroundImage = value ? Resources.CancelImage : null;
            }
        }

        #region Wrapper Properties

        public ConsoleBuffer Buffer
        {
            get { return console.Buffer; }
            set { console.Buffer = value; }
        }

        public override Color BackColor
        {
            get { return console.BackColor; }
            set { console.BackColor = value; }
        }

        public override Color ForeColor
        {
            get { return console.ForeColor; }
            set { console.ForeColor = value; }
        }

        public IDictionary<ConsoleColor, Brush> BackgroundBrushes
        {
            get { return console.BackgroundBrushes; }
        }

        public IDictionary<ConsoleColor, Brush> ForegroundBrushes
        {
            get { return console.ForegroundBrushes; }
        }

        public bool ProcessKeyStrokes
        {
            get { return console.ProcessKeyStrokes; }
            set { console.ProcessKeyStrokes = value; }
        }

        public int CursorBlinkInterval
        {
            get { return console.CursorBlinkInterval; }
            set { console.CursorBlinkInterval = value; }
        }

        public ConsoleDisplay.CursorShowMode CursorMode
        {
            get { return console.CursorMode; }
            set { console.CursorMode = value; }
        }

        #endregion

    }

    public class CommandInputEventArgs : EventArgs
    {
        public string CommandText { get; private set; }

        public CommandInputEventArgs(string commandText)
        {
            CommandText = commandText;
        }
    }
}