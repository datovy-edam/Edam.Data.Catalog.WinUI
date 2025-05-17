using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;


// -----------------------------------------------------------------------------
using Edam.DataObjects.Trees;

namespace Edam.Data.CatalogModel;

/// <summary>
/// Provide Cloning services.
/// </summary>
public class CatalogClone
{

   /// <summary>
   /// Find the Project folder that we want to clone in the target. 
   /// </summary>
   /// <remarks>
   /// It is assumed that the full path is of a leaf data item (like a file)
   /// </remarks>
   /// <param name="fullPath">leaf full path</param>
   /// <returns>the Project path is returned</returns>
   public static string GetProjectPath(string fullPath)
   {
      // find the Project folder that we want to clone in the target
      var l = fullPath.Split('/');
      string outPath = String.Empty;
      for (int i = 0; i < l.Length - 2; i++)
      {
         if (!String.IsNullOrWhiteSpace(l[i]))
            outPath += "/" + l[i];
      }
      return l.Length >= 3 ? outPath + "/" : String.Empty;
   }

   /// <summary>
   /// Item and Data Upsert async.
   /// </summary>
   /// <param name="builder">catalog builder</param>
   /// <param name="item">item to Upsert info about</param>
   /// <param name="source">source catalog service</param>
   /// <param name="target">target catalog service</param>
   /// <returns>added/updated path item is returned</returns>
   public static async Task<CatalogPathItem> ItemDataUpsertAsync(
      CatalogTreeBuilder builder,
      ItemInfo item, ICatalogService? source, ICatalogService? target)
   {
      // find and/or add item to the target catalog
      var pathItem = await builder.GetItemAsync(item.FullPath);

      // if this is a leaf then clone the data
      if (pathItem.Item.ItemType == TreeItemType.Leaf)
      {
         // get the item-data list from the source item-id
         var itms = await source.ItemData.GetItemDataAsync(item.Id);

         // if there are more than one items in the list create Item-Data.
         foreach (var ditem in itms)
         {
            var ndataItm = target.ItemData.CreateDataLeaf(
               pathItem.Item, pathItem.Name, dataValue: ditem.Data);
            ndataItm.ContentType = ditem.ContentType;
            ndataItm.ContentTypeId = ditem.ContentTypeId;
            var addedItem = await target.ItemData.AddItemAsync(ndataItm);
         }
      }

      return pathItem;
   }

   /// <summary>
   /// Close given item from a source catalog to another.
   /// </summary>
   /// <remarks>
   /// Note that this implementation assumes that you whant to clone the entire 
   /// content of the given root-path and therefore the given item path must 
   /// match the given root-path.
   /// </remarks>
   /// <param name="sourcePath">source path</param>
   /// <param name="targetPath">source path</param>
   /// <param name="item">item</param>
   /// <param name="source">source catalog</param>
   /// <param name="target">target catalog</param>
   public static async void Clone(string sourcePath, string targetPath,
      CatalogPathItem item, ICatalogService? source, ICatalogService? target)
   {
      // if either source or target is null or are the same, nothing to do
      if (source == null || target == null || source == target) return;

      // get a list of all items to be cloned
      var items = source.Item.GetBranch(sourcePath);

      // get base client
      var tclient = target.Instance as ICatalogBaseClient;
      var cataloger = tclient.Cataloger == null ?
         new CatalogTreeBuilder(target, null) : tclient.Cataloger;

      // go through the list of items and clone those...
      //List<KeyValuePair<string,string>> visited = 
      //   new List<KeyValuePair<string,string>>();

      foreach (var itm in items)
      {
         var pitem = await ItemDataUpsertAsync(cataloger, itm, source, target);
      }
   }

   /// <summary>
   /// Project paths start on the parent of the parent folder of the leaf.
   /// For example: /Some-Root/PROJECT/Arguments/document.json
   /// </summary>
   /// <param name="targetPath">target path</param>
   /// <param name="pathItem">path item</param>
   /// <param name="source">source catalog</param>
   /// <param name="target">target catalog</param>
   public static void CloneProjectLeaf(string targetPath,
      CatalogPathItem pathItem, ICatalogService? source, ICatalogService? target)
   {
      // find the Project folder that we want to clone in the target
      string path = CatalogClone.GetProjectPath(pathItem.Item.FullPath);
      Clone(path, targetPath, pathItem, source, target);
   }

   /// <summary>
   /// Project paths start on the parent of the parent folder of the leaf.
   /// For example: /Some-Root/PROJECT/Arguments/document.json
   /// </summary>
   /// <param name="targetPath">target path</param>
   /// <param name="item">item</param>
   /// <param name="source">source catalog</param>
   /// <param name="target">target catalog</param>
   public static void CloneProjectLeaf(string targetPath,
      ItemInfo item, ICatalogService? source, ICatalogService? target)
   {
      // State the source Item that should be the actual Project
      CatalogPathItem pathItem = new CatalogPathItem(item);
      CloneProjectLeaf(targetPath, pathItem, source, target);
   }

}
