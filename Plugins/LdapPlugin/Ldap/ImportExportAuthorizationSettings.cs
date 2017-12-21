namespace pGina.Plugin.Ldap
{
    internal class ImportExportAuthorizationSettings
    {
        public string[] GroupAuthzRules { get; set; }

        public bool AuthzRequireAuth { get; set; }

        public bool AuthzAllowOnError { get; set; }

        public bool AuthzDefault { get; set; }
    }
}
