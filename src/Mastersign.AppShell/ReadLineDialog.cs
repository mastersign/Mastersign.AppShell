
namespace de.mastersign.shell
{
    public partial class ReadLineDialog : BaseDialog
    {
        private static ReadLineDialog dlg;

        public ReadLineDialog()
        {
            InitializeComponent();
            SetMessage("");
        }

        public static string ReadLine()
        {
            if (dlg == null)
            {
                dlg = new ReadLineDialog();
            }
            dlg.txtInput.Text = "";
            dlg.ShowDialog();
            return dlg.txtInput.Text;
        }
    }
}