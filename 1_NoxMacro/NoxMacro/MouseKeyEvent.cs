using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;


namespace NoxMacro
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

    class MouseKeyEvent
    {
        private const uint LBDOWN = 0x00000002; // 왼쪽 마우스 버튼 눌림
        private const uint LBUP = 0x00000004; // 왼쪽 마우스 버튼 떼어짐
        private const uint RBDOWN = 0x00000008; // 오른쪽 마우스 버튼 눌림
        private const uint RBUP = 0x000000010; // 오른쪽 마우스 버튼 떼어짐
        private const uint MBDOWN = 0x00000020; // 휠 버튼 눌림
        private const uint MBUP = 0x000000040; // 휠 버튼 떼어짐
        private const uint WHEEL = 0x00000800; //휠 스크롤


        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern int GetCursorPos(out POINT lpPoint);


        public void GetCursorXY(out int CurX, out int CurY)
        {
            var pt = GetCursorPosition();

            CurX = pt.X;
            CurY = pt.Y;
        }

        public void SetMouseWheel(int UpDown)
        {
            mouse_event(WHEEL, 0, 0, UpDown, 0);
        }

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);

            return lpPoint;
        }

        public void SetCursorXY(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void Click_event(int count)
        {
            for (var i = 0; i < count; i++)
            {

                mouse_event(LBDOWN, 0, 0, 0, 0);
                mouse_event(LBUP, 0, 0, 0, 0);

            }
        }

        public void MoveMouse(int Target_X, int Target_Y)
        {
            int Current_X = 0, Current_Y = 0;
            GetCursorXY(out Current_X, out Current_Y);

            double inclination = Math.Abs(((double)Target_Y - (double)Current_Y) / ((double)Target_X - (double)Current_X)); // ex) 1/3

            int leftRight = 0;  // LEFT -> DecreaseX,   RIGHT -> increaseX
            int upDown = 0;     // UP -> DecreaseY,     DOWN -> increaseY 

            if (Current_X < Target_X)
            {
                leftRight = 1;  // right
            }
            else
            {
                leftRight = -1; // left
            }

            if (Current_Y < Target_Y)
            {
                upDown = 1; // increase Y = DOWN
            }
            else
            {
                upDown = -1; // decrease Y = UP
            }

            int moveCnt = 1;
            const int speed = 10;
            while (moveCnt < 9999)
            {
                int slowMove_X = Current_X + (moveCnt * leftRight);
                int slowMove_Y = Current_Y + ((int)((double)moveCnt * inclination) * upDown);
                SetCursorXY(slowMove_X, slowMove_Y);
                moveCnt += 1;

                if (moveCnt % speed == 0)
                {
                    Thread.Sleep(1);
                }

                if (Target_X == slowMove_X)
                {
                    Console.WriteLine("Target Position [ X ] = {0}", slowMove_X);
                    Console.WriteLine("Target Position [ Y ] = {0}", slowMove_Y);
                    break;
                }
            }
        }

    }
}

