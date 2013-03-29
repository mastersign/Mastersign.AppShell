using System.Management.Automation.Host;
using System.Text;

namespace de.mastersign.shell
{
    partial class ConsoleBufferReader
    {
        private class ReadContext
        {
            public ConsoleBufferReader Reader { get; private set; }

            public Coordinates StartCursorPosition { get; private set; }
            public Coordinates MaxCursorPosition { get; set; }

            private StringBuilder LineBuilder { get; set; }
            public int LinePosition { get; set; }

            private ConsoleBuffer Buffer { get; set; }
            private History.Navigator HistoryNavigator { get; set; }

            public ReadContext(ConsoleBufferReader reader)
            {
                Reader = reader;
                Buffer = reader.Buffer;
                HistoryNavigator = reader.History.GetNavigator();
                StartCursorPosition = Buffer.CursorPosition;
                MaxCursorPosition = StartCursorPosition;
                LineBuilder = new StringBuilder();
                LinePosition = 0;
            }

            public string Line
            {
                get { return LineBuilder.ToString(); }
            }

            public string LineBreak()
            {
                Buffer.Writer.InnerWriteLineBreak(Buffer.BackgroundColor);
                UpdateMaxCursorPosition();
                string line = Line;
                HistoryNavigator.CompleteOperationWith(line);
                return line;
            }

            public void DoMoveLeft()
            {
                if (LinePosition <= 0) return;

                LinePosition--;
                Buffer.CursorPosition = GetCursorPositionFromInputBuffer(LinePosition, StartCursorPosition);
            }

            public void DoMoveRight()
            {
                if (LinePosition >= LineBuilder.Length) return;

                LinePosition++;
                Buffer.CursorPosition = GetCursorPositionFromInputBuffer(LinePosition, StartCursorPosition);
            }

            public void DoMoveHome()
            {
                if (LinePosition <= 0) return;

                LinePosition = 0;
                Buffer.CursorPosition = StartCursorPosition;
            }

            public void DoMoveEnd()
            {
                if (LinePosition >= LineBuilder.Length) return;

                LinePosition = LineBuilder.Length;
                Buffer.CursorPosition = GetCursorPositionFromInputBuffer(LineBuilder.Length, StartCursorPosition);
            }

            public void DoChar(char character)
            {
                LineBuilder.Insert(LinePosition, character);
                var maxCP = Max(MaxCursorPosition, Buffer.CursorPosition);
                Buffer.StartWritingOperation();
                Buffer.Writer.InnerWriteForRegion(
                    Buffer.CursorPosition, ref maxCP,
                    LineBuilder.ToString().Substring(LinePosition),
                    false);
                MaxCursorPosition = maxCP;
                Buffer.Writer.InnerWriteChar(Buffer.ForegroundColor, Buffer.BackgroundColor, character);
                Buffer.FinishWritingOperation();
                LinePosition++;
            }

            public void DoBackspace()
            {
                if (LineBuilder.Length <= 0 || !Buffer.Writer.IsBackspacePossible) return;

                LinePosition--;
                LineBuilder.Remove(LinePosition, 1);
                string tail = LineBuilder.ToString().Substring(LinePosition);
                Buffer.StartWritingOperation();
                Buffer.Writer.InnerWriteBackspace(tail, MaxCursorPosition);
                Buffer.FinishWritingOperation();
                UpdateMaxCursorPosition();
            }

            public void DoDelete()
            {
                if (LineBuilder.Length <= 0 || LinePosition >= LineBuilder.Length) return;

                LineBuilder.Remove(LinePosition, 1);
                var maxCP = MaxCursorPosition;
                Buffer.StartWritingOperation();
                Buffer.Writer.InnerWriteForRegion(
                    GetCursorPositionFromInputBuffer(LinePosition, StartCursorPosition),
                    ref maxCP, LineBuilder.ToString().Substring(LinePosition), false);
                MaxCursorPosition = maxCP;
                Buffer.FinishWritingOperation();
                UpdateMaxCursorPosition();
            }

            public void DoEscape()
            {
                if (LineBuilder.Length <= 0) return;

                LineBuilder.Remove(0, LineBuilder.Length);
                LinePosition = 0;
                Buffer.StartWritingOperation();
                var maxCP = MaxCursorPosition;
                Buffer.Writer.InnerWriteForRegion(StartCursorPosition, ref maxCP, "", false);
                MaxCursorPosition = maxCP;
                Buffer.CursorPosition = StartCursorPosition;
                Buffer.FinishWritingOperation();

                HistoryNavigator.Reset();
            }

            public void DoHistoryBack()
            {
                if (!HistoryNavigator.CanMoveBack) return;

                LineBuilder.Remove(0, LineBuilder.Length);
                LineBuilder.Append(HistoryNavigator.MoveBack());
                LinePosition = LineBuilder.Length;

                Buffer.StartWritingOperation();
                var maxCP = MaxCursorPosition;
                Buffer.Writer.InnerWriteForRegion(
                    GetCursorPositionFromInputBuffer(0, StartCursorPosition),
                    ref maxCP, LineBuilder.ToString(), true);
                MaxCursorPosition = maxCP;
                Buffer.FinishWritingOperation();
            }

            public void DoHistoryForward()
            {
                if (!HistoryNavigator.CanMoveForward) return;

                LineBuilder.Remove(0, LineBuilder.Length);
                LineBuilder.Append(HistoryNavigator.MoveForward());
                LinePosition = LineBuilder.Length;

                Buffer.StartWritingOperation();
                var maxCP = MaxCursorPosition;
                Buffer.Writer.InnerWriteForRegion(
                    GetCursorPositionFromInputBuffer(0, StartCursorPosition),
                    ref maxCP, LineBuilder.ToString(), true);
                MaxCursorPosition = maxCP;
                Buffer.FinishWritingOperation();
            }

            private void UpdateMaxCursorPosition()
            {
                MaxCursorPosition = Max(Buffer.CursorPosition, MaxCursorPosition);
            }

            private Coordinates Max(Coordinates p1, Coordinates p2)
            {
                var cp = p1;
                if (p2.Y > cp.Y)
                {
                    cp.Y = p2.Y;
                    cp.X = p2.X;
                }
                else if (p2.Y == cp.Y &&
                         p2.X > cp.X)
                {
                    cp.X = p2.X;
                }
                return cp;
            }

            private int GetInputBufferFromCursorPosition(Coordinates startCP)
            {
                var cp = Buffer.CursorPosition;
                int diffX = cp.X - startCP.X;
                int diffY = cp.Y - startCP.Y;
                return diffY*Buffer.BufferSize.Width + diffX;
            }

            private Coordinates GetCursorPositionFromInputBuffer(int pos, Coordinates startCP)
            {
                int bw = Buffer.BufferSize.Width;
                return new Coordinates(
                    (startCP.X + pos)%bw,
                    startCP.Y + (startCP.X + pos)/bw);
            }
        }
    }
}