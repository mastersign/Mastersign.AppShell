using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Management.Automation.Host;
using System.Windows.Forms;
using Rectangle=System.Drawing.Rectangle;
using Size=System.Drawing.Size;

namespace de.mastersign.shell
{
    public partial class ConsoleDisplay : UserControl
    {
        public enum CursorShowMode
        {
            Show,
            Hide,
            ShowFocused,
        }

        private readonly string MEASURE_TEXT = new string('_', 80);
        private const float HORIZONTAL_SHIFT = -1.5f;

        private ConsoleBuffer buffer;

        public bool UseImediateRefresh { get; set; }

        public bool DefinesWindowSize { get; set; }

        private bool processKeyStrokes;

        private bool cursorSwitch;
        private CursorShowMode cursorMode;

        private float charWidth, charHeight;

        private KeyEventArgs lastKeyEventArgs;
        private KeyPressEventArgs lastKeyPressEventArgs;

        private readonly Dictionary<ConsoleColor, Brush> backgroundBrushes
            = new Dictionary<ConsoleColor, Brush>();

        private readonly Dictionary<ConsoleColor, Brush> foregroundBrushes
            = new Dictionary<ConsoleColor, Brush>();

        public ConsoleDisplay()
        {
            DefinesWindowSize = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.Opaque, true);

            InitializeComponent();

            InitColors();
        }

        private void InitColors()
        {
            var bb = backgroundBrushes;
            var fb = foregroundBrushes;
            bb[ConsoleColor.Black] = new SolidBrush(Color.FromArgb(0, 0, 0));
            bb[ConsoleColor.Blue] = new SolidBrush(Color.FromArgb(0, 0, 255));
            bb[ConsoleColor.Cyan] = new SolidBrush(Color.FromArgb(0, 210, 240));
            bb[ConsoleColor.DarkBlue] = new SolidBrush(Color.FromArgb(0, 0, 112));
            bb[ConsoleColor.DarkCyan] = new SolidBrush(Color.FromArgb(0, 112, 128));
            bb[ConsoleColor.DarkGray] = new SolidBrush(Color.FromArgb(64, 64, 64));
            bb[ConsoleColor.DarkGreen] = new SolidBrush(Color.FromArgb(0, 96, 32));
            bb[ConsoleColor.DarkMagenta] = new SolidBrush(Color.FromArgb(112, 0, 128));
            bb[ConsoleColor.DarkRed] = new SolidBrush(Color.FromArgb(128, 0, 0));
            bb[ConsoleColor.DarkYellow] = new SolidBrush(Color.FromArgb(128, 92, 0));
            bb[ConsoleColor.Gray] = new SolidBrush(Color.FromArgb(148, 148, 148));
            bb[ConsoleColor.Green] = new SolidBrush(Color.FromArgb(0, 160, 64));
            bb[ConsoleColor.Magenta] = new SolidBrush(Color.FromArgb(255, 0, 255));
            bb[ConsoleColor.Red] = new SolidBrush(Color.FromArgb(255, 0, 0));
            bb[ConsoleColor.White] = new SolidBrush(Color.FromArgb(255, 255, 255));
            bb[ConsoleColor.Yellow] = new SolidBrush(Color.FromArgb(232, 180, 0));

            foreach (var k in bb.Keys)
            {
                fb[k] = bb[k];
            }
        }

        public IDictionary<ConsoleColor, Brush> BackgroundBrushes
        {
            get { return backgroundBrushes; }
        }

        public IDictionary<ConsoleColor, Brush> ForegroundBrushes
        {
            get { return foregroundBrushes; }
        }

        public bool ProcessKeyStrokes
        {
            get { return processKeyStrokes; }
            set
            {
                if (processKeyStrokes == value)
                {
                    return;
                }
                processKeyStrokes = value;
            }
        }

        public int CursorBlinkInterval
        {
            get { return timerCursor.Interval; }
            set { timerCursor.Interval = value; }
        }

        public CursorShowMode CursorMode
        {
            get { return cursorMode; }
            set
            {
                cursorMode = value;
                InvalidateCursor();
            }
        }

        public ConsoleBuffer Buffer
        {
            get { return buffer; }
            set
            {
                if (buffer != null)
                {
                    buffer.SizeChanged -= buffer_InvalidatingEventHandler;
                    buffer.ContentChanged -= buffer_InvalidatingEventHandler;
                    buffer.CursorPositionChanged -= buffer_InvalidatingEventHandler;
                    buffer.CursorHeightChanged -= buffer_InvalidatingEventHandler;
                    buffer.WindowSizeChanged -= buffer_InvalidatingEventHandler;
                    buffer.WindowPositionChanged -= buffer_InvalidatingEventHandler;

                    buffer.WriteOperationFinished -= buffer_WriteOperationFinished;

                    buffer.GatherSize -= buffer_GatherSize;
                }
                buffer = value;
                if (buffer != null)
                {
                    buffer.SizeChanged += buffer_InvalidatingEventHandler;
                    buffer.ContentChanged += buffer_InvalidatingEventHandler;
                    buffer.CursorPositionChanged += buffer_InvalidatingEventHandler;
                    buffer.CursorHeightChanged += buffer_InvalidatingEventHandler;
                    buffer.WindowSizeChanged += buffer_InvalidatingEventHandler;
                    buffer.WindowPositionChanged += buffer_InvalidatingEventHandler;

                    buffer.WriteOperationFinished += buffer_WriteOperationFinished;

                    buffer.GatherSize += buffer_GatherSize;

                    ShellDisplay_Resize(this, new EventArgs());
                }
            }
        }

        private void buffer_GatherSize(object sender, SizeGatheringEventArgs e)
        {
            using (var g = CreateGraphics())
            {
                var size = MeasureWindowSize(g, MaximumSize);
                e.NotifyMaxSize(
                    new System.Management.Automation.Host.Size(
                        size.Width, size.Height));
            }
        }

        private Size MeasureCurrentSize(Graphics g)
        {
            return MeasureWindowSize(
                g, new Size(
                       ClientSize.Width - Padding.Left - Padding.Right,
                       ClientSize.Height - Padding.Top - Padding.Bottom));
        }

        private Size MeasureWindowSize(Graphics g, Size windowSize)
        {
            if (charWidth == 0f || charHeight == 0f)
            {
                MeasureChar(g);
            }
            return new Size(
                (int) (windowSize.Width/charWidth),
                (int) (windowSize.Height/charHeight));
        }

        private void MeasureChar(Graphics g)
        {
            CharacterRange[] ranges = { new CharacterRange(0, MEASURE_TEXT.Length) };
            var stringFormat = new StringFormat();
            stringFormat.SetMeasurableCharacterRanges(ranges);
            var regions = g.MeasureCharacterRanges(MEASURE_TEXT, Font, 
                new RectangleF(0f, 0f, float.MaxValue, float.MaxValue),
                stringFormat);

            //charWidth = g.MeasureString(MEASURE_TEXT, Font).Width/MEASURE_TEXT.Length;
            charWidth = (float)Math.Round(regions[0].GetBounds(g).Width / (MEASURE_TEXT.Length));
            charHeight = Font.GetHeight(g);
        }

        private Rectangle CalcCharRegion(Coordinates position)
        {
            var b = Buffer;
            int wx = position.X - b.WindowPosition.X;
            int wy = position.Y - b.WindowPosition.Y;
            float x = wx*charWidth + Padding.Left;
            float y = wy*charHeight + Padding.Top;
            return new Rectangle(
                (int) Math.Floor(x - 0.5),
                (int) Math.Floor(y - 0.5),
                (int) Math.Ceiling(charWidth + 1),
                (int) Math.Ceiling(charHeight + 1));
        }

        private void buffer_InvalidatingEventHandler(object sender, EventArgs e)
        {
            if (!buffer.IsInsideOfAWritingOperation)
            {
                Invalidate();
            }
        }

        private void buffer_WriteOperationFinished(object sender, EventArgs e)
        {
            if (UseImediateRefresh)
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker) Refresh);
                }
                else
                {
                    Refresh();
                }
            }
            else
            {
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DesignMode)
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (buffer == null)
            {
                return;
            }

            var b = Buffer;
            var bbs = BackgroundBrushes;
            var fbs = ForegroundBrushes;
            var fb = fbs[b.ForegroundColor];
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.FillRectangle(bbs[b.BackgroundColor], ClientRectangle);

            if (charWidth == 0f)
            {
                MeasureChar(g);
            }

            int wW = b.WindowSize.Width;
            int wH = b.WindowSize.Height;
            for (int l = b.WindowPosition.Y;
                 l < wH + b.WindowPosition.Y && l < b.BufferSize.Height;
                 l++)
            {
                for (int c = b.WindowPosition.X;
                     c < wW + b.WindowPosition.X && c < b.BufferSize.Width;
                     c++)
                {
                    float px = (c - b.WindowPosition.X)*charWidth + Padding.Left;
                    float py = (l - b.WindowPosition.Y)*charHeight + Padding.Top;
                    var cell = b[l, c];
                    if (cell.BackgroundColor != b.BackgroundColor)
                    {
                        g.FillRectangle(
                            bbs[cell.BackgroundColor],
                            px, py, charWidth, charHeight);
                    }
                    if (cell.Character != ' ')
                    {
                        if (cell.ForegroundColor != b.ForegroundColor)
                        {
                            g.DrawString(
                                new String(cell.Character, 1),
                                Font, fbs[cell.ForegroundColor], px + HORIZONTAL_SHIFT, py);
                        }
                        else
                        {
                            g.DrawString(
                                new String(cell.Character, 1),
                                Font, fb, px + HORIZONTAL_SHIFT, py);
                        }
                    }
                    if (b.CursorPosition.X == c &&
                        b.CursorPosition.Y == l &&
                        (CursorMode == CursorShowMode.Show ||
                         (CursorMode == CursorShowMode.ShowFocused && Focused)) &&
                        cursorSwitch)
                    {
                        if (b.CursorSize == 0)
                        {
                            g.DrawString("_", Font, fb, px, py);
                        }
                        else
                        {
                            g.FillRectangle(
                                fb,
                                (float) Math.Ceiling(px),
                                (float) Math.Ceiling(py + (charHeight*((100f - b.CursorSize)/100f))),
                                (float) Math.Floor(charWidth),
                                (float) Math.Floor(charHeight*(b.CursorSize/100f)));
                        }
                    }
                }
            }

            //if (Focused)
            //{
            //    g.DrawRectangle(
            //        SystemPens.Highlight, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            //}
        }

        private void ShellDisplay_Resize(object sender, EventArgs e)
        {
            UpdateBufferWindowSize();
            Invalidate();
        }

        private void UpdateBufferWindowSize()
        {
            if (DesignMode || Buffer == null || !DefinesWindowSize)
            {
                return;
            }
            using (var g = CreateGraphics())
            {
                var size = MeasureCurrentSize(g);
                Buffer.WindowSize = new System.Management.Automation.Host.Size(
                    size.Width, size.Height);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Tab ||
                 keyData == Keys.Left ||
                 keyData == Keys.Right ||
                 keyData == Keys.Up ||
                 keyData == Keys.Down ||
                 keyData == Keys.PageUp ||
                 keyData == Keys.PageDown) &&
                Buffer != null &&
                ProcessKeyStrokes)
            {
                Buffer.NotifyKeyEvent(new KeyInfo(
                                          (int) keyData,
                                          (char) 0,
                                          KeyHelper.BuildControlKeyStates(keyData),
                                          true));
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShellDisplay_KeyDown(object sender, KeyEventArgs e)
        {
            lastKeyEventArgs = e;
            if (Buffer != null && ProcessKeyStrokes && !KeyHelper.IsPrintable(e.KeyCode))
            {
                Buffer.NotifyKeyEvent(KeyHelper.BuildKeyInfo(e, true));
            }
            e.Handled = true;
        }

        private void ShellDisplay_KeyPress(object sender, KeyPressEventArgs e)
        {
            lastKeyPressEventArgs = e;
            if (Buffer != null && ProcessKeyStrokes)
            {
                Buffer.NotifyKeyEvent(KeyHelper.BuildKeyInfo(lastKeyEventArgs, e, true));
            }
            e.Handled = true;
        }

        private void ShellDisplay_KeyUp(object sender, KeyEventArgs e)
        {
            if (Buffer != null && ProcessKeyStrokes)
            {
                if (lastKeyPressEventArgs != null)
                {
                    Buffer.NotifyKeyEvent(
                        KeyHelper.BuildKeyInfo(e, lastKeyPressEventArgs, false));
                }
                else
                {
                    Buffer.NotifyKeyEvent(KeyHelper.BuildKeyInfo(e, false));
                }
            }
            lastKeyEventArgs = null;
            lastKeyPressEventArgs = null;
            e.Handled = true;
        }

        private void timerCursor_Tick(object sender, EventArgs e)
        {
            cursorSwitch = !cursorSwitch;
            InvalidateCursor();
        }

        private void InvalidateCursor()
        {
            if (DesignMode || Buffer == null || !Visible)
            {
                return;
            }
            var cr = CalcCharRegion(Buffer.CursorPosition);
            if (ClientRectangle.IntersectsWith(cr))
            {
                Invalidate(cr);
            }
        }

        private void ShellDisplay_FontChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            MeasureChar(CreateGraphics());
        }

        private void ShellDisplay_VisibleChanged(object sender, EventArgs e)
        {
            if (DesignMode) return;

            timerCursor.Enabled = Visible;
            if (Visible)
            {
                UpdateBufferWindowSize();
            }
        }
    }
}