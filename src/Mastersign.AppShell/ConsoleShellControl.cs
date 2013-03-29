using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management.Automation.Host;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    public partial class ConsoleShellControl : UserControl, IShellControl
    {
        private bool blockRecalcScrollbars;
        private readonly ScrollMessageFilter scrollMessageFilter;

        public ConsoleShellControl()
        {
            InitializeComponent();
            Display.MouseWheel += NotifyMouseWheel;
            Disposed += ConsoleShellControl_Disposed;

            scrollMessageFilter = new ScrollMessageFilter(Display);
            scrollMessageFilter.ScrollSensibleControls.Add(vScrollBar);
            scrollMessageFilter.ScrollSensibleControls.Add(hScrollBar);
            Application.AddMessageFilter(scrollMessageFilter);
        }

        private void ConsoleShellControl_Disposed(object sender, EventArgs e)
        {
            Application.RemoveMessageFilter(scrollMessageFilter);
        }

        private void NotifyMouseWheel(object sender, MouseEventArgs e)
        {
            var wp = Buffer.WindowPosition;
            int ny = wp.Y - (e.Delta > 0 ? 2 : -2);

            if (ny < 0)
            {
                ny = 0;
            }
            if (ny > Buffer.BufferSize.Height - Buffer.WindowSize.Height)
            {
                ny = Buffer.BufferSize.Height - Buffer.WindowSize.Height;
            }
            if (wp.Y == ny)
            {
                return;
            }
            wp.Y = ny;
            Buffer.WindowPosition = wp;
            SaveRecalcScrollbars();
            Display.Invalidate();
        }

        public ConsoleDisplay Display
        {
            get { return shellDisplay; }
        }

        public void UseShell(IShellComponent shell)
        {
            Buffer = shell.Buffer;
        }

        public ConsoleBuffer Buffer
        {
            get { return shellDisplay.Buffer; }
            set
            {
                if (shellDisplay.Buffer == value)
                {
                    return;
                }
                if (shellDisplay.Buffer != null)
                {
                    var b = shellDisplay.Buffer;
                    b.SizeChanged -= Buffer_ScrollbarRelevantEventHandler;
                    //b.WindowPositionChanged -= Buffer_ScrollbarRelevantEventHandler;
                    b.WindowSizeChanged -= Buffer_ScrollbarRelevantEventHandler;
                    b.WriteOperationFinished -= Buffer_ScrollbarRelevantEventHandler;
                }
                shellDisplay.Buffer = value;
                if (shellDisplay.Buffer != null)
                {
                    var b = shellDisplay.Buffer;
                    b.SizeChanged += Buffer_ScrollbarRelevantEventHandler;
                    //b.WindowPositionChanged += Buffer_ScrollbarRelevantEventHandler;
                    b.WindowSizeChanged += Buffer_ScrollbarRelevantEventHandler;
                    b.WriteOperationFinished += Buffer_ScrollbarRelevantEventHandler;
                }
                SaveRecalcScrollbars();
            }
        }

        private void Buffer_ScrollbarRelevantEventHandler(object sender, EventArgs e)
        {
            SaveRecalcScrollbars();
        }

        private void SaveRecalcScrollbars()
        {
            if (blockRecalcScrollbars) { return; }
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RecalcScrollbars);
            }
            else
            {
                RecalcScrollbars();
            }
        }

        private void RecalcScrollbars()
        {
            if (blockRecalcScrollbars) { return; }

            if (Buffer == null)
            {
                vScrollBar.Enabled = false;
                hScrollBar.Enabled = false;
            }
            else
            {
                if (Buffer.BufferSize.Width > Buffer.WindowSize.Width)
                {
                    hScrollBar.Maximum = Buffer.BufferSize.Width - 1;
                    hScrollBar.SmallChange = 1;
                    hScrollBar.LargeChange = Buffer.WindowSize.Width;
                    hScrollBar.Value = Buffer.WindowPosition.X;
                    hScrollBar.Enabled = true;
                }
                else
                {
                    hScrollBar.Enabled = false;
                }
                if (Buffer.BufferSize.Height > Buffer.WindowSize.Height)
                {
                    vScrollBar.Maximum = Buffer.BufferSize.Height - 1;
                    vScrollBar.SmallChange = 1;
                    vScrollBar.LargeChange = Buffer.WindowSize.Height;
                    vScrollBar.Value = Buffer.WindowPosition.Y;
                    vScrollBar.Enabled = true;
                }
                else
                {
                    vScrollBar.Enabled = false;
                }
            }
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }
            blockRecalcScrollbars = true;
            if (Buffer != null)
            {
                Buffer.WindowPosition = new Coordinates(
                    e.NewValue, Buffer.WindowPosition.Y);
            }
            blockRecalcScrollbars = false;
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }
            blockRecalcScrollbars = true;
            if (Buffer != null)
            {
                Buffer.WindowPosition = new Coordinates(
                    Buffer.WindowPosition.X, e.NewValue);
            }
            blockRecalcScrollbars = false;
        }

        #region Wrapper Properties

        public override Color BackColor
        {
            get { return Display.BackColor; }
            set
            {
                Display.BackColor = value;
                base.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get { return Display.ForeColor; }
            set
            {
                Display.ForeColor = value;
                base.ForeColor = value;
            }
        }

        public IDictionary<ConsoleColor, Brush> BackgroundBrushes
        {
            get { return Display.BackgroundBrushes; }
        }

        public IDictionary<ConsoleColor, Brush> ForegroundBrushes
        {
            get { return Display.ForegroundBrushes; }
        }

        public bool ProcessKeyStrokes
        {
            get { return Display.ProcessKeyStrokes; }
            set { Display.ProcessKeyStrokes = value; }
        }

        public int CursorBlinkInterval
        {
            get { return Display.CursorBlinkInterval; }
            set { Display.CursorBlinkInterval = value; }
        }

        public ConsoleDisplay.CursorShowMode CursorMode
        {
            get { return Display.CursorMode; }
            set { Display.CursorMode = value; }
        }

        #endregion

    }
}