using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Runtime.InteropServices;

namespace NoxMacro
{
    class ProcessClass
    {
        //---> 프로세스 찾기 
        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //---> 프로세스 캡쳐
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        
        //---> 프로세스 좌표 S E T
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, uint uFlags);

        //---> 프로세스 좌표 G E T
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        private IntPtr findwindow;
        
        public ProcessClass(string _procName)
        {
            findwindow = FindWindow(null, _procName);
        }

        public void SetProcessPosition(int pos_X, int pos_Y, int pWidth, int pHieght)
        {

            //Graphics Graphicsdata = Graphics.FromHwnd(findwindow);
            //Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

            SetWindowPos(findwindow, IntPtr.Zero, pos_X, pos_Y, pWidth, pHieght, 0x0040);
        }


        RECT rect;
        public void GetProcessPosition(out int Left, out int Top)
        {            
            GetWindowRect(new HandleRef(this, findwindow), out rect);
            Console.WriteLine(rect.Right);
            Console.WriteLine(rect.Left);
            Left = rect.Left;
            Console.WriteLine(rect.Top);
            Top = rect.Top;
            Console.WriteLine(rect.Bottom);
            Console.WriteLine("Form Size (W):" + (rect.Right - rect.Left));            // Process Width
            Console.WriteLine("Form Size (H) : " + (rect.Bottom - rect.Top));        // Process Height
        }

        public Bitmap GetProcessImage()
        {
            Bitmap bmp = null;

            if (findwindow != IntPtr.Zero)
            {
                ////플레이어를 찾았을 경우 클릭이벤트를 발생시킬 핸들을 가져옵니다.
                //IntPtr hwnd_child = FindWindowEx(findwindow, IntPtr.Zero, "RenderWindow", "TheRender");
                //IntPtr lparam = new IntPtr(x | (y << 16));

                ////플레이어 핸들에 클릭 이벤트를 전달합니다.
                //SendMessage(hwnd_child, WM_LBUTTONDOWN, 1, lparam);
                //SendMessage(hwnd_child, WM_LBUTTONUP, 0, lparam);
                // https://yc0345.tistory.com/224

                Console.WriteLine("PASS");

                Graphics Graphicsdata = Graphics.FromHwnd(findwindow);
                Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                bmp = new Bitmap(rect.Width + 35, rect.Height + 35); // free size = + 35
                Console.WriteLine("W W W >>> {0}", rect.Width);
                Console.WriteLine("H H H >>> {0}", rect.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(findwindow, hdc, 0x2);
                    g.ReleaseHdc(hdc);
                }
            }
            else
            {
                Console.WriteLine("FAIL");
            }

            return bmp;
        }
    }
}
