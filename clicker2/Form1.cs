using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HWND = System.IntPtr;

namespace clicker2
{
    /// <summary>Contains functionality to get all the open windows.</summary>
    public static class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<HWND, string> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern long SetCursorPos(int x, int y);
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point point);

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(4000);
            Point p = new Point(1092, 0);
            GetCursorPos(out p);
            using (StreamWriter sw = new StreamWriter(@"C:\projects\clicker2\clicker2\set.txt", false, System.Text.Encoding.Default))
            {
                sw.WriteLine("x: " + p.X);
                sw.WriteLine("y: " + p.Y);
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            List<string> all_lines = new List<string>();
            using (StreamReader sr1 = new StreamReader(@"C:\projects\clicker2\clicker2\adres.txt"))
            {
                string line;
                while ((line = sr1.ReadLine()) != null)
                    all_lines.Add(line);
            }
            await Task.Delay(5000);
            while (true)
            {
                System.Diagnostics.Process paint = System.Diagnostics.Process.GetProcessesByName("mspaint").FirstOrDefault(); //musor
                if (paint != null)
                {
                    for (int i = 0; i < Convert.ToInt16(all_lines[1]); i++)
                    {
                        Point p1 = new Point(Convert.ToInt16(all_lines[2]), Convert.ToInt16(all_lines[3]));
                        ClientToScreen(GetDesktopWindow(), ref p1);
                        SetCursorPos(p1.X, p1.Y);
                        await Task.Delay(25);
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        await Task.Delay(25);
                    }
                    for (int i = 0; i < 3; i++)
                    {

                        Point p1 = new Point(Convert.ToInt16(all_lines[5 + i * 3]), Convert.ToInt16(all_lines[6 + i * 3]));
                        ClientToScreen(GetDesktopWindow(), ref p1);
                        SetCursorPos(p1.X, p1.Y);
                        await Task.Delay(50);
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        await Task.Delay(25);
                    }
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    SendKeys.Send("^{SUBTRACT}");
                    //    await Task.Delay(50);
                    //}

                    //
                    Point p;
                    for (int i = 0; i < Convert.ToInt16(all_lines[22]); i++)
                    {
                        GetCursorPos(out p);
                        if (p.X < 10 && p.Y < 10)
                        {
                            await Task.Delay(500);
                            break;
                        }
                        await Task.Delay(200);
                    }
                    //
                    for (int i = 0; i < 2; i++)
                    {
                        Point p1 = new Point(Convert.ToInt16(all_lines[14 + i * 3]), Convert.ToInt16(all_lines[15 + i * 3]));
                        ClientToScreen(GetDesktopWindow(), ref p1);
                        SetCursorPos(p1.X, p1.Y);
                        await Task.Delay(50);
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        await Task.Delay(25);
                    }
                }
                await Task.Delay(100);
                SendKeys.Send("{Down}");
                await Task.Delay(50);
                SendKeys.Send("{ENTER}");
                await Task.Delay(Convert.ToInt16(all_lines[20]));
                //
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread.Sleep(4000);
            foreach (KeyValuePair<IntPtr, string> window in OpenWindowGetter.GetOpenWindows())
            {
                IntPtr handle = window.Key;
                string title = window.Value;

                System.Diagnostics.Debug.WriteLine("{0}: {1}", handle, title);
            }
        }
    }
}
