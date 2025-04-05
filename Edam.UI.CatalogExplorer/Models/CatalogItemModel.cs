using Edam.Data.CatalogModel;
using Edam.DataObjects.Trees;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.UI.Catalog.Models;


/// <summary>
/// Observable Item Model
/// </summary>
public class CatalogItemModel
{
   public string Name { get; set; }
   public TreeItemType ItemType { get; set; }
   public ObservableCollection<CatalogItemModel> Children { get; set; } =
       new ObservableCollection<CatalogItemModel>();
   public CatalogItemInfo Item { get; set; }

   public override string ToString()
   {
      return Name;
   }
}
