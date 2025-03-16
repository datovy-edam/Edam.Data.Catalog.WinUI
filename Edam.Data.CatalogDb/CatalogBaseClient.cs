using Edam.Data.CatalogModel;
using Edam.Diagnostics;
using Edam.Net.Web;
using Edam.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Text;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogDb;

public partial class CatalogBaseClient : ICatalogBaseClient
{

   #region -- 1.00 - Fields and Properties declration/definitions

   public const string DELETED = "Deleted";
   public const string ROOT_ID = "[root]";
   public const string ROOT_PATH = "/";

   public const string DEVELOPMENT_URI =
      "https://localhost:7069/catalogservice/";

   public const string TAG_CONTAINER_ID = "containerId";
   public const string TAG_CONTAINER_GUID = "id";
   public const string TAG_ITEM_ID = "id";
   public const string TAG_ITEM_PATH = "path";
   public const string TAG_ITEM_NAME = "name";
   public const string TAG_DATA_ID = "id";
   public const string TAG_CONTENT_TYPE_ID = "contentTypeId";

   public const string URI_SESSION_INFO = "session/info";

   public const string URI_CONTAINER_ID = "container/id";
   public const string URI_CONTAINER_INFO = "container/info";
   public const string URI_CONTAINER_LIST = "container/list";
   public const string URI_CONTAINER_ENLIST = "container/enlist";
   public const string URI_CONTAINER_DELIST = "container/delist";
   public const string URI_CONTAINER_ROOT_ITEM_ID = "container/item/root/id";
   public const string URI_CONTAINER_ITEMS = "container/items/id";

   public const string URI_ITEM_ADD = "catalog/item";
   public const string URI_ITEM_ID = "catalog/item/id";
   public const string URI_ITEM_PATH = "catalog/item/path";

   public const string URI_BRANCH_ITEMS = "catalog/branch/items";

   public const string URI_ITEM_DATA_ITEM = "catalog/data/item";
   public const string URI_ITEM_DATA_ITEM_ID = "catalog/data/item/id";
   public const string URI_ITEM_DATA_ITEM_NAME = "catalog/data/item/name";
   public const string URI_ITEM_DATA_ID = "catalog/data/id";

   public const string URI_DATA_ID = "catalog/data/id";

   public const string URI_CONTENT_TYPE_ID = "catalog/content/type/id";

   protected string _BaseURI;

   protected CatalogTreeBuilder? _builder = null;
   protected string _lastSessionId;
   protected HttpRequestInfo _httpRequestInfo;
   protected WebApiClient _client;
   protected IResultsLog _resultsLog = new ResultLog();
   protected string _defaultConnectionString;

   /// <summary>
   /// Catalog Tree Builder used when a dynamic tree is needed.
   /// </summary>
   public CatalogTreeBuilder Cataloger
   {
      get { return _builder; }
   }

   /// <summary>
   /// Data Context required Default Connection String in case another
   /// is not provided.
   /// </summary>
   public string ConnectionString
   {
      get { return _defaultConnectionString; }
   }

   /// <summary>
   /// Web API Client used when HTTP services will be used.
   /// </summary>
   public WebApiClient Client
   {
      get { return _client; }
   }

   /// <summary>
   /// Base URI such as an HTTP for WebAPI or a file system path.
   /// </summary>
   public string BaseURI
   {
      get { return _BaseURI; }
   }

   /// <summary>
   /// Catalog Client Session ID.
   /// </summary>
   public string LastSessionId
   {
      get { return _lastSessionId; }
   }

   /// <summary>
   /// Diagnostics Log results gathered while processing requests.
   /// </summary>
   public IResultsLog ResultsLog
   {
      get { return _resultsLog; }
   }

   /// <summary>
   /// Default and Current Containers.
   /// </summary>
   public ContainerInfo DefaultContainer { get; set; }
   public ContainerInfo CurrentContainer { get; set; }

   /// <summary>
   /// Catalog Service Container, Item, and ItemData helpers.
   /// </summary>
   public ICatalogContainer Container { get; set; }
   public ICatalogItem Item { get; set; }
   public ICatalogItemData ItemData { get; set; }

   #endregion
   #region -- 1.50 - Constructure and Initialization

   public CatalogBaseClient(string sessionId, string baseUri)
   {
      HttpRequestInfo req = new HttpRequestInfo();
      req.BaseUri = String.IsNullOrWhiteSpace(baseUri) ?
         DEVELOPMENT_URI : baseUri;
      req.ContentType = WebApiContentType.ApplicationJson;
      _httpRequestInfo = req;
      _lastSessionId = sessionId;
   }

   public CatalogBaseClient(string sessionId, HttpRequestInfo connectionInfo)
   {
      _httpRequestInfo = connectionInfo;
      _lastSessionId = sessionId;
   }

   public CatalogBaseClient(string connectionInfo)
   {
      _defaultConnectionString = connectionInfo;
   }

   /// <summary>
   /// Initialize Service Client Async.
   /// </summary>
   /// <param name="sessionId">session Id</param>
   /// <param name="containerId">container Id to search for</param>
   /// <returns>default or found container instance is returned</returns>
   public async Task<ContainerInfo> InitializeClientAsync(
      string sessionId, string? containerId = null)
   {
      if (_client != null)
      {
         return DefaultContainer;
      }

      if (_resultsLog == null)
         _resultsLog = new ResultLog();
      _resultsLog.Clear();
      if (String.IsNullOrWhiteSpace(_lastSessionId) &&
         !String.IsNullOrWhiteSpace(sessionId))
      {
         _lastSessionId = sessionId;
      }

      _client = new WebApiClient(_httpRequestInfo);

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _lastSessionId);
      pars.Add(TAG_CONTAINER_ID, containerId);
      ContainerInfo? container = null;

      var req = URI_SESSION_INFO + pars.ToString();
      try
      {
         container = await _client.GetDataFromJsonAsync<ContainerInfo?>(req);
         DefaultContainer = CurrentContainer = container;
      }
      catch (Exception ex)
      {
         _resultsLog.Failed(ex);
      }

      return container;
   }

   /// <summary>
   /// Initialize Service Client.
   /// </summary>
   /// <param name="sessionId">session Id</param>
   /// <param name="containerId">container Id to search for</param>
   /// <returns>default or found container instance is returned</returns>
   public ContainerInfo? InitializeClient(
      string sessionId, string? containerId = null)
   {
      ContainerInfo? container = null;
      Task<ContainerInfo> result =
         InitializeClientAsync(sessionId, containerId);
      result.Wait();
      if (result.Status == TaskStatus.RanToCompletion)
      {
         container = result.Result;
      }
      return container;
   }

   #endregion

}
