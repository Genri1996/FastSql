using System.Collections.Generic;
using System.Web.Mvc;

namespace CursachPrototype.ViewModels
{
    public class DeleteColumnVm
    {
        public string TableName { get; set; }
        public List<SelectListItem> AvailableColumns = new List<SelectListItem>();
        public string ColumnName { get; set; }
    }
}