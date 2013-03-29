using System;

namespace de.mastersign.shell
{
    public class ExitEventArgs : EventArgs
    {
        public int ExitCode { get; private set; }

        public ExitEventArgs(int value)
        {
            ExitCode = value;
        }
    }
}