using System;
using System.Windows;
using System.Drawing;
using System.IO;

using System.Threading;
using System.Windows.Input;
using System.Runtime.InteropServices;


// 2020-04-13, 00:30
// 키보드 후킹 전단계,
// 후킹 클래스로 따로 관리하고, Main Closing 할때 Class.후킹해제 할것
namespace NoxMacro
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        static string Desktop_Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static string TEST_PATH = string.Format("{0}\\{1}", Desktop_Path, "Macro_Folder");
        static string sScreen_Path = string.Format("{0}\\{1}", TEST_PATH, "Wik_Screen");
        //static string sScreen_Path = @"D:\MacroTEST\Wik_Screen";    //------------------------------------------------------------------------ Debug

        static string sFind_Path = string.Format("{0}\\{1}", TEST_PATH, "Wik_Find");
        //static string sFind_Path = @"D:\MacroTEST\Wik_Find";    //------------------------------------------------------------------------------  Debug

        int findImgCount = 0;
        string processName;
        public MainWindow()
        {
            InitializeComponent();            

            InitFolder_Find(out findImgCount);
            InitFolder_Screen();

            InitEvent();            
        }

        private void InitEvent()
        {
            this.KeyUp += MainWindow_KeyUp;
        }



        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.F1:
                    FullScreen_ImageSearch();
                    break;

                case Key.F5:
                    processName = "카카오톡"; // or LD-Player~~
                    Process_ImageSearch();
                    break;

                case Key.F6:
                    MessageBox.Show("zz");
                    break;
            }
        }

        private void FullScreen_ImageSearch()
        {
            int screenCnt = 0;
            for (int cnt = 1; cnt <= findImgCount; cnt++)
            {
                bool ret = false;
                while (ret != true && screenCnt < 10)
                {
                    string sFindImg = string.Format("{0}\\Find{1}.PNG", sFind_Path, cnt);
                    string sScreenImg = string.Format("{0}\\Screen{1}.PNG", sScreen_Path, screenCnt++);
                    ScreenCapture(sScreenImg);
                    ret = Click_Find_Image("full",sScreenImg, sFindImg);
                    Thread.Sleep(10);
                }
            }
        }


        private void Process_ImageSearch()
        {
            int screenCnt = 0;
            // 파일 없을때 예외처리 넣기 ㄱㄱ
            // PNG 파일만 처리 ㄱㄱ
            // 
            for (int cnt = 1; cnt <= findImgCount; cnt++)
            {
                bool ret = false;
                while (ret != true && screenCnt < 10)
                {
                    string sFindImg = string.Format("{0}\\Find{1}.PNG", sFind_Path, cnt);
                    string sScreenImg = string.Format("{0}\\Screen{1}.PNG", sScreen_Path, screenCnt++);
                    ProcessCapture(sScreenImg);
                    ret = Click_Find_Image("proc", sScreenImg, sFindImg);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(3000);
            }
        }


        private int ProcLeft = 0;
        private int ProcTop = 0;
        private bool ProcessCapture(string sPathName)
        {
            ProcessClass aa = new ProcessClass(processName);
            //aa.SetProcessPosition();
            aa.GetProcessPosition(out int _Left, out int _Top);
            ProcLeft = _Left;
            ProcTop = _Top;
            var bbb = aa.GetProcessImage();
            bbb.Save(sPathName);    // 여러번 캡처할 경우 에러 처리 ㄱㄱ

            return false;
        }


        private void InitFolder_Find(out int _findImgCount)
        {
            DirectoryInfo drFindFolder = new DirectoryInfo(sFind_Path);
            if (drFindFolder.Exists == false)
            {
                drFindFolder.Create();
                _findImgCount = 0;
                Console.WriteLine("Find Create.");
            }
            else
            {
                _findImgCount = drFindFolder.GetFiles().Length;
                Console.WriteLine("Find Folder Count: " + _findImgCount);
            }
        }

        private void InitFolder_Screen()
        {
            DirectoryInfo drScreenFolder = new DirectoryInfo(sScreen_Path);
            if (drScreenFolder.Exists == false)
            {
                drScreenFolder.Create();
                Console.WriteLine("Screen Create.");
            }
        }

        private bool Click_Find_Image(string ImageFrame, string _sScreen_img, string _sFind_img)
        {
            if (!(new FileInfo(_sFind_img).Exists))
            {
                Console.WriteLine("\n\t [ERROR]: There no \"Find\" File.");
                return false;
            }

            var cvs = new CvSharp(_sScreen_img, _sFind_img);
            var mouse = new MouseKeyEvent();
            double ImageCompareValue = 0;
            int posX = 0, posY = 0;
            if (ImageFrame == "full")
            {
                ImageCompareValue = cvs.ImageSearch_FullScreen(out int Target_X, out int Target_Y);
                posX = Target_X;
                posY = Target_Y;
            }
            else if (ImageFrame == "proc")
            {
                ImageCompareValue = cvs.ImageSearch_Process(out int Target_X, out int Target_Y);
                posX = Target_X + ProcLeft;
                posY = Target_Y + ProcTop;
            }
            else Console.WriteLine("ERROR");
           

            if (ImageCompareValue > 0.8)
            {
                mouse.MoveMouse(posX, posY);
                mouse.Click_event(2);
                return true;
            }
            else
            {
                Console.WriteLine("\n\t [ERROR]: Image Not Found. ({0})", _sFind_img);
                return false;
            }
        }


        private bool ScreenCapture(string sPathName)
        {
            if (File.Exists(sPathName))
            {
                File.Delete(sPathName);
            }

            var screenLeft = SystemParameters.VirtualScreenLeft;
            var screenTop = SystemParameters.VirtualScreenTop;
            var screenWidth = SystemParameters.VirtualScreenWidth;
            var screenHeight = SystemParameters.VirtualScreenHeight;

            using (var bmp = new Bitmap((int)screenWidth, (int)screenHeight))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    Opacity = .0;
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    bmp.Save(sPathName);
                    Opacity = 1;
                }
            }

            if (File.Exists(sPathName))
            {
                return true;
            }
            // else
            return false;
        }

        private bool IsChatRoomCapture(string test)
        {
            if (File.Exists(test))
            {
                File.Delete(test);
            }
            MouseKeyEvent ttt = new MouseKeyEvent();
            ttt.GetCursorXY(out int posX, out int posY);

            int rectGap = 25;

            //ttt.SetCursorXY(posX - rectGap, posY - rectGap);
            //Thread.Sleep(1000);
            //ttt.SetCursorXY(posX + rectGap, posY - rectGap);
            //Thread.Sleep(1000);
            //ttt.SetCursorXY(posX - rectGap, posY + rectGap);
            //Thread.Sleep(1000);
            //ttt.SetCursorXY(posX + rectGap, posY + rectGap);

            int startPointX = posX - rectGap;
            int startPointY = posY - rectGap;
            
            var screenWidth = rectGap * 2;
            var screenHeight = rectGap * 2;

            using (var bmp = new Bitmap((int)screenWidth, (int)screenHeight))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    Opacity = .0;
                    g.CopyFromScreen((int)startPointX, (int)startPointY, 0, 0, bmp.Size);
                    bmp.Save(test);
                    Opacity = 1;
                }
            }

            if (File.Exists(test))
            {
                return true;
            }
            // else
            return false;
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenChatMacro(false);
        }

        private void OpenChatMacro(bool write)
        {
            ProcessClass aa = new ProcessClass("카카오톡");
            MouseKeyEvent ttt = new MouseKeyEvent();

            aa.GetProcessPosition(out int z, out int b);
            aa.SetProcessPosition(10, 10, 500, 855);
            Thread.Sleep(50);

            for (int j = 1; j <= 99; j++)
            {
                int posX = 120;
                int posY = 100;

                for (int i = 1; i <= 999; i++)
                {
                    posY += 70;
                    ttt.SetCursorXY(posX, posY);

                    string temp = sScreen_Path + "\\" + j + "_" + i + ".PNG";
                    string tt = @"C:\Users\MYCOM\Desktop\Macro_Folder\Wik_Find\\TTT.PNG";
                    IsChatRoomCapture(temp);

                    bool check = ImageCmp(temp, tt);
                    if (!check)
                    {
                        ttt.Click_event(2);
                        Thread.Sleep(500);

                        System.Windows.Forms.SendKeys.SendWait("^v");
                        Thread.Sleep(200);

                        if(write == true)
                        {
                            System.Windows.Forms.SendKeys.SendWait("{Enter}");
                            Thread.Sleep(200);
                        }                                                

                        System.Windows.Forms.SendKeys.SendWait("{ESC}");
                        Thread.Sleep(200);
                    }
                    else
                    {
                        return;
                    }
                }

                ttt.SetMouseWheel(-850);
                Thread.Sleep(300);
            }
        }
        private bool ImageCmp(string img1, string img2)
        {

            var src = new Bitmap(img1);
            var bmp = new Bitmap(img2);
            string srcInfo, bmpInfo;

            for (int i = 0; i < src.Width; i++)
            {
                for (int j = 0; j < src.Height; j++)
                {
                    srcInfo = src.GetPixel(i, j).ToString();
                    bmpInfo = bmp.GetPixel(i, j).ToString();

                    if (srcInfo != bmpInfo)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenChatMacro(true);
        }
    }
}



