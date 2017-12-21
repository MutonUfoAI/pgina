namespace pGina.Plugin.MySQLAuth
{
    internal class ImportExportSettings
    {
        //Connection
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        // User table
        public string Table { get; set; }
        public int HashEncoding { get; set; }
        public string UsernameColumn { get; set; }
        public string HashMethodColumn { get; set; }
        public string PasswordColumn { get; set; }
        public string UserTablePrimaryKeyColumn { get; set; }

        // Group table
        public string GroupTableName { get; set; }
        public string GroupNameColumn { get; set; }
        public string GroupTablePrimaryKeyColumn { get; set; }

        // User-Group table
        public string UserGroupTableName { get; set; }
        public string UserForeignKeyColumn { get; set; }
        public string GroupForeignKeyColumn { get; set; }

        // Authz Settings
        public string[] GroupAuthzRules { get; set; }
        public bool AuthzRequireMySqlAuth { get; set; }

        // Gateway settings
        public string[] GroupGatewayRules { get; set; }
        public bool PreventLogonOnServerError { get; set; }
    }
}