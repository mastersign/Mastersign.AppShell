using System;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace de.mastersign.shell
{
    public interface IShellComponent
    {
        event EventHandler<ExitEventArgs> Exit;

        PSHost Host { get; }

        ConsoleBuffer Buffer { get; }

        Runspace Runspace { get; }
        BaseUI UI { get; }

        void StartShell();
        void StartShell(IRunspaceConfigurationFactory factory);
        void StartShell(params System.Reflection.Assembly[] snapIns);

        void SetVariable(string name, object value);
        object GetVariable(string name);

        //Collection<PSObject> ExecuteCommand(string command, params object[] input);

        void InvokeSync(CommandInfo info);
        void InvokeAsync(CommandInfo info);

        void WriteLine(string msg);
        void WriteVerboseLine(string msg);
        void WriteDebugLine(string msg);
        void WriteWarningLine(string msg);
        void WriteErrorLine(string msg);

        void ClearBuffer();

        void MoveBufferWindowPosition(int x, int y);

        /// <summary>
        /// Bricht die aktuell ausgeführten 
        /// </summary>
        void CancelProcessing();

        string GetPrompt();
    }
}