using System.Management.Automation.Runspaces;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;

namespace de.mastersign.shell
{
    public class Argument
    {
        public string Name { get; private set; }
        public bool HasValue { get; private set; }
        public object Value { get; private set; }

        public Argument(string name)
        {
            Name = name;
            HasValue = false;
        }

        public Argument(string name, object value)
        {
            Name = name;
            Value = value;
            HasValue = true;
        }

        internal CommandParameter ToCommandParameter()
        {
            return HasValue
                       ? new CommandParameter(Name, Value)
                       : new CommandParameter(Name);
        }
    }

    public class CommandInfo
    {
        public bool IsScriptBlock { get; private set; }
        public string Source { get; private set; }

        private readonly List<CommandParameter> arguments;
        public Command Command
        {
            get
            {
                var cmd = new Command(Source, IsScriptBlock, LocalScope);
                foreach (var arg in arguments)
                {
                    cmd.Parameters.Add(arg);
                }
                return cmd;
            }
        }

        public bool WriteDecorations { get; set; }
        public bool LocalScope { get; set; }
        public bool OutputToHost { get; set; }
        public IEnumerable Input { get; private set; }
        public InvocationCallback Callback { get; set; }
        public System.Windows.Forms.Control CallbackSyncContext { get; set; }
        public bool RetrieveOutput { get; set; }
        public bool ExecutionFinished { get; private set; }
        internal Pipeline Pipe { get; set; }
        public ICollection Results { get; internal set; }
        public ICollection Errors { get; internal set; }

        private StringBuilder output;
        private OutputType lastOutputType = OutputType.Default;
        private IShellComponent shell;

        public CommandInfo(string source)
        {
            Source = source;
            OutputToHost = true;
            IsScriptBlock = true;
        }

        public CommandInfo(string command, IEnumerable input, params Argument[] arguments)
        {
            Source = command;
            Input = input;
            this.arguments = new List<CommandParameter>();
            foreach (var arg in arguments)
            {
                this.arguments.Add(arg.ToCommandParameter());
            }
            OutputToHost = true;
        }

        internal void CallbackInvoker(object state)
        {
            if (CallbackSyncContext != null && CallbackSyncContext.InvokeRequired)
            {
                CallbackSyncContext.Invoke(Callback, this);
            }
            else
            {
                Callback(this);
            }
        }

        internal void AppendOutput(OutputType type, string text)
        {
            if (output == null)
            {
                output = new StringBuilder();
            }
            if (type != lastOutputType)
            {
                switch (type)
                {
                    case OutputType.Default:
                        output.AppendLine("---- Standard ----");
                        break;
                    case OutputType.Verbose:
                        output.AppendLine("---- Verbose ----");
                        break;
                    case OutputType.Debug:
                        output.AppendLine("---- Debug ----");
                        break;
                    case OutputType.Warning:
                        output.AppendLine("---- Warning ----");
                        break;
                    case OutputType.Error:
                        output.AppendLine("---- Error ----");
                        break;
                }
                lastOutputType = type;
            }
            output.Append(text);
        }

        private void OnShellOutput(object sender, OutputEventArgs e)
        {
            AppendOutput(e.Type, e.Text);
        }

        public string Output
        {
            get
            {
                return output != null
                    ? output.ToString()
                    : null;
            }
        }

        internal void StartExecution(IShellComponent shellComponent)
        {
            if (!RetrieveOutput) { return; }
            shell = shellComponent;
            shell.UI.Output += OnShellOutput;
        }

        internal void EndExecution()
        {
            if (shell == null) { return; }
            shell.UI.Output -= OnShellOutput;
            ExecutionFinished = true;
        }
    }
}