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
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public ItemInfo AddItem(ItemInfo item)
   {
      _Client.ResultsLog.Clear();
      // try to find the item in the file system..
      if (!File.Exists(item.FullPath))
      {
         File.Create(item.FullPath);
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
   /// <param name="name">path name</param>
   /// <returns>return found Item by path name</returns>
   public ItemInfo GetItemByPath(string name)
   {
      _Client.ResultsLog.Clear();
      var itms = _Client.Cataloger.GetItems();
      var itm = itms.Find(x => x.FullPath == name);
      return itm;
   }

   /// <summary>
   /// Delete Item Async.
   /// </summary>
   /// <param name="id">id (Guid) of item to delete</param>
   /// <returns>request status is returned</returns>
   public async Task<RequestStatus> DeleteItemAsync(Guid itemId)
   {
      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_ID, itemId.ToString());
      RequestStatus result = RequestStatus.Unknown;

      var req = CatalogBaseClient.URI_ITEM_ID + pars.ToString();
      try
      {
         var response = await _Client.Client.DeleteAsync<RequestResponseInfo>(req);
         if (response != null && response.Success)
         {
            result = response.Status;
         }
         else
         {
            result = RequestStatus.Failed;
         }
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return result;
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
            if (File.Exists(itm.FullPath))
            {
               _Client.Cataloger.DeleteItem(itm.FullPath);
               File.Delete(itm.FullPath);
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
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public ItemInfo? CreateBranch(
      string path, string? description = null, Guid? containerId = null)
   {
      ItemInfo item = new ItemInfo();
      item.FullPath = path;
      item.Description = description;
      item.ContainerId = containerId.HasValue ? containerId.Value :
         _Client.CurrentContainer.Id;
      item.ItemType = DataObjects.Trees.TreeItemType.Branch;

      CatalogPathItem pitem = new CatalogPathItem(item);

      _Client.Cataloger.AddItem(pitem);

      return item;
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
   /// 
   /// </summary>
   /// <param name="path"></param>
   /// <returns></returns>
   /// <exception cref="NotImplementedException"></exception>
   public async Task<List<ItemInfo?>> GetBranchAsync(string? path = null)
   {
      return GetBranch(path);
   }

   /// <summary>
   /// Add Data Item.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public List<ItemInfo?> GetBranch(string? path)
   {
      _Client.ResultsLog.Clear();
      var list = _Client.Cataloger.GetItems();
      List<ItemInfo?> oitems = new List<ItemInfo?>();
      foreach (var item in list)
      {
         if (Regex.IsMatch(path, item.FullPath + "*"))
         {
            oitems.Add(item);
         }
      }
      return oitems;
   }

   #endregion

}
