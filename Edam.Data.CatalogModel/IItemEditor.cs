using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogModel
{

   public interface IItemContent
   {
      object? ItemInstance { get; set; }
      string Content { get; set; }
   }

}
