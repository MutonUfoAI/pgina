namespace pGina.Plugin.MySqlLogger
{
    internal class ImportExportSettings
    {
        public bool EventMode { get; set; }
        public bool SessionMode { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string SessionTable { get; set; }
        public string EventTable { get; set; }

        public bool EvtLogon { get; set; }
        public bool EvtLogoff { get; set; }
        public bool EvtLock { get; set; }
        public bool EvtUnlock { get; set; }
        public bool EvtConsoleConnect { get; set; }
        public bool EvtConsoleDisconnect { get; set; }
        public bool EvtRemoteControl { get; set; }
        public bool EvtRemoteConnect { get; set; }
        public bool EvtRemoteDisconnect { get; set; }

        public bool UseModifiedName { get; set; }
    }
}