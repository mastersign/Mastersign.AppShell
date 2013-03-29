using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Windows.Forms;
using Size=System.Drawing.Size;

namespace de.mastersign.shell
{
    public partial class PromptDialog : BaseDialog
    {
        private static PromptDialog dlg;

        private List<Field> Fields { get; set; }
        private Dictionary<string, PSObject> Values { get; set; }

        public static Dictionary<string, PSObject> Prompt(string caption,
            string message, Collection<FieldDescription> descriptions)
        {
            if (dlg == null)
            {
                dlg = new PromptDialog();
            }
            dlg.Initialize(caption, message, descriptions);
            dlg.ShowDialog();
            return new Dictionary<string, PSObject>(dlg.Values);
        }

        public PromptDialog()
        {
            InitializeComponent();
            Fields = new List<Field>();
            Values = new Dictionary<string, PSObject>();
        }

        private void Initialize(string caption, string message,
            IEnumerable<FieldDescription> descriptions)
        {
            Values.Clear();
            Fields.Clear();
            tableLayoutPanel.Controls.Clear();
            tableLayoutPanel.RowStyles.Clear();

            SetCaption(caption);
            SetMessage(message);

            int line = 0;
            foreach (var d in descriptions)
            {
                var f = new Field(d);
                Fields.Add(f);
                f.InsertIntoTable(tableLayoutPanel, line++);
            }

            Size = GetPreferredSize(new Size(600, 0));
        }

        protected override void OnFinish()
        {
            foreach (var f in Fields)
            {
                Values.Add(f.Description.Label, f.Value);
            }
        }

        private class Field
        {
            public FieldDescription Description { get; private set; }

            public Label CaptionLabel { get; private set; }

            public TextBox InputTextBox { get; private set; }

            public Field(FieldDescription description)
            {
                Description = description;
                CaptionLabel = new Label
                    {
                        Text = description.Name,
                        Tag = this,
                        Margin = new Padding(0, 12, 6, 6),
                        AutoSize = true,
                    };
                InputTextBox = new TextBox
                    {
                        Text = description.DefaultValue != null 
                            ? description.DefaultValue.ToString() 
                            : "",
                        Tag = this,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(6, 9, 0, 3),
                    };
            }

            public void InsertIntoTable(TableLayoutPanel table, int index)
            {
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                table.Controls.Add(CaptionLabel, 0, index);
                table.Controls.Add(InputTextBox, 1, index);
            }

            public PSObject Value
            {
                get { return new PSObject(InputTextBox.Text); }
            }
        }
    }
}