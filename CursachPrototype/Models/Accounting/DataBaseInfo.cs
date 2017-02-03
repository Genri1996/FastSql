using System;
using DataProxy;

namespace CursachPrototype.Models.Accounting
{
    /// <summary>
    /// Information about user`s database
    /// </summary>
    public class DataBaseInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateOfCreating { get; set; }

        public DateTime DateOfDeleting { get; set; }

        public bool IsPublic { get; set; }

        public bool IsAnonymous { get; set; }

        public bool IsAutoName { get; set; }

        public string ConnectionString { get; set; }

        public DbmsType DbmsType { get; set; }
        /// <summary>
        /// To User
        /// </summary>
        public string ForeignKey { get; set; }
    }
}