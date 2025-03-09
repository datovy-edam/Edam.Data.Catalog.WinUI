using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------------
using Edam.Application;
using Edam.Data.CatalogModel;
using catDb = Edam.Data.CatalogDb;
using catSrv = Edam.Data.CatalogService;
using Edam.Diagnostics;

namespace Edam.UI.CatalogExplorer;

public class CatalogServiceHelper
{
   public static string _SessionId = Guid.NewGuid().ToString();

   /// <summary>
   /// Get Catalog Service instance...
   /// </summary>
   /// <returns>Catalog Service instance is returned</returns>
   public static async Task<ICatalogService> GetClientInstanceAsync(
       string? baseUri = null)
   {
      var _conUri = String.IsNullOrWhiteSpace(baseUri) ?
          AppSettings.GetString("CatalogServiceBaseUri") :
          baseUri;

      // initialize repository
      catSrv.CatalogInstance instance = new catSrv.CatalogInstance();
      var instResults = instance.GetCatalog(_SessionId,
         catSrv.CatalogInstance.EDAM_BASE_URI, _conUri);

      if (instResults.Success)
      {
         await instResults.Instance.InitializeClientAsync(_SessionId, "");
         return instResults.Instance;
      }
      return null;
   }

   /// <summary>
   /// Get Catalog Service instance...
   /// </summary>
   /// <returns>Catalog Service instance is returned</returns>
   public static ICatalogService GetLocalInstance(
       string? connectionString = null)
   {
      ResultsLog<ICatalogService?> results = null;
      var _conString = String.IsNullOrWhiteSpace(connectionString) ?
          AppSettings.GetConnectionString("catalogDb") :
          connectionString;

      // initialize repository
      catDb.CatalogInstance instance = new catDb.CatalogInstance();
      results = instance.GetCatalog(_SessionId,
         catDb.CatalogInstance.EDAM_FILE_SYSTEM_DB, connectionString);

      if (results.Success)
      {
         results.Instance.SetContainer(_SessionId, "");
         return results.Instance;
      }
      return null;
   }

   /// <summary>
   /// Get Catalog Service instance...
   /// </summary>
   /// <param name="connectionUri">connection URI 
   /// (Connection String or Base URI)</param>
   /// <returns>Catalog Service instance is returned</returns>
   public static async Task<ICatalogService> GetInstanceAsync(
       string? connectionUri = null)
   {
      ICatalogService result = null;

      if (Environment.OSVersion.Platform == PlatformID.Other)
      {
         // initialize repository
         result = await
             CatalogServiceHelper.GetClientInstanceAsync(connectionUri);
      }
      else
      {
         result = GetLocalInstance(connectionUri);
      }

      return result;
   }

   /// <summary>
   /// Get Instance Async...
   /// </summary>
   /// <param name="connectionString">connection string</param>
   /// <returns>instance is returned</returns>
   //public static async Task<ICatalogService> GetInstanceAsync(
   //    string? connectionString = null)
   //{
   //    ICatalogService instance = null;
   //    await Task.Run(() => {
   //        instance = GetInstance(connectionString);
   //    });
   //    return instance;
   //}

   /// <summary>
   /// Get Catalog to build its tree and access data.
   /// </summary>
   /// <param name="connectionUri">connection string</param>
   /// <returns>instance of catalog is returned</returns>
   public static async Task<CatalogInfo?> GetCatalogAsync(
       string? connectionUri = null)
   {
      CatalogInfo catalog = null;
      try
      {
         ICatalogService instance = await GetInstanceAsync(connectionUri);
         catalog = new CatalogInfo(instance, _SessionId);
         await catalog.InitializeCatalogAsync("", buildTree: true);
      }
      catch (Exception ex)
      {
         ResultLog.DefaultLog.Failed(ex);
      }
      return catalog;
   }

   /// <summary>
   /// Get Catalog Async...
   /// </summary>
   /// <param name="connectionUri">connection string</param>
   /// <returns>instance of catalog is returned</returns>
   //public static async Task<CatalogInfo> GetCatalogAsync(
   //    string? connectionUri = null)
   //{
   //    CatalogInfo catalog = null;
   //    await Task.Run(() => {
   //        catalog = GetCatalog(connectionUri);
   //    });
   //    return catalog;
   //}
}
