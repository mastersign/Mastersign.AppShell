using System;
using System.Management.Automation.Host;

namespace de.mastersign.shell
{
    public class SizeGatheringEventArgs : EventArgs
    {
        private Size size = new Size(int.MaxValue, int.MaxValue);

        public void NotifyMaxSize(Size maxSize)
        {
            size.Width = Math.Min(size.Width, maxSize.Width);
            size.Height = Math.Min(size.Height, maxSize.Height);
        }

        public Size MaxSize
        {
            get { return size; }
        }
    }
}