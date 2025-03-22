using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Requests;
using Edam.Diagnostics;
using Edam.InOut;
using Edam.Net;
using Edam.Net.Web;
using Edam.Text;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogDb;
using Edam.Data.CatalogServiceClient;

namespace Edam.Data.CatalogService;

public class CatalogFileSystemClient :
   CatalogBaseClient, ICatalogClient, ICatalogService
{

   #region -- 1.00 - Fields and Properties declration/definitions

   public const string FILE_SYSTEM = "File System";

   protected CatalogInfo _catalog;
   protected string _defaultRootFileFolder;
   protected CatalogPathItem _rootItem;
   protected FolderFileItemInfo? _RootItem;

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

   public CatalogFileSystemClient(
      string sessionId, string baseUri, ICatalogContainer container) : 
      base(sessionId, baseUri)
   {
      _BaseURI = baseUri;
      if (String.IsNullOrWhiteSpace(baseUri))
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
         _defaultRootFileFolder = baseUri;
      }

      // try to find a container based on this base URI...
      var cnts = container.GetContainers();
      var fcontainer = cnts.Find(
         (x) => x.ContainerURI == _defaultRootFileFolder);
      if (fcontainer == null)
      {
         // create a new container for given URI
         fcontainer = container.EnlistContainer(
            Guid.NewGuid().ToString(), FILE_SYSTEM + " Container",
            _defaultRootFileFolder);
      }
      else
      {
         container.SetContainer(sessionId, fcontainer.ContainerId);
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

      InitializeFileItems(_defaultRootFileFolder);
   }

   /// <summary>
   /// Read the folder files and put them into a CatalogTreeBuilder dictionary
   /// by going through all folder - files and child folders children.
   /// </summary>
   /// <param name="baseUri">that should be a folder full path name</param>
   public async void InitializeFileItems(string baseUri)
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
