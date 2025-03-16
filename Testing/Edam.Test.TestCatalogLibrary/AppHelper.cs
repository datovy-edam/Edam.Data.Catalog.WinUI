

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Data.CatalogDb;

namespace Edam.Test.TestCatalogLibrary;

public class AppHelper
{

   private static CatalogServiceInstance _catalogInstance;
   public static CatalogServiceInstance CatalogInstance
   {
      get { return _catalogInstance; }
   }

   public static void InitializeTest()
   {
      if (_catalogInstance != null)
      {
         return;
      }

      _catalogInstance = new CatalogBuilderServiceInstance(null);
   }

}

