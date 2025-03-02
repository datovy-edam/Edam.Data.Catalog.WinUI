using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.DataObjects.Requests;

namespace Edam.Data.CatalogModel;

public interface ICatalogService
{

   ContainerInfo DefaultContainer { get; set; }
   ContainerInfo CurrentContainer { get; set; }

   Task<ContainerInfo> GetContainerAsync(string? containerId, bool checkId = true);
   ContainerInfo GetContainer(string? containerId, bool checkId = true);

   ContainerInfo SetContainer(string sessionId, string containerId);
   ContainerInfo EnlistContainer(string containerId, string description);
   ContainerInfo DelistContainer(string containerId);

   Task<ItemInfo> CreateBranchAsync(string path, string? description = null,
      Guid? containerId = null);
   ItemInfo CreateBranch(string path, string? description = null, 
      Guid? containerId = null);

   ItemInfo CreateLeaf(string path, string name,
      Guid? id = null, string? description = null, string? dataValue = null);

   ItemDataInfo CreateDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, byte[] dataValue = null);
   ItemDataInfo CreateDataLeaf(ItemInfo item, string name,
      Guid? dataId = null, string dataValue = null);

   Task<ItemInfo> GetContainerRootItemAsync(Guid id);
   ItemInfo GetContainerRootItem(Guid containerId);

   Task<List<ContainerInfo>> GetContainersAsync();
   List<ContainerInfo> GetContainers();

   List<ItemInfo> GetContainerItems(Guid containerId);
   ContentTypeInfo GetContentType(string contentTypeId);
   ItemDataInfo GetDataByName(Guid itemId, string name);

   ItemInfo? GetItem(Guid itemId);

   Task<ItemInfo> GetItemByPathAsync(string path);
   ItemInfo GetItemByPath(string name);

   RequestStatus DeleteItem(Guid itemId);

   Task<List<ItemInfo?>> GetBranchAsync(string? path = null);
   List<ItemInfo?> GetBranch(string? path = null);

   Task<ItemInfo> AddItemAsync(ItemInfo item);
   ItemInfo AddItem(ItemInfo item);

   Task<ItemDataInfo> AddItemAsync(ItemDataInfo item);
   ItemDataInfo AddItem(ItemDataInfo item);

   ItemDataInfo GetData(Guid dataId);
   Task<List<ItemDataInfo>> GetItemDataAsync(Guid itemId);
   List<ItemDataInfo> GetItemData(Guid itemId);
   RequestStatus DeleteItemData(Guid itemId);
   RequestStatus DeleteData(Guid dataId);

}
