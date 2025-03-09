using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.DataObjects.Medias;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogDb;


/// <summary>
/// Support for Catalog/File System repository inqueries and requests.  Any 
/// request will be processed first by 
/// </summary>
public class CatalogBuilderServiceInstance : 
   CatalogServiceInstance, ICatalogService
{

   private string _SessionId = null;
   public string SessionId
   {
      get { return _SessionId; }
   }

   private CatalogInfo _Catalog;
   private CatalogTreeBuilder _Builder;

   public CatalogInfo Catalog
   { get { return _Catalog; } }
   public CatalogTreeBuilder Builder
   {  get { return _Builder; } }

   public CatalogBuilderServiceInstance(string? defaultConnectionString) :
      base(defaultConnectionString)
   {
      InitializeDbContext();
   }

   #region -- 1.00 - Constructor and Initialization

   /// <summary>
   /// Initialize Repository
   /// </summary>
   protected void InitializeDbContext()
   {
      var connectionString =
         String.IsNullOrWhiteSpace(_defaultConnectionString) ?
            AppSettings.GetConnectionString("catalogDb") :
            _defaultConnectionString;

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
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="sessionId"></param>
   /// <param name="containerId"></param>
   /// <returns></returns>
   public ContainerInfo? SetContainer(
      string sessionId, string containerId)
   {
      if (_SessionId == null)
      {
         _SessionId = sessionId;

         _Catalog = new CatalogInfo(this, _SessionId);
         _Catalog.InitializeCatalogAsync("", false);

         _Builder = new CatalogTreeBuilder(this, Catalog);
      }
      else
      {
         sessionId = _SessionId;
      }
      return base.Container.SetContainer(sessionId, containerId);
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
      var pitem = await Builder.GetItemAsync(item);
      var itm = await base.Item.AddItemAsync(pitem.Item);
      return itm;
   }

   #endregion

}
