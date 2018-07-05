using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SmartQQ;
using System.Drawing;

namespace SmartQQTest
{
    public class ConsoleHelper
    {
        /// <summary>
        /// 在控制台绘制图片
        /// </summary>
        /// <param name="source"></param>
        public static void ConsoleWriteImage(Bitmap source)
        {
            Bitmap bmp = (Bitmap)source;

            int w = bmp.Width;
            int h = bmp.Height;

            int max = 28;

            // we need to scale down high resolution images...
            int complexity = (int)Math.Floor(Convert.ToDecimal(((w / max) + (h / max)) / 2));

            if (complexity < 1) { complexity = 1; }

            for (var x = 0; x < w; x += complexity)
            {
                for (var y = 0; y < h; y += complexity)
                {
                    Color clr = bmp.GetPixel(x, y);
                    Console.ForegroundColor = getNearestConsoleColor(clr);
                    Console.Write("█");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
        private static ConsoleColor getNearestConsoleColor(Color color)
        {
            // this is very likely to be awful and hilarious
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int total = r + g + b;
            decimal darkThreshold = 0.35m; // how dark a color has to be overall to be the dark version of a color

            ConsoleColor cons = ConsoleColor.White;


            if (total >= 39 && total < 100 && areClose(r, g) && areClose(g, b) && areClose(r, b))
            {
                cons = ConsoleColor.DarkGray;
            }

            if (total >= 100 && total < 180 && areClose(r, g) && areClose(g, b) && areClose(r, b))
            {
                cons = ConsoleColor.Gray;
            }


            // if green is the highest value
            if (g > b && g > r)
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkGreen;
                }
                else
                {
                    cons = ConsoleColor.Green;
                }
            }

            // if red is the highest value
            if (r > g && r > b)
            {

                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkRed;
                }
                else
                {
                    cons = ConsoleColor.Red;
                }
            }

            // if blue is the highest value
            if (b > g && b > r)
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkBlue;
                }
                else
                {
                    cons = ConsoleColor.Blue;
                }
            }


            if (r > b && g > b && areClose(r, g))
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkYellow;
                }
                else
                {
                    cons = ConsoleColor.Yellow;
                }
            }



            if (b > r && g > r && areClose(b, g))
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkCyan;
                }
                else
                {
                    cons = ConsoleColor.Cyan;
                }
            }





            if (r > g && b > g && areClose(r, b))
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkMagenta;
                }
                else
                {
                    cons = ConsoleColor.Magenta;
                }
            }

            if (total >= 180 && areClose(r, g) && areClose(g, b) && areClose(r, b))
            {
                cons = ConsoleColor.White;
            }


            // BLACK
            if (total < 39)
            {
                cons = ConsoleColor.Black;
            }





            return cons;
        }
        /// <summary>
        /// Returns true if the numbers are pretty close
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool areClose(int a, int b)
        {
            int diff = Math.Abs(a - b);

            if (diff < 30)
            {
                return true;
            }
            else return false;

        }
        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="filePathName"></param>
        private static Process ShowImage(string filePathName)
        {
            //建立新的系统进程    
            Process process = new Process();

            //设置图片的真实路径和文件名    
            process.StartInfo.FileName = filePathName;

            //设置进程运行参数，这里以最大化窗口方法显示图片。    
            process.StartInfo.Arguments = "rundl132.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen";

            //此项为是否使用Shell执行程序，因系统默认为true，此项也可不设，但若设置必须为true    
            process.StartInfo.UseShellExecute = true;

            //此处可以更改进程所打开窗体的显示样式，可以不设    
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.Start();

            return process;
            //Process process = Process.Start(filePathName, "rundl132.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen");
        }
    }
}
