namespace de.mastersign.shell.demo
{
    partial class GraphicalShellForm
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
            this.shellControl = new de.mastersign.shell.GraphicalShellControl();
            this.shell = new de.mastersign.shell.GraphicalShell(this.components);
            this.SuspendLayout();
            // 
            // shellControl
            // 
            this.shellControl.CancelButtonEnabled = false;
            this.shellControl.CursorBlinkInterval = 500;
            this.shellControl.CursorMode = de.mastersign.shell.ConsoleDisplay.CursorShowMode.Hide;
            this.shellControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shellControl.Location = new System.Drawing.Point(0, 0);
            this.shellControl.Name = "shellControl";
            this.shellControl.ProcessKeyStrokes = true;
            this.shellControl.PromptText = "C:\\Windows\\system32\\>";
            this.shellControl.Size = new System.Drawing.Size(764, 362);
            this.shellControl.TabIndex = 0;
            // 
            // shell
            // 
            this.shell.GraphicalShellControl = null;
            this.shell.Exit += new System.EventHandler<de.mastersign.shell.ExitEventArgs>(this.shell_Exit);
            // 
            // GraphicalShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 362);
            this.Controls.Add(this.shellControl);
            this.Name = "GraphicalShellForm";
            this.Text = "Mastersign.AppShell Demo";
            this.Load += new System.EventHandler(this.GraphicalShellForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private de.mastersign.shell.GraphicalShell shell;
        private de.mastersign.shell.GraphicalShellControl shellControl;
    }
}