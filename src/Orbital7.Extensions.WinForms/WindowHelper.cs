﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Orbital7.Extensions.WinForms
{
    public static class WindowHelper
    {
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, ref Int32 lpdwProcessId);
        [DllImport("User32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        static extern int ShowWindowAsync(IntPtr hwnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_NORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_MAX = 10;
        private const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
        private const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
        private const int SPIF_SENDCHANGE = 0x2;

        public static void ForceWindowToForeground(long handle)
        {
            ForceWindowToForeground(new IntPtr(handle));
        }

        // Source: http://stackoverflow.com/questions/46030/c-force-form-focus
        public static void ForceWindowToForeground(IntPtr hWnd)
        {
            if (IsIconic(hWnd))
                ShowWindowAsync(hWnd, SW_RESTORE);
            ShowWindowAsync(hWnd, SW_SHOW);
            SetForegroundWindow(hWnd);
            // Code from Karl E. Peterson, www.mvps.org/vb/sample.htm      
            // Converted to Delphi by Ray Lischner      
            // Published in The Delphi Magazine 55, page 16      
            // Converted to C# by Kevin Gale      
            IntPtr foregroundWindow = GetForegroundWindow();
            IntPtr Dummy = IntPtr.Zero;
            uint foregroundThreadId = GetWindowThreadProcessId(foregroundWindow, Dummy);
            uint thisThreadId = GetWindowThreadProcessId(hWnd, Dummy);
            if (AttachThreadInput(thisThreadId, foregroundThreadId, true))
            {
                BringWindowToTop(hWnd); // IE 5.5 related hack        
                SetForegroundWindow(hWnd);
                AttachThreadInput(thisThreadId, foregroundThreadId, false);
            }
            if (GetForegroundWindow() != hWnd)
            {
                // Code by Daniel P. Stasinski        
                // Converted to C# by Kevin Gale        
                IntPtr Timeout = IntPtr.Zero;
                SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, Timeout, 0);
                SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, Dummy, SPIF_SENDCHANGE);
                BringWindowToTop(hWnd); // IE 5.5 related hack        
                SetForegroundWindow(hWnd);
                SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, Timeout, SPIF_SENDCHANGE);
            }
        }

        public static NativeWindow GetNativeWindow()
        {
            NativeWindow mainWindow = null;

            // Get the current window and continue only if the main window has a title (defects #594253 and #595230).
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            if (!string.IsNullOrEmpty(currentProcess.MainWindowTitle))
            {
                mainWindow = new NativeWindow();
                mainWindow.AssignHandle(currentProcess.MainWindowHandle);
            }

            return mainWindow;
        }

        public static IntPtr GetActiveWindow()
        {
            return GetForegroundWindow();
        }

        public static DialogResult ShowDialog(Form dialog)
        {
            DialogResult dialogResult = DialogResult.Cancel;

            // Update the UI.
            System.Windows.Forms.Application.DoEvents();

            // Try to use the main window.
            NativeWindow mainWindow = GetNativeWindow();
            if (mainWindow != null)
            {
                dialogResult = dialog.ShowDialog(mainWindow);
                mainWindow.ReleaseHandle();

                // Update the UI.
                System.Windows.Forms.Application.DoEvents();
            }
            else
            {
                dialogResult = dialog.ShowDialog();
                System.Windows.Forms.Application.DoEvents();
            }

            return dialogResult;
        }
    }
}
