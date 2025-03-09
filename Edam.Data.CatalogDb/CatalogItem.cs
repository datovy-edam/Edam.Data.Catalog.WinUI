using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogModel;
using Edam.DataObjects.Requests;
using Microsoft.EntityFrameworkCore;

namespace Edam.Data.CatalogDb;

public class CatalogItem : ICatalogItem
{

   #region -- 4.00 - Properties and Definitions

   private CatalogContext? DbContext { get; set; } = null;

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public CatalogItem(ICatalogBaseClient client,
      CatalogContext context)
   {
      _Client = client;
      DbContext = context;
   }

   #endregion
   #region -- 4.00 - Item Management

   /// <summary>
   /// Get Root Item of given container.
   /// </summary>
   /// <param name="containerId">container id</param>
   /// <returns>instance of Item is returned</returns>
   public ItemInfo GetContainerRootItem(Guid id)
   {
      var items = from item in DbContext.Items
                  where item.Name == CatalogBaseClient.ROOT_ID &&
                        item.FullPath == CatalogBaseClient.ROOT_PATH &&
                        item.ContainerId == id
                  select item;
      var l = items.ToList();
      return l.Count > 0 ? l[0] : null;
   }

   /// <summary>
   /// Get Container Items.
   /// </summary>
   /// <param name="id">Container ID</param>
   /// <returns></returns>
   public List<ItemInfo> GetContainerItems(Guid id)
   {
      var items = from x in DbContext.Items
                  where x.ContainerId == id
                  select x;
      return items.ToList<ItemInfo>();
   }

   /// <summary>
   /// Get Item by its unique full path.
   /// </summary>
   /// <param name="path">unique full path</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public async Task<ItemInfo> GetItemByPathAsync(string path)
   {
      return GetItemByPath(path);
   }

   /// <summary>
   /// Get Item by its unique full path.
   /// </summary>
   /// <param name="path">unique full path</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public ItemInfo GetItemByPath(string path)
   {
      var ditems = from x in DbContext.Items
                   where x.FullPath == path &&
                         x.ContainerId == _Client.CurrentContainer.Id
                   select x;
      if (ditems.Any())
      {
         return ditems.ToList<ItemInfo>()[0];
      }
      return null;
   }

   /// <summary>
   /// Get Item.
   /// </summary>
   /// <param name="id">File-Item ID</param>
   /// <returns>Get list of data items for given file-item id</returns>
   public ItemInfo? GetItem(Guid id)
   {
      var ditems = from x in DbContext.Items
                   where x.Id == id
                   select x;
      var list = ditems.ToList<ItemInfo>();
      return list.Count > 0 ? list[0] : null;
   }

   /// <summary>
   /// Get Branch items (of current container).
   /// </summary>
   /// <param name="path">path to fetch first level items</param>
   /// <returns>Get list of items for given partial path</returns>
   public List<ItemInfo?> GetBranch(string? path = null)
   {
      var spath = String.IsNullOrWhiteSpace(path) ? "/" : path;
      var ditems = DbContext.Items.
         Where((x) => EF.Functions.Like(x.FullPath, spath + "%") &&
            x.Container.Id == _Client.CurrentContainer.Id);

      var list = ditems.ToList<ItemInfo>();
      return list;
   }

   /// <summary>
   /// Get Branch items (of current container) async.
   /// </summary>
   /// <param name="path">path to fetch first level items</param>
   /// <returns>Get list of items for given partial path</returns>
   public async Task<List<ItemInfo>> GetBranchAsync(string? path = null)
   {
      return GetBranch(path);
   }

   /// <summary>
   /// Add item (or update) in repository based on its full path.
   /// </summary>
   /// <param name="item">add item</param>
   public virtual ItemInfo AddItem(ItemInfo item)
   {
      ItemInfo ritem;
      var iitem = GetItemByPath(item.FullPath);
      if (iitem == null)
      {
         ritem = item;
         DbContext.Items.Add(item);
      }
      //else if (item.Id != iitem.Id)
      //{
      //   item.Id = iitem.Id;
      //   ritem = iitem;
      //   DbContext.Items.Update(iitem);
      //}
      else
      {
         ritem = iitem;
         return ritem;
      }
      DbContext.SaveChanges();
      return ritem;
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   public async Task<ItemInfo> AddItemAsync(ItemInfo item)
   {
      return AddItem(item);
   }

   /// <summary>
   /// Delete Item-Data and Item.
   /// </summary>
   /// <param name="id">Container ID</param>
   /// <returns>request status (Completed) is returned, else (Unknown)</returns>
   public virtual RequestStatus DeleteItem(Guid id)
   {
      RequestStatus status = RequestStatus.Unknown;
      var item = GetItem(id);
      if (item != null)
      {
         var ditems = _Client.ItemData.GetItemData(item.Id);
         foreach (var ditem in ditems)
         {
            DbContext.DataItems.Remove(ditem);
         }
         DbContext.Items.Remove(item);
         DbContext.SaveChanges();
         status = RequestStatus.Completed;
      }
      return status;
   }

   /// <summary>
   /// Get Container Root Item Async.
   /// </summary>
   /// <param name="containerId">container Id to search</param>
   /// <returns>found root item is returned</returns>
   public async Task<ItemInfo> GetContainerRootItemAsync(Guid containerId)
   {
      if (containerId == null)
      {
         return null;
      }
      return GetContainerRootItem(containerId);
   }

   /// <summary>
   /// Create container root item.
   /// </summary>
   /// <param name="containerId">(optional) container id, else the container Id
   /// of the current container is used</param>
   public virtual ItemInfo CreateRootItem(Guid? containerId = null)
   {
      Guid cid = containerId == null ? 
         _Client.CurrentContainer.Id : containerId.Value;
      var root = GetContainerRootItem(cid);

      ItemInfo item = null;
      if (root == null)
      {
         item = CreateBranch(CatalogBaseClient.ROOT_ID, "root", cid);
         item.ContainerId = cid;
         item.FullPath = CatalogBaseClient.ROOT_PATH;

         DbContext.Items.Add(item);
         DbContext.SaveChanges();
      }
      else
      {
         item = root;
      }
      return item;
   }

   /// <summary>
   /// Create Branch.
   /// </summary>
   /// <param name="name">name of branch</param>
   /// <param name="description">description</param>
   /// <param name="containerId">container id [default: CurrentContainer.Id]
   /// </param>
   /// <returns>file item instance is returned</returns>
   public virtual ItemInfo CreateBranch(
      string name, string? description = null, Guid? containerId = null)
   {
      var desc = description ?? name;
      var item = new ItemInfo();

      if (containerId == null)
      {
         item.ContainerId = _Client.CurrentContainer.Id;
         item.Container = _Client.CurrentContainer;
      }
      else
      {
         var container = _Client.Container.GetContainer(containerId.Value);
         item.Container = container;
         item.ContainerId = containerId.Value;
      }

      item.ItemType = DataObjects.Trees.TreeItemType.Branch;
      item.Name = name;
      item.Description = desc;
      item.FullPath = String.Empty;
      return item;
   }

   /// <summary>
   /// Create Branch async.
   /// </summary>
   /// <param name="name">name of branch</param>
   /// <param name="description">description</param>
   /// <param name="containerId">container id [default: CurrentContainer.Id]
   /// </param>
   /// <returns>file item instance is returned</returns>
   public async Task<ItemInfo> CreateBranchAsync(
      string name, string? description = null, Guid? containerId = null)
   {
      return CreateBranch(name, description, containerId);
   }

   /// <summary>
   /// Create/Update Leaf.
   /// </summary>
   /// <param name="path">full path</param>
   /// <param name="name">name of branch</param>
   /// <param name="description">description</param>
   /// <returns>file item instance is returned</returns>
   public virtual ItemInfo CreateLeaf(string path, string name,
      Guid? id = null, string? description = null, string? dataValue = null)
   {
      if (String.IsNullOrWhiteSpace(path))
      {
         path = "//Archive";
      }

      var desc = description ?? name;
      var item = new ItemInfo();

      item.Id = id ?? item.Id;
      item.ContainerId = _Client.CurrentContainer.Id;
      item.Container = _Client.CurrentContainer;

      item.ItemType = DataObjects.Trees.TreeItemType.Leaf;
      item.FullPath = path;
      item.Name = name;
      item.Description = desc;
      item.FullPath = String.Empty;

      if (dataValue != null)
      {
         _Client.ItemData.CreateDataLeaf(item, name, null, dataValue);
      }

      return item;
   }

   #endregion

}
