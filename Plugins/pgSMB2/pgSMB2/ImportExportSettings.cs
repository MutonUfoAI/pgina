using System.Collections.Generic;

namespace pGina.Plugin.pgSMB2
{
    internal class ImportExportSettings
    {
        public string SMBshare { get; set; }
        public string RoamingSource { get; set; }
        public string Filename { get; set; }
        public string TempComp { get; set; }
        public int ConnectRetry { get; set; }
        public string Compressor { get; set; }
        public string CompressCLI { get; set; }
        public string UncompressCLI { get; set; }

        public string HomeDir { get; set; }
        public string HomeDirDrive { get; set; }
        public string ScriptPath { get; set; }
        public int MaxStore { get; set; }
        public string MaxStoreExclude { get; set; }
        public string[] MaxStoreText { get; set; }
        public string ACE { get; set; }
    }
}