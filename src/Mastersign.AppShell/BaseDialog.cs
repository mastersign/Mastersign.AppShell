using System;
using System.Drawing;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    public partial class BaseDialog : Form
    {
        public BaseDialog()
        {
            InitializeComponent();
            btnOK.MinimumSize = btnOK.Size;
        }

        private void PromptForm_Load(object sender, EventArgs e)
        {
            Size = GetPreferredSize(new Size(600, 0));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            OnFinish();
            Close();
        }

        protected virtual void OnFinish()
        {
        }

        protected void SetMessage(string msg)
        {
            lblMessage.Text = msg;
            lblMessage.MinimumSize = lblMessage.Size;
            lblMessage.Visible = !string.IsNullOrEmpty(msg);
        }

        protected void SetCaption(string caption)
        {
            Text = string.IsNullOrEmpty(caption) 
                ? "Benutzereingabe" 
                : caption;
        }
    }
}