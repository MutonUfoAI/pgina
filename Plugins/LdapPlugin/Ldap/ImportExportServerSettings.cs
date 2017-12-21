namespace pGina.Plugin.Ldap
{
    internal class ImportExportServerSettings
    {
        public string[] LdapHost { get; set; }

        public int LdapPort { get; set; }

        public int LdapTimeout { get; set; }

        public bool UseSsl { get; set; }

        public bool UseTls { get; set; }

        public bool RequireCert { get; set; }

        public string ServerCertFile { get; set; }

        public bool UseAuthBindForAuthzAndGateway { get; set; }

        public string SearchDN { get; set; }

        // SetDefaultEncryptedSetting
        public string SearchPW { get; set; }

        public string[] AttribConv { get; set; }
    }
}
