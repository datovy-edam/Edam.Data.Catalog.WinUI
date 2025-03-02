using Edam.Data.CatalogModel;
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
   /// 
   /// </summary>
   /// <param name="sessionId"></param>
   /// <param name="containerId"></param>
   /// <returns></returns>
   public override ContainerInfo? SetContainer(
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
      return base.SetContainer(sessionId, containerId);
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
      var itm = await base.AddItemAsync(pitem.Item);
      return itm;
   }

   #endregion

}
