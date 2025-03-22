using Edam.Data.CatalogDb;
using Edam.Data.CatalogModel;
using Edam.DataObjects;
using Edam.DataObjects.Requests;
using Edam.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogServiceClient;

public class CatalogFileSystemItemData: ICatalogItemData
{

   #region -- 4.00 - Properties and Definitions

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public CatalogFileSystemItemData(ICatalogBaseClient client)
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
      return AddItem(item);
   }

   /// <summary>
   /// Add Data Item.
   /// </summary>
   /// <param name="item">item to ask</param>
   /// <returns>created item is returned, else null</returns>
   public ItemDataInfo AddItem(ItemDataInfo item)
   {
      _Client.ResultsLog.Clear();
      var itm = _Client.Cataloger.GetItem(item.Id);
      File.WriteAllBytes(itm.Item.FullPath, item.Data);
      return item;
   }

   /// <summary>
   /// Get Item Data Async.
   /// </summary>
   /// <param name="itemId">item ID whose data items are requested</param>
   /// <returns>List of item data instances are returned</returns>
   public async Task<List<ItemDataInfo>> GetItemDataAsync(Guid itemId)
   {
      return GetItemData(itemId);
   }

   /// <summary>
   /// Get Item Data.
   /// </summary>
   /// <param name="itemId">item ID whose data items are requested</param>
   /// <returns>List of item data instances are returned</returns>
   public List<ItemDataInfo> GetItemData(Guid itemId)
   {
      _Client.ResultsLog.Clear();
      var pitem = _Client.Cataloger.GetItem(itemId);
      List<ItemDataInfo> ditem = new List<ItemDataInfo>();
      var data = File.ReadAllBytes(pitem.Item.FullPath);
      ItemDataInfo itemData = pitem.ToItemData(data);
      ditem.Add(itemData);
      return ditem;
   }

   /// <summary>
   /// Delete Item Data (and all related data leafs) By Item ID Async.
   /// </summary>
   /// <param name="itemId">(guid) id</param>
   /// <returns>request status (code) is returned</returns>
   public async Task<RequestStatus> DeleteItemDataAsync(Guid itemId)
   {
      return DeleteItemData(itemId);
   }

   /// <summary>
   /// Delete Item Data (and all related data leafs) By Item ID.
   /// </summary>
   /// <remarks>(inner) ResultsLog contains the result</remarks>
   /// <param name="itemId">(guid) id</param>
   public RequestStatus DeleteItemData(Guid itemId)
   {
      _Client.ResultsLog.Clear();
      RequestStatus response = RequestStatus.Unknown;
      var pitem = _Client.Cataloger.GetLeafItems(itemId);
      if (pitem != null && pitem.Count > 0)
      {
         foreach (var item in pitem)
         {
            File.Delete(item.Item.FullPath);
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
      return DeleteData(dataId);
   }

   /// <summary>
   /// Delete Data By Id.
   /// </summary>
   /// <param name="dataId">data (blob) Id</param>
   /// <returns>Request Status is returned</returns>
   public RequestStatus DeleteData(Guid dataId)
   {
      _Client.ResultsLog.Clear();
      RequestStatus response = RequestStatus.Unknown;
      var pitem = _Client.Cataloger.GetItem(dataId);
      if (pitem != null)
      {
         File.Delete(pitem.Item.FullPath);
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
      var pitem = new CatalogPathItem(item);
      var dataItem = pitem.ToItemData(dataValue);
      dataItem.ContentType = pitem.GetContentType();
      
      return dataItem;
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
      var pitem = new CatalogPathItem(item);
      var dataItem = pitem.ToItemData(dataValue);
      dataItem.ContentType = pitem.GetContentType();

      return dataItem;
   }

   /// <summary>
   /// Get Data By Name Async.
   /// </summary>
   /// <param name="itemId">Guid of item Id</param>
   /// <param name="name">data (blob) name</param>
   /// <returns>instance of ItemDataInfo is returned</returns>
   public async Task<ItemDataInfo> GetDataByNameAsync(Guid itemId, string name)
   {
      return GetDataByName(itemId, name);
   }

   /// <summary>
   /// Get Data By Name.
   /// </summary>
   /// <param name="itemId">Guid of item Id</param>
   /// <param name="name">data (blob) name</param>
   /// <returns>instance of ItemDataInfo is returned</returns>
   public ItemDataInfo GetDataByName(Guid itemId, string name)
   {
      _Client.ResultsLog.Clear();
      var dataItems = GetItemData(itemId);
      ItemDataInfo itemData = dataItems != null && dataItems.Count > 0 ?
         dataItems[0] : null;

      return itemData;
   }

   /// <summary>
   /// Get Data Async.
   /// </summary>
   /// <param name="dataId">Guid Data ID</param>
   /// <returns>Instance of ItemDataInfo is returned</returns>
   public async Task<ItemDataInfo> GetDataAsync(Guid dataId)
   {
      return GetData(dataId);
   }

   /// <summary>
   /// Get Data Async.
   /// </summary>
   /// <param name="dataId">Guid Data ID</param>
   /// <returns>Instance of ItemDataInfo is returned</returns>
   public ItemDataInfo GetData(Guid dataId)
   {
      _Client.ResultsLog.Clear();
      var dataItems = GetItemData(dataId);
      ItemDataInfo itemData = dataItems != null && dataItems.Count > 0 ?
         dataItems[0] : null;

      return itemData;
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
