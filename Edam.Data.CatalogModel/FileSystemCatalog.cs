using Edam.Diagnostics;
using Edam.InOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogModel;


public class FileSystemCatalog
{

   /// <summary>
   /// Get a single project file/folder artifacts...
   /// </summary>
   /// <param name="folderPath">name of project</param>
   /// <returns>instance of FolderFileItemInfo is returned with requested 
   /// project and file/folder artifacts</returns>
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

}
