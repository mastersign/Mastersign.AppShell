using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    internal class ScrollMessageFilter : IMessageFilter
    {
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private readonly Control targetControl;
        private readonly List<Control> sensibleControls;

        public ICollection<Control> ScrollSensibleControls { get { return sensibleControls; } }

        public ScrollMessageFilter(Control targetControl)
        {
            if (targetControl == null)
            {
                throw new ArgumentNullException("targetControl");
            }
            this.targetControl = targetControl;
            sensibleControls = new List<Control>();
            sensibleControls.Add(targetControl);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                var pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                var hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero &&
                    hWnd != m.HWnd &&
                    IsSensible(Control.FromHandle(hWnd)))
                {
                    SendMessage(targetControl.Handle, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        private bool IsSensible(Control control)
        {
            return control != null &&
                   sensibleControls.Contains(control);
        }
    }
}
