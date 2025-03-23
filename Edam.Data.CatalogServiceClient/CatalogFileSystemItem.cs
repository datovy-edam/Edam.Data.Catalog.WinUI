using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogDb;
using Edam.Data.CatalogModel;
using Edam.Text;
using Edam.DataObjects.Requests;
using System.Text.RegularExpressions;

namespace Edam.Data.CatalogServiceClient;

public class CatalogFileSystemItem : ICatalogItem
{

   #region -- 4.00 - Properties and Definitions

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public CatalogFileSystemItem(ICatalogBaseClient client)
   {
      _Client = client;
   }

   #endregion
   #region -- 4.00 - Container - Item Management

   /// <summary>
   /// Get Container Root Item Async.
   /// </summary>
   /// <param name="id">Guid of container whose root item is requested</param>
   /// <returns>root item is returned</returns>
   public async Task<ItemInfo> GetContainerRootItemAsync(Guid id)
   {
      return GetContainerRootItem(id);
   }

   /// <summary>
   /// Get Container Root Item.
   /// </summary>
   /// <param name="id">Guid of container whose root item is requested</param>
   /// <returns>root item is returned</returns>
   public ItemInfo GetContainerRootItem(Guid id)
   {
      _Client.ResultsLog.Clear();
      var container = _Client.Container.GetContainer(id);
      var l = _Client.Cataloger.GetPathItems();
      return l.Count > 0 ? l[0].Item : null;
   }

   /// <summary>
   /// Get Container Items Async.
   /// </summary>
   /// <param name="id">container id whose items are requested</param>
   /// <returns>list of items is returned</returns>
   public async Task<List<ItemInfo>> GetContainerItemsAsync(Guid id)
   {
      return GetContainerItems(id);
   }

   /// <summary>
   /// Get Container Items.
   /// </summary>
   /// <param name="id">container id whose items are requested</param>
   /// <returns>list of items is returned</returns>
   public List<ItemInfo> GetContainerItems(Guid id)
   {
      _Client.ResultsLog.Clear();
      var container = _Client.Container.GetContainer(id);
      return _Client.Cataloger.GetItems();
   }

   #endregion
   #region -- 4.00 - Items Management

   /// <summary>
   /// Add Item Async.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public async Task<ItemInfo> AddItemAsync(ItemInfo item)
   {
      return AddItem(item);
   }

   /// <summary>
   /// Add Item.
   /// </summary>
   /// <remarks>
   /// Items full path names should not include the root.
   /// </remarks>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public ItemInfo AddItem(ItemInfo item)
   {
      _Client.ResultsLog.Clear();
      var pathname = _Client.Cataloger.ExtendFullPathName(item);

      // does branch already registered?
      var fitem = _Client.Cataloger.GetPathItem(item.FullPath);
      if (fitem != null)
      {
         return fitem.Item;
      }

      // is this a leaf?
      if (item.ItemType == DataObjects.Trees.TreeItemType.Leaf)
      {
         if (!File.Exists(pathname))
         {
            // create an empty file...
            File.WriteAllBytes(pathname, []);
         }
         return item;
      }

      // try to find the item in the file system..
      if (!Directory.Exists(pathname))
      {
         Directory.CreateDirectory(pathname);
         _Client.Cataloger.CreateRegisterItem(item);
      }
      return item;
   }

   /// <summary>
   /// Get Item Async.
   /// </summary>
   /// <param name="id">Item id to find.</param>
   /// <returns>return found Item by id (Guid)</returns>
   public async Task<ItemInfo?> GetItemAsync(Guid id)
   {
      return GetItem(id);
   }

   /// <summary>
   /// Get Item Async.
   /// </summary>
   /// <param name="id">Item id to find.</param>
   /// <returns>return found Item by id (Guid)</returns>
   public ItemInfo GetItem(Guid id)
   {
      _Client.ResultsLog.Clear();
      var itms = _Client.Cataloger.GetItems();
      var itm = itms.Find(x => x.Id == id);
      return itm;
   }

   /// <summary>
   /// Get Item by Path Async.
   /// </summary>
   /// <param name="name">path name</param>
   /// <returns>return found Item by path name</returns>
   public async Task<ItemInfo> GetItemByPathAsync(string name)
   {
      return GetItemByPath(name);
   }

   /// <summary>
   /// Get Item.
   /// </summary>
   /// <param name="path">path name</param>
   /// <returns>return found Item by path name</returns>
   public ItemInfo GetItemByPath(string path)
   {
      _Client.ResultsLog.Clear();
      var item = _Client.Cataloger.GetPathItem(path);
      return item != null ? item.Item : null;
   }

   #endregion
   #region -- 4.00 - Delete Item methods...

   /// <summary>
   /// Delete Item Async.
   /// </summary>
   /// <param name="id">id (Guid) of item to delete</param>
   /// <returns>request status is returned</returns>
   public async Task<RequestStatus> DeleteItemAsync(Guid itemId)
   {
      return DeleteItem(itemId);
   }

   /// <summary>
   /// Delete Item.
   /// </summary>
   /// <remarks>check for LastLog results</remarks>
   /// <param name="id">id (Guid) of item to delete</param>
   public RequestStatus DeleteItem(Guid id)
   {
      _Client.ResultsLog.Clear();
      var response = RequestStatus.Unknown;
      try
      {
         var itm = GetItem(id);
         if (itm != null)
         {
            var pathname = _Client.Cataloger.ExtendFullPathName(itm);
            if (Directory.Exists(pathname))
            {
               _Client.Cataloger.DeleteItem(itm.FullPath);
               Directory.Delete(pathname, true);
            }
         }
         response = RequestStatus.Completed;
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
         response = RequestStatus.Failed;
      }
      return response;
   }

   #endregion
   #region -- 4.00 - Manage Branches and Leafs

   /// <summary>
   /// Create a Branch using given path.
   /// </summary>
   /// <remarks>note that the path may really have a leaf pattern such as one
   /// that include a file and an extension at the end, if so a Leaf will be
   /// created instead</remarks>
   /// <param name="path">full path</param>
   /// <param name="description">description</param>
   /// <param name="containerId">target container</param>
   /// <returns>found or created branch is returned</returns>
   public async Task<ItemInfo> CreateBranchAsync(
      string path, string? description = null, Guid? containerId = null)
   {
      return CreateBranch(path, description, containerId);
   }

   /// <summary>
   /// Add Data Item.
   /// </summary>
   /// <param name="path">folder/branch path not including root folder
   /// subpath</param>
   /// <param name="description">item description</param>
   /// <param name="containerId">target container, else currenc container
   /// will be used</param>
   /// <returns>created item is returned, else null</returns>
   public ItemInfo? CreateBranch(
      string path, string? description = null, Guid? containerId = null)
   {
      // does branch already registered?
      var fitem = _Client.Cataloger.GetPathItem(path);
      if (fitem != null)
      {
         return fitem.Item;
      }

      // no item was found, register this new branch
      ItemInfo item = new()
      {
         FullPath = path,
         Description = description,
         ContainerId = containerId.HasValue ? containerId.Value :
            _Client.CurrentContainer.Id,
         ItemType = DataObjects.Trees.TreeItemType.Branch
      };

      var pitem = new CatalogPathItem(item);

      var ritem = AddItem(item);

      return ritem;
   }

   /// <summary>
   /// Create Root Item.  This is called only in the Catalog Instance and it
   /// should never be invoked anywhere else.
   /// </summary>
   /// <param name="containerId"></param>
   /// <returns></returns>
   public ItemInfo CreateRootItem(Guid? containerId)
   {
      throw new Exception(
         "ClientCatalogItem::CreateRootItem has no implementation");
      //return null;
   }

   /// <summary>
   /// Get Branches asynchronously that its beginning match 
   /// with given path pattern.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public async Task<List<ItemInfo?>> GetBranchAsync(string? path = null)
   {
      return GetBranch(path);
   }

   /// <summary>
   /// Get Branches that its beginning match with given path pattern.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public List<ItemInfo?> GetBranch(string? path)
   {
      _Client.ResultsLog.Clear();
      List<ItemInfo?> oitems = new();

      // does branch already registered?
      var fitem = _Client.Cataloger.GetPathItem(path);
      if (fitem != null)
      {
         oitems.Add(fitem.Item);
      }
      return oitems;
   }

   #endregion

}
