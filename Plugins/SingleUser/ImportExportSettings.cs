namespace pGina.Plugin.SingleUser
{
    internal class ImportExportSettings
    {
        public string Username { get; set; }

        public string Domain { get; set; }

        public string Password { get; set; }

        public bool RequirePlugins { get; set; }

        public bool RequireAllPlugins { get; set; }

        public string[] RequiredPluginList { get; set; }
    }
}