using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Data.CatalogModel;

namespace Edam.UI.Catalog.Models;

public class ItemContent : IItemContent
{

    private CatalogItemInfo? _catalogItem;

    public object? ItemInstance
    {
        get { return _catalogItem; }
        set { _catalogItem = value as CatalogItemInfo; }
    }

    public string Content { get; set; } = string.Empty;

    public CatalogItemInfo? Item
    {
        get { return _catalogItem; }
        set { _catalogItem = value; }
    }

}
