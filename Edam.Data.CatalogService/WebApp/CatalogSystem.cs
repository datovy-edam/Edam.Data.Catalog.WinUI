using Edam.Application;
using Edam.Data.CatalogDb;
using Edam.Data.CatalogModel;
using Edam.DataObjects.Trees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

// -----------------------------------------------------------------------------
namespace Edam.Data.CatalogService;

/// <summary>
/// File System support by instantiating an ICatalogService.
/// </summary>
public class CatalogSystem
{

   // TODO: Instantite the service with depency-injection.
   private ICatalogService? _Instance;
   public ICatalogService Instance
   {
      get { return _Instance; }
   }

   private Dictionary<string, CatalogInfo> _Catalogs { get; set; } = new();
   private List<ContainerInfo> _Containers { get; set; } = new();

   #region -- 1.00 - Constructor and Initialization

   public CatalogSystem()
   {
      _Instance = GetCatalogService();
   }

   /// <summary>
   /// Initialize the Catalog Service...
   /// </summary>
   /// <returns>instance of a Catalog is returned</returns>
   private static ICatalogService? GetCatalogService()
   {
      ICatalogService? catalog = null;
      var inst = new CatalogInstance();
      string sessionId = Guid.NewGuid().ToString();
      var results = inst.GetCatalog(
         sessionId, CatalogInstance.EDAM_FILE_SYSTEM_DB);
      if (results != null)
      {
         catalog = results.Instance;
      }
      return catalog;
   }

   #endregion
   #region -- 4.00 - Container Support

   /// <summary>
   /// Get Containers List.
   /// </summary>
   /// <returns>list is returned</returns>
   public List<ContainerInfo> GetContainerList()
   {
      if (_Containers.Count == 0)
      {
         var clist = Instance.Container.GetContainers();
         foreach (var c in clist)
         {
            _Containers.Add(c);
         }
      }
      return _Containers;
   }

   /// <summary>
   /// Setup Container.
   /// </summary>
   /// <param name="sessionId"></param>
   /// <param name="containerId"></param>
   /// <returns></returns>
   public ContainerInfo? SetContainer(
      string sessionId, string containerId)
   {
      WebAppService.SetupSession(String.IsNullOrWhiteSpace(sessionId) ?
         new Guid().ToString() : sessionId);
      ContainerInfo? container = 
         _Instance.Container.SetContainer(sessionId, containerId);

      return container;
   }

   #endregion
   #region -- 4.00 - Catalog Support

   /// <summary>
   /// Get Catalog for given containerId.
   /// </summary>
   /// <param name="containerId">container Id</param>
   /// <returns>if found, CatalogInfo instance is returned else null</returns>
   public CatalogInfo? GetCatalog(string containerId)
   {
      CatalogInfo? result = null;
      if (_Catalogs.TryGetValue(containerId, out var catalog))
      {
         result = catalog;
      }
      else
      {

      }
      return result;
   }

   /// <summary>
   /// Get Catalog Service for current Container.
   /// </summary>
   /// <returns>instance of the container is returned</returns>
   public CatalogInfo? GetCatalog()
   {
      CatalogInfo? catalog = 
         GetCatalog(Instance.CurrentContainer.ContainerId);
      return catalog;
   }

   /// <summary>
   /// Get the Catalog Root Item for given containerId.
   /// </summary>
   /// <param name="containerId">container Id</param>
   /// <returns>if root item is found it is returned else null</returns>
   public CatalogItemInfo? GetCatalogRootItem(string containerId)
   {
      CatalogInfo? catalog = GetCatalog(containerId);
      CatalogItemInfo? rootItem = null;

      if (catalog != null)
      {
         rootItem = catalog.RootTreeItem;
      }

      return rootItem;
   }

   /// <summary>
   /// Get Builder...
   /// </summary>
   /// <returns>instance of builder is returned</returns>
   public CatalogTreeBuilder GetBuilder()
   {
      var catalog = GetCatalog();
      CatalogTreeBuilder builder =
         new CatalogTreeBuilder(catalog.CatalogService, catalog);
      return builder;
   }

   /// <summary>
   /// Get item for given path.
   /// </summary>
   /// <param name="path">file path that will return an item that </param>
   /// <returns>file item info is returned</returns>
   public async Task<ItemInfo?> GetItemAsync(string path)
   {
      var builder = GetBuilder();
      var pitem = await builder.GetItemAsync(path);
      return pitem.Item;
   }

   #endregion

}
