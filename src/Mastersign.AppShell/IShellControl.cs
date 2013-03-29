using System;
using System.Collections.Generic;
using System.Drawing;

namespace de.mastersign.shell
{
    public interface IShellControl
    {
        ConsoleBuffer Buffer { get; set; }
        IDictionary<ConsoleColor, Brush> BackgroundBrushes { get; }
        IDictionary<ConsoleColor, Brush> ForegroundBrushes { get; }
        bool ProcessKeyStrokes { get; set; }
        int CursorBlinkInterval { get; set; }
        ConsoleDisplay.CursorShowMode CursorMode { get; set; }
        void UseShell(IShellComponent shell);
    }
}