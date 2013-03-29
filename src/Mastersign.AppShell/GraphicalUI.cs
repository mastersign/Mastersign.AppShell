using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace de.mastersign.shell
{
    internal class GraphicalUI : BaseUI
    {
        public GraphicalUI(ConsoleBuffer buffer) : base(buffer)
        {
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message,
                                                            Collection<FieldDescription> descriptions)
        {
            return PromptDialog.Prompt(caption, message, descriptions);
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices,
                                            int defaultChoice)
        {
            return ChoiceForm.Prompt(caption, message, choices, defaultChoice);
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName,
                                                         string targetName, PSCredentialTypes allowedCredentialTypes,
                                                         PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName,
                                                         string targetName)
        {
            throw new NotImplementedException();
        }

        public override string ReadLine()
        {
            return ReadLineDialog.ReadLine();
        }

        public override SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException();
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            throw new NotImplementedException();
        }
    }
}