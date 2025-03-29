using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogService;

namespace Edam.Test.TestCatalogLibrary;

public class CatalogFileFolderClient
{

   /// <summary>
   /// Get Client.
   /// </summary>
   /// <param name="path"></param>
   /// <returns></returns>
   public static CatalogFileSystemClient GetClient(string path)
   {
      var container = AppHelper.CatalogInstance.Container;
      return new CatalogFileSystemClient(
         Guid.NewGuid().ToString(), path, container);
   }

}
