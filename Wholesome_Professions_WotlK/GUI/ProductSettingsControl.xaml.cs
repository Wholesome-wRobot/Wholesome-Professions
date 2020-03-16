﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace Wholesome_Professions_WotlK.GUI
{
    /// <summary>
    /// Logique d'interaction pour ProductSettingsControl.xaml
    /// </summary>
    public partial class ProductSettingsControl : UserControl
    {
        public ProductSettingsControl()
        {
            InitializeComponent();
            ServerRate.Value = WholesomeProfessionsSettings.CurrentSetting.ServerRate;
            BroadcasterInterval.Value = WholesomeProfessionsSettings.CurrentSetting.BroadcasterInterval;
            LogDebug.IsChecked = WholesomeProfessionsSettings.CurrentSetting.LogDebug;
        }

        private void BroadcasterIntervalChanged(object sender, RoutedEventArgs e)
        {
            WholesomeProfessionsSettings.CurrentSetting.BroadcasterInterval = (int)BroadcasterInterval.Value;
            WholesomeProfessionsSettings.CurrentSetting.Save();
        }

        private void LogDebugChanged(object sender, RoutedEventArgs e)
        {
            WholesomeProfessionsSettings.CurrentSetting.LogDebug = (bool)LogDebug.IsChecked;
            WholesomeProfessionsSettings.CurrentSetting.Save();
        }

        private void ServerRateChanged(object sender, RoutedEventArgs e)
        {
            WholesomeProfessionsSettings.CurrentSetting.ServerRate = (int)ServerRate.Value;
            WholesomeProfessionsSettings.CurrentSetting.Save();
        }
    }
}
