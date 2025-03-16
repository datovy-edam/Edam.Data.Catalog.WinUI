using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogModel;
using Edam.Data.CatalogDb;
using Edam.UI.Catalog.Models;
using Edam.UI.CatalogExplorer;

namespace Edam.UI.Catalog.Controls;

public class CatalogViewModel
{

   public const string CATALOG_INITIALIZED = "CATALOG-INITIALIZED";

   public AppModelState State = null;
   public bool HasCatalog = false;

   public CatalogInfo? Catalog = null;
   public CatalogItem RootItem = null;

   public NotificationEventHandler NotifyEvent { get; set; }

   /// <summary>
   /// Initialize Catalog
   /// </summary>
   public async Task GetCatalogAsync(AppModelState state)
   {
      string connectionUri = state.GetConnectionUri();

      Catalog = await CatalogServiceHelper.GetCatalogAsync(
          connectionUri);

   }

   /// <summary>
   /// Post Item.
   /// </summary>
   /// <param name="path"></param>
   /// <param name="payload"></param>
   /// <returns></returns>
   public async Task<ItemDataInfo> PostItemAsync(string path, byte[] payload)
   {
      ItemDataInfo? itemData = null;
      CatalogTreeBuilder builder =
          new CatalogTreeBuilder(Catalog.CatalogService, Catalog);
      var item = await builder.GetItemAsync(path);
      if (payload != null && payload.Length > 0)
      {
         itemData = item.ToItemData(payload);
         var rItem =
            await Catalog.CatalogService.ItemData.AddItemAsync(itemData);
      }
      return itemData;
   }

   /// <summary>
   /// Post Item.
   /// </summary>
   /// <param name="path"></param>
   /// <param name="payload"></param>
   /// <returns></returns>
   public async Task<ItemDataInfo> PostItemAsync(string path, string payload)
   {
      byte[] bytes = Encoding.ASCII.GetBytes(payload);
      return await PostItemAsync(path, bytes);
   }

   /// <summary>
   /// Get Item Data.
   /// </summary>
   /// <param name="item">item instance of CatalogItem</param>
   /// <returns>ItemDataInfo instance is returned</returns>
   public async Task<List<ItemDataInfo>> GetItemDataAsync(CatalogItem item)
   {
      CatalogPathItem pitem = item.Item.Tag as CatalogPathItem;
      var idata =
         await Catalog.CatalogService.ItemData.GetItemDataAsync(pitem.Item.Id);
      return idata;
   }

}
