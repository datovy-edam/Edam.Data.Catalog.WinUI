using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogModel;
using Edam.DataObjects.Entities;
using Edam.Diagnostics;

namespace Edam.Data.CatalogDb;

public class CatalogInstance : ICatalogs
{
   public const string EDAM_FILE_SYSTEM_DB = "edam.file.system.db";

   private static string _CatalogName = EDAM_FILE_SYSTEM_DB;

   public string GetCurrentCatalogName()
   {
      return _CatalogName;
   }

   public string GetDefaultCatalogName()
   {
      return EDAM_FILE_SYSTEM_DB;
   }

   /// <summary>
   /// Get an instance of a Catalog Service by name.
   /// </summary>
   /// <param name="sessionId">session ID</param>
   /// <param name="invariantName">name of the catalog instance</param>
   /// <param name="connectionString">default connection string</param>
   /// <returns>an instance of the requested catalog is returned</returns>
   public ResultsLog<ICatalogService?> GetCatalog(
      string sessionId, string invariantName, string? connectionString = null)
   {
      ResultsLog<ICatalogService?> results = new ResultsLog<ICatalogService?>();
      if (!String.IsNullOrWhiteSpace(invariantName))
      {
         switch(invariantName)
         {
            case EDAM_FILE_SYSTEM_DB:
               _CatalogName = EDAM_FILE_SYSTEM_DB;
               results.Instance = new CatalogBuilderServiceInstance(
                  connectionString);
               results.Succeeded();
               break;
            default:
               results.Failed(EventCode.NotSupported);
               break;
         }
      }
      else
      {
         results.Failed(EventCode.ArgumentOrParameterExpectedNotFound);
      }
      return results;
   }
}
