using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BingWallpaper
{
    class Program
    {
        #region
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
      UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;
        #endregion

        static public void SetAsWallpaper(String path)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 0.ToString()); // 2 is stretched
            key.SetValue(@"TileWallpaper", 0.ToString());

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Trying to fetch the wallpaper...\n\n");

            //Folder to download the wallpaper
            string dwFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Bing Wallpapers\\";


            XmlDocument doc1 = new XmlDocument();
            doc1.Load("https://www.bing.com/HPImageArchive.aspx?format=xlm&idx=0&n=1"); //for a specific region, use "&mkt=en-us" at the end.


            string baseUrl = doc1.SelectSingleNode(@"/images/image/urlBase/text()").Value;

            WebClient webClient = new WebClient();
            string fileURL = "https://www.bing.com" + baseUrl + "_1920x1080.jpg";
            string savedFile = dwFolder + baseUrl.Substring(11, baseUrl.Length - 11) + ".jpg";

            //check if directory exists, if not create it
            bool folderExists = Directory.Exists(dwFolder);
            if (!folderExists)
            {
                Directory.CreateDirectory(dwFolder);
            }

            //check if file already exists
            if (File.Exists(savedFile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error:\nFile already exists. Press any key to continue.");
                Console.ReadKey();
            }

            //if not, then download
            else
            {
                webClient.DownloadFile(fileURL, savedFile);

                SetAsWallpaper(savedFile);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Download completed. The file name is: " + baseUrl.Substring(11, baseUrl.Length - 11) + Environment.NewLine + "Press any key to continue.");
                Console.ReadKey();
            }

           
        }
    }
}
