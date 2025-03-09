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

public class ClientCatalogItemData : ICatalogItemData
{

   #region -- 4.00 - Properties and Definitions

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public ClientCatalogItemData(ICatalogBaseClient client)
   {
      _Client = client;
   }

   #endregion
   #region -- 4.00 - Item Data Management

   /// <summary>
   /// Add Data Item Async.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public async Task<ItemDataInfo> AddItemAsync(ItemDataInfo item)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      ItemDataInfo result = null;

      var req = CatalogBaseClient.URI_ITEM_ADD + pars.ToString();
      try
      {
         result = await _Client.Client.PostAsync<ItemDataInfo>(req, item);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return result;
   }

   /// <summary>
   /// Add Data Item.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public ItemDataInfo AddItem(ItemDataInfo item)
   {
      ItemDataInfo? itm = null;
      Task<ItemDataInfo> result = AddItemAsync(item);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         itm = result.Result;
      }
      return itm;
   }

   /// <summary>
   /// Get Item Data Async.
   /// </summary>
   /// <param name="itemId">item ID whose data items are requested</param>
   /// <returns>List of item data instances are returned</returns>
   public async Task<List<ItemDataInfo>> GetItemDataAsync(Guid itemId)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_ID, itemId.ToString());
      List<ItemDataInfo> items = null;

      var req = CatalogBaseClient.URI_ITEM_DATA_ITEM_ID + pars.ToString();
      try
      {
         items = await _Client.Client.GetDataFromJsonAsync<List<ItemDataInfo>>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return items;
   }

   /// <summary>
   /// Get Item Data.
   /// </summary>
   /// <param name="itemId">item ID whose data items are requested</param>
   /// <returns>List of item data instances are returned</returns>
   public List<ItemDataInfo> GetItemData(Guid itemId)
   {
      List<ItemDataInfo?> items = null;
      Task<List<ItemDataInfo>> result = GetItemDataAsync(itemId);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         items = result.Result;
      }
      return items;
   }

   /// <summary>
   /// Delete Item Data (and all related data leafs) By Item ID Async.
   /// </summary>
   /// <param name="itemId">(guid) id</param>
   /// <returns>request status (code) is returned</returns>
   public async Task<RequestStatus> DeleteItemDataAsync(Guid itemId)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_ID, itemId.ToString());
      RequestStatus result = RequestStatus.Unknown;

      var req = CatalogBaseClient.URI_ITEM_DATA_ITEM_ID + pars.ToString();
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
   /// Delete Item Data (and all related data leafs) By Item ID.
   /// </summary>
   /// <remarks>(inner) ResultsLog contains the result</remarks>
   /// <param name="itemId">(guid) id</param>
   public RequestStatus DeleteItemData(Guid itemId)
   {
      RequestStatus response = RequestStatus.Unknown;
      Task<RequestStatus> result = DeleteItemDataAsync(itemId);
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

   /// <summary>
   /// Delete Data By Id Async.
   /// </summary>
   /// <param name="dataId">data (blob) Id</param>
   /// <returns>Request Status is returned</returns>
   public async Task<RequestStatus> DeleteDataAsync(Guid dataId)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_ID, dataId.ToString());
      RequestStatus result = RequestStatus.Unknown;

      var req = CatalogBaseClient.URI_DATA_ID + pars.ToString();
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
   /// Delete Data By Id.
   /// </summary>
   /// <param name="dataId">data (blob) Id</param>
   /// <returns>Request Status is returned</returns>
   public RequestStatus DeleteData(Guid dataId)
   {
      RequestStatus response = RequestStatus.Unknown;
      Task<RequestStatus> result = DeleteDataAsync(dataId);
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

   /// <summary>
   /// Create Data Leaf.
   /// </summary>
   /// <remarks>Item data is not created, call AddItem to do so</remarks>
   /// <param name="item">parent item</param>
   /// <param name="name">data item name</param>
   /// <param name="dataId">(optional) data id</param>
   /// <param name="dataValue"(blob) data value</param>
   /// <returns>instance of ItemData is returned</returns>
   public ItemDataInfo CreateDataLeaf(
      ItemInfo item, string name, Guid? dataId = null, byte[] dataValue = null)
   {
      var data = ItemDataInfo.CreateDataLeaf(item.Id, name, dataId);
      var pitem = new CatalogPathItem(item);
      data.ContentType = pitem.GetContentType();
      return data;
   }

   /// <summary>
   /// Create Data Leaf.
   /// </summary>
   /// <remarks>Item data is not created, call AddItem to do so</remarks>
   /// <param name="item">parent item</param>
   /// <param name="name">data item name</param>
   /// <param name="dataId">(optional) data id</param>
   /// <param name="dataValue"(blob) data value</param>
   /// <returns>instance of ItemData is returned</returns>
   public ItemDataInfo CreateDataLeaf(
      ItemInfo item, string name, Guid? dataId = null, string dataValue = null)
   {
      var data = ItemDataInfo.CreateDataLeaf(item.Id, name, dataId);
      data.DataText = dataValue;
      var pitem = new CatalogPathItem(item);
      data.ContentType = pitem.GetContentType();
      return data;
   }

   /// <summary>
   /// Get Data By Name Async.
   /// </summary>
   /// <param name="itemId">Guid of item Id</param>
   /// <param name="name">data (blob) name</param>
   /// <returns>instance of ItemDataInfo is returned</returns>
   public async Task<ItemDataInfo> GetDataByNameAsync(Guid itemId, string name)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_ITEM_ID, itemId.ToString());
      pars.Add(CatalogBaseClient.TAG_ITEM_NAME, itemId.ToString());
      ItemDataInfo item = null;

      var req = CatalogBaseClient.URI_ITEM_DATA_ITEM_NAME + pars.ToString();
      try
      {
         item = await _Client.Client.GetDataFromJsonAsync<ItemDataInfo>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return item;
   }

   /// <summary>
   /// Get Data By Name.
   /// </summary>
   /// <param name="itemId">Guid of item Id</param>
   /// <param name="name">data (blob) name</param>
   /// <returns>instance of ItemDataInfo is returned</returns>
   public ItemDataInfo GetDataByName(Guid itemId, string name)
   {
      ItemDataInfo? item = null;
      Task<ItemDataInfo> result = GetDataByNameAsync(itemId, name);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         item = result.Result;
      }
      return item;
   }

   /// <summary>
   /// Get Data Async.
   /// </summary>
   /// <param name="dataId">Guid Data ID</param>
   /// <returns>Instance of ItemDataInfo is returned</returns>
   public async Task<ItemDataInfo> GetDataAsync(Guid dataId)
   {
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_DATA_ID, dataId.ToString());
      ItemDataInfo item = null;

      var req = CatalogBaseClient.URI_ITEM_DATA_ID + pars.ToString();
      try
      {
         item = await _Client.Client.GetDataFromJsonAsync<ItemDataInfo>(req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
      }

      return item;
   }

   /// <summary>
   /// Get Data Async.
   /// </summary>
   /// <param name="dataId">Guid Data ID</param>
   /// <returns>Instance of ItemDataInfo is returned</returns>
   public ItemDataInfo GetData(Guid dataId)
   {
      ItemDataInfo? item = null;
      Task<ItemDataInfo> result = GetDataAsync(dataId);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         item = result.Result;
      }
      return item;
   }

   /// <summary>
   /// Get Content Type...
   /// </summary>
   /// <param name="contentTypeId"></param>
   /// <returns></returns>
   /// <exception cref="NotImplementedException"></exception>
   public ContentTypeInfo GetContentType(string contentTypeId)
   {
      throw new Exception(
         "ClientCatalogItemData::GetContentType - should never be called");
   }

   #endregion

}
