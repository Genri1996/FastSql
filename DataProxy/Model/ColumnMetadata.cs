using System;

namespace DataProxy.Model
{
    public class ColumnMetadata
    {
        public Type ColumnType { get; set; }
        public string ColumnName { get; set; }
        public string TableName { get; set; }
        public string DataBaseName { get; set; }
        public int OrdinalPosition { get; set; }
        public bool IsNullable { get; set; }
        public int MaximumCharacters { get; set; }
        public object ColumnDefaultValue { get; set; }
    }
}