using System;
using System.Collections.Generic;
using System.Management.Automation.Host;

namespace de.mastersign.shell
{
    public class ConsoleBufferWriter
    {
        public ConsoleBuffer Buffer { get; private set; }

        public ConsoleBufferWriter(ConsoleBuffer buffer)
        {
            Buffer = buffer;
        }

        public void CursorToNextLine()
        {
            if (Buffer.CursorPosition.Y < Buffer.BufferSize.Height - 1)
            {
                Buffer.CursorPosition = new Coordinates(0, Buffer.CursorPosition.Y + 1);
            }
            else
            {
                Buffer.ScrollOneLineUp();
                Buffer.CursorPosition = new Coordinates(0, Buffer.CursorPosition.Y);
            }
            MoveWindowToCursor();
        }

        public void CursorToNextCell()
        {
            if (Buffer.CursorPosition.X < Buffer.BufferSize.Width - 1)
            {
                Buffer.CursorPosition = new Coordinates(
                    Buffer.CursorPosition.X + 1, Buffer.CursorPosition.Y);
                MoveWindowToCursor();
            }
            else
            {
                CursorToNextLine();
            }
        }

        public void MoveWindowToCursor()
        {
            var wp = Buffer.WindowPosition;
            var ws = Buffer.WindowSize;
            var cp = Buffer.CursorPosition;
            if (cp.X <= wp.X && cp.X > 0)
            {
                wp.X = cp.X - 1;
            }
            else if (cp.X < wp.X)
            {
                wp.X = cp.X;
            }
            else if (cp.X > wp.X + (ws.Width - 1))
            {
                wp.X = cp.X - (ws.Width - 1);
            }
            if (cp.Y < wp.Y)
            {
                wp.Y = cp.Y;
            }
            else if (cp.Y > wp.Y + (ws.Height - 1))
            {
                wp.Y = cp.Y - (ws.Height - 1);
            }
            Buffer.WindowPosition = wp;
        }

        public void WriteLineBreak(ConsoleColor backgroundColor)
        {
            Buffer.StartWritingOperation();

            InnerWriteLineBreak(backgroundColor);

            Buffer.FinishWritingOperation();
        }

        internal void InnerWriteLineBreak(ConsoleColor backgroundColor)
        {
            var bs = Buffer.BufferSize;
            var cp = Buffer.CursorPosition;
            for (int c = cp.X; c < bs.Width; c++)
            {
                Buffer[cp.Y, c] = new BufferCell(
                    ' ',
                    Buffer.ForegroundColor, backgroundColor, BufferCellType.Complete);
            }
            CursorToNextLine();
        }

        public void WriteLineBreak()
        {
            WriteLineBreak(Buffer.BackgroundColor);
        }

        public void WriteKey(KeyInfo info)
        {
            Buffer.StartWritingOperation();

            InnerWriteKey(info);

            Buffer.FinishWritingOperation();
        }

        private void InnerWriteKey(KeyInfo info)
        {
            if (info.KeyDown &&
                (info.ControlKeyState | ControlKeyStates.LeftCtrlPressed)
                != ControlKeyStates.LeftCtrlPressed &&
                (info.ControlKeyState | ControlKeyStates.RightCtrlPressed)
                != ControlKeyStates.RightCtrlPressed &&
                (info.ControlKeyState | ControlKeyStates.LeftAltPressed)
                != ControlKeyStates.LeftAltPressed &&
                (info.ControlKeyState | ControlKeyStates.RightAltPressed)
                != ControlKeyStates.RightAltPressed
                )
            {
                InnerWriteChar(
                    Buffer.ForegroundColor, Buffer.BackgroundColor, info.Character);
            }
        }

        public void WriteChar(char character)
        {
            Buffer.StartWritingOperation();

            InnerWriteChar(Buffer.ForegroundColor, Buffer.BackgroundColor, character);

            Buffer.FinishWritingOperation();
        }

        internal bool InnerWriteChar(
            ConsoleColor foregroundColor, ConsoleColor backgroundColor, char character)
        {
            if (character == '\n')
            {
                InnerWriteLineBreak(backgroundColor);
            }
            else if (character == '\r')
            {
                // nichts
            }
            else
            {
                Buffer[Buffer.CursorPosition] = new BufferCell(
                    character,
                    foregroundColor, backgroundColor, BufferCellType.Complete);
                CursorToNextCell();
                return true;
            }
            return false;
        }

        internal bool IsBackspacePossible
        {
            get
            {
                var cp = Buffer.CursorPosition;
                return cp.X > 0 || cp.Y > 0;
            }
        }

        internal void InnerWriteBackspace(string tail, Coordinates rewriteEnd)
        {
            var cp = Buffer.CursorPosition;
            bool remove = false;
            if (cp.X > 0)
            {
                cp.X--;
                remove = true;
            }
            else if (cp.Y > 0)
            {
                cp.X = Buffer.BufferSize.Width - 1;
                cp.Y--;
                remove = true;
            }
            if (remove)
            {
                InnerWriteForRegion(cp, ref rewriteEnd, tail, false);
                Buffer.CursorPosition = cp;

                MoveWindowToCursor();
            }
        }

        internal void InnerWriteForRegion(Coordinates start, ref Coordinates end, string value, bool moveCursor)
        {
            var bs = Buffer.BufferSize;
            int cnt = 0;
            var pos = start;
            while (cnt < value.Length ||
                   pos.Y < end.Y ||
                   (pos.Y == end.Y && pos.X < end.X))
            {
                char character = cnt < value.Length ? value[cnt] : ' ';
                Buffer[pos.Y, pos.X] = new BufferCell(character,
                                                      Buffer.ForegroundColor, Buffer.BackgroundColor,
                                                      BufferCellType.Complete);
                cnt++;
                pos.X++;
                if (pos.X >= bs.Width)
                {
                    pos.X = 0;
                    pos.Y++;
                }
                if (cnt == value.Length && moveCursor)
                {
                    Buffer.CursorPosition = pos;
                }
            }
            end = pos;
        }

        public void Write(string value)
        {
            Buffer.StartWritingOperation();

            InnerWrite(Buffer.ForegroundColor, Buffer.BackgroundColor, value);

            Buffer.FinishWritingOperation();
        }

        public void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            Buffer.StartWritingOperation();

            InnerWrite(foregroundColor, backgroundColor, value);

            Buffer.FinishWritingOperation();
        }

        internal void InnerWrite(
            ConsoleColor foregroundColor, ConsoleColor backgroundColor, IEnumerable<char> value)
        {
            foreach (var c in value)
            {
                InnerWriteChar(foregroundColor, backgroundColor, c);
            }
        }

        public void WriteLine(string line)
        {
            Buffer.StartWritingOperation();

            InnerWriteLine(Buffer.ForegroundColor, Buffer.BackgroundColor, line);

            Buffer.FinishWritingOperation();
        }

        public void WriteLine(
            ConsoleColor foregroundColor, ConsoleColor backgroundColor, string line)
        {
            Buffer.StartWritingOperation();

            InnerWriteLine(foregroundColor, backgroundColor, line);

            Buffer.FinishWritingOperation();
        }

        internal void InnerWriteLine(
            ConsoleColor foregroundColor, ConsoleColor backgroundColor, IEnumerable<char> line)
        {
            if (line == null) throw new ArgumentNullException("line");

            bool beginAtLineStart = Buffer.CursorPosition.X == 0;
            int lineLength = 0;
            foreach (var c in line)
            {
                InnerWriteChar(foregroundColor, backgroundColor, c);
                lineLength++;
            }
            if (!beginAtLineStart || lineLength != Buffer.BufferSize.Width)
            {
                InnerWriteLineBreak(backgroundColor);
            }
        }
    }
}