// Copyright Alex Shvedov
//             Modified by MercuryP with color specifications
// 2014-09-23  Modified by KiloBravo  Placed in class 
// Use this code in any way you want

using System;
using System.Drawing;                    // for Color (add reference to  System.Drawing.assembly)
using System.Runtime.InteropServices;    // for StructLayout

using System.Diagnostics;                // for Debug

namespace ConsolAnyColor
{
    class ConsolAnyColorClass
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SMALL_RECT
        {
            internal short Left;
            internal short Top;
            internal short Right;
            internal short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct COLORREF
        {
            internal uint ColorDWORD;

            internal COLORREF(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }

            internal COLORREF(uint r, uint g, uint b)
            {
                ColorDWORD = r + (g << 8) + (b << 16);
            }

            internal Color GetColor() => Color.FromArgb((int)(0x000000FFU & ColorDWORD), (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);

            internal void SetColor(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            internal int cbSize;
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal ushort wAttributes;
            internal SMALL_RECT srWindow;
            internal COORD dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            internal COLORREF black;
            internal COLORREF darkBlue;
            internal COLORREF darkGreen;
            internal COLORREF darkCyan;
            internal COLORREF darkRed;
            internal COLORREF darkMagenta;
            internal COLORREF darkYellow;
            internal COLORREF gray;
            internal COLORREF darkGray;
            internal COLORREF blue;
            internal COLORREF green;
            internal COLORREF cyan;
            internal COLORREF red;
            internal COLORREF magenta;
            internal COLORREF yellow;
            internal COLORREF white;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY
        {
            internal int cbSize;
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal ushort wAttributes;
            internal SMALL_RECT srWindow;
            internal COORD dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal COLORREF[] colors;

            /* 
            internal COLORREF black;
            internal COLORREF darkBlue;
            internal COLORREF darkGreen;
            internal COLORREF darkCyan;
            internal COLORREF darkRed;
            internal COLORREF darkMagenta;
            internal COLORREF darkYellow;
            internal COLORREF gray;
            internal COLORREF darkGray;
            internal COLORREF blue;
            internal COLORREF green;
            internal COLORREF cyan;
            internal COLORREF red;
            internal COLORREF magenta;
            internal COLORREF yellow;
            internal COLORREF white;    
            */
        }


        const int STD_OUTPUT_HANDLE = -11;                                        // per WinBase.h
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);    // per WinBase.h

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        // private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        // private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY csbe);

        // Set a specific console color to an RGB color
        // The default console colors used are gray (foreground) and black (background)
        public static int SetColor(ConsoleColor colorToSet, Color targetColor) => SetColor(colorToSet, new COLORREF(targetColor));

        public static int SetColor(ConsoleColor colorToSet, uint r, uint g, uint b) => SetColor(colorToSet, new COLORREF(r, g, b));

        public static int SetColor(ConsoleColor colorToSet, COLORREF targetColor)
        {
            // CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX_ARRAY();
            csbe.cbSize = (int)Marshal.SizeOf(csbe);                    // 96 = 0x60


            IntPtr hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);    // 7
            if (hConsoleOutput == INVALID_HANDLE_VALUE)
            {
                return Marshal.GetLastWin32Error();
            }
            bool brc = GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                return Marshal.GetLastWin32Error();
            }

            csbe.colors[(int)colorToSet] = targetColor;
            /*
            switch (colorToSet)
            {
                case ConsoleColor.Black:
                    csbe.black = targetColor;
                    break;
                case ConsoleColor.DarkBlue:
                    csbe.darkBlue = targetColor;
                    break;
                case ConsoleColor.DarkGreen:
                    csbe.darkGreen = targetColor;
                    break;
                case ConsoleColor.DarkCyan:
                    csbe.darkCyan = targetColor;
                    break;
                case ConsoleColor.DarkRed:
                    csbe.darkRed = targetColor;
                    break;
                case ConsoleColor.DarkMagenta:
                    csbe.darkMagenta = targetColor;
                    break;
                case ConsoleColor.DarkYellow:
                    csbe.darkYellow = targetColor;
                    break;
                case ConsoleColor.Gray:
                    csbe.gray = targetColor;
                    break;
                case ConsoleColor.DarkGray:
                    csbe.darkGray = targetColor;
                    break;
                case ConsoleColor.Blue:
                    csbe.blue = targetColor;
                    break;
                case ConsoleColor.Green:
                    csbe.green = targetColor;
                    break;
                case ConsoleColor.Cyan:
                    csbe.cyan = targetColor;
                    break;
                case ConsoleColor.Red:
                    csbe.red = targetColor;
                    break;
                case ConsoleColor.Magenta:
                    csbe.magenta = targetColor;
                    break;
                case ConsoleColor.Yellow:
                    csbe.yellow = targetColor;
                    break;
                case ConsoleColor.White:
                    csbe.white = targetColor;
                    break;
            }
            */ 

            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;
            brc = SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                return Marshal.GetLastWin32Error();
            }
            return 0;
        }

        public static int SetScreenColors(Color foregroundColor, Color backgroundColor)
        {
            int irc;
            irc = SetColor(ConsoleColor.Gray, foregroundColor);
            if (irc != 0) return irc;
            irc = SetColor(ConsoleColor.Black, backgroundColor);
            if (irc != 0) return irc;

            return 0;
        }

    }
}