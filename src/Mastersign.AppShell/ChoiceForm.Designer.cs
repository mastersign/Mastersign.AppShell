namespace de.mastersign.shell
{
    partial class ChoiceForm
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.Size = new System.Drawing.Size(570, 20);
            // 
            // ChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(594, 104);
            this.MaximumSize = new System.Drawing.Size(600, 1000);
            this.MinimumSize = new System.Drawing.Size(600, 132);
            this.Name = "ChoiceForm";
            this.Text = "Auswahl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
