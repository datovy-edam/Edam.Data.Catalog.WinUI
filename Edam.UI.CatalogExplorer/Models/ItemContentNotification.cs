using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Data.CatalogModel;
using Edam.Diagnostics;
using Edam.UI.Catalog.Controls;

namespace Edam.UI.Catalog.Models;

public enum ItemContentNotificationType
{
    Unknown = 0,
    SetContent = 1,
    GetContent = 2,
}

public class ItemContentNotificationArgs
{
    public CatalogViewModel? Catalog { get; set; } = null;
    public IResultsLog Results { get; set; } = new ResultLog();
    public ItemContentNotificationType Type { get; set; } =
        ItemContentNotificationType.Unknown;
    public IItemContent? ItemContent { get; set; } = null;

    public ItemContentNotificationArgs(
        ItemContentNotificationType type, IItemContent? itemContent)
    {
        Type = type;
        ItemContent = itemContent;
    }
}

public delegate Task ItemContentNotificationAsync(
    object sender, ItemContentNotificationArgs args);

