using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.DataObjects.Objects;
using Edam.DataObjects.Requests;
using Edam.Diagnostics;
using Edam.Net;
using Edam.Net.Web;
using Edam.Text;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogDb;

namespace Edam.Data.CatalogService;

public partial class CatalogClient : CatalogBaseClient, ICatalogClient
{

   #region -- 1.00 - Fields and Properties declration/definitions

   #endregion
   #region -- 1.50 - Constructure and Initialization

   public CatalogClient(string sessionId, string baseUri) :
     base(sessionId, baseUri)
   {
      InitializeClientInstance();
   }

   public CatalogClient(string sessionId, HttpRequestInfo connectionInfo) :
     base(sessionId, connectionInfo)
   {
      InitializeClientInstance();
   }

   private void InitializeClientInstance()
   {
      Container = new ClientCatalogContainer(this);
      Item = new ClientCatalogItem(this);
      ItemData = new ClientCatalogItemData(this);
   }

   #endregion
   #region -- 4.00 - Catalog Client Support

   /// <summary>
   /// Initialize Service Client with default container Async.
   /// </summary>
   /// <param name="sessionId">session Id</param>
   /// <returns>default or found container instance is returned</returns>
   public static async Task<CatalogClient> InitializeClientAsync(
      string sessionId)
   {
      CatalogClient client = new(sessionId, String.Empty);
      var container = await client.InitializeClientAsync(sessionId, null);
      return client;
   }

   /// <summary>
   /// Initialize Service Client with default container.
   /// </summary>
   /// <param name="sessionId">session Id</param>
   /// <returns>default or found container instance is returned</returns>
   public static CatalogClient InitializeClient(string sessionId)
   {
      CatalogClient client = new(sessionId, String.Empty);
      var catalog = client.InitializeClient(sessionId, null);
      return client;
   }

   #endregion
   #region -- 4.00 - Manage Other Requests...

   /// <summary>
   /// Get Content Type Async.
   /// </summary>
   /// <param name="contentTypeId">content type ID to get</param>
   /// <returns>Instance of ContentTypeInfo is returned</returns>
   public async Task<ContentTypeInfo> GetContentTypeAsync(string contentTypeId)
   {
      _resultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _lastSessionId);
      pars.Add(TAG_CONTENT_TYPE_ID, contentTypeId);
      ContentTypeInfo item = null;

      var req = URI_CONTENT_TYPE_ID + pars.ToString();
      try
      {
         item = await _client.GetDataFromJsonAsync<ContentTypeInfo>(req);
      }
      catch (Exception ex)
      {
         _resultsLog.Failed(ex);
      }

      return item;
   }

   /// <summary>
   /// Get Content Type.
   /// </summary>
   /// <param name="contentTypeId">content type ID to get</param>
   /// <returns>Instance of ContentTypeInfo is returned</returns>
   public ContentTypeInfo GetContentType(string contentTypeId)
   {
      ContentTypeInfo? item = null;
      Task<ContentTypeInfo> result = GetContentTypeAsync(contentTypeId);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         item = result.Result;
      }
      return item;
   }

   #endregion

}
