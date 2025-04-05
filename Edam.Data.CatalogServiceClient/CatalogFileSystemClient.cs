using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.InOut;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogDb;
using Edam.Data.CatalogServiceClient;

namespace Edam.Data.CatalogService;

public class CatalogFileSystemClient : CatalogBaseClient, ICatalogClient, 
   ICatalogService
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

   #endregion
   #region -- 1.50 - Constructure and Initialization

   public CatalogFileSystemClient(string sessionId, string baseUri) :
      base(sessionId, baseUri)
   {
      _lastSessionId = sessionId;
      _BaseURI = baseUri;
   }

   /// <summary>
   /// Initialize Client.
   /// </summary>
   /// <param name="container"></param>
   public async Task InitializeClientAsync(ICatalogContainer container)
   { 
      if (String.IsNullOrWhiteSpace(_BaseURI))
      {
         _defaultRootFileFolder = AppSettings.GetSectionString(
            "DefaultRootFileFolder", AppSettings.APP_SETTINGS_SECTION_KEY);
         if (!String.IsNullOrWhiteSpace(_defaultRootFileFolder))
         {
            _defaultRootFileFolder = _defaultRootFileFolder.Replace("\\", "/");
         }
      }
      else
      {
         _defaultRootFileFolder = _BaseURI;
      }

      // try to find a container based on this base URI...
      var cnts = container.GetContainers();
      var fcontainer = cnts.Find(
         (x) => x.ContainerURI == _defaultRootFileFolder);
      if (fcontainer == null)
      {
         // create a new container for given URI
         fcontainer = container.EnlistContainer(
            "root-folder", FILE_SYSTEM + " Container",
            _defaultRootFileFolder);
      }
      else
      {
         container.SetContainer(_lastSessionId, fcontainer.ContainerId);
      }

      CurrentContainer = fcontainer;

      // use given container management instance...
      Container = container;
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
      _catalog = new CatalogInfo(this, String.Empty);
      _catalog.RootPathItem = _rootItem;
      CatalogTreeBuilder builder = new CatalogTreeBuilder(this, _catalog);
      builder.RegisterRootItem(_rootItem);
      Cataloger = builder;
      _builder = await CatalogFileSystem.FileSystemToCatalogAsync(
         baseUri, builder);
   }

   #endregion

}
