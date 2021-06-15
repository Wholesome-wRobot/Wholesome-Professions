using robotManager.Helpful;
using robotManager.Products;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Wholesome_Professions_WotlK.Helpers;

public static class AutoUpdater
{
    public static void CheckUpdate(string mainVersion)
    {
        DateTime dateBegin = new DateTime(2020, 1, 1);
        DateTime currentDate = DateTime.Now;

        long elapsedTicks = currentDate.Ticks - dateBegin.Ticks;
        elapsedTicks /= 10000000;

        double timeSinceLastUpdate = elapsedTicks - WholesomeProfessionsSettings.CurrentSetting.LastUpdateDate;
        string currentFile = Others.GetCurrentDirectory + $@"\Products\{Main.productName}.dll";
        string oldFile = Others.GetCurrentDirectory + $@"\Products\{Main.productName} dmp";

        // On supprime la vieille version
        if (File.Exists(oldFile))
        {
            try
            {
                var fs = new FileStream(oldFile, FileMode.Open);
                if (fs.CanWrite)
                {
                    Logger.Log("Deleting dump file");
                    fs.Close();
                    File.Delete(oldFile);
                }
                fs.Close();
            }
            catch
            {
                ShowReloadMessage();
                return;
            }
        }

        // If last update try was < 10 seconds ago, we exit to avoid looping
        if (timeSinceLastUpdate < 10)
        {
            Logger.Log($"Last update attempt was {timeSinceLastUpdate} seconds ago. Exiting updater.");
            return;
        }

        try
        {
            WholesomeProfessionsSettings.CurrentSetting.LastUpdateDate = elapsedTicks;
            WholesomeProfessionsSettings.CurrentSetting.Save();

            Logger.Log("Starting updater");
            string onlineFile = "https://github.com/Wholesome-wRobot/Wholesome-professions/raw/master/Wholesome_Professions_WotlK/Compiled/WRobot/Products/Wholesome%20Professions%20WotlK.dll";
            string onlineVersion = "https://raw.githubusercontent.com/Wholesome-wRobot/Wholesome-professions/master/Wholesome_Professions_WotlK/Compiled/Version.txt";

            var onlineVersionContent = new System.Net.WebClient { Encoding = Encoding.UTF8 }.DownloadString(onlineVersion);

            Logger.Log($"Online Version : {onlineVersionContent}");
            if (onlineVersionContent == null || onlineVersionContent.Length > 10 || onlineVersionContent == mainVersion)
            {
                Logger.Log($"Your version is up to date ({mainVersion})");
                return;
            }

            var onlineFileContent = new System.Net.WebClient { Encoding = Encoding.UTF8 }.DownloadData(onlineFile);
            Logger.LogDebug(onlineFileContent.Length.ToString());

            if (onlineFileContent != null && onlineFileContent.Length > 0)
            {
                Logger.Log($"Your version : {mainVersion}");
                Logger.Log("Trying to update");
                
                File.Move(currentFile, oldFile);

                Logger.Log("Writing file");
                File.WriteAllBytes(currentFile, onlineFileContent); // replace user file by online file
                
                Thread.Sleep(2000);
                
                ShowReloadMessage();
            }
        }
        catch (Exception e)
        {
            Logging.Write("Auto update: " + e);
        }
    }

    private static void ShowReloadMessage()
    {
        MessageBox.Show($"A new version of {Main.productName} has been downloaded. Please restart WRobot.");
        Logger.LogLineBroadcastImportant($"A new version of {Main.productName} has been downloaded, please restart WRobot".ToUpper());
        Products.DisposeProduct();
    }
}
