namespace DataProxy.Models
{
    /// <summary>
    /// Encapsulates information about Db creation
    /// </summary>
    public class CreateDatabaseObject
    {
        public DbmsType SelectedDbms { get; set; }
    
        public string DataBaseName { get; set; }

        /// <summary>
        /// Flag that ask for protection
        /// </summary>
        public bool IsProtectionRequired { get; set; }

        public string DataBaseLogin { get; set; }

        /// <summary>
        /// Only of protection required
        /// </summary>
        public string DataBasePassword { get; set; }
    }
}
