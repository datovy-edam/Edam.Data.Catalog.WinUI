using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

using Edam.Data.CatalogModel;
using Edam.DataObjects.Requests;

namespace Edam.Data.CatalogDb;

public class CatalogItemData : ICatalogItemData
{

   #region -- 4.00 - Properties and Definitions

   private CatalogContext? DbContext { get; set; } = null;

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public CatalogItemData(ICatalogBaseClient client,
      CatalogContext context)
   {
      _Client = client;
      DbContext = context;
   }

   #endregion
   #region -- 4.00 - Item Data Context Support

   /// <summary>
   /// Get Item Data entries.
   /// </summary>
   /// <param name="itemId">File-Item ID</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public List<ItemDataInfo> GetItemDataByItemId(Guid itemId)
   {
      var ditems = from x in DbContext.DataItems
                   where x.ItemId == itemId
                   select x;
      return ditems.ToList<ItemDataInfo>();
   }

   /// <summary>
   /// Get Item Data entries.
   /// </summary>
   /// <param name="id">File-Item ID</param>
   /// <returns>Get list of data items for given item id</returns>
   public List<ItemDataInfo> GetItemData(Guid id)
   {
      var ditems = from x in DbContext.DataItems
                   where x.ItemId == id
                   select x;
      return ditems.ToList<ItemDataInfo>();
   }

   /// <summary>
   /// Get Item Data entries async.
   /// </summary>
   /// <param name="id">File-Item ID</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public async Task<List<ItemDataInfo>> GetItemDataAsync(Guid id)
   {
      return GetItemData(id);
   }

   /// <summary>
   /// Get Item Data entries.
   /// </summary>
   /// <param name="id">File-Item ID</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public ItemDataInfo GetData(Guid id)
   {
      var ditems = from x in DbContext.DataItems
                   where x.Id == id
                   select x;
      var list = ditems.ToList<ItemDataInfo>();
      return list.Count > 0 ? list[0] : null;
   }

   /// <summary>
   /// Get Item Data by name.
   /// </summary>
   /// <param name="fileItemId">File-Item ID</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public ItemDataInfo GetDataByName(Guid fileItemId, string name)
   {
      var ditems = from x in DbContext.DataItems
                   where x.ItemId == fileItemId &&
                         x.Name == name
                   select x;
      var list = ditems.ToList<ItemDataInfo>();
      return list.Count > 0 ? list[0] : null;
   }

   /// <summary>
   /// Get Content Type.
   /// </summary>
   /// <param name="id">Container ID</param>
   /// <returns></returns>
   public ContentTypeInfo GetContentType(string contentTypeId)
   {
      var items = from x in DbContext.ContentTypes
                  where x.TypeId == contentTypeId
                  select x;
      var list = items.ToList<ContentTypeInfo>();
      return list.Count > 0 ? list[0] : null;
   }

   #endregion
   #region -- 4.00 - Manage Item Data

   /// <summary>
   /// Add given data item (or update).  Understand that a data item is uniquely
   /// identified by its name.
   /// </summary>
   /// <param name="item">add item</param>
   public async virtual Task<ItemDataInfo> AddItemAsync(ItemDataInfo item)
   {
      bool saveIt = true;
      ItemDataInfo ritem;
      var ditem = GetDataByName(item.ItemId, item.Name);
      if (ditem == null)
      {
         var contentType = GetContentType(item.ContentTypeId);
         item.ContentType = contentType;
         ritem = item;
         DbContext.DataItems.Add(item);
      }
      else if (item.Id == ditem.Id)
      {
         ritem = ditem;
         DbContext.DataItems.Update(ditem);
      }
      else
      {
         ritem = ditem;
         saveIt = false;
      }
      if (saveIt)
      {
         try
         {
            await DbContext.SaveChangesAsync();
         }
         catch (Exception ex)
         {

         }
      }
      return ritem;
   }

   /// <summary>
   /// Add given data item (or update) async.  Understand that a data item is 
   /// uniquely identified by its name.
   /// </summary>
   /// <param name="item">add item</param>
   public ItemDataInfo AddItem(ItemDataInfo item)
   {
      ItemDataInfo dinfo = null;
      var task = AddItemAsync(item);
      task.Wait();
      if (task.Status == TaskStatus.RanToCompletion)
      {
         dinfo = task.Result;
      }
      return dinfo;
   }

   /// <summary>
   /// Create/Update file item data.
   /// </summary>
   /// <param name="item">parent file item</param>
   /// <param name="name">unique data item name</param>
   /// <param name="dataId">data id</param>
   /// <param name="dataValue">data value</param>
   /// <returns>file item instance is returned</returns>
   public virtual ItemDataInfo CreateDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, byte[] dataValue = null)
   {
      var data = ItemDataInfo.CreateDataLeaf(item.Id, name, dataId);
      data.Data = dataValue;

      return data;
   }

   /// <summary>
   /// Create/Update file item data.
   /// </summary>
   /// <param name="item">parent file item</param>
   /// <param name="name">unique data item name</param>
   /// <param name="dataId">data id</param>
   /// <param name="dataValue">data value</param>
   /// <returns>file item instance is returned</returns>
   public virtual ItemDataInfo CreateDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, string dataValue = null)
   {
      var data = ItemDataInfo.CreateDataLeaf(item.Id, name, dataId);
      data.DataText = dataValue;
      return data;
   }

   /// <summary>
   /// Create/Update file item data.
   /// </summary>
   /// <param name="item">parent file item</param>
   /// <param name="name">unique data item name</param>
   /// <param name="dataId">data id</param>
   /// <param name="dataValue">data value</param>
   /// <returns>file item instance is returned</returns>
   public virtual ItemDataInfo AddDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, byte[] dataValue = null)
   {
      var data = CreateDataLeaf(item, name, dataId, dataValue);
      return AddItem(data);
   }

   /// <summary>
   /// Create/Update file item data.
   /// </summary>
   /// <param name="item">parent file item</param>
   /// <param name="name">unique data item name</param>
   /// <param name="dataId">data id</param>
   /// <param name="dataValue">data value</param>
   /// <returns>file item instance is returned</returns>
   public virtual ItemDataInfo AddDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, string dataValue = null)
   {
      var data = CreateDataLeaf(item, name, dataId, dataValue);
      return AddItem(data);
   }

   /// <summary>
   /// Delete Item-Data and Item.
   /// </summary>
   /// <param name="itemId">Item ID</param>
   /// <returns>request status (Completed) is returned, else (Unknown)</returns>
   public virtual RequestStatus DeleteData(Guid id)
   {
      RequestStatus status = RequestStatus.Unknown;
      var ditem = GetData(id);
      if (ditem != null)
      {
         DbContext.DataItems.Remove(ditem);
         DbContext.SaveChanges();
         status = RequestStatus.Completed;
      }
      return status;
   }

   /// <summary>
   /// Delete Item-Data and Item.
   /// </summary>
   /// <param name="itemId">Item ID</param>
   /// <returns>request status (Completed) is returned, else (Unknown)</returns>
   public virtual RequestStatus DeleteItemData(Guid itemId)
   {
      RequestStatus status = RequestStatus.Unknown;
      var item = _Client.Item.GetItem(itemId);
      if (item != null)
      {
         var ditems = GetItemData(item.Id);
         foreach (var ditem in ditems)
         {
            DbContext.DataItems.Remove(ditem);
         }
         DbContext.SaveChanges();
         status = RequestStatus.Completed;
      }
      return status;
   }

   #endregion

}
