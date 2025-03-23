using Edam.DataObjects.Trees;
using Edam.InOut;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogModel;

/// <summary>
/// Catalog Tree Builder based on the common TreeItem.
/// </summary>
public class CatalogTreeBuilder
{

   #region -- 4.00 - Fields and Properties...

   private int _ItemCount = 0;

   private List<CatalogPathItem> _pathItems;
   private List<ItemInfo> _items;

   private ICatalogService _Service;
   private CatalogInfo _CatalogInfo;
   private Dictionary<string, CatalogPathItem>? _Dictionary;

   public Dictionary<string, CatalogPathItem>? Dictionary
   {
      get { return _Dictionary; }
   }

   public CatalogTreeBuilder(ICatalogService service, CatalogInfo catalog)
   {
      _Dictionary = catalog.CatalogDictionary ??
         new Dictionary<string, CatalogPathItem>();
      _Service = service;
      _CatalogInfo = catalog;
   }

   #endregion
   #region -- 4.00 - Items Support and Management

   /// <summary>
   /// Get Items within the builder dictionary.
   /// </summary>
   /// <param name="init">true if get a fresh list</param>
   /// <returns>returns the list of Path Items</returns>
   public List<CatalogPathItem> GetPathItems(bool init = false)
   {
      if (_pathItems == null || init)
      {
         _pathItems = _Dictionary.Values.ToList<CatalogPathItem>();
      }
      return _pathItems;
   }

   /// <summary>
   /// Get Items within the builder dictionary.
   /// </summary>
   /// <param name="init">true if get a fresh list</param>
   /// <returns>returns the list of ItemInfoes</returns>
   public List<ItemInfo> GetItems(bool init = false)
   {
      var pitems = GetPathItems(init);
      _items = new List<ItemInfo>();
      foreach (var item in pitems)
      {
         _items.Add(item.Item);
      }
      return _items;
   }

   /// <summary>
   /// Get an Item.
   /// </summary>
   /// <param name="path"></param>
   /// <returns></returns>
   public CatalogPathItem GetPathItem(string path)
   {
      CatalogPathItem value = null;
      if (_Dictionary.TryGetValue(path.ToLower(), out value))
      {
         return value;
      }
      return null;
   }

   /// <summary>
   /// Get Leaf Items within the builder dictionary.
   /// </summary>
   /// <param name="init">true if get a fresh list</param>
   /// <returns>returns the list of Path Items</returns>
   public List<CatalogPathItem> GetLeafItems(Guid parentItemId)
   {
      List<CatalogPathItem> items = new List<CatalogPathItem>();
      var l = GetItems();
      //l.FindAll((x) => x.ItemType == TreeItemType.Leaf &&
      //   x.Item)
      return items;
   }

   /// <summary>
   /// Get Leaf Items within the builder dictionary.
   /// </summary>
   /// <param name="init">true if get a fresh list</param>
   /// <returns>returns the list of Path Items</returns>
   public List<ItemInfo> GetItems(string path)
   {
      string fpath = path.ToLower();
      List<ItemInfo> items = new List<ItemInfo>();
      var l = GetItems();
      var ritems = l.FindAll((x) => 
         Regex.Match(x.FullPath.ToLower(), fpath + "*").Success);
      return ritems;
   }

   /// <summary>
   /// Get an Item 
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
   public CatalogPathItem GetItem(Guid id)
   {
      var l = GetPathItems();
      var itm = l.Find(l => l.Item.Id == id);
      return itm;
   }

   /// <summary>
   /// Add item into the dictionary.
   /// </summary>
   /// <param name="item"></param>
   public void AddItem(CatalogPathItem item)
   {
      CatalogPathItem value;
      if (!_Dictionary.TryGetValue(item.Full.ToLower(), out value))
      {
         _Dictionary.Add(item.Item.FullPath.ToLower(), item);
      }
   }

   /// <summary>
   /// Delete Item from the builder dictionary.
   /// </summary>
   /// <param name="path">path to remove from dictionary</param>
   public void DeleteItem(string path)
   {
      var items = GetItems(path);
      foreach (var item in items)
      {
         _Dictionary.Remove(item.FullPath.ToLower());
      }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   //public CatalogPathItem ToPathItem(ItemInfo item)
   //{
   //   CatalogPathItem pitem = new CatalogPathItem(item);
   //   return pitem;
   //}


   /// <summary>
   /// Get extended given path.
   /// </summary>
   /// <param name="item">item whose name is to be extended</param>
   /// <returns>extended path name is returned</returns>
   public string ExtendPathName(string path)
   {
      return _CatalogInfo.RootPathItem.DriverName + path;
   }

   /// <summary>
   /// Get fully extended path name based on given Item.
   /// </summary>
   /// <param name="item">item whose name is to be extended</param>
   /// <returns>fully extended path name is returned</returns>
   public string ExtendFullPathName(ItemInfo item)
   {
      return _CatalogInfo.RootPathItem.RootFullPath + 
         item.FullPath.Substring(1);
   }

   /// <summary>
   /// Find or Create a Registered Item...
   /// </summary>
   /// <param name="item">item</param>
   /// <returns>Path Iterm instance is returned</returns>
   public CatalogPathItem CreateRegisterItem(ItemInfo item)
   {
      string path = item.FullPath.ToLower();
      if (!_Dictionary.TryGetValue(path, out var pitem))
      {
         pitem = GetItem(item);
      }
      return pitem;
   }

   #endregion
   #region -- 4.00 - Create and Update Items...

   /// <summary>
   /// Create item.
   /// </summary>
   /// <param name="pathItem">path item information to create new item</param>
   /// <returns>return the created item</returns>
   public static CatalogItemInfo CreateItem(CatalogPathItem pathItem)
   {
      // create a new catalog item
      CatalogItemInfo citem = new CatalogItemInfo
      {
         Name = pathItem.NameFull,
         Title = pathItem.Item.Description,
         Tag = pathItem
      };

      if (pathItem.Item.ItemType == TreeItemType.Branch)
      {
         citem.Type = TreeItemType.Branch;
         citem.Icon = CatalogInfo.IconBranch;
      }
      else
      {
         citem.Type = TreeItemType.Leaf;
         citem.Icon = CatalogInfo.IconLeaf;
      }

      string[] items = pathItem.Full.Split('/');
      citem.Level = new short[items.Length];
      for (int i = 0; i < items.Length; i++)
      {
         citem.Level[i] = (short)i;
      }

      pathItem.TreeItem = citem;

      return citem;
   }

   /// <summary>
   /// Create a File Item and register id.
   /// </summary>
   /// <param name="item"></param>
   /// <param name="path"></param>
   /// <returns></returns>
   private async Task<ItemInfo> CreateItemAsync(
      CatalogItemInfo item, string path)
   {
      ItemInfo pitem = new ItemInfo();
      pitem.Name = item.Name;
      pitem.Description = item.Title;
      pitem.ItemType = item.Type == TreeItemType.Branch ?
         TreeItemType.Branch : TreeItemType.Leaf;
      pitem.FullPath = path;
      pitem.Container = _Service.CurrentContainer;
      pitem.ContainerId = pitem.Container.Id;

      await _Service.Item.AddItemAsync(pitem);
      return pitem;
   }

   #endregion
   #region -- 4.00 - Item Registration...

   /// <summary>
   /// Update repository.
   /// </summary>
   /// <param name="pathItem"></param>
   /// <param name="updateParent"></param>
   /// <param name="updateItem"></param>
   private async Task UpdateRepositoryAsync(CatalogPathItem pathItem, 
      bool updateParent, bool updateItem)
   {
      ItemInfo item;
      if (updateParent)
      {
         item = await CreateItemAsync(
            pathItem.Parent.TreeItem, pathItem.Full);
      }
      if (updateItem)
      {
         pathItem.Item.Name = pathItem.NameFull;
         pathItem.Item.Description = pathItem.NameFull.
            Replace('.',' ').Replace('/',' ');
         //pathItem.Item.Container = _Service.CurrentContainer;
         pathItem.Item.ContainerId = _Service.CurrentContainer.Id;
         pathItem.Item.FullPath = pathItem.Full;
         pathItem.Item.ItemType = 
            pathItem.MediaFormat == DataObjects.Medias.MediaFormat.Unknown ?
               TreeItemType.Branch : TreeItemType.Leaf;

         pathItem.TreeItem.Title = pathItem.Item.Description;

         // this looks like it makes no sence for a Leaf but what it does is
         // to register the path in the repository, if the repository already
         // exists like in a file system then Leafs need to be treaded with
         // care in the AddItem method... (go there to look)
         await _Service.Item.AddItemAsync(pathItem.Item);
      }
   }

   /// <summary>
   /// Register the root item will allow to set the root based on a path and
   /// all child entries will be based on this root.  See FileSystem client.
   /// </summary>
   /// <param name="item">root item to register</param>
   public void RegisterRootItem(CatalogPathItem item)
   {
      if (_Dictionary.Count > 0)
      {
         return;
      }
      if (_CatalogInfo != null && _CatalogInfo.RootPathItem == null)
      {
         _CatalogInfo.RootPathItem = item;
      }
      _Dictionary.Add(item.Full.ToLower(), item);
   }

   /// <summary>
   /// Register leaf parent branch full path.
   /// </summary>
   /// <param name="fullPath">full path to register</param>
   public async Task<CatalogPathItem> RegisterBranchPathAsync(string fullPath)
   {
      //if (fullPath == null || fullPath[0] != '/')
      //{
      //   throw new ArgumentException(
      //      "CatalogTreeBuilder: a path must start with a slash.");
      //}

      var l = fullPath.Split('/');
      string path = String.Empty;
      CatalogPathItem parent = null;

      // must always start at 1 since all entries should start with a '/'
      for (int i = 0; i < l.Length; i++)
      {
         if (l[i] == String.Empty)
         {
            continue;
         }

         string name = l[i].Trim();
         path += "/" + name;
         if (path == String.Empty || path == "/")
         {
            continue;
         }
         
         // add branch to repository
         ItemInfo item = await _Service.Item.CreateBranchAsync(path);
         item.FullPath = path;
         item = await _Service.Item.AddItemAsync(item);

         // add branch to registry
         var pathItem = new CatalogPathItem(item);
         if (!_Dictionary.TryGetValue(path.ToLower(), out CatalogPathItem value))
         {
            _Dictionary.TryAdd(path.ToLower(), pathItem);

            // setup tree item as needed
            if (pathItem.TreeItem == null)
            {
               pathItem.TreeItem = CreateItem(pathItem);
               pathItem.TreeItem.Number = ++_ItemCount;
            }
         }
         else
         {
            pathItem = value;
         }

         // finally add child to parent as needed
         if (parent == null && !_CatalogInfo.RootTreeItem.Children.TryGetValue(
            pathItem.TreeItem, out CatalogItemInfo rootChild))
         {
            // add item to the root node
            _CatalogInfo.RootTreeItem.Children.Add(pathItem.TreeItem);
         }
         else if (parent != null && !parent.TreeItem.Children.TryGetValue(
            pathItem.TreeItem, out CatalogItemInfo child))
         {
            parent.TreeItem.Children.Add(pathItem.TreeItem);
         }

         // setup next parent
         parent = pathItem;
      }

      return parent;
   }

   #endregion
   #region -- 4.00 - Get / Fetch Item

   /// <summary>
   /// Get Catalog Item information from a File Item.
   /// </summary>
   /// <param name="pathItem">file item</param>
   /// <returns>instance of catalog item is returned</returns>
   public async Task<CatalogItemInfo> GetItemAsync(CatalogPathItem pathItem)
   {
      bool updateParent = false;
      bool updateItem = false;

      // try to find, if not found try to add paths
      if (!_Dictionary.TryGetValue(pathItem.Full.ToLower(), out CatalogPathItem item))
      {
         // add full path for new item
         _Dictionary.TryAdd(pathItem.Full.ToLower(), pathItem);
         updateItem = true;
         item = pathItem;
      }

      // setup tree item as needed
      if (item.TreeItem == null)
      {
         item.TreeItem = CreateItem(pathItem);
         item.TreeItem.Number = ++_ItemCount;
      }

      // is this the root path? if so, skip it...
      if (pathItem.Path == _CatalogInfo.RootItem.FullPath)
      {
         return _CatalogInfo.RootTreeItem;
      }

      // setup parent as needed
      if (item.Parent == null)
      {
         item.Parent = await RegisterBranchPathAsync(pathItem.Path);
      }

      // finally add child to parent as needed
      if (pathItem.TreeItem != null &&
         !item.Parent.TreeItem.Children.TryGetValue(
         pathItem.TreeItem, out CatalogItemInfo child))
      {
         item.Parent.TreeItem.Children.Add(pathItem.TreeItem);
      }

      // item.Parent.TreeItem.Children.Add(item.TreeItem);

      // update repository
      await UpdateRepositoryAsync(pathItem, updateParent, updateItem);

      return item.TreeItem;
   }

   /// <summary>
   /// Get Catalog Item information from a File Item.
   /// </summary>
   /// <param name="item">file item</param>
   /// <returns>instance of catalog item is returned</returns>
   public async Task<CatalogPathItem> GetItemAsync(ItemInfo item)
   {
      var pitem = new CatalogPathItem(item);
      var citem = await GetItemAsync(pitem);
      return pitem;
   }

   /// <summary>
   /// Get Catalog Item information from a File Item.
   /// </summary>
   /// <param name="item">file item</param>
   /// <returns>instance of catalog item is returned</returns>
   public CatalogPathItem GetItem(ItemInfo item)
   {
      var pitem = new CatalogPathItem(item);
      var task = Task.Run(() => GetItemAsync(pitem));
      task.Wait();
      if (task.Status == TaskStatus.RanToCompletion)
      {
         var citem = task.Result;
      }
      return pitem;
   }

   /// <summary>
   /// Get Catalog Item information based on given full path name.
   /// </summary>
   /// <param name="fullPath">full path</param>
   /// <returns>instance of catalog item is returned</returns>
   public async Task<CatalogPathItem> GetItemAsync(string fullPath)
   {
      CatalogPathItem item = null;
      ItemInfo fitem = new ItemInfo();
      var pitem = await _Service.Item.GetItemByPathAsync(fullPath);
      if (pitem != null)
      {
         fitem = pitem;
      }
      else
      {
         fitem.FullPath = fullPath;
      }

      item = new CatalogPathItem(fitem);
      if (pitem == null)
      {
         var citem = await GetItemAsync(item);
      }

      return item;
   }

   /// <summary>
   /// Get Catalog Item information based on given folder-file instance.
   /// </summary>
   /// <param name="item">folder-file instance</param>
   /// <returns>instance of catalog item is returned</returns>
   public async Task<CatalogPathItem> GetItemAsync(FolderFileItemInfo item)
   {
      // if this is the root item just return
      if (item.Full == _CatalogInfo.RootPathItem.RootFullPath)
         return _CatalogInfo.RootPathItem;

      string path = "/" + item.Full.Substring(
         _CatalogInfo.RootPathItem.RootFullPath.Length);

      // try to catalog and register the item
      CatalogPathItem pitem = null;
      ItemInfo fitem = new ItemInfo
      {
         ItemType = item.Type == ItemType.Folder ?
            TreeItemType.Branch : TreeItemType.Leaf
      };

      var foundItem = await _Service.Item.GetItemByPathAsync(path);
      if (foundItem != null)
      {
         fitem = foundItem;
      }
      else
      {
         fitem.FullPath = path;
      }

      pitem = new CatalogPathItem(fitem);
      pitem.Parent = _CatalogInfo.RootPathItem;
      if (foundItem == null)
      {
         var citem = await GetItemAsync(pitem);
      }

      return pitem;
   }

   #endregion
   #region -- 4.00 - Path and Branch Support

   /// <summary>
   /// Convert to a Catalog Path Item.
   /// </summary>
   /// <param name="item">file item to convert</param>
   /// <returns>instance of CatalogItemInfo is returned</returns>
   public static CatalogPathItem ToPathItem(ItemInfo item)
   {
      CatalogPathItem pitem = new CatalogPathItem(item);
      pitem.TreeItem = CreateItem(pitem);
      return pitem;
   }

   /// <summary>
   /// Given a path, clean it up and return a valid path.
   /// </summary>
   /// <param name="path">path to review</param>
   /// <returns>clen-up path is returned</returns>
   public static string GetPath(string? path)
   {
      // is this an empty path? if so, set it up as the root path
      string spath = String.IsNullOrWhiteSpace(path) ? "/" : path;
      var lastChar = spath.Length > 1 ? spath[spath.Length - 1] : ' ';
      if (lastChar == '/')
      {
         spath = spath.Substring(0, spath.Length - 1);
      }

      // does we have a driver of sort specified?
      int indx = spath.IndexOf('/');
      if (indx >= 0)
      {
         spath = spath.Substring(indx);
      }

      return spath;
   }

   /// <summary>
   /// Get branch items for current container.
   /// </summary>
   /// <param name="path">path to branch</param>
   public async Task GetBranchAsync(string? path = null)
   {
      string spath = GetPath(path);
      spath = spath.Length > 1 ? spath + "/" : spath;
      var items = await _Service.Item.GetBranchAsync(spath);
      foreach (var item in items)
      {
         await GetItemAsync(item);
      }
   }

   #endregion
   #region -- 4.00 - Tree Builder methods...

   /// <summary>
   /// Build the tree based on the catalog items path
   /// </summary>
   /// <param name="resetCatalog">reset catalog [default: true]</param>
   /// <returns>instance of Catalog is returned</returns>
   public async Task BuildTreeAsync(bool resetCatalog = true)
   {
      if (resetCatalog)
      {
         _Dictionary = _CatalogInfo.CatalogDictionary;
         _Dictionary.Clear();
      }

      await GetBranchAsync();
   }

   #endregion

}
