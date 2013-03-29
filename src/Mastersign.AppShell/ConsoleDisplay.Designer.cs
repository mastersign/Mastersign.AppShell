namespace de.mastersign.shell
{
    partial class ConsoleDisplay
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
            this.components = new System.ComponentModel.Container();
            this.timerCursor = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerCursor
            // 
            this.timerCursor.Interval = 500;
            this.timerCursor.Tick += new System.EventHandler(this.timerCursor_Tick);
            // 
            // ConsoleDisplay
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Silver;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ConsoleDisplay";
            this.Size = new System.Drawing.Size(422, 221);
            this.VisibleChanged += new System.EventHandler(this.ShellDisplay_VisibleChanged);
            this.FontChanged += new System.EventHandler(this.ShellDisplay_FontChanged);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ShellDisplay_KeyUp);
            this.Resize += new System.EventHandler(this.ShellDisplay_Resize);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ShellDisplay_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShellDisplay_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerCursor;


    }
}