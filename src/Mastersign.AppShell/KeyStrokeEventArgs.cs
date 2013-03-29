using System;
using System.Management.Automation.Host;

namespace de.mastersign.shell
{
    public class KeyStrokeEventArgs : EventArgs
    {
        public KeyInfo KeyInfo { get; private set; }

        public KeyStrokeEventArgs(KeyInfo keyInfo)
        {
            KeyInfo = keyInfo;
        }

        public bool IsCancelKeyStroke
        {
            get { return KeyHelper.IsCancelKey(KeyInfo); }
        }
    }
}