namespace de.mastersign.shell.demo
{
    partial class ConsoleShellForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleShellForm));
            this.shellControl = new de.mastersign.shell.ConsoleShellControl();
            this.shell = new de.mastersign.shell.ConsoleShell(this.components);
            this.SuspendLayout();
            // 
            // shellControl
            // 
            this.shellControl.CursorBlinkInterval = 500;
            this.shellControl.CursorMode = de.mastersign.shell.ConsoleDisplay.CursorShowMode.Show;
            this.shellControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shellControl.Location = new System.Drawing.Point(0, 0);
            this.shellControl.Name = "shellControl";
            this.shellControl.ProcessKeyStrokes = true;
            this.shellControl.Size = new System.Drawing.Size(764, 362);
            this.shellControl.TabIndex = 0;
            // 
            // shell
            // 
            this.shell.Exit += new System.EventHandler<de.mastersign.shell.ExitEventArgs>(this.shell_Exit);
            // 
            // ConsoleShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 362);
            this.Controls.Add(this.shellControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConsoleShellForm";
            this.Text = "Application Core ConsoleShell";
            this.Load += new System.EventHandler(this.ShellForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private de.mastersign.shell.ConsoleShellControl shellControl;
        private de.mastersign.shell.ConsoleShell shell;
    }
}