using Edam.Data.CatalogModel;
using Edam.DataObjects.Requests;
using Edam.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogDb;

namespace Edam.Data.CatalogService;

public class ClientCatalogItem : ICatalogItem
{

   #region -- 4.00 - Properties and Definitions

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public ClientCatalogItem(ICatalogBaseClient client)
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
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_CONTAINER_GUID, id.ToString());
      ItemInfo item = null;

      var req = CatalogBaseClient.URI_CONTAINER_ROOT_ITEM_ID + pars.ToString();
      try
      {
         item = await _Client.Client.GetDataFromJsonAsync<ItemInfo>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return item;
   }

   /// <summary>
   /// Get Container Root Item.
   /// </summary>
   /// <param name="id">Guid of container whose root item is requested</param>
   /// <returns>root item is returned</returns>
   public ItemInfo GetContainerRootItem(Guid id)
   {
      ItemInfo? item = null;
      Task<ItemInfo> result = GetContainerRootItemAsync(id);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         item = result.Result;
      }
      return item;
   }

   /// <summary>
   /// Get Container Items Async.
   /// </summary>
   /// <param name="id">container id whose items are requested</param>
   /// <returns>list of items is returned</returns>
   public async Task<List<ItemInfo>> GetContainerItemsAsync(Guid id)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_CONTAINER_GUID, id.ToString());
      List<ItemInfo> item = null;

      var req = CatalogBaseClient.URI_CONTAINER_ITEMS + pars.ToString();
      try
      {
         item = await _Client.Client.GetDataFromJsonAsync<List<ItemInfo>>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return item;
   }

   /// <summary>
   /// Get Container Items.
   /// </summary>
   /// <param name="id">container id whose items are requested</param>
   /// <returns>list of items is returned</returns>
   public List<ItemInfo> GetContainerItems(Guid id)
   {
      List<ItemInfo> items = null;
      Task<List<ItemInfo>> result = GetContainerItemsAsync(id);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         items = result.Result;
      }
      return items;
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
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      ItemInfo result = null;

      var req = CatalogBaseClient.URI_ITEM_ADD + pars.ToString();
      try
      {
         result = await _Client.Client.PostAsync<ItemInfo>(req, item);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return result;
   }

   /// <summary>
   /// Add Item.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public ItemInfo AddItem(ItemInfo item)
   {
      ItemInfo? itm = null;
      Task<ItemInfo> result = AddItemAsync(item);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         itm = result.Result;
      }
      return itm;
   }

   /// <summary>
   /// Get Item Async.
   /// </summary>
   /// <param name="id">Item id to find.</param>
   /// <returns>return found Item by id (Guid)</returns>
   public async Task<ItemInfo?> GetItemAsync(Guid id)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_ID, _Client.LastSessionId);
      ItemInfo result = null;

      var req = CatalogBaseClient.URI_ITEM_ID + pars.ToString();
      try
      {
         result = await _Client.Client.GetDataFromJsonAsync<ItemInfo>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return result;
   }

   /// <summary>
   /// Get Item Async.
   /// </summary>
   /// <param name="id">Item id to find.</param>
   /// <returns>return found Item by id (Guid)</returns>
   public ItemInfo GetItem(Guid id)
   {
      ItemInfo? itm = null;
      Task<ItemInfo> result = GetItemAsync(id);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         itm = result.Result;
      }
      return itm;
   }

   /// <summary>
   /// Get Item by Path Async.
   /// </summary>
   /// <param name="name">path name</param>
   /// <returns>return found Item by path name</returns>
   public async Task<ItemInfo> GetItemByPathAsync(string name)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_PATH, name);
      ItemInfo result = null;

      var req = CatalogBaseClient.URI_ITEM_PATH + pars.ToString();
      try
      {
         result = await _Client.Client.GetDataFromJsonAsync<ItemInfo>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return result;
   }

   /// <summary>
   /// Get Item.
   /// </summary>
   /// <param name="name">path name</param>
   /// <returns>return found Item by path name</returns>
   public ItemInfo GetItemByPath(string name)
   {
      ItemInfo? itm = null;
      Task<ItemInfo> result = GetItemByPathAsync(name);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         itm = result.Result;
      }
      return itm;
   }

   /// <summary>
   /// Delete Item Async.
   /// </summary>
   /// <param name="id">id (Guid) of item to delete</param>
   /// <returns>request status is returned</returns>
   public async Task<RequestStatus> DeleteItemAsync(Guid itemId)
   {
      _Client.ResultsLog.Clear();

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
      RequestStatus response = RequestStatus.Unknown;
      Task<RequestStatus> result = DeleteItemAsync(id);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         response = result.Result;
         if (response == RequestStatus.Failed)
         {
            _Client.ResultsLog.Failed(RequestStatus.Failed.ToString());
         }
         else
         {
            _Client.ResultsLog.Succeeded();
         }
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
      ItemInfo item = new ItemInfo();
      item.FullPath = path;
      item.Description = description;
      item.ContainerId = containerId.HasValue ? containerId.Value :
         _Client.CurrentContainer.Id;
      item.ItemType = DataObjects.Trees.TreeItemType.Branch;

      CatalogPathItem pitem = new CatalogPathItem(item);

      ItemInfo ritem = await AddItemAsync(pitem.Item);

      return ritem;
   }

   /// <summary>
   /// Add Data Item.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public ItemInfo? CreateBranch(
      string path, string? description = null, Guid? containerId = null)
   {
      ItemInfo? item = null;
      Task<ItemInfo?> result =
         CreateBranchAsync(path, description, containerId.Value);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         item = result.Result;
      }
      return item;
   }

   /// <summary>
   /// Create a Leaf using given path.
   /// </summary>
   /// <remarks>note that the path may really have a branch pattern such as one
   /// that do not include a file and an extension at the end, if so a Branch 
   /// will be created instead</remarks>
   /// <param name="path">full path</param>
   /// <param name="description">description</param>
   /// <param name="containerId">target container</param>
   /// <returns>found or created branch is returned</returns>
   public ItemInfo CreateLeaf(
      string path, string name, Guid? id = null, string? description = null,
      string? dataValue = null)
   {
      ItemInfo item = new ItemInfo();
      item.FullPath = path;
      item.Description = description;
      item.ContainerId = _Client.CurrentContainer.Id;
      item.ItemType = DataObjects.Trees.TreeItemType.Branch;

      CatalogPathItem pitem = new CatalogPathItem(item);

      ItemInfo ritem = AddItem(pitem.Item);

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
   /// 
   /// </summary>
   /// <param name="path"></param>
   /// <returns></returns>
   /// <exception cref="NotImplementedException"></exception>
   public async Task<List<ItemInfo?>> GetBranchAsync(string? path = null)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_PATH, path);
      List<ItemInfo> item = null;

      var req = CatalogBaseClient.URI_BRANCH_ITEMS + pars.ToString();
      try
      {
         item = await _Client.Client.GetDataFromJsonAsync<List<ItemInfo>>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return item;
   }

   /// <summary>
   /// Add Data Item.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public List<ItemInfo?> GetBranch(string? path)
   {
      List<ItemInfo?>? items = null;
      Task<List<ItemInfo?>> result = GetBranchAsync(path);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         items = result.Result;
      }
      return items;
   }

   #endregion

}
