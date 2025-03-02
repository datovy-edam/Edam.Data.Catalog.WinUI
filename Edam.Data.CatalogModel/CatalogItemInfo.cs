using Edam.DataObjects.Trees;
using System.Data.Common;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Edam.Data.CatalogModel;

/// <summary>
/// Catalog Item based on the common TreeItem.
/// </summary>
public class CatalogItemInfo : TreeItem, ITreeItem
{

   #region -- 1.00 - Fields and Properties...

   public const string TYPE_Catalog = "Catalog";
   public const string TYPE_FILE = "File";

   public string TypeText
   {
      get { return Type.ToString(); }
      set
      {
         Type = value == TYPE_Catalog ?
            TreeItemType.Branch : TreeItemType.Leaf;
      }
   }

   public bool HasChildren
   {
      get { return Children != null && Children.Count > 0; }
   }

   public HashSet<CatalogItemInfo> Children { get; set; } =
      new HashSet<CatalogItemInfo>();
   public CatalogItemInfo Parent { get; set; }

   #endregion
   #region -- 1.50 - Initialization

   // call CatalogTreeBuilder.CreateItem(FileItemInfo)...

   #endregion
   #region -- 4.00 - Catalog JSON Serialization Support

   /// <summary>
   /// From a root element to 
   /// </summary>
   /// <param name="root"></param>
   /// <returns></returns>
   public String ToCatalogJsonText(CatalogItemInfo? root)
   {
      return TreeItem.ToDirectoryJsonText(root);
   }

   /// <summary>
   /// From a JSON file system info to 
   /// </summary>
   /// <param name="jsonText"></param>
   /// <returns></returns>
   public CatalogItemInfo? FromCatalogJsonText(string jsonText)
   {
      return TreeItem.FromDirectoryJsonText<CatalogItemInfo?>(jsonText);
   }

   #endregion

}
