using System;
using System.Management.Automation.Host;

namespace de.mastersign.shell
{
    public abstract class BaseUI : PSHostUserInterface
    {
        public event EventHandler<OutputEventArgs> Output;

        public static ConsoleColor verboseColor = ConsoleColor.Gray;
        public static ConsoleColor debugColor = ConsoleColor.Green;
        public static ConsoleColor warningColor = ConsoleColor.Yellow;
        public static ConsoleColor errorColor = ConsoleColor.Red;

        public ConsoleBuffer Buffer { get; private set; }

        public bool ErrorWritten { get; protected set; }
        public bool WarningWritten { get; protected set; }

        protected BaseUI(ConsoleBuffer buffer)
        {
            Buffer = buffer;
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return Buffer; }
        }

        public void ResetWrittenFlags()
        {
            ErrorWritten = false;
            WarningWritten = false;
        }

        public override void Write(
            ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            Buffer.Writer.Write(foregroundColor, backgroundColor, value);
        }

        public override void Write(string value)
        {
            Buffer.Writer.Write(value);
            OnOutput(OutputType.Default, value);
        }

        public override void WriteLine(string value)
        {
            Buffer.Writer.WriteLine(value);
            OnOutput(OutputType.Default, value + Environment.NewLine);
        }

        public override void WriteVerboseLine(string message)
        {
            Buffer.Writer.WriteLine(verboseColor, Buffer.BackgroundColor, 
                "INFO: " + message);
            OnOutput(OutputType.Verbose, message + Environment.NewLine);
        }

        public override void WriteDebugLine(string message)
        {
            Buffer.Writer.WriteLine(debugColor, Buffer.BackgroundColor, 
                "DEBUG: " + message);
            OnOutput(OutputType.Debug, message + Environment.NewLine);
        }

        public override void WriteWarningLine(string message)
        {
            Buffer.Writer.WriteLine(warningColor, Buffer.BackgroundColor, 
                "WARNUNG: " + message);
            WarningWritten = true;
            OnOutput(OutputType.Warning, message + Environment.NewLine);
        }

        public override void WriteErrorLine(string message)
        {
            Buffer.Writer.WriteLine(errorColor, Buffer.BackgroundColor, 
                message);
            ErrorWritten = true;
            OnOutput(OutputType.Error, message + Environment.NewLine);
        }

        protected virtual void OnOutput(OutputType type, string text)
        {
            if (Output != null)
            {
                Output(this, new OutputEventArgs(type, text));
            }
        }
    }
}