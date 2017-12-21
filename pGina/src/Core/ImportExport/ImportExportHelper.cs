/*
	Copyright (c) 2017, pGina Team
	All rights reserved.

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are met:
		* Redistributions of source code must retain the above copyright
		  notice, this list of conditions and the following disclaimer.
		* Redistributions in binary form must reproduce the above copyright
		  notice, this list of conditions and the following disclaimer in the
		  documentation and/or other materials provided with the distribution.
		* Neither the name of the pGina Team nor the names of its contributors
		  may be used to endorse or promote products derived from this software without
		  specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
	ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
	DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY
	DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
	(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
	LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
	ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using log4net;
using Newtonsoft.Json.Linq;
using pGina.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace pGina.Core.ImportExport
{
    public class ImportExportHelper
    {
        private static ILog m_logger = LogManager.GetLogger("ImportExportHelper");

        public static ImportExportReport SetImportExportSettings(ImportExportSettings importExportSettings)
        {
            var report = new ImportExportReport() { Rows = new List<ImportExportReportRow>() };
            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Import started")));
            var allplugins = PluginLoader.AllPlugins;
            if (!Assembly.GetExecutingAssembly().GetName().Version.ToString().Equals(importExportSettings.Version))
            {
                report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Warning, string.Format("pGina Version mismatch import version: {0}, pGina version: {1} ", importExportSettings.Version, Assembly.GetExecutingAssembly().GetName().Version.ToString())));
            }
            if (importExportSettings.GeneralSettings != null)
            {
                report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Importing GeneralSettings")));
                if (File.Exists(importExportSettings.GeneralSettings.TileImage))
                {
                    Settings.Get.TileImage = importExportSettings.GeneralSettings.TileImage;
                }
                else
                {
                    report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("TileImage not set, File does not exists on this system {0}", importExportSettings.GeneralSettings.TileImage)));
                }

                if (importExportSettings.GeneralSettings.MOTDSettings != null)
                {
                    report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Importing GeneralSettings.MOTD")));
                    // MOTD stuff
                    Settings.Get.EnableMotd = importExportSettings.GeneralSettings.MOTDSettings.Enable;
                    Settings.Get.Motd = importExportSettings.GeneralSettings.MOTDSettings.Text;
                }

                // Service status checkbox
                Settings.Get.ShowServiceStatusInLogonUi = importExportSettings.GeneralSettings.ShowServiceStatusInLogonUi;

                // Save unlock setting
                Settings.Get.UseOriginalUsernameInUnlockScenario = importExportSettings.GeneralSettings.UseOriginalUsernameInUnlockScenario;

                // Display last username in logon screen
                Settings.Get.LastUsernameEnable = importExportSettings.GeneralSettings.LastUsernameEnable;

                Settings.Get.PreferLocalAuthentication = importExportSettings.GeneralSettings.PreferLocalAuthentication;

                // ntp server
                string[] ntpservers = importExportSettings.GeneralSettings.NTPServers.ToArray();
                Settings.Get.ntpservers = ntpservers;

                if (importExportSettings.GeneralSettings.EmailNotificationSettings != null)
                {
                    report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Importing GeneralSettings.EmailNotificationSettings")));
                    // email notification
                    Settings.Get.notify_smtp = importExportSettings.GeneralSettings.EmailNotificationSettings.SMTP;
                    Settings.Get.notify_email = importExportSettings.GeneralSettings.EmailNotificationSettings.Email;
                    Settings.Get.notify_user = importExportSettings.GeneralSettings.EmailNotificationSettings.User;
                    Settings.Get.SetEncryptedSetting("notify_pass", importExportSettings.GeneralSettings.EmailNotificationSettings.Password);
                    Settings.Get.notify_cred = importExportSettings.GeneralSettings.EmailNotificationSettings.UseCredentials;
                    Settings.Get.notify_ssl = importExportSettings.GeneralSettings.EmailNotificationSettings.UseSSL;
                }
            }

            if (importExportSettings.PluginSettings != null && importExportSettings.PluginSettings.Any())
            {
                report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Importing PluginsSettings")));
            }
            foreach (var plugin in allplugins)
            {
                try
                {
                    // There are plugins settings
                    if (importExportSettings.PluginSettings != null)
                    {
                        var importplugin = importExportSettings.PluginSettings.FirstOrDefault(b => b.Uuid == plugin.Uuid);
                        if (importplugin != null)
                        {
                            if (!plugin.Version.Equals(importplugin.Version))
                            {
                                report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Warning, string.Format("Plugin Version mismatch import version: {0}, plugin version: {1} ", importplugin.Version, plugin.Version)));
                            }
                            PluginSettings.SetMask(plugin.Uuid, importplugin.AuthenticateEnabled, importplugin.AuthorizeEnabled,
                                importplugin.GatewayEnabled, importplugin.NotificationEnabled, importplugin.ChangePasswordEnabled);
                            if (plugin is IPluginImportExport)
                            {
                                var pluginImportExport = (IPluginImportExport)plugin;
                                pluginImportExport.Import(importplugin.Settings);
                                report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Import succeeded for plugin: {0}/{1}", plugin.Uuid, plugin.Name)));
                            }
                            else
                            {
                                report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Warning, string.Format("Plugin has no Import/Export fuction, plugin: {0}/{1}", plugin.Uuid, plugin.Name)));
                            }
                        }
                        else if (importExportSettings.DisableNonExportedPlugins)
                        {
                            // Only enable exported plugins
                            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Disable plugin {0} / {1}", plugin.Uuid, plugin.Name)));
                            PluginSettings.SetMask(plugin.Uuid, false, false, false, false, false);
                        }
                    }
                }
                catch (Exception e)
                {
                    report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Error, string.Format("Import malfunction for plugin: {0}/{1}, Error: {2}", plugin.Uuid, plugin.Name, e)));
                }
            }

            if ((importExportSettings.AuthenticatePluginOrder != null && importExportSettings.AuthenticatePluginOrder.Any())
                ||
                (importExportSettings.AuthorizePluginOrder != null && importExportSettings.AuthorizePluginOrder.Any())
                ||
                (importExportSettings.GatewayPluginOrder != null && importExportSettings.GatewayPluginOrder.Any())
                ||
                (importExportSettings.NotificationPluginOrder != null && importExportSettings.NotificationPluginOrder.Any())
                ||
                (importExportSettings.ChangePasswordPluginOrder != null && importExportSettings.ChangePasswordPluginOrder.Any()))
            {
                report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Importing PluginOrder")));

            }

            SavePluginOrder(importExportSettings.AuthenticatePluginOrder, typeof(IPluginAuthentication));
            SavePluginOrder(importExportSettings.AuthorizePluginOrder, typeof(IPluginAuthorization));
            SavePluginOrder(importExportSettings.GatewayPluginOrder, typeof(IPluginAuthenticationGateway));
            SavePluginOrder(importExportSettings.NotificationPluginOrder, typeof(IPluginEventNotifications));
            SavePluginOrder(importExportSettings.ChangePasswordPluginOrder, typeof(IPluginChangePassword));

            if (importExportSettings.DisabledCredentialProviders != null)
            {
                if (importExportSettings.DisabledCredentialProviders.Any())
                {
                    report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Importing DisabledCredentialProviders")));
                }
                var credentialProviders = CredProvFilterConfig.LoadCredProvsAndFilterSettings();
                foreach (var credentialProvider in credentialProviders)
                {
                    var importCredentialProvider = importExportSettings.DisabledCredentialProviders.FirstOrDefault(b => b.Uuid.Equals(credentialProvider.Uuid));
                    if (importCredentialProvider != null)
                    {
                        report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Disabling CredentialProvider {0}", credentialProvider.Name)));
                        credentialProvider.FilterChangePass = importCredentialProvider.FilterChangePass;
                        credentialProvider.FilterCredUI = importCredentialProvider.FilterCredUI;
                        credentialProvider.FilterLogon = importCredentialProvider.FilterLogon;
                        credentialProvider.FilterUnlock = importCredentialProvider.FilterUnlock;
                    }
                    else
                    {
                        credentialProvider.FilterChangePass = false;
                        credentialProvider.FilterCredUI = false;
                        credentialProvider.FilterLogon = false;
                        credentialProvider.FilterUnlock = false;
                    }
                }
                CredProvFilterConfig.SaveFilterSettings(credentialProviders);
            }

            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Import finished")));
            return report;
        }

        private static ImportExportReportRow LogMessage(ImportExportReportMessageLevel level, string message)
        {
            switch (level)
            {
                case ImportExportReportMessageLevel.Info:
                    m_logger.Info(message);
                    return new ImportExportReportRow { MessageLevel = level, Message = message };
                case ImportExportReportMessageLevel.Warning:
                    m_logger.Warn(message);
                    return new ImportExportReportRow { MessageLevel = level, Message = message };
                default:
                    m_logger.Error(message);
                    return new ImportExportReportRow { MessageLevel = level, Message = message };
            }
        }

        private static void SavePluginOrder(List<Guid> listOfPluginUuid, Type type)
        {
            if (listOfPluginUuid != null)
            {
                PluginSettings.SavePluginOrder(listOfPluginUuid, type);
            }
        }

        public static ExportResponse GetImportExportSettings()
        {
            var report = new ImportExportReport() { Rows = new List<ImportExportReportRow>() };
            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Export started")));
            var exportsettings = new ImportExportSettings { PluginSettings = new List<ImportExportPluginSetting>(), DisableNonExportedPlugins = true };


            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Exporting GeneralSettings")));
            string[] ntpservers = Settings.Get.ntpservers;

            exportsettings.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            exportsettings.GeneralSettings = new ImportExportGeneralSettings
            {
                TileImage = Settings.Get.GetSetting("TileImage", null),
                MOTDSettings = new ImportExportMOTDSettings
                {
                    Enable = Settings.Get.EnableMotd,
                    Text = Settings.Get.GetSetting("Motd")
                },
                ShowServiceStatusInLogonUi = Settings.Get.ShowServiceStatusInLogonUi,
                UseOriginalUsernameInUnlockScenario = Settings.Get.UseOriginalUsernameInUnlockScenario,
                LastUsernameEnable = Settings.Get.LastUsernameEnable,
                PreferLocalAuthentication = Settings.Get.PreferLocalAuthentication,
                NTPServers = ntpservers.ToList(),
                EmailNotificationSettings = new ImportExportEmailNotificationSettings
                {
                    SMTP = Settings.Get.GetSetting("notify_smtp"),
                    Email = Settings.Get.GetSetting("notify_email"),
                    User = Settings.Get.GetSetting("notify_user"),
                    Password = Settings.Get.GetEncryptedSetting("notify_pass"),                    
                    UseCredentials = Settings.Get.notify_cred,
                    UseSSL = Settings.Get.notify_ssl
                }
            };

            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Exporting PluginsSettings")));
            foreach (var plugin in PluginLoader.AllPlugins)
            {
                try
                {
                    int pluginMask = Settings.Get.GetSetting(plugin.Uuid.ToString());

                    var authenticateEnabled = PluginLoader.IsEnabledFor<IPluginAuthentication>(pluginMask);
                    var authorizeEnabled = PluginLoader.IsEnabledFor<IPluginAuthorization>(pluginMask);
                    var changePasswordEnabled = PluginLoader.IsEnabledFor<IPluginChangePassword>(pluginMask);
                    var gatewayEnabled = PluginLoader.IsEnabledFor<IPluginAuthenticationGateway>(pluginMask);
                    var notificationEnabled = PluginLoader.IsEnabledFor<IPluginEventNotifications>(pluginMask);

                    // Only export enabled plugin-settings
                    if (authenticateEnabled || authorizeEnabled || changePasswordEnabled || gatewayEnabled || notificationEnabled)
                    {
                        JToken settings = null;
                        if (plugin is IPluginImportExport)
                        {
                            settings = ((IPluginImportExport)plugin).Export();
                            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Plugin exporting with settings, plugin: {0}/{1}", plugin.Uuid, plugin.Name)));
                        }
                        else
                        {
                            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Warning, string.Format("Plugin exporting without settings, plugin: {0}/{1}", plugin.Uuid, plugin.Name)));
                        }
                        exportsettings.PluginSettings.Add(new ImportExportPluginSetting
                        {
                            Name = plugin.Name,
                            Version = plugin.Version,
                            Uuid = plugin.Uuid,
                            Settings = settings,
                            AuthenticateEnabled = authenticateEnabled,
                            AuthorizeEnabled = authorizeEnabled,
                            ChangePasswordEnabled = changePasswordEnabled,
                            GatewayEnabled = gatewayEnabled,
                            NotificationEnabled = notificationEnabled
                        });
                    }
                }
                catch (Exception e)
                {
                    report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Error, string.Format("Export malfunction for plugin: {0}/{1}, Error: {2}", plugin.Uuid, plugin.Name, e)));
                }
            }

            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Exporting PluginOrders")));
            exportsettings.AuthenticatePluginOrder = PluginLoader.GetOrderedPluginsOfType<IPluginAuthentication>().Select(b => b.Uuid).ToList();
            exportsettings.AuthorizePluginOrder = PluginLoader.GetOrderedPluginsOfType<IPluginAuthorization>().Select(b => b.Uuid).ToList();
            exportsettings.GatewayPluginOrder = PluginLoader.GetOrderedPluginsOfType<IPluginAuthenticationGateway>().Select(b => b.Uuid).ToList();
            exportsettings.NotificationPluginOrder = PluginLoader.GetOrderedPluginsOfType<IPluginEventNotifications>().Select(b => b.Uuid).ToList();
            exportsettings.ChangePasswordPluginOrder = PluginLoader.GetOrderedPluginsOfType<IPluginChangePassword>().Select(b => b.Uuid).ToList();

            exportsettings.DisabledCredentialProviders = new List<ImportExportDisabledCredentialProvider>();
            var credentialProviders = CredProvFilterConfig.LoadCredProvsAndFilterSettings();
            foreach (var credentialProvider in credentialProviders)
            {
                if (credentialProvider.FilterEnabled())
                {
                    report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Exporting Settings Disabled CredentialProvider: {0}", credentialProvider.Name)));
                    exportsettings.DisabledCredentialProviders.Add(new ImportExportDisabledCredentialProvider
                    {
                        Uuid = credentialProvider.Uuid,
                        Name = credentialProvider.Name,
                        FilterChangePass = credentialProvider.FilterChangePass,
                        FilterCredUI = credentialProvider.FilterCredUI,
                        FilterLogon = credentialProvider.FilterLogon,
                        FilterUnlock = credentialProvider.FilterUnlock
                    });
                }
            }

            report.Rows.Add(LogMessage(ImportExportReportMessageLevel.Info, string.Format("Export finished")));

            return new ExportResponse { Settings = exportsettings, Report = report };
        }
    }
}
