using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.DataObjects.Medias;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogDb;


/// <summary>
/// Support for Catalog EF based repository inqueries and requests.
/// </summary>
public class CatalogBuilderServiceInstance : 
   CatalogServiceInstance, ICatalogService
{

   public CatalogBuilderServiceInstance(string? defaultConnectionString) :
      base(defaultConnectionString)
   {
      InitializeDbContext();
   }

   #region -- 1.00 - Constructor and Initialization

   /// <summary>
   /// Initialize Repository
   /// </summary>
   protected async void InitializeDbContext()
   {
      var connectionString =
         String.IsNullOrWhiteSpace(_defaultConnectionString) ?
            AppSettings.GetConnectionString("catalogDb") :
            _defaultConnectionString;
      if (_defaultConnectionString == null)
      {
         _defaultConnectionString = connectionString;
      }

      // get DbContext
      DbContext = new CatalogContext(connectionString);
      if (!DbContext.Database.CanConnect())
      {
         try
         {
            DbContext.Database.EnsureCreated();
         }
         catch (Exception ex)
         {

         }
      }

      // using DbContext initialize other instance objects
      Container = new CatalogContainer(this, DbContext);
      Item = new CatalogItem(this, DbContext);
      ItemData = new CatalogItemData(this, DbContext);

      // add content-types as needed
      if (!DbContext.ContentTypes.Any())
      {
         var types = new ContentTypeInfo[]
         {
         new ContentTypeInfo(MediaContentTypeHelper.JSONLD, "json-ld document"),
         new ContentTypeInfo(MediaContentTypeHelper.JsonDocument,
            "json document"),
         new ContentTypeInfo(MediaContentTypeHelper.XmlDocument, "xml text"),
         new ContentTypeInfo(MediaContentTypeHelper.TextFile, "text document"),
         new ContentTypeInfo(MediaContentTypeHelper.OfficeExcelXmlFile,
            "excel open xml document"),
         new ContentTypeInfo(MediaContentTypeHelper.JAVASCRIPT,
            "javascript document")
         };
         foreach (var type in types)
         {
            DbContext.ContentTypes.Add(type);
         }
         DbContext.SaveChanges();
      }

      // define default container
      if (!DbContext.Containers.Any())
      {
         DefaultContainer = new ContainerInfo();
         DbContext.Containers.Add(DefaultContainer);
         DbContext.SaveChanges();
      }
      else
      {
         DefaultContainer = Container.GetContainer(null);
      }
      CurrentContainer = DefaultContainer;

      // define default container root item
      if (!DbContext.Items.Any())
      {
         Item.CreateRootItem();
      }

      // initialize catalog and builder

      _Catalog = new CatalogInfo(this, this, _SessionId);
      await _Catalog.InitializeCatalogAsync("", false);

      _builder = new CatalogTreeBuilder(this, _Catalog);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="containerId"></param>
   /// <returns></returns>
   public ContainerInfo? SetContainer(string containerId)
   {
      return base.Container.SetContainer(_SessionId, containerId);
   }

   #endregion
   #region -- 4.00 - File Item Branch - Leaf Support

   /// <summary>
   /// Add Item by building path itesm (branches and leaf).
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   public async Task<ItemInfo> AddItemAsync(ItemInfo item)
   {
      var pitem = await _builder.GetItemAsync(item);
      var itm = await base.Item.AddItemAsync(pitem.Item);
      return itm;
   }

   #endregion

}
