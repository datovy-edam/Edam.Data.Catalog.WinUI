using Edam.Diagnostics;
using Edam.InOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogModel;

/// <summary>
/// Support management of File System catalogs.
/// </summary>
public class CatalogFileSystem
{

   /// <summary>
   /// Get all folder - file artifacts...
   /// </summary>
   /// <param name="folderPath">folder path</param>
   /// <returns>instance of log-results with inner FolderFileItemInfo is 
   /// returned with requested project and file/folder artifacts</returns>
   public static ResultsLog<FolderFileItemInfo> GetFileSystemItems(
      string folderPath)
   {
      ResultsLog<FolderFileItemInfo?> rlog = 
         new ResultsLog<FolderFileItemInfo?>();
      string cdir = Directory.GetCurrentDirectory();

      FolderFileItemInfo finfo = null;
      try
      {
         finfo = FolderFileReader.GetFolderFileInfo(folderPath);
         rlog.Instance = finfo;
         rlog.Succeeded();
      }
      catch (Exception ex)
      {
         rlog.Failed(ex);
      }
      finally
      {
         Directory.SetCurrentDirectory(cdir);
      }

      return rlog;
   }

   /// <summary>
   /// Prepare builder asynchrolously by visiting each File System entry
   /// and inserting it into the builder internal dictionary.
   /// </summary>
   /// <param name="item">root file system folder item</param>
   /// <param name="builder">builder instance</param>
   /// <returns>preapred builder instance is returned</returns>
   public static async Task<CatalogTreeBuilder> PrepareBuilderAsync(
      FolderFileItemInfo item, CatalogTreeBuilder? builder)
   {
      var citem = await builder.GetItemAsync(item.Full);
      foreach(var child in item.Children)
      {
          await PrepareBuilderAsync(child, builder);
      }
      return builder;
   }

   /// <summary>
   /// Read File System from given start folder path and store results in
   /// a CatalogTreeBuilder asynchronously.
   /// </summary>
   /// <param name="folderPath">root folder path</param>
   /// <param name="builder">builder instance, if null a new one will be
   /// created</param>
   /// <returns>instance of CatalogTreeBuilder is returned</returns>
   public static async Task<CatalogTreeBuilder?> FileSystemToCatalogAsync(
      string folderPath, CatalogTreeBuilder? builder = null)
   {
      CatalogTreeBuilder? tbuilder = builder ?? null;
      if (tbuilder != null)
      {
         var results = GetFileSystemItems(folderPath);
         if (results.Success)
         {
            await PrepareBuilderAsync(results.Instance, tbuilder);
         }
      }
      return tbuilder;
   }

}
