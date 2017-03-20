using System;
using System.Data;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryGenerators
{
    public class MySqlChangesFixator:ChangesFixator
    {
        public MySqlChangesFixator(DataBaseInfo dbInfo) : base(dbInfo)
        {
        }

        protected override string GetFormattedDateForQuery(DateTime dateTime)
        {
           throw new NotImplementedException();
        }

        protected override bool HasColumnDefaultValue(DataColumn column)
        {
            throw new NotImplementedException();
        }
    }
}