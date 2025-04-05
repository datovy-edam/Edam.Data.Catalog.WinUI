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
   public static async Task<CatalogFileSystemClient> GetClientAsync(string path)
   {
      var container = AppHelper.CatalogInstance.Container;
      var client = new CatalogFileSystemClient(Guid.NewGuid().ToString(), path);
      await client.InitializeClientAsync(container);
      return client;
   }

}
