using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Edam.Data.CatalogModel;
using Edam.Diagnostics;
using Edam.UI.Catalog.Controls;

namespace Edam.UI.Catalog.Models;

public class CatalogFolder
{

    private CatalogViewModel _Catalog;
    private StorageFolder _Root;

    public CatalogFolder(CatalogViewModel catalogInfo)
    {
        _Catalog = catalogInfo;
    }

    /// <summary>
    /// Get File.
    /// </summary>
    /// <param name="file">Storage File details</param>
    public async Task LoadFile(StorageFile file, string path)
    {
        // var props = await file.GetBasicPropertiesAsync();
        var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
        using (var dataReader = Windows.Storage.Streams.
            DataReader.FromBuffer(buffer))
        {
            byte[] b = new byte[buffer.Length];
            dataReader.ReadBytes(b);
            await _Catalog.PostItemAsync(path, b);
        }
    }

    /// <summary>
    /// Get Folder/Files Items and add those to the Catalog.
    /// </summary>
    /// <param name="items"></param>
    private async Task LoadItems(IReadOnlyList<IStorageItem> items, string path)
    {
        foreach (var item in items)
        {
            if (item is StorageFolder)
            {
                StorageFolder folder = (StorageFolder)item;
                var fitems = await folder.GetItemsAsync();
                await LoadItems(fitems, path + "/" + folder.Name);
            }
            else if (item is StorageFile)
            {
                await LoadFile((StorageFile)item, path + "/" + item.Name);
            }
        }
    }

    /// <summary>
    /// (file system storage) Folder to Catalog.
    /// </summary>
    /// <param name="catalog">catalog</param>
    /// <returns></returns>
    public async Task<ResultsLog<bool>> FolderToCatalogAsync()
    {
        ResultsLog<bool> resultsLog = new ResultsLog<bool>();
        StorageFolder storageFolder = null;
        try
        {
            _Root = await AppSession.GetFolderAsync();
            var items = await _Root.GetItemsAsync();
            await LoadItems(items, "/" + _Root.Name);
            resultsLog.Succeeded();
        }
        catch (Exception ex)
        {
            resultsLog.Failed(ex);
        }
        return resultsLog;
    }

}
