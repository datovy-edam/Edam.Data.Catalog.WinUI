using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.InOut;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogDb;
using Edam.Data.CatalogServiceClient;

namespace Edam.Data.CatalogService;

public class CatalogFileSystemClient : CatalogBaseClient, ICatalogClient, 
   ICatalogService, ICatalogBaseClient
{

   #region -- 1.00 - Fields and Properties declration/definitions

   public const string FILE_SYSTEM = "File System";

   protected CatalogInfo _catalog;
   protected string _defaultRootFileFolder;
   protected CatalogPathItem _rootItem;
   protected FolderFileItemInfo? _RootItem;

   public CatalogInfo Catalog
   {
      get { return _catalog; }
   }

   public object Instance
   {
      get { return this; }
   }

   public ItemInfo RootItem
   {
      get { return _catalog.RootItem; }
   }

   public string DefaultRootFileFolder
   {
      get { return _defaultRootFileFolder; }
   }

   #endregion
   #region -- 1.50 - Constructure and Initialization

   /// <summary>
   /// Initialize file system catalog instance and container using given ID.
   /// </summary>
   /// <param name="sessionId"></param>
   /// <param name="defaultContainerId">(required) default container id</param>
   /// <param name="baseUri">base uri for given default container</param>
   public CatalogFileSystemClient(
      string sessionId, string defaultContainerId, string? baseUri = null) :
      base(sessionId, baseUri)
   {
      _SessionId = sessionId;
      _BaseURI = baseUri;
      DefaultContainerId = defaultContainerId;
   }

   /// <summary>
   /// Get base URI.
   /// </summary>
   /// <param name="baseUri">base uri (default: null)</param>
   /// <returns>the uri is returned</returns>
   public static string? GetBaseURI(string? baseUri = null)
   {
      string? uri = null;

      if (String.IsNullOrWhiteSpace(baseUri))
      {
         uri = AppSettings.GetSectionString(
            "DefaultRootFileFolder", AppSettings.APP_SETTINGS_SECTION_KEY);
         if (!String.IsNullOrWhiteSpace(uri))
         {
            uri = uri.Replace("\\", "/");
         }
      }
      else
      {
         uri = baseUri;
      }
      return uri;
   }

   /// <summary>
   /// Initialize Client.
   /// </summary>
   /// <param name="catalogContainer">default catalog container</param>
   public async Task InitializeClientAsync(
      ICatalogContainer? catalogContainer = null)
   {
      _defaultRootFileFolder = GetBaseURI(_BaseURI);

      // get the default catalog container...
      var containerService = catalogContainer;
      if (containerService == null)
      {
         containerService = CatalogServiceInstance.DefaultInstance.Container;
      }

      // try to find a container based on this base URI...
      var cnts = containerService.GetContainers();

      ContainerInfo fcontainer = cnts.Find(
         (x) => x.ContainerId == DefaultContainerId);

      // try finding the catalog base on its base URI or path
      if (fcontainer == null)
      {
         fcontainer = cnts.Find(
            (x) => x.ContainerURI == _defaultRootFileFolder);
      }

      if (fcontainer == null)
      {
         // create a new container for given URI
         fcontainer = containerService.EnlistContainer(
            DefaultContainerId, FILE_SYSTEM + " Container",
            _defaultRootFileFolder);
      }
      else
      {
         containerService.SetContainer(_SessionId, fcontainer.ContainerId);
      }

      CurrentContainer = fcontainer;

      // use given container management instance...
      Container = containerService;
      Item = new CatalogFileSystemItem(this);

      // get/create root item
      ItemInfo rootItem = new()
      {
         ContainerId = CurrentContainer.Id,
         Id = Guid.NewGuid(),
         Container = CurrentContainer,
         Description = FILE_SYSTEM + " Entry",
         FullPath = _defaultRootFileFolder,
         ItemType = DataObjects.Trees.TreeItemType.Branch
      };
      rootItem.Name = rootItem.Id.ToString();

      //var _catalog = new CatalogItem(this, this._builder.);
      //_catalog.Add(this.RootTreeItem);
      _rootItem = new CatalogPathItem(rootItem);
      _rootItem.TreeItem = new CatalogItemInfo();
      _rootItem.TreeItem.Name = "(root)";
      _rootItem.TreeItem.Type = DataObjects.Trees.TreeItemType.Branch;

      // finally, setup Item Data
      ItemData = new CatalogFileSystemItemData(this);

      await InitializeFileItems(_defaultRootFileFolder);
   }

   /// <summary>
   /// Read the folder files and put them into a CatalogTreeBuilder dictionary
   /// by going through all folder - files and child folders children.
   /// </summary>
   /// <param name="baseUri">that should be a folder full path name</param>
   public async Task InitializeFileItems(string baseUri)
   {
      _catalog = new CatalogInfo(
         CatalogServiceInstance.DefaultInstance, this, String.Empty);
      _catalog.RootPathItem = _rootItem;
      CatalogTreeBuilder builder = new CatalogTreeBuilder(this, _catalog);
      builder.RegisterRootItem(_rootItem);
      Cataloger = builder;
      _builder = await CatalogFileSystem.FileSystemToCatalogAsync(
         baseUri, builder);
   }

   #endregion
   #region 4.00 - Catalog File System Client Support

   /// <summary>
   /// Get Client.
   /// </summary>
   /// <param name="defaultContainerId">default container-id</param>
   /// <param name="path">if null the base URI should be defined in the </param>
   /// <returns>instance of CatalogFileSystemClient is returned</returns>
   public static async Task<CatalogFileSystemClient> GetClientAsync(
      string defaultContainerId, string? path = null)
   {
      ICatalogContainer? container = null; // AppHelper.CatalogInstance.Container;
      var client = new CatalogFileSystemClient(
         sessionId: Guid.NewGuid().ToString(),
         defaultContainerId: defaultContainerId,
         baseUri: path);
      await client.InitializeClientAsync(container);
      return client;
   }

   /// <summary>
   /// Get File System Catalog Client.
   /// </summary>
   /// <param name="fileSystemPath">if null the base URI should be defined 
   /// in the app-settings file.</param>
   /// <returns>instance of CatalogFileSystemClient is returned</returns>
   public static CatalogFileSystemClient GetClient(
      string defaultContainerId, string? fileSystemPath = null)
   {
      var task = GetClientAsync(
         defaultContainerId: defaultContainerId,
         path: fileSystemPath);
      task.Wait();
      return task.Result;
   }

   #endregion

}
