using robotManager.Helpful;
using robotManager.Products;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Wholesome_Professions_WotlK.GUI;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Plugin;
using wManager.Wow.Helpers;

public class Main : IProduct
{
    public static IProfession primaryProfession = null;
    public static IProfession secondaryProfession = null;
    public static int amountProfessionsSelected = 0;
    public static string productName = "Wholesome Professions WotLK";
    public static int wowVersion = GetWowVersion();

    private readonly BackgroundWorker _pulseThread = new BackgroundWorker();
    public bool IsStarted { get; private set; } = false;
    ProductSettingsControl _settingsUserControl;

    public string version = "0.1.8"; // Must match version in Version.txt

    public void Initialize()
    {
        try
        {
            Directory.CreateDirectory(Application.StartupPath + "\\Profiles\\Wholesome Professions\\");
            WholesomeProfessionsSettings.Load();
            WholesomeProfessionsSave.Load();
            TravelHelper.AddAllOffmeshConnections();
            Logger.Log($"{productName} version {version} loaded");
        }
        catch (Exception e)
        {
            Logging.WriteError("Main > Initialize(): " + e);
        }
    }

    public void Dispose()
    {
        try
        {
            Stop();
            Logging.Status = "Dispose Product Complete";
            Logging.Write("Dispose Product Complete");
        }
        catch (Exception e)
        {
            Logging.WriteError("Main > Dispose(): " + e);
        }
    }

    public void Start()
    {
        try
        {
            AutoUpdater.CheckUpdate(this);
            IsStarted = true;
            FrameHelper.CreateDebugFrame();
            FrameHelper.CreateBroadcastFrame();

            WRobotSettings.SetRecommendedWRobotSettings();

            _pulseThread.DoWork += DoBackgroundPulse;
            _pulseThread.RunWorkerAsync();

            Broadcaster.InitializeTimer();

            if (Bot.Pulse())
            {
                PluginsManager.LoadAllPlugins();
                SetProfessions();
                Logging.Status = "Start Product Complete";
                Logging.Write("Start Product Complete");
            }
            else
            {
                IsStarted = false;
                Logging.Status = "Start Product failed";
                Logging.Write("Start Product failed");
            }
        }
        catch (Exception e)
        {
            IsStarted = false;
            Logging.WriteError("Main > Start(): " + e);
        }
    }

    public void Stop()
    {
        try
        {
            Lua.RunMacroText("/stopcasting");
            MovementManager.StopMove();

            WRobotSettings.RestoreUserWRobotSettings();

            _pulseThread.DoWork -= DoBackgroundPulse;
            _pulseThread.Dispose();

            Broadcaster.broadcastTimer.Elapsed -= Broadcaster.SetTimerReady;

            Bot.Dispose();
            IsStarted = false;
            PluginsManager.DisposeAllPlugins();
            Logging.Status = "Stop Product Complete";
            Logging.Write("Stop Product Complete");
        }
        catch (Exception e)
        {
            Logging.WriteError("Main > Stop(): " + e);
        }
    }

    // Broadcaster
    private void DoBackgroundPulse(object sender, DoWorkEventArgs args)
    {
        while (IsStarted)
        {
            try
            {
                if (Conditions.InGameAndConnectedAndProductStartedNotInPause && IsStarted && Broadcaster.autoBroadcast)
                {
                    Broadcaster.BroadCastSituation();
                }
            }
            catch (Exception arg)
            {
                Logging.WriteError(string.Concat(arg), true);
            }
            Thread.Sleep(100);
        }
    }

    // GUI
    public System.Windows.Controls.UserControl Settings
    {
        get
        {
            try
            {
                if (_settingsUserControl == null)
                    _settingsUserControl = new ProductSettingsControl();
                return _settingsUserControl;
            }
            catch (Exception e)
            {
                Logger.Log("> Main > Settings(): " + e);
            }
            return null;
        }
    }

    private void SetProfessions()
    {
        primaryProfession = new Tailoring();
        secondaryProfession = new Enchanting();
        primaryProfession.SetOtherProfession();
        secondaryProfession.SetOtherProfession();
        amountProfessionsSelected = 2;
    }

    private static int GetWowVersion()
    {
        string[] forWow = wManager.Information.ForWow.Split(new Char[] { '.' });
        return Int32.Parse(forWow[0]);
    }
}
