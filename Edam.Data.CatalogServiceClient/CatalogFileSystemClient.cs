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

   protected CatalogInfo _catalog;
   protected string _defaultRootFileFolder;
   protected FolderFileItemInfo? _RootItem;
   public ItemInfo RootItem
   {
      get { return null; }
   }

   #endregion
   #region -- 1.50 - Constructure and Initialization

   public CatalogFileSystemClient(
      string sessionId, string baseUri, ICatalogContainer container) : 
      base(sessionId, baseUri)
   {
      if (String.IsNullOrWhiteSpace(baseUri))
      {
         _defaultRootFileFolder = AppSettings.GetSectionString(
            "DefaultRootFileFolder", AppSettings.APP_SETTINGS_SECTION_KEY);
         if (!String.IsNullOrWhiteSpace(_defaultRootFileFolder))
         {
            _defaultRootFileFolder = _defaultRootFileFolder.Replace("\\", "/");
         }
      }

      // use given container management instance...
      Container = container;
      Item = new CatalogFileSystemItem(this);
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
      CatalogTreeBuilder builder = new CatalogTreeBuilder(this, _catalog);
      _builder = await CatalogFileSystem.FileSystemToCatalogAsync(
         baseUri, builder);

   }

   #endregion

}
