using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MenetrendTervezo.classes
{
    class HTMLrender
    {
        private static FileInfo filename = null;

        public static void Render(string html, int width, int height, string filename)
        {
            HTMLrender.filename = new FileInfo(filename);
            var browser = new WebBrowser { ScrollBarsEnabled = false };
            browser.Width = width;
            browser.Height = height;
            browser.DocumentText = html;
            browser.DocumentCompleted += WebBrowser_DocumentCompleted;

            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            browser.Dispose();

            //var chrome = new ChromiumWebBrowser();            
            //browser.Width = width;
            //browser.Height = height;
            //browser.FrameLoadEnd += Browser_FrameLoadEnd;
            //browser.InitializeLifetimeService();

            //browser.LoadHtml(html);
            //var valami = browser.IsBrowserInitialized;

            //while (!browser.IsLoading)
            //{
            //    Application.DoEvents();
            //}
        }

        private static void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser b = sender as WebBrowser;
            var bitmap = new Bitmap(b.Width, b.Height);
            Rectangle r = new Rectangle(0, 0, b.Width, b.Height);
            b.DrawToBitmap(bitmap, r);

            if (filename.Extension == ".png") bitmap.Save(HTMLrender.filename.FullName, ImageFormat.Png);
            else if (filename.Extension == ".jpg") bitmap.Save(HTMLrender.filename.FullName, ImageFormat.Jpeg);

            if (MessageBox.Show("Kívánja megtekinteni a fájlt?", "Export kész!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(HTMLrender.filename.FullName);
            }

            HTMLrender.filename = null;
        }

        private static void Wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
