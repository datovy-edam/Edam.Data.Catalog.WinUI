using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Edam.Data.CatalogModel;
using System.Collections.ObjectModel;
using Edam.UI.Catalog.Models;
using Edam.Diagnostics;

// -----------------------------------------------------------------------------

namespace Edam.UI.Catalog.Controls;

/// <summary>
/// Catalog Explorer pannel view-model support...
/// </summary>
public class CatalogExplorerViewModel : ObservableObject
{

   //private string? _defaultConnectionString;
   //private INavigator _navigator;

   public CatalogViewModel CatalogBase { get; set; }

   public ObservableCollection<CatalogItemModel> DataSource { get; set; } =
       new ObservableCollection<CatalogItemModel>();

   public ItemContentNotificationAsync NotifyEventAsync = null;

   /// <summary>
   /// Setup Catalog... that will create a tree structured using an
   /// observable collection that is passed to a Tree-Viewer.  While 
   /// switching from container to container the root element of the
   /// container and related tree - items / children are used.
   /// </summary>
   public void SetupCatalogAsync(CatalogInfo? catalog)
   {
      // check that we have a valid catalog...
      if (catalog == null)
      {
         var ex = new ArgumentNullException(nameof(catalog));
         ResultLog.Trace(ex, "CatalogExplorerViewModel::SetupCatalogAsync");
         return;
      }

      // for some reason if you Clear the collection it throw an exception
      if (DataSource.Count > 0)
      {
         DataSource.Clear();
      }

      // get root element observable item
      CatalogBase.RootItem = ToCatalogItem(catalog.RootTreeItem);

      // don't show root item so add first level items (the children)
      foreach (var itm in CatalogBase.RootItem.Children)
      {
         DataSource.Add(itm);
      }

      // notify that a new Catalog - tree has been initialized...
      if (CatalogBase.NotifyEvent != null)
      {
         var args = new NotificationEventArgs
         {
            Results = new ResultLog(),
            EventID = CatalogViewModel.CATALOG_INITIALIZED,
            Data = catalog
         };
         args.Results.Succeeded();
         CatalogBase.NotifyEvent(this, args);
      }
   }

   /// <summary>
   /// Initialize Catalog Async...
   /// </summary>
   public async Task InitializeCatalogAsync(AppModelState state)
   {
      await CatalogBase.GetCatalogAsync(state);
      SetupCatalogAsync(CatalogBase.Catalog);
   }

   /// <summary>
   /// Container Change is reported... update explorer accordingly.
   /// </summary>
   /// <param name="item">container instance</param>
   /// <returns>Task is returned</returns>
   public void InitializeCatalogAsync(ContainerItem item)
   {
      SetupCatalogAsync(item.Catalog);
   }

   /// <summary>
   /// Given a Catalog Item build corresponding observable item...
   /// </summary>
   /// <param name="item">item to go through children and build tree</param>
   /// <returns>observable item</returns>
   public CatalogItemModel ToCatalogItem(CatalogItemInfo item)
   {
      CatalogItemModel itm = new CatalogItemModel()
      {
         Name = item.Name,
         Item = item,
         ItemType = item.Type,
      };

      foreach (var node in item.Children)
      {
         itm.Children.Add(ToCatalogItem(node));
      }
      return itm;
   }

   /// <summary>
   /// Notify Event.
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   private async Task NotifyEvent(IItemContent item)
   {
      if (NotifyEventAsync != null)
      {
         ItemContentNotificationArgs args =
             new ItemContentNotificationArgs(
                 ItemContentNotificationType.SetContent, item);
         args.Catalog = CatalogBase;
         await NotifyEventAsync(this, args);
      }
   }

   /// <summary>
   /// Set Editor Text Content...
   /// </summary>
   /// <param name="item"></param>
   /// <returns></returns>
   public async Task SetEditorTextContent(CatalogItemModel? item)
   {
      if (item != null)
      {
         var citem = item as CatalogItemModel;
         var items = await CatalogBase.GetItemDataAsync(citem);
         var data = items != null && items.Count > 0 ? items[0] : null;
         if (data != null)
         {
            ItemContent icontent = new ItemContent
            {
               Item = citem.Item,
               Content = data.DataText
            };
            NotifyEvent(icontent as IItemContent);
         }
      }
   }

}
