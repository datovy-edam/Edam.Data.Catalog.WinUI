using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Edam.Data.CatalogModel;
using Edam.Data.CatalogDb;
using System.Collections.ObjectModel;
using Edam.UI.Catalog.Models;
using Edam.Diagnostics;
using Edam.Application;
using Edam.DataObjects.Trees;
using Microsoft.UI.Xaml.Input;

// -----------------------------------------------------------------------------

namespace Edam.UI.Catalog.Controls;

public class CatalogExplorerViewModel : ObservableObject
{

    //private string? _defaultConnectionString;
    //private INavigator _navigator;

    public CatalogViewModel CatalogBase { get; set; }

    public ObservableCollection<CatalogItem> DataSource { get; set; } =
        new ObservableCollection<CatalogItem>();

    public ItemContentNotificationAsync NotifyEventAsync = null;

    /// <summary>
    /// Initialize Catalog
    /// </summary>
    public async Task InitializeCatalogAsync(AppModelState state)
    {
        await CatalogBase.GetCatalogAsync(state);

        // for some reason if you Clear the collection it throw an exception
        if (DataSource.Count > 0)
        {
            DataSource.Clear();
        }

        // get root element observable item
        CatalogBase.RootItem = GetData(CatalogBase.Catalog.RootTreeItem);

        // don't show root item so add first level items (the children)
        foreach (var itm in CatalogBase.RootItem.Children)
        {
            DataSource.Add(itm);
        }

        if (CatalogBase.NotifyEvent != null)
        {
            var args = new NotificationEventArgs
            {
                Results = new ResultLog(),
                EventID = CatalogViewModel.CATALOG_INITIALIZED,
                Data = CatalogBase.Catalog
            };
            args.Results.Succeeded();
            CatalogBase.NotifyEvent(this, args);
        }
    }

    /// <summary>
    /// Given a Catalog Item build corresponding observable item...
    /// </summary>
    /// <param name="item">item to go through children and build tree</param>
    /// <returns>observable item</returns>
    public CatalogItem GetData(CatalogItemInfo item)
    {
        CatalogItem itm = new CatalogItem()
        {
            Name = item.Name,
            Item = item,
            ItemType = item.Type,
        };

        foreach(var node in item.Children)
        {
            itm.Children.Add(GetData(node));
        }
        return itm;
    }

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

    public async Task SetEditorTextContent(CatalogItem item)
    {
        if (item != null)
        {
            var citem = item as CatalogItem;
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

/// <summary>
/// Observable Item
/// </summary>
public class CatalogItem
{
    public string Name { get; set; }
    public TreeItemType ItemType { get; set; }
    public ObservableCollection<CatalogItem> Children { get; set; } = 
        new ObservableCollection<CatalogItem>();
    public CatalogItemInfo Item { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
