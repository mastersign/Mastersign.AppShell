using System;
using System.Management.Automation.Host;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    public partial class ConsoleBufferReader
    {
        public ConsoleBuffer Buffer { get; private set; }

        public History History { get; private set; }

        public ConsoleBufferReader(ConsoleBuffer buffer)
        {
            Buffer = buffer;
            History = new History();
        }

        public string ReadLine()
        {
            var context = new ReadContext(this);
            string ret = null;

            try
            {
                do
                {
                    var info = Buffer.ReadKey(ReadKeyOptions.IncludeKeyDown | ReadKeyOptions.NoEcho);
                    if (info.Character == '\r')
                    {
                        if ((info.ControlKeyState & ControlKeyStates.ShiftPressed) == ControlKeyStates.ShiftPressed)
                        {
                            context.DoChar('\n');
                        }
                        else
                        {
                            ret = context.LineBreak();
                        }
                    }
                    else if (info.Character == '\b')
                    {
                        context.DoBackspace();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.Delete)
                    {
                        context.DoDelete();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.Right)
                    {
                        context.DoMoveRight();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.Left)
                    {
                        context.DoMoveLeft();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.Home)
                    {
                        context.DoMoveHome();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.End)
                    {
                        context.DoMoveEnd();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.Escape)
                    {
                        context.DoEscape();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.Up)
                    {
                        context.DoHistoryBack();
                    }
                    else if (info.VirtualKeyCode == (int) Keys.Down)
                    {
                        context.DoHistoryForward();
                    }
                    else if (info.Character == 0)
                    {
                        // nichts
                    }
                    else if (KeyHelper.IsPrintable((Keys) info.VirtualKeyCode) ||
                             info.Character == ' ')
                    {
                        context.DoChar(info.Character);
                    }
                } while (ret == null);
            }
            catch (ObjectDisposedException ode)
            {
                throw new OperationCanceledException("Die ConsoleShell wurde beendet.", ode);
            }

            return ret;
        }
    }
}