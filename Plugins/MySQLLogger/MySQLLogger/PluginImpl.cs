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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using pGina.Shared.Interfaces;
using pGina.Shared.Settings;
using pGina.Shared.Types;

using Abstractions.WindowsApi;
using log4net;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using pGina.Shared;

namespace pGina.Plugin.MySqlLogger
{
    enum LoggerMode { EVENT, SESSION };

    public class PluginImpl : IPluginConfiguration, IPluginEventNotifications, IPluginImportExport
    {
        public static readonly Guid PluginUuid = new Guid("B68CF064-9299-4765-AC08-ACB49F93F892");
        private ILog m_logger = LogManager.GetLogger("MySqlLoggerPlugin");

        public string Description
        {
            get { return "Logs various events to a MySQL database."; }
        }

        public string Name
        {
            get { return "MySQL Logger"; }
        }

        public Guid Uuid
        {
            get { return PluginUuid; }
        }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public void Configure()
        {
            Configuration dlg = new Configuration();
            dlg.ShowDialog();
        }

        public void Import(JToken pluginSettings)
        {
            var importSettings = pluginSettings.ToObject<ImportExportSettings>();
            Settings.Store.EventMode = importSettings.EventMode;
            Settings.Store.SessionMode = importSettings.SessionMode;
            Settings.Store.Host = importSettings.Host.EmptyStringIfNull();
            Settings.Store.Port = importSettings.Port;
            Settings.Store.User = importSettings.User.EmptyStringIfNull();
            Settings.Store.SetEncryptedSetting("Password", importSettings.Password.EmptyStringIfNull());
            Settings.Store.SessionTable = importSettings.SessionTable.EmptyStringIfNull();
            Settings.Store.EventTable = importSettings.EventTable.EmptyStringIfNull();

            Settings.Store.EvtLogon = importSettings.EvtLogon;
            Settings.Store.EvtLogoff = importSettings.EvtLogoff;
            Settings.Store.EvtLock = importSettings.EvtLock;
            Settings.Store.EvtUnlock = importSettings.EvtUnlock;
            Settings.Store.EvtConsoleConnect = importSettings.EvtConsoleConnect;
            Settings.Store.EvtConsoleDisconnect = importSettings.EvtConsoleDisconnect;
            Settings.Store.EvtRemoteControl = importSettings.EvtRemoteControl;
            Settings.Store.EvtRemoteConnect = importSettings.EvtRemoteConnect;
            Settings.Store.EvtRemoteDisconnect = importSettings.EvtRemoteDisconnect;

            Settings.Store.UseModifiedName = importSettings.UseModifiedName;
        }

        public JToken Export()
        {
            var exportsettings = new ImportExportSettings
            {
                EventMode = Settings.Store.EventMode,
                SessionMode = Settings.Store.SessionMode,
                Host = Settings.Store.Host,
                Port = Settings.Store.Port,
                User = Settings.Store.User,
                Password = Settings.Store.GetEncryptedSetting("Password"),
                SessionTable = Settings.Store.SessionTable,
                EventTable = Settings.Store.EventTable,

                EvtLogon = Settings.Store.EvtLogon,
                EvtLogoff = Settings.Store.EvtLogoff,
                EvtLock = Settings.Store.EvtLock,
                EvtUnlock = Settings.Store.EvtUnlock,
                EvtConsoleConnect = Settings.Store.EvtConsoleConnect,
                EvtConsoleDisconnect = Settings.Store.EvtConsoleDisconnect,
                EvtRemoteControl = Settings.Store.EvtRemoteControl,
                EvtRemoteConnect = Settings.Store.EvtRemoteConnect,
                EvtRemoteDisconnect = Settings.Store.EvtRemoteDisconnect,

                UseModifiedName = Settings.Store.UseModifiedName
            };
            return JToken.FromObject(exportsettings);
        }

        public void SessionChange(int SessionId, System.ServiceProcess.SessionChangeReason Reason, SessionProperties properties)
        {
            if (properties == null)
                return;

            m_logger.DebugFormat("SessionChange({0}) - ID: {1}", Reason.ToString(), SessionId);

            //If SessionMode is enabled, send event to it.
            if ((bool)Settings.Store.SessionMode)
            {
                ILoggerMode mode = LoggerModeFactory.getLoggerMode(LoggerMode.SESSION);
                mode.Log(SessionId, Reason, properties);
            }

            //If EventMode is enabled, send event to it.
            if ((bool)Settings.Store.EventMode)
            {
                ILoggerMode mode = LoggerModeFactory.getLoggerMode(LoggerMode.EVENT);
                mode.Log(SessionId, Reason, properties);
            }

            //Close the connection if it's still open
            LoggerModeFactory.closeConnection();

        }

        public void Starting()
        {

        }

        public void Stopping()
        {

        }


    }
}
