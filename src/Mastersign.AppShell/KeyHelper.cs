using System.Management.Automation.Host;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace de.mastersign.shell
{
    internal static partial class KeyHelper
    {
        static KeyHelper()
        {
            InitPrintables();
        }

        public static bool IsCancelKey(KeyInfo keyInfo)
        {
            return keyInfo.Character == 'c' &&
                   (keyInfo.ControlKeyState == ControlKeyStates.LeftCtrlPressed) ||
                   (keyInfo.ControlKeyState == ControlKeyStates.RightCtrlPressed);
        }

        public static KeyInfo BuildKeyInfo(KeyEventArgs e, bool keyDown)
        {
            return new KeyInfo(
                (int) e.KeyCode,
                (char) 0,
                GetCurrentControlKeyStates(),
                keyDown);
        }

        public static KeyInfo BuildKeyInfo(
            KeyEventArgs lastKeyEventArgs, KeyPressEventArgs keyPressEventArgs, bool keyDown)
        {
            return new KeyInfo(
                (int) lastKeyEventArgs.KeyCode,
                keyPressEventArgs.KeyChar,
                GetCurrentControlKeyStates(),
                keyDown);
        }

        public static ControlKeyStates BuildControlKeyStates(Keys keyData)
        {
            ControlKeyStates cks = 0;
            if ((keyData & Keys.LControlKey) != 0 ||
                (keyData & Keys.ControlKey) != 0 ||
                (keyData & Keys.Control) != 0)
            {
                cks |= ControlKeyStates.LeftCtrlPressed;
            }
            if ((keyData & Keys.RControlKey) != 0 ||
                (keyData & Keys.ControlKey) != 0 ||
                (keyData & Keys.Control) != 0)
            {
                cks |= ControlKeyStates.RightCtrlPressed;
            }
            if ((keyData & Keys.LShiftKey) != 0 ||
                (keyData & Keys.RShiftKey) != 0 ||
                (keyData & Keys.ShiftKey) != 0 ||
                (keyData & Keys.Shift) != 0)
            {
                cks |= ControlKeyStates.ShiftPressed;
            }
            if ((keyData & Keys.Alt) != 0)
            {
                cks |= ControlKeyStates.LeftAltPressed |
                       ControlKeyStates.RightAltPressed;
            }
            if ((keyData & Keys.CapsLock) != 0)
            {
                cks |= ControlKeyStates.CapsLockOn;
            }
            if ((keyData & Keys.Scroll) != 0)
            {
                cks |= ControlKeyStates.ScrollLockOn;
            }
            if ((keyData & Keys.NumLock) != 0)
            {
                cks |= ControlKeyStates.NumLockOn;
            }
            return cks;
        }

        public static bool IsPrintable(Keys keyCode)
        {
            return PrintableKeys[keyCode];
        }

        private static ControlKeyStates GetCurrentControlKeyStates()
        {
            ControlKeyStates cks = 0x00;
            if (GetKeyState((uint) Keys.CapsLock) != 0)
            {
                cks |= ControlKeyStates.CapsLockOn;
            }
            if (GetKeyState((uint) Keys.Alt) != 0)
            {
                cks |= ControlKeyStates.LeftAltPressed;
                cks |= ControlKeyStates.RightAltPressed;
            }
            if (GetKeyState((uint) Keys.LControlKey) != 0)
            {
                cks |= ControlKeyStates.LeftCtrlPressed;
            }
            if (GetKeyState((uint) Keys.NumLock) != 0)
            {
                cks |= ControlKeyStates.NumLockOn;
            }
            if (GetKeyState((uint) Keys.RControlKey) != 0)
            {
                cks |= ControlKeyStates.RightCtrlPressed;
            }
            if (GetKeyState((uint) Keys.Scroll) != 0)
            {
                cks |= ControlKeyStates.ScrollLockOn;
            }
            if (GetKeyState((uint) Keys.Shift) != 0)
            {
                cks |= ControlKeyStates.ShiftPressed;
            }
            return cks;
        }

        [DllImport("user32.dll")]
        private static extern short GetKeyState(uint key);

        //[DllImport("user32.dll")]
        //private static extern bool GetKeyboardState(byte[] lpKeyState);

        //[DllImport("user32.dll")]
        //private static extern uint MapVirtualKey(Keys uCode, VirtualKeyMapType mapType);

        //public enum VirtualKeyMapType : uint
        //{
        //    MAPVK_VK_TO_VSC = 0x0,
        //    MAPVK_VSC_TO_VK = 0x1,
        //    MAPVK_VK_TO_CHAR = 0x2,
        //    MAPVK_VSC_TO_VK_EX = 0x3,
        //    MAPVK_VK_TO_VSC_EX = 0x4,
        //}
    }
}