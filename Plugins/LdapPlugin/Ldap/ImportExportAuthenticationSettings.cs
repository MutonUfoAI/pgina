namespace pGina.Plugin.Ldap
{
    internal class ImportExportAuthenticationSettings
    {
        public bool AllowEmptyPasswords { get; set; }

        public string DnPattern { get; set; }

        public bool DoSearch { get; set; }

        public string SearchFilter { get; set; }

        public string[] SearchContexts { get; set; }
    }
}
