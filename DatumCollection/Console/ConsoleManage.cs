using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DatumCollection.Console
{
    /// <summary>
    /// 控制台管理
    /// </summary>
    public static class ConsoleManage
    {
        static IntPtr _handle;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOW = 5;

        static ConsoleManage()
        {
            _handle = GetConsoleWindow();
        }
        /// <summary>
        /// 最大化
        /// </summary>
        public static void Maximize()
        {
            ShowWindow(_handle, SW_SHOW);
        }

        /// <summary>
        /// 最小化
        /// </summary>
        public static void Minimize()
        {
            ShowWindow(_handle, SW_SHOWMINIMIZED);
        }

        /// <summary>
        ///隐藏
        /// </summary>
        public static void Hide()
        {
            ShowWindow(_handle, SW_HIDE);
        }
    }
}
