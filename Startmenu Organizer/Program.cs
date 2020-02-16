using System;
using System.IO;
using System.Linq;

namespace Startmenu_Organizer
{
    class Program
    {

        public static string[] whitelistArray = { "Microsoft", "Windows", "Accessibility", "Accessories", "System Tools" };
        public static string[] blacklistArray = { "Uninstall", "EULA", "End User License Agreement", "Manual", "deinstallieren", "entfernen" };

        static void Main(string[] args)
        {
            string[] menus =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Programs",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs",
            };
            foreach (var menuPath in menus)
            {
                cleanupMenu(menuPath);
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        static void cleanupMenu(string menuPath)
        {
            Console.WriteLine("Cleaning " + menuPath);
            try
            {
                //Get all files
                string[] entries = Directory.GetFileSystemEntries(menuPath, "*.lnk", SearchOption.AllDirectories);
                string[] urls = Directory.GetFileSystemEntries(menuPath, "*.url", SearchOption.AllDirectories);

                //Loop through each file
                foreach (var longPath in entries)
                {
                    //Get short path and filename of file

                    string shortPath = longPath.Remove(longPath.IndexOf(menuPath), menuPath.Length);
                    string fileName = Path.GetFileName(longPath);

                    //Check if file is in protected folder
                    if (!whitelistArray.Any(shortPath.Contains))
                    {
                        //Check if file is removable junk
                        if (blacklistArray.Any(shortPath.Contains))
                        {
                            File.Delete(longPath);
                        }
                        else
                        {
                            string newPath = menuPath + "\\" + fileName;
                            //Check if file is in root folder already
                            if (longPath != newPath)
                            {
                                //Check if file exists in new location

                                if (File.Exists(newPath))
                                {
                                    File.Delete(newPath);
                                }
                                File.Move(longPath, newPath);
                            }
                        }
                    }
                }
                foreach (var urlPath in urls)
                {
                    File.Delete(urlPath);
                }
                //Delete empty folders
                processDirectory(menuPath);
            }
            catch (InvalidCastException e)
            {
                //Console.WriteLine(e);
                Console.WriteLine("Could not reorganize " + menuPath);
            }
        }

        static void processDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                processDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}
