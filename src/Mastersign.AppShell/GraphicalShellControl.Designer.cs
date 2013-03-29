namespace de.mastersign.shell
{
    partial class GraphicalShellControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPrompt = new System.Windows.Forms.Panel();
            this.txtPrompt = new System.Windows.Forms.TextBox();
            this.panelInput = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblIcon = new System.Windows.Forms.Label();
            this.console = new de.mastersign.shell.ConsoleShellControl();
            this.commandInputTextBox = new de.mastersign.shell.CommandInputTextBox();
            this.panelPrompt.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPrompt
            // 
            this.panelPrompt.Controls.Add(this.btnCancel);
            this.panelPrompt.Controls.Add(this.txtPrompt);
            this.panelPrompt.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPrompt.Location = new System.Drawing.Point(0, 133);
            this.panelPrompt.Name = "panelPrompt";
            this.panelPrompt.Size = new System.Drawing.Size(404, 20);
            this.panelPrompt.TabIndex = 2;
            // 
            // txtPrompt
            // 
            this.txtPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrompt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPrompt.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrompt.Location = new System.Drawing.Point(4, 4);
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.ReadOnly = true;
            this.txtPrompt.Size = new System.Drawing.Size(372, 13);
            this.txtPrompt.TabIndex = 0;
            this.txtPrompt.TabStop = false;
            this.txtPrompt.Text = "C:\\Windows\\system32\\>";
            // 
            // panelInput
            // 
            this.panelInput.BackColor = System.Drawing.SystemColors.Window;
            this.panelInput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelInput.Controls.Add(this.lblIcon);
            this.panelInput.Controls.Add(this.commandInputTextBox);
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInput.Location = new System.Drawing.Point(0, 153);
            this.panelInput.Name = "panelInput";
            this.panelInput.Size = new System.Drawing.Size(404, 28);
            this.panelInput.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackgroundImage = global::de.mastersign.shell.Properties.Resources.CancelImage;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Location = new System.Drawing.Point(382, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(22, 20);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblIcon
            // 
            this.lblIcon.Image = global::de.mastersign.shell.Properties.Resources.PromptImage;
            this.lblIcon.Location = new System.Drawing.Point(2, 0);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(24, 24);
            this.lblIcon.TabIndex = 5;
            // 
            // console
            // 
            this.console.BackColor = System.Drawing.SystemColors.Control;
            this.console.Buffer = null;
            this.console.CursorBlinkInterval = 500;
            this.console.CursorMode = de.mastersign.shell.ConsoleDisplay.CursorShowMode.Hide;
            this.console.Dock = System.Windows.Forms.DockStyle.Fill;
            this.console.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.console.ForeColor = System.Drawing.SystemColors.ControlText;
            this.console.Location = new System.Drawing.Point(0, 0);
            this.console.Name = "console";
            this.console.ProcessKeyStrokes = true;
            this.console.Size = new System.Drawing.Size(404, 133);
            this.console.TabIndex = 3;
            this.console.TabStop = false;
            this.console.Enter += new System.EventHandler(this.console_Enter);
            // 
            // commandInputTextBox
            // 
            this.commandInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commandInputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.commandInputTextBox.Font = new System.Drawing.Font("Lucida Console", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commandInputTextBox.Location = new System.Drawing.Point(32, 4);
            this.commandInputTextBox.Multiline = true;
            this.commandInputTextBox.Name = "commandInputTextBox";
            this.commandInputTextBox.Runspace = null;
            this.commandInputTextBox.Size = new System.Drawing.Size(367, 16);
            this.commandInputTextBox.TabIndex = 4;
            this.commandInputTextBox.CancelRequest += new System.EventHandler(this.commandInputTextBox_CancelRequest);
            this.commandInputTextBox.HistoryBackRequested += new System.EventHandler(this.commandInputTextBox_HistoryUpRequested);
            this.commandInputTextBox.TypingLineBreak += new System.EventHandler(this.commandInputTextBox_TypingLineBreak);
            this.commandInputTextBox.HistoryForwardRequested += new System.EventHandler(this.commandInputTextBox_HistoryDownRequested);
            this.commandInputTextBox.TypingEscape += new System.EventHandler(this.commandInputTextBox_TypingEscape);
            this.commandInputTextBox.EnterPressed += new System.EventHandler(this.commandInputTextBox_EnterPressed);
            // 
            // GraphicalShellControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.console);
            this.Controls.Add(this.panelPrompt);
            this.Controls.Add(this.panelInput);
            this.Name = "GraphicalShellControl";
            this.Size = new System.Drawing.Size(404, 181);
            this.panelPrompt.ResumeLayout(false);
            this.panelPrompt.PerformLayout();
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPrompt;
        private System.Windows.Forms.TextBox txtPrompt;
        private ConsoleShellControl console;
        private CommandInputTextBox commandInputTextBox;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Button btnCancel;
    }
}
