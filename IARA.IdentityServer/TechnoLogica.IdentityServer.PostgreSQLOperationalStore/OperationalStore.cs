namespace TechnoLogica.IdentityServer.PostgreOperationalStore
{
    public class OperationalStore
    {
        public string DataProtectionConnectionStringName { get; set; }
        public string DistributedCacheConnectionStringName { get; set; }
        public string ConnectionStringName { get; set; }
        public bool EnableCleanup { get; set; }
        public int CleanupInterval { get; set; }
    }
}
