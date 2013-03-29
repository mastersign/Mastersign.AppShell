using System;
using System.ComponentModel;
using System.Management.Automation.Host;
using System.Threading;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    public partial class ConsoleBuffer : PSHostRawUserInterface, IComponent
    {
        #region Component

        public void Dispose()
        {
            IsDisposed = true;
            inputEvent.Set();
            if (Disposed != null)
            {
                Disposed(this, new EventArgs());
            }
        }

        public bool IsDisposed { get; private set; }

        public event EventHandler Disposed;

        public ISite Site { get; set; }

        #endregion

        private bool initialized;

        public event EventHandler<SizeGatheringEventArgs> GatherSize;

        public ConsoleBufferWriter Writer { get; private set; }
        public ConsoleBufferReader Reader { get; private set; }

        public ConsoleBuffer()
        {
            InitializeComponent();
            Initialize();
        }

        public ConsoleBuffer(IContainer container)
        {
            InitializeComponent();
            Initialize();

            container.Add(this);
        }

        private void Initialize()
        {
            CursorSize = 20;
            BufferSize = new Size(100, 500);
            Writer = new ConsoleBufferWriter(this);
            Reader = new ConsoleBufferReader(this);
            ForegroundColor = ConsoleColor.DarkGray;
            BackgroundColor = ConsoleColor.White;
            initialized = true;
        }

        #region Content

        private BufferCell[][] buffer;
        private int topLine;

        public BufferCell this[int line, int column]
        {
            get { return buffer[RelativeLine(line)][column]; }
            set { buffer[RelativeLine(line)][column] = value; }
        }

        public BufferCell this[Coordinates coordinates]
        {
            get { return buffer[RelativeLine(coordinates.Y)][coordinates.X]; }
            set { buffer[RelativeLine(coordinates.Y)][coordinates.X] = value; }
        }

        public event EventHandler ContentChanged;

        protected void OnContentChanged()
        {
            if (!initialized)
            {
                return;
            }
            if (ContentChanged != null)
            {
                ContentChanged(this, new EventArgs());
            }
        }

        public void ScrollOneLineUp(BufferCell fill)
        {
            topLine++;
            if (topLine >= BufferSize.Height)
            {
                topLine = 0;
            }
            FillLine(bufferSize.Height - 1, fill);
        }

        public void ScrollOneLineUp()
        {
            ScrollOneLineUp(
                new BufferCell(
                    ' ',
                    ForegroundColor, backgroundColor, BufferCellType.Complete));
        }

        protected int RelativeLine(int line)
        {
            return (line + topLine)%bufferSize.Height;
        }

        public void ClearContent()
        {
            SetBufferContents(
                new Rectangle(0, 0, BufferSize.Width, BufferSize.Height),
                new BufferCell(' ', ForegroundColor, BackgroundColor, BufferCellType.Complete));
        }

        public void FillRand()
        {
            var r = new Random();
            for (int y = 0; y < BufferSize.Height; y++)
            {
                for (int x = 0; x < BufferSize.Width; x++)
                {
                    buffer[y][x] = new BufferCell(
                        (char) r.Next(32, 127),
                        ForegroundColor, BackgroundColor, BufferCellType.Complete);
                }
            }
            OnContentChanged();
        }

        public void SetLine(int line, string text)
        {
            int rl = RelativeLine(line);
            if (text.Length > BufferSize.Width)
            {
                throw new ArgumentException();
            }
            for (int i = 0; i < text.Length; i++)
            {
                buffer[rl][i] = new BufferCell(
                    text[i],
                    ForegroundColor, BackgroundColor, BufferCellType.Complete);
            }
            for (int i = text.Length; i < BufferSize.Width; i++)
            {
                buffer[rl][i] = new BufferCell(
                    ' ',
                    ForegroundColor, BackgroundColor, BufferCellType.Complete);
            }
            OnContentChanged();
        }

        public void FillLine(int line, BufferCell fill)
        {
            int rl = RelativeLine(line);
            for (int i = 0; i < BufferSize.Width; i++)
            {
                buffer[rl][i] = fill;
            }
            OnContentChanged();
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            var ret = new BufferCell[
                rectangle.Bottom - rectangle.Top,
                rectangle.Right - rectangle.Left];

            for (int l = rectangle.Top; l < rectangle.Bottom; l++)
            {
                int rl = RelativeLine(l);
                for (int c = rectangle.Left; c < rectangle.Right; c++)
                {
                    ret[l - rectangle.Top, c - rectangle.Left] = buffer[rl][c];
                }
            }
            return ret;
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            if (rectangle.Left == -1 &&
                rectangle.Top == -1 &&
                rectangle.Right == -1 &&
                rectangle.Bottom == -1)
            {
                ClearContent();
                return;
            }
            for (int l = rectangle.Top; l < rectangle.Bottom; l++)
            {
                int rl = RelativeLine(l);
                for (int c = rectangle.Left; c < rectangle.Right; c++)
                {
                    buffer[rl][c] = fill;
                }
            }
            OnContentChanged();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            for (int l = 0; l < contents.GetLength(0); l++)
            {
                int rl = RelativeLine(origin.Y + l);
                for (int c = 0; c < contents.GetLength(1); c++)
                {
                    buffer[rl][origin.X + c] = contents[c, l];
                }
            }
            OnContentChanged();
        }

        public override void ScrollBufferContents(
            Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            var org = GetBufferContents(source);
            SetBufferContents(destination, org);
            // TODO clip und fill werden nicht berücksichtigt
        }

        #endregion

        #region BufferSize

        private Size bufferSize;

        public override Size BufferSize
        {
            get { return bufferSize; }
            set
            {
                if (bufferSize.Equals(value))
                {
                    return;
                }
                bufferSize = value;

                buffer = new BufferCell[bufferSize.Height][];
                for (int line = 0; line < bufferSize.Height; line++)
                {
                    buffer[line] = new BufferCell[bufferSize.Width];
                }
                topLine = 0;

                OnSizeChanged();
            }
        }

        public event EventHandler SizeChanged;

        protected void OnSizeChanged()
        {
            if (!initialized)
            {
                return;
            }
            if (SizeChanged != null)
            {
                SizeChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Cursor

        private Coordinates cursorPosition;

        public override Coordinates CursorPosition
        {
            get { return cursorPosition; }
            set
            {
                if (cursorPosition.Equals(value))
                {
                    return;
                }
                if (value.X < 0 || value.Y < 0 ||
                    value.X >= BufferSize.Width ||
                    value.Y >= BufferSize.Height)
                {
                    throw new ArgumentException();
                }

                cursorPosition = value;

                OnCursorPositionChanged();
            }
        }

        public event EventHandler CursorPositionChanged;

        protected void OnCursorPositionChanged()
        {
            if (!initialized)
            {
                return;
            }
            if (CursorPositionChanged != null)
            {
                CursorPositionChanged(this, new EventArgs());
            }
        }

        private int cursorSize;

        public override int CursorSize
        {
            get { return cursorSize; }
            set
            {
                if (cursorSize == value)
                {
                    return;
                }
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }

                cursorSize = value;

                OnCursorHeightChanged();
            }
        }

        public event EventHandler CursorHeightChanged;

        protected void OnCursorHeightChanged()
        {
            if (!initialized)
            {
                return;
            }
            if (CursorHeightChanged != null)
            {
                CursorHeightChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Window

        private Coordinates windowPosition;

        public override Coordinates WindowPosition
        {
            get { return windowPosition; }
            set
            {
                if (windowPosition.Equals(value))
                {
                    return;
                }
                windowPosition = value;
                OnWindowPositionChanged();
            }
        }

        public event EventHandler WindowPositionChanged;

        protected void OnWindowPositionChanged()
        {
            if (!initialized)
            {
                return;
            }
            if (WindowPositionChanged != null)
            {
                WindowPositionChanged(this, new EventArgs());
            }
        }

        private Size windowSize;

        public override Size WindowSize
        {
            get { return windowSize; }
            set
            {
                if (windowSize.Equals(value))
                {
                    return;
                }
                windowSize = value;
                OnWindowSizeChanged();
            }
        }

        public event EventHandler WindowSizeChanged;

        protected void OnWindowSizeChanged()
        {
            if (WindowSizeChanged != null)
            {
                WindowSizeChanged(this, new EventArgs());
            }
        }

        private string windowTitle;

        public override string WindowTitle
        {
            get { return windowTitle; }
            set
            {
                if (windowTitle == value)
                {
                    return;
                }
                windowTitle = value;
                OnWindowTitleChanged();
            }
        }

        public event EventHandler WindowTitleChanged;

        protected void OnWindowTitleChanged()
        {
            if (WindowTitleChanged != null)
            {
                WindowTitleChanged(this, new EventArgs());
            }
        }

        public override Size MaxPhysicalWindowSize
        {
            get
            {
                var sgea = new SizeGatheringEventArgs();
                if (GatherSize != null)
                {
                    GatherSize(this, sgea);
                }
                return sgea.MaxSize;
            }
        }

        public override Size MaxWindowSize
        {
            get
            {
                var maxPhysicalSize = MaxPhysicalWindowSize;
                var retSize = new Size(
                    Math.Min(bufferSize.Width, maxPhysicalSize.Width),
                    Math.Min(bufferSize.Height, maxPhysicalSize.Height));
                return retSize;
            }
        }

        #endregion

        #region Color

        private ConsoleColor backgroundColor;

        public override ConsoleColor BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if (backgroundColor.Equals(value))
                {
                    return;
                }
                backgroundColor = value;
                OnBackgroundColorChanged();
            }
        }

        public event EventHandler BackgroundColorChanged;

        protected void OnBackgroundColorChanged()
        {
            if (BackgroundColorChanged != null)
            {
                BackgroundColorChanged(this, new EventArgs());
            }
        }

        private ConsoleColor foregroundColor;

        public override ConsoleColor ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                if (foregroundColor.Equals(value))
                {
                    return;
                }
                foregroundColor = value;
                OnForegroundColorChanged();
            }
        }

        public event EventHandler ForegroundColorChanged;

        protected void OnForegroundColorChanged()
        {
            if (ForegroundColorChanged != null)
            {
                ForegroundColorChanged(this, new EventArgs());
            }
        }

        #endregion

        #region WriteManagement

        public bool IsInsideOfAWritingOperation { get; private set; }

        public event EventHandler WriteOperationFinished;

        public void StartWritingOperation()
        {
            IsInsideOfAWritingOperation = true;
        }

        public void FinishWritingOperation()
        {
            IsInsideOfAWritingOperation = false;
            if (WriteOperationFinished != null)
            {
                WriteOperationFinished(this, new EventArgs());
            }
        }

        #endregion

        #region Output

        private readonly SynchronizedQueue<KeyInfo> inputQueue = new SynchronizedQueue<KeyInfo>();
        private readonly AutoResetEvent inputEvent = new AutoResetEvent(false);

        public event EventHandler<KeyStrokeEventArgs> CancelKeyStroke;
        public event EventHandler<KeyStrokeEventArgs> KeyStroke;

        public void NotifyKeyEvent(KeyInfo info)
        {
            inputQueue.Enqueue(info);
            inputEvent.Set();

            var ea = new KeyStrokeEventArgs(info);
            if (KeyStroke != null)
            {
                KeyStroke(this, ea);
            }

            if (!ea.IsCancelKeyStroke)
            {
                return;
            }
            if (CancelKeyStroke != null)
            {
                CancelKeyStroke(this, ea);
            }
        }

        public override bool KeyAvailable
        {
            get { return inputQueue.Count > 0; }
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            if ((options & ReadKeyOptions.IncludeKeyDown) != ReadKeyOptions.IncludeKeyDown &&
                (options & ReadKeyOptions.IncludeKeyUp) != ReadKeyOptions.IncludeKeyUp)
            {
                throw new ArgumentException();
            }

            KeyInfo keyInfo;
            do
            {
                while (!IsDisposed && inputQueue.Count == 0)
                {
                    inputEvent.WaitOne();
                }
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                keyInfo = inputQueue.Dequeue();
            } while (!CheckKeyInfo(options, keyInfo));

            if ((options & ReadKeyOptions.NoEcho) != ReadKeyOptions.NoEcho)
            {
                NotifyEchoKeyStroke(keyInfo);
            }

            return keyInfo;
        }

        private static bool CheckKeyInfo(ReadKeyOptions options, KeyInfo info)
        {
            bool allowCtrlC_criteria = true;
            bool keyUp_criteria = true;
            bool keyDown_criteria = true;

            if ((options & ReadKeyOptions.AllowCtrlC)
                != ReadKeyOptions.AllowCtrlC)
            {
                allowCtrlC_criteria = !KeyHelper.IsCancelKey(info);
            }
            if ((options & ReadKeyOptions.IncludeKeyDown)
                != ReadKeyOptions.IncludeKeyDown)
            {
                keyDown_criteria = !info.KeyDown;
            }
            if ((options & ReadKeyOptions.IncludeKeyUp)
                != ReadKeyOptions.IncludeKeyUp)
            {
                keyUp_criteria = info.KeyDown;
            }

            return
                allowCtrlC_criteria &&
                keyDown_criteria &&
                keyUp_criteria;
        }

        private void NotifyEchoKeyStroke(KeyInfo keyInfo)
        {
            char c = keyInfo.Character;
            if (!keyInfo.KeyDown ||
                c < 32 ||
                keyInfo.VirtualKeyCode == (int) Keys.Tab ||
                keyInfo.VirtualKeyCode == (int) Keys.Back)
            {
                return;
            }
            //if (keyInfo.VirtualKeyCode == (int)System.Windows.Forms.Keys.Enter)
            //{
            //    //Writer.WriteLineBreak();
            //    c = '\n';
            //    //return;
            //}
            Writer.WriteChar(c);
        }

        public override void FlushInputBuffer()
        {
            inputQueue.Clear();
        }

        #endregion
    }
}