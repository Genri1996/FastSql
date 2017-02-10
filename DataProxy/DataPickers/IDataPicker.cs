using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProxy.Model;

namespace DataProxy.DataPickers
{
    interface IDataPicker
    {
        List<ColumnMetadata> GetTableMetadata();
    }
}
