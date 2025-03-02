using Edam.Data.CatalogModel;
using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogServiceClient;

public class CatalogInstance
{
   public const string EDAM_BASE_URI = "edam.base.uri.db";
   private static string _CatalogName = EDAM_BASE_URI;

   /// <summary>
   /// Get an instance of a Catalog Service by name.
   /// </summary>
   /// <param name="sessionId">session ID</param>
   /// <param name="invariantName">name of the catalog instance</param>
   /// <param name="baseUri">default service base URI</param>
   /// <returns>an instance of the requested catalog is returned</returns>
   public ResultsLog<ICatalogClient?> GetCatalog(
      string sessionId, string invariantName, string baseUri)
   {
      ResultsLog<ICatalogClient?> results = new ResultsLog<ICatalogClient?>();
      if (!String.IsNullOrWhiteSpace(invariantName))
      {
         switch (invariantName)
         {
            case EDAM_BASE_URI:
               _CatalogName = EDAM_BASE_URI;
               results.Instance = new CatalogClient(sessionId, baseUri);
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
