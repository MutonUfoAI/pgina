namespace pGina.Plugin.Email
{
    internal class ImportExportSettings
    {
        public string Server { get; set; }

        public bool UseSsl { get; set; }

        public string Protocol { get; set; }

        public string Port { get; set; }

        public bool AppendDomain { get; set; }

        public string Domain { get; set; }

        public int NetworkTimeout { get; set; }
    }
}
