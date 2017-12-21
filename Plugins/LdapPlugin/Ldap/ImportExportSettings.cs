namespace pGina.Plugin.Ldap
{
    internal class ImportExportSettings
    {
        public ImportExportServerSettings ServerSettings { get; set; }

        public ImportExportAuthenticationSettings AuthenticationSettings { get; set; }

        public ImportExportAuthorizationSettings AuthorizationSettings { get; set; }

        public string[] GroupGatewayRules { get; set; }

        public string[] ChangePasswordAttributes { get; set; }
    }
}