using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Edam.Data.CatalogModel;
using Edam.UI.Catalog.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Edam.UI.Catalog.Controls;
public sealed partial class CatalogExplorerControl : UserControl
{
    private object _lastSelected = null;
    private CatalogExplorerViewModel _ViewModel =
        new CatalogExplorerViewModel();
    public CatalogExplorerViewModel ViewModel
    {
        get { return _ViewModel; }
    }
    public CatalogExplorerControl()
    {
        this.InitializeComponent();
        DataContext = _ViewModel;

    }

    private void TreeView_SelectionChanged(
        TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        _lastSelected = args.AddedItems.Count > 0 ?
            args.AddedItems.First() : null;
    }

    private void TreeView_DoubleTapped(
        object sender, DoubleTappedRoutedEventArgs e)
    {
        ViewModel.SetEditorTextContent(_lastSelected as CatalogItem);
    }

    private async void UploadFile_Click(object sender, PointerRoutedEventArgs e)
    {
        var file = await AppSession.GetFileAsync();
    }

    private async void UploadFolder_Click(object sender, PointerRoutedEventArgs e)
    {
        //var folder = await AppSession.GetFolderAsync();
        var catFolder = new CatalogFolder(_ViewModel.CatalogBase);
        await catFolder.FolderToCatalogAsync();
    }
}

public class ExplorerItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate FolderTemplate { get; set; }
    public DataTemplate FileTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        var explorerItem = (CatalogItem)item;
        if (explorerItem.ItemType == DataObjects.Trees.TreeItemType.Branch)
            return FolderTemplate;

        return FileTemplate;
    }
}
