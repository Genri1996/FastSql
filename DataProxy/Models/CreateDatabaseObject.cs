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
        /// Anonymous database
        /// </summary>
        public bool IsDataBaseAnonymous { get; set; }

        public int HoursToBeStored { get; set; }

        /// <summary>
        /// Protected Database
        /// </summary>
        public bool IsProtectionRequired { get; set; }

        public string DataBaseLogin { get; set; }

        /// <summary>
        /// Only of protection required
        /// </summary>
        public string DataBasePassword { get; set; }
    }
}
