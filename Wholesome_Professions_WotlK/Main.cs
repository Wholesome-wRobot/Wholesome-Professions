﻿using robotManager.Helpful;
using robotManager.Products;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Wholesome_Professions_WotlK.GUI;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Plugin;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class Main : IProduct
{
    public static IProfession currentProfession = null;
    private readonly BackgroundWorker _pulseThread = new BackgroundWorker();
    public bool IsStarted { get; private set; } = false;
    ProductSettingsControl _settingsUserControl;

    public static string version = "0.1.4";// Must match version in Version.txt

    public void Initialize()
    {
        try
        {
            Directory.CreateDirectory(Application.StartupPath + "\\Profiles\\Wholesome Professions\\");
            WholesomeProfessionsSettings.Load();
            WholesomeProfessionsSave.Load();
            AutoUpdater.CheckUpdate(version);
            Logger.Log($"Wholesome Professions WotlK version {version} loaded");
            TravelHelper.AddAllOffmeshConnections();
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
            IsStarted = true;

            _pulseThread.DoWork += DoBackgroundPulse;
            _pulseThread.RunWorkerAsync();

            Broadcaster.InitializeTimer();

            if (Bot.Pulse())
            {
                PluginsManager.LoadAllPlugins();
                currentProfession = new Tailoring();
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
}
