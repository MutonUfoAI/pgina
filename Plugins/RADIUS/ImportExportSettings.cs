namespace pGina.Plugin.RADIUS
{
    internal class ImportExportSettings
    {
        public bool EnableAuth { get; set; }
        public bool EnableAcct { get; set; }
        public string Server { get; set; }
        public int AuthPort { get; set; }
        public int AcctPort { get; set; }
        public string SharedSecret { get; set; }
        public int Timeout { get; set; }
        public int Retry { get; set; }

        public bool SendNASIPAddress { get; set; }
        public bool SendNASIdentifier { get; set; }
        public string NASIdentifier { get; set; }
        public bool SendCalledStationID { get; set; }
        public string CalledStationID { get; set; }

        public bool AcctingForAllUsers { get; set; }
        public bool SendInterimUpdates { get; set; }
        public bool ForceInterimUpdates { get; set; }
        public int InterimUpdateTime { get; set; }

        public bool AllowSessionTimeout { get; set; }
        public bool WisprSessionTerminate { get; set; }

        public bool UseModifiedName { get; set; }
        public string IPSuggestion { get; set; }
    }
}