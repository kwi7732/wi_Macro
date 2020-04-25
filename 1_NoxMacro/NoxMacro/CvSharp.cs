using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

using System.Drawing;
using System.Windows;

using System.Windows.Threading;

namespace NoxMacro
{
    class CvSharp
    {
        private string sScreen_img;
        private string sFind_img;

        Mat screen = null, find = null, res = null;
        public CvSharp(string _sSetScreen, string _sSetImage)
        {
            this.sScreen_img = _sSetScreen;
            this.sFind_img = _sSetImage;
        }

        public double ImageSearch_FullScreen(out int getX, out int getY)
        {
            getX = 0;
            getY = 0;

            try
            {
                screen = OpenCvSharp.Extensions.BitmapConverter.ToMat(new Bitmap(sScreen_img));
                find = OpenCvSharp.Extensions.BitmapConverter.ToMat(new Bitmap(sFind_img));

                res = screen.MatchTemplate(find, TemplateMatchModes.CCoeffNormed);

                Cv2.MinMaxLoc(res, out double minval, out double maxval, out OpenCvSharp.Point minloc, out OpenCvSharp.Point maxloc);

                getX = maxloc.X;
                getY = maxloc.Y;

                Console.WriteLine("유사도 = " + maxval);

                return maxval;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                screen.Dispose();
                find.Dispose();
                res.Dispose();
            }

            return -1;
        }




        public double ImageSearch_Process(out int getX, out int getY)
        {
            getX = 0;
            getY = 0;

            try
            {
                screen = OpenCvSharp.Extensions.BitmapConverter.ToMat(new Bitmap(sScreen_img));
                find = OpenCvSharp.Extensions.BitmapConverter.ToMat(new Bitmap(sFind_img));

                res = screen.MatchTemplate(find, TemplateMatchModes.CCoeffNormed);

                Cv2.MinMaxLoc(res, out double minval, out double maxval, out OpenCvSharp.Point minloc, out OpenCvSharp.Point maxloc);


                getX = maxloc.X;
                getY = maxloc.Y;

                Console.WriteLine("유사도 = " + maxval);

                Console.WriteLine("getX >>> {0}", getX);
                Console.WriteLine("getY >>> {0}", getY);//여기 

                return maxval;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                screen.Dispose();
                find.Dispose();
                res.Dispose();
            }

            return -1;
        }       
    }
}

