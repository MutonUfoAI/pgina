namespace pGina.Plugin.LocalMachine
{
    internal class ImportExportSettings
    {
        public bool AlwaysAuthenticate { get; set; }

        public bool AuthzLocalAdminsOnly { get; set; }

        public string[] AuthzLocalGroups { get; set; }

        public bool AuthzApplyToAllUsers { get; set; }

        public bool MirrorGroupsForAuthdUsers { get; set; }

        public bool GroupCreateFailIsFail { get; set; }

        public string[] MandatoryGroups { get; set; }

        public bool RemoveProfiles { get; set; }

        public bool ScramblePasswords { get; set; }

        public bool ScramblePasswordsWhenLMAuthFails { get; set; }

        public string[] ScramblePasswordsExceptions { get; set; }

        public string[] CleanupUsers { get; set; }

        public int BackgroundTimerSeconds { get; set; }     
    }
}
