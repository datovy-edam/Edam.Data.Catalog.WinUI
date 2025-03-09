using Edam.DataObjects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogModel;

public interface ICatalogItemData
{
   ContentTypeInfo GetContentType(string contentTypeId);

   ItemDataInfo CreateDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, byte[] dataValue = null);
   ItemDataInfo CreateDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, string dataValue = null);
   ItemDataInfo GetDataByName(Guid itemId, string name);

   Task<ItemDataInfo> AddItemAsync(ItemDataInfo item);
   ItemDataInfo AddItem(ItemDataInfo item);

   ItemDataInfo GetData(Guid dataId);
   Task<List<ItemDataInfo>> GetItemDataAsync(Guid itemId);
   List<ItemDataInfo> GetItemData(Guid itemId);
   RequestStatus DeleteItemData(Guid itemId);
   RequestStatus DeleteData(Guid dataId);

}
