using Edam.DataObjects.Trees;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogModel;

public class CatalogInfo
{

   #region -- 1.00 - Fields and Properties...

   public const string CATALOG_FILE_NAME = "catalog.json";

   /// <summary>
   /// Icon Branch and Leaf is based on an encoded string
   /// </summary>
   public static string IconBranch { get; set; }
   public static string IconLeaf { get; set; }

   private ICatalogService? _CatalogService { get; set; }
   public ICatalogService? CatalogService
   {
      get { return _CatalogService; }
   }

   private HashSet<CatalogItemInfo?>? _Catalog;

   /// <summary>
   /// A dictionary based on the path for each item
   /// </summary>
   private Dictionary<string, CatalogPathItem>? _CatalogDictionary = 
      new Dictionary<string, CatalogPathItem>();
   //private int _CatalogChangeCount = 0;

   public Dictionary<string, CatalogPathItem>? CatalogDictionary
   {
      get { return _CatalogDictionary; }
   }

   //private ICatalogService? _CatalogInstance = null;

   public TreeItem? CatalogEntry { get; set; } = null;

   /// <summary>
   /// File based Root item
   /// </summary>
   public CatalogPathItem RootPathItem { get; set; }
   public ItemInfo RootItem
   {
      get { return RootPathItem.Item; }
   }
   public CatalogItemInfo? RootTreeItem
   {
      get { return RootPathItem.TreeItem; }
   }

   #endregion
   #region -- 1.50 - Initialization

   public CatalogInfo(ICatalogService service, string sessionId)
   {
      _CatalogService = service;
      //var container = service.SetContainer(sessionId, String.Empty);
   }

   /// <summary>
   /// Initialize Catalog...
   /// </summary>
   /// <param name="containerId">container Id</param>
   /// <param name="buildTree">(optional) true to build the tree. 
   /// Default: false</param>
   /// <returns>Task is returned</returns>
   public async Task<CatalogInfo> InitializeCatalogAsync(
      string containerId, bool buildTree = false)
   {
      var container = await _CatalogService.Container.GetContainerAsync(containerId);

      // create root item...
      var rootItem = await _CatalogService.Item.GetContainerRootItemAsync(
         _CatalogService.DefaultContainer.Id);
      RootPathItem = CatalogTreeBuilder.ToPathItem(rootItem);

      // build catalog tree
      if (buildTree)
      {
         var builder = new CatalogTreeBuilder(_CatalogService, this);
         await builder.BuildTreeAsync();
      }

      // add catalog
      _Catalog = new HashSet<CatalogItemInfo>();
      _Catalog.Add(this.RootTreeItem);

      return this;
   }

   #endregion

}
