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
   /// 
   /// </summary>
   /// <param name="item"></param>
   /// <param name="builder"></param>
   /// <returns></returns>
   public static async Task<CatalogTreeBuilder> FileSystemToCatalogAsync(
      FolderFileItemInfo item, CatalogTreeBuilder? builder)
   {
      var citem = await builder.GetItemAsync(item.Full);
      foreach(var child in item.Children)
      {
          await FileSystemToCatalogAsync(child, builder);
      }
      return builder;
   }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="folderPath"></param>
   /// <param name="builder"></param>
   public static async void FileSystemToCatalogAsync(
      string folderPath, CatalogTreeBuilder? builder)
   {
      var results = GetFileSystemItems(folderPath);
      if (results.Success)
      {
         await FileSystemToCatalogAsync(results.Instance, builder);
      }
   }

}
