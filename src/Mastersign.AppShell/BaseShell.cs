using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Threading;
using System.IO;

namespace de.mastersign.shell
{
    public class BaseShell : Component, IShellComponent
    {
        private IContainer components;
        #region Component


        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buffer = new de.mastersign.shell.ConsoleBuffer(this.components);
            // 
            // buffer
            // 
            this.buffer.WindowTitle = "Application Core Shell";
        }

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private ConsoleBuffer buffer;

        #endregion

        protected bool GuiSynchronized { get; set; }

        protected bool exitFlag;
        private int exitCode;

        private InvocationQueue queue;

        //public event EventHandler AsyncInvocationCompleted;

        protected BaseShell()
        {
            InitializeComponent();
            Initialize();
        }

        protected BaseShell(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initialize();
        }

        protected ShellHost ShellHost { get; private set; }
        protected bool IsStarted { get; private set; }

        public ConsoleBuffer Buffer
        {
            get { return buffer; }
        }

        public Runspace Runspace { get; private set; }

        public PSHost Host
        {
            get { return ShellHost; }
        }

        public virtual BaseUI UI
        {
            get { return null; }
        }

        public event EventHandler<ExitEventArgs> Exit;

        private void Initialize()
        {
            //Buffer.BufferSize = new Size(100, 500);
            //Buffer.ClearContent();

            var ui = UI;
            if (ui != null)
            {
                ShellHost = new ShellHost(ui);
                ShellHost.ShouldExit += ShellHost_ShouldExit;
            }

            queue = new InvocationQueue(Invoke);
            queue.ProcessingStarts += queue_ProcessingStarts;
            queue.ProcessingEnds += queue_ProcessingEnds;
        }

        private void queue_ProcessingStarts(object sender, EventArgs e)
        {
            ProcessingStarts();
        }

        private void queue_ProcessingEnds(object sender, EventArgs e)
        {
            ProcessingEnds();
        }

        public virtual void StartShell()
        {
            StartShell((IRunspaceConfigurationFactory)null);
        }

        public virtual void StartShell(params Assembly[] snapIns)
        {
            StartShell(new AssemblyRunspaceConfigurationFactory(snapIns));
        }

        public virtual void StartShell(IRunspaceConfigurationFactory factory)
        {
            if (Host == null)
            {
                throw new InvalidOperationException(
                    "BaseShell kann nicht direkt verwendet werden. Verwenden Sie eine erbende Klasse, die UI überschreibt.");
            }

            if (IsStarted)
            {
                return;
            }
            IsStarted = true;

            Buffer.ClearContent();

            var config = factory != null
                ? factory.CreateRunspaceConfiguration()
                : RunspaceConfiguration.Create();

            Runspace = RunspaceFactory.CreateRunspace(Host, config);
            Runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
            Runspace.Open();
            Runspace.DefaultRunspace = Runspace;
        }

        private void SimpleOut(IEnumerable<PSObject> objects)
        {
            foreach (var o in objects)
            {
                UI.WriteLine(o.ToString());
            }
        }

        protected virtual void StartInvocation(CommandInfo info) { }

        protected virtual void EndInvocation(CommandInfo info) { }

        protected virtual void ProcessingStarts() { }

        protected virtual void ProcessingEnds() { }

        //private Collection<PSObject> ExecuteCommand(string command, out Collection<Object> errors, params object[] input)
        //{
        //    StartCommandExecution(command, false);
        //    using (var pipe = Runspace.CreatePipeline(command))
        //    {
        //        pipe.Input.Write(input, true);
        //        try
        //        {
        //            var objects = pipe.Invoke();
        //            errors = pipe.Error.ReadToEnd();
        //            FinishCommandExecution(false);
        //            return objects;
        //        }
        //        catch (Exception exc)
        //        {
        //            UI.WriteErrorLine(exc.ToString());
        //            errors = pipe.Error.ReadToEnd();
        //            FinishCommandExecution(false);
        //            throw;
        //        }
        //    }
        //}

        private void Invoke(CommandInfo info)
        {
            var pipe = Runspace.CreatePipeline();
            info.Pipe = pipe;

            if (info.IsScriptBlock)
            {
                pipe.Commands.AddScript((info.LocalScope ? ". " : "") + info.Source);
            }
            else
            {
                pipe.Commands.Add(info.Command);
                if (info.Input != null)
                {
                    pipe.Input.Write(info.Input, true);
                    pipe.Input.Close();
                }
            }
            if (info.OutputToHost)
            {
                pipe.Commands.Add("Out-Default");
            }

            StartInvocation(info);
            info.StartExecution(this);

            Exception breakingException = null;
            try
            {
                info.Results = pipe.Invoke();
            }
            catch (Exception exc)
            {
                breakingException = exc;
            }

            info.Errors = pipe.Error.ReadToEnd();
            if (info.OutputToHost)
            {
                foreach (var error in info.Errors)
                {
                    UI.WriteErrorLine(error.ToString());
                }
            }

            if (breakingException != null)
            {
                info.Errors = new object[] { breakingException };
                if (info.OutputToHost)
                {
                    if (breakingException is RuntimeException)
                    {
                        var re = (RuntimeException)breakingException;
                        var ii = re.ErrorRecord.InvocationInfo;
                        UI.WriteErrorLine(breakingException.Message
                            + (ii != null ? ii.PositionMessage : "")
                            + "\n    + CategoryInfo:          " + re.ErrorRecord.CategoryInfo
                            + "\n    + FullyQualifiedErrorId: " + re.ErrorRecord.FullyQualifiedErrorId);
                    }
                    else
                    {
                        UI.WriteErrorLine(breakingException.ToString());
                    }
                }
            }

            pipe.Dispose();

            info.EndExecution();
            EndInvocation(info);
            if (info.Callback != null)
            {
                ThreadPool.QueueUserWorkItem(info.CallbackInvoker, null);
            }
        }

        /// <summary>
        /// Führt einen Befehl asynchron aus.
        /// </summary>
        /// <param name="info">Der Befehl.</param>
        public void InvokeAsync(CommandInfo info)
        {
            queue.Enqueue(info);
        }

        /// <summary>
        /// Führt einen Befehl synchron aus.
        /// </summary>
        /// <param name="info">Der Befehl.</param>
        public void InvokeSync(CommandInfo info)
        {
            if (!GuiSynchronized)
            {
                var e = new ManualResetEvent(false);

                var callback = info.Callback;
                info.Callback = ci =>
                {
                    if (callback != null) { callback(ci); }
                    e.Set();
                };

                InvokeAsync(info);

                e.WaitOne();
            }
            else
            {
                var working = true;
                var callback = info.Callback;
                info.Callback = ci =>
                {
                    if (callback != null) { callback(ci); }
                    working = false;
                };

                InvokeAsync(info);

                while (working)
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Bricht die aktuell ausgeführten 
        /// </summary>
        public void CancelProcessing()
        {
            queue.Cancel();
        }

        #region Alternative Async Invokation

        //protected void InvokeCommandAsync(string command, params object[] input)
        //{
        //    StartCommandExecution(command, false);
        //    var pipe = Runspace.CreatePipeline(command);
        //    pipe.Commands.Add("Out-Default");
        //    pipe.Input.Write(input, true);
        //    pipe.StateChanged += pipe_StateChanged;
        //    try
        //    {
        //        pipe.InvokeAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        UI.WriteErrorLine(e.ToString());
        //    }
        //}

        //private void pipe_StateChanged(object sender, PipelineStateEventArgs e)
        //{
        //    if (e.PipelineStateInfo.State == PipelineState.Failed)
        //    {
        //        UI.WriteErrorLine(e.PipelineStateInfo.Reason.ToString());
        //    }
        //    if (e.PipelineStateInfo.State == PipelineState.Completed)
        //    {
        //        var pipe = (Pipeline)sender;
        //        SimpleOut(ReadPipeline(pipe.Output));
        //    }

        //    OnAsyncInvocationCompleted();
        //}

        //private static IEnumerable<PSObject> ReadPipeline(PipelineReader<PSObject> r)
        //{
        //    while (!r.EndOfPipeline)
        //    {
        //        yield return r.Read();
        //    }
        //}

        //private void OnAsyncInvocationCompleted()
        //{
        //    if (AsyncInvocationCompleted != null)
        //    {
        //        AsyncInvocationCompleted(this, EventArgs.Empty);
        //    }
        //    FinishCommandExecution(false);
        //}

        #endregion

        public void SetVariable(string name, object value)
        {
            //            const string cmdTemplate = @"
            //foreach ($v in $input) 
            //{{
            //  Set-Content -Path Variable:\\{0} -Value $v
            //  break
            //}}
            //";
            //            string cmd = string.Format(
            //                cmdTemplate,
            //                name);
            //            InvokeCommand(cmd, false, value);
            Runspace.SessionStateProxy.SetVariable(name, value);
        }

        public object GetVariable(string name)
        {
            return Runspace.SessionStateProxy.GetVariable(name);
        }

        public string GetPrompt()
        {
            var pipe = Runspace.CreatePipeline("prompt");
            Collection<PSObject> result = null;
            try
            {
                result = pipe.Invoke();
            }
            catch (Exception e)
            {
                UI.WriteWarningLine("Prompt-Error:\n" + e);
            }

            return result != null && result.Count > 0
                       ? result[0].ToString()
                       : "Prompt-Error> ";
        }

        private void ShellHost_ShouldExit(object sender, ExitEventArgs e)
        {
            exitFlag = true;
            exitCode = e.ExitCode;
            if (Exit != null)
            {
                if (SynchronizationContext.Current != null)
                {
                    SynchronizationContext.Current.Post(
                        o => Exit(this, new ExitEventArgs(exitCode)),
                        null);
                }
                else
                {
                    Exit(this, new ExitEventArgs(exitCode));
                }
            }
        }

        #region Wrappers

        public void WriteLine(string msg)
        {
            UI.WriteLine(msg);
        }

        public void WriteVerboseLine(string msg)
        {
            UI.WriteVerboseLine(msg);
        }

        public void WriteDebugLine(string msg)
        {
            UI.WriteDebugLine(msg);
        }

        public void WriteWarningLine(string msg)
        {
            UI.WriteWarningLine(msg);
        }

        public void WriteErrorLine(string msg)
        {
            UI.WriteErrorLine(msg);
        }

        public void ClearBuffer()
        {
            Buffer.ClearContent();
        }

        public void MoveBufferWindowPosition(int x, int y)
        {
            Buffer.WindowPosition = new Coordinates(x, y);
        }

        #endregion
    }
}