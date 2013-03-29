using System;
using System.Collections.Generic;
using System.Management.Automation.Host;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    public partial class ChoiceForm : BaseDialog
    {
        private readonly List<RadioButton> buttons = new List<RadioButton>();
        private int choice;

        public ChoiceForm(string caption, string message, IEnumerable<ChoiceDescription> choices, int defaultChoice)
        {
            InitializeComponent();
            SetCaption(caption);
            SetMessage(message);
            Initialize(choices, defaultChoice);
        }

        public int Choice
        {
            get { return choice; }
        }

        private void Initialize(IEnumerable<ChoiceDescription> choices, int defaultChoice)
        {
            int max = 0;
            int cnt = 0;
            panelContent.SuspendLayout();
            foreach (var c in choices)
            {
                var rad = new RadioButton
                    {
                        Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                        Left = 20,
                        Top = max,
                        Text = c.Label,
                        Checked = cnt == defaultChoice,
                        Tag = cnt,
                    };
                rad.CheckedChanged += RadoiButtonCheckedChanged;
                buttons.Add(rad);
                panelContent.Controls.Add(rad);
                cnt++;
                max += rad.Height;
            }
            //var diff = Height - panelContent.Height;
            //Height = diff + max;
            panelContent.ResumeLayout();
        }

        private void RadoiButtonCheckedChanged(object sender, EventArgs e)
        {
            var rad = (RadioButton)sender;
            if (rad.Checked)
            {
                choice = (int)rad.Tag;
            }
        }

        public static int Prompt(string caption, string message, IEnumerable<ChoiceDescription> choices, int defaultChoice)
        {
            using (var form = new ChoiceForm(caption, message, choices, defaultChoice))
            {
                var res = form.ShowDialog();
                return res == DialogResult.OK ? form.Choice : defaultChoice;
            }
        }
    }
}
