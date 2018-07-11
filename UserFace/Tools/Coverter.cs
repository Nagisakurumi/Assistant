using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserFace.Tools
{
    public class Coverter
    {
        /// <summary>
        /// 转换Bitmap到BitmapSource
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapSource GetBitmapSource(System.Drawing.Bitmap bmp)
        {
            System.Windows.Media.Imaging.BitmapSource bmpf = null;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                bmpf = System.Windows.Media.Imaging.BitmapFrame.Create(ms, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);

            }
            return bmpf;
            //return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
