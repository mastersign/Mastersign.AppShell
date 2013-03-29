using System;

namespace de.mastersign.shell
{
    public class OutputEventArgs : EventArgs
    {
        public string Text { get; private set; }
        public OutputType Type { get; private set; }

        public OutputEventArgs(OutputType type, string text)
        {
            Text = text;
            Type = type;
        }
    }
}