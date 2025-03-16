using Edam.DataObjects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogModel;

public interface ICatalogItem
{

   Task<ItemInfo> CreateBranchAsync(string path, string? description = null,
      Guid? containerId = null);
   ItemInfo CreateBranch(string path, string? description = null,
      Guid? containerId = null);

   ItemInfo CreateRootItem(Guid? containerId = null);

   Task<ItemInfo> GetContainerRootItemAsync(Guid id);
   ItemInfo GetContainerRootItem(Guid containerId);

   List<ItemInfo> GetContainerItems(Guid containerId);
   ItemInfo? GetItem(Guid itemId);

   Task<ItemInfo> GetItemByPathAsync(string path);
   ItemInfo GetItemByPath(string name);

   RequestStatus DeleteItem(Guid itemId);

   Task<List<ItemInfo?>> GetBranchAsync(string? path = null);
   List<ItemInfo?> GetBranch(string? path = null);

   Task<ItemInfo> AddItemAsync(ItemInfo item);
   ItemInfo AddItem(ItemInfo item);

}
