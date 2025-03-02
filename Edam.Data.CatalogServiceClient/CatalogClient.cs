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

namespace Edam.Data.CatalogServiceClient;


public class CatalogClient : ICatalogClient, ICatalogService
{

    #region -- 1.00 - Fields and Properties declration/definitions

    private const string DEVELOPMENT_URI =
       "https://localhost:7069/catalogservice/";

    private const string TAG_CONTAINER_ID = "containerId";
    private const string TAG_CONTAINER_GUID = "id";
    private const string TAG_ITEM_ID = "id";
    private const string TAG_ITEM_PATH = "path";
    private const string TAG_ITEM_NAME = "name";
    private const string TAG_DATA_ID = "id";
    private const string TAG_CONTENT_TYPE_ID = "contentTypeId";

    private const string URI_SESSION_INFO = "session/info";

    private const string URI_CONTAINER_ID = "container/id";
    private const string URI_CONTAINER_INFO = "container/info";
    private const string URI_CONTAINER_LIST = "container/list";
    private const string URI_CONTAINER_ENLIST = "container/enlist";
    private const string URI_CONTAINER_DELIST = "container/delist";
    private const string URI_CONTAINER_ROOT_ITEM_ID = "container/item/root/id";
    private const string URI_CONTAINER_ITEMS = "container/items/id";

    private const string URI_ITEM_ADD = "catalog/item";
    private const string URI_ITEM_ID = "catalog/item/id";
    private const string URI_ITEM_PATH = "catalog/item/path";

    private const string URI_BRANCH_ITEMS = "catalog/branch/items";

    private const string URI_ITEM_DATA_ITEM = "catalog/data/item";
    private const string URI_ITEM_DATA_ITEM_ID = "catalog/data/item/id";
    private const string URI_ITEM_DATA_ITEM_NAME = "catalog/data/item/name";
    private const string URI_ITEM_DATA_ID = "catalog/data/id";

    private const string URI_DATA_ID = "catalog/data/id";

    private const string URI_CONTENT_TYPE_ID = "catalog/content/type/id";

    private string _lastSessionId;
    private HttpRequestInfo _httpRequestInfo;
    private WebApiClient _client;
    private IResultsLog _resultsLog = new ResultLog();

    public ContainerInfo DefaultContainer { get; set; }
    public ContainerInfo CurrentContainer { get; set; }

    #endregion
    #region -- 1.50 - Constructure and Initialization

    public CatalogClient(string sessionId, string baseUri)
    {
        HttpRequestInfo req = new HttpRequestInfo();
        req.BaseUri = String.IsNullOrWhiteSpace(baseUri) ?
           DEVELOPMENT_URI : baseUri;
        req.ContentType = WebApiContentType.ApplicationJson;
        _httpRequestInfo = req;
        _lastSessionId = sessionId;
    }

    public CatalogClient(string sessionId, HttpRequestInfo connectionInfo)
    {
        HttpRequestInfo req = new HttpRequestInfo();
        _httpRequestInfo = connectionInfo;
        _lastSessionId = sessionId;
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
    #region -- 4.00 - Container Management

    /// <summary>
    /// Delist (remove or delete) container Async.
    /// </summary>
    /// <param name="containerId">container Id</param>
    /// <param name="description">container description</param>
    /// <param name="status">(optional) status code [default:null]</param>
    /// <returns>if found, delisted container info is returned else null
    /// </returns>
    public async Task<ContainerInfo> DelistContainerAsync(
       string containerId, string description, string? status)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_CONTAINER_ID, containerId);
        pars.Add(QueryStringTag.Description, description);
        pars.Add(QueryStringTag.Status, status);
        ContainerInfo container = null;

        var req = URI_CONTAINER_DELIST + pars.ToString();
        try
        {
            container = await _client.GetDataFromJsonAsync<ContainerInfo>(req);
            DefaultContainer = CurrentContainer = container;
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return container;
    }

    /// <summary>
    /// Delist (remove or delete) container.
    /// </summary>
    /// <param name="containerId">container Id</param>
    /// <param name="description">container description</param>
    /// <param name="status">(optional) status code [default:null]</param>
    /// <returns>if found, delisted container info is returned else null
    /// </returns>
    public ContainerInfo DelistContainer(
       string containerId, string? description, string? status)
    {
        ContainerInfo? container = null;
        Task<ContainerInfo> result = DelistContainerAsync(
           containerId, description, status);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            container = result.Result;
        }
        return container;
    }

    /// <summary>
    /// Delist (remove or delete) container Async.
    /// </summary>
    /// <param name="containerId">container Id</param>
    /// <returns>if found, delisted container info is returned else null
    /// </returns>
    public ContainerInfo DelistContainer(string containerId)
    {
        String description = String.Empty;
        String status = String.Empty;
        ContainerInfo? container = null;
        Task<ContainerInfo> result = DelistContainerAsync(
           containerId, description, status);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            container = result.Result;
        }
        return container;
    }

    /// <summary>
    /// Enlist (add) container Async.
    /// </summary>
    /// <param name="containerId">container Id</param>
    /// <param name="description">container description</param>
    /// <param name="status">(optional) status code [default:null]</param>
    /// <returns>enlisted container info is returned else null</returns>
    public async Task<ContainerInfo> EnlistContainerAsync(
       string containerId, string description)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_CONTAINER_ID, containerId);
        pars.Add(QueryStringTag.Description, description);
        ContainerInfo container = null;

        var req = URI_CONTAINER_ENLIST + pars.ToString();
        try
        {
            container = await _client.GetDataFromJsonAsync<ContainerInfo>(req);
            DefaultContainer = CurrentContainer = container;
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return container;
    }

    /// <summary>
    /// Enlist (add) container.
    /// </summary>
    /// <param name="containerId">container Id</param>
    /// <param name="description">container description</param>
    /// <param name="status">(optional) status code [default:null]</param>
    /// <returns>enlisted container info is returned else null</returns>
    public ContainerInfo EnlistContainer(
       string containerId, string description)
    {
        ContainerInfo? container = null;
        Task<ContainerInfo> result = EnlistContainerAsync(
           containerId, description);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            container = result.Result;
        }
        return container;
    }

    /// <summary>
    /// Geg Container Async.
    /// </summary>
    /// <param name="containerId">container Id to search for</param>
    /// <param name="checkId">(optional) verify that it exists [default:true]
    /// </param>
    /// <returns>Container details are returne if exists</returns>
    public async Task<ContainerInfo> GetContainerAsync(
       string? containerId, bool checkId = true)
    {
        if (_client == null)
        {

        }

        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_CONTAINER_ID, containerId);
        ContainerInfo container = null;

        var req = URI_CONTAINER_INFO + pars.ToString();
        try
        {
            container = await _client.GetDataFromJsonAsync<ContainerInfo>(req);
            DefaultContainer = CurrentContainer = container;
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return container;
    }

    /// <summary>
    /// Geg Container.
    /// </summary>
    /// <param name="containerId">container Id to search for</param>
    /// <param name="checkId">(optional) verify that it exists [default:true]
    /// </param>
    /// <returns>Container details are returne if exists</returns>
    public ContainerInfo GetContainer(
       string? containerId, bool checkId = true)
    {
        ContainerInfo? container = null;
        Task<ContainerInfo> result = GetContainerAsync(containerId, checkId);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            container = result.Result;
        }
        return container;
    }

    /// <summary>
    /// Set Container Async.
    /// </summary>
    /// <param name="containerId">container Id to search for</param>
    /// <returns>Container details are returne if exists</returns>
    public async Task<ContainerInfo> SetContainerAsync(
       string sessionId, string containerId)
    {
        return await GetContainerAsync(containerId);
    }

    /// <summary>
    /// Set Container.
    /// </summary>
    /// <param name="containerId">container Id to search for</param>
    /// <returns>Container details are returne if exists</returns>
    public ContainerInfo SetContainer(
       string sessionId, string containerId)
    {
        return GetContainer(containerId);
    }

    /// <summary>
    /// Get the list of Containers Async.
    /// </summary>
    /// <returns>list of containers is returned</returns>
    public async Task<List<ContainerInfo>> GetContainersAsync()
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        List<ContainerInfo> list = null;

        var req = URI_CONTAINER_LIST + pars.ToString();
        try
        {
            list = await _client.GetDataFromJsonAsync<List<ContainerInfo>>(
               req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
            list = new List<ContainerInfo>();
        }

        return list;
    }

    /// <summary>
    /// Get the list of Containers.
    /// </summary>
    /// <returns>list of containers is returned</returns>
    public List<ContainerInfo> GetContainers()
    {
        List<ContainerInfo> list = null;
        Task<List<ContainerInfo>> result = GetContainersAsync();
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            list = result.Result;
        }
        return list;
    }

    /// <summary>
    /// Get Container Root Item Async.
    /// </summary>
    /// <param name="id">Guid of container whose root item is requested</param>
    /// <returns>root item is returned</returns>
    public async Task<ItemInfo> GetContainerRootItemAsync(Guid id)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_CONTAINER_GUID, id.ToString());
        ItemInfo item = null;

        var req = URI_CONTAINER_ROOT_ITEM_ID + pars.ToString();
        try
        {
            item = await _client.GetDataFromJsonAsync<ItemInfo>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return item;
    }

    /// <summary>
    /// Get Container Root Item.
    /// </summary>
    /// <param name="id">Guid of container whose root item is requested</param>
    /// <returns>root item is returned</returns>
    public ItemInfo GetContainerRootItem(Guid id)
    {
        ItemInfo? item = null;
        Task<ItemInfo> result = GetContainerRootItemAsync(id);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            item = result.Result;
        }
        return item;
    }

    /// <summary>
    /// Get Container Items Async.
    /// </summary>
    /// <param name="id">container id whose items are requested</param>
    /// <returns>list of items is returned</returns>
    public async Task<List<ItemInfo>> GetContainerItemsAsync(Guid id)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_CONTAINER_GUID, id.ToString());
        List<ItemInfo> item = null;

        var req = URI_CONTAINER_ITEMS + pars.ToString();
        try
        {
            item = await _client.GetDataFromJsonAsync<List<ItemInfo>>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return item;
    }

    /// <summary>
    /// Get Container Items.
    /// </summary>
    /// <param name="id">container id whose items are requested</param>
    /// <returns>list of items is returned</returns>
    public List<ItemInfo> GetContainerItems(Guid id)
    {
        List<ItemInfo> items = null;
        Task<List<ItemInfo>> result = GetContainerItemsAsync(id);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            items = result.Result;
        }
        return items;
    }

    #endregion
    #region -- 4.00 - Items Management

    /// <summary>
    /// Add Item Async.
    /// </summary>
    /// <param name="item">item to ask</param>
    /// <returns>created item is returned, else null</returns>
    public async Task<ItemInfo> AddItemAsync(ItemInfo item)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        ItemInfo result = null;

        var req = URI_ITEM_ADD + pars.ToString();
        try
        {
            result = await _client.PostAsync<ItemInfo>(req, item);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return result;
    }

    /// <summary>
    /// Add Item.
    /// </summary>
    /// <param name="item">item to ask</param>
    /// <returns>created item is returned, else null</returns>
    public ItemInfo AddItem(ItemInfo item)
    {
        ItemInfo? itm = null;
        Task<ItemInfo> result = AddItemAsync(item);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            itm = result.Result;
        }
        return itm;
    }

    /// <summary>
    /// Get Item Async.
    /// </summary>
    /// <param name="id">Item id to find.</param>
    /// <returns>return found Item by id (Guid)</returns>
    public async Task<ItemInfo?> GetItemAsync(Guid id)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_ID, _lastSessionId);
        ItemInfo result = null;

        var req = URI_ITEM_ID + pars.ToString();
        try
        {
            result = await _client.GetDataFromJsonAsync<ItemInfo>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return result;
    }

    /// <summary>
    /// Get Item Async.
    /// </summary>
    /// <param name="id">Item id to find.</param>
    /// <returns>return found Item by id (Guid)</returns>
    public ItemInfo GetItem(Guid id)
    {
        ItemInfo? itm = null;
        Task<ItemInfo> result = GetItemAsync(id);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            itm = result.Result;
        }
        return itm;
    }

    /// <summary>
    /// Get Item by Path Async.
    /// </summary>
    /// <param name="name">path name</param>
    /// <returns>return found Item by path name</returns>
    public async Task<ItemInfo> GetItemByPathAsync(string name)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_PATH, name);
        ItemInfo result = null;

        var req = URI_ITEM_PATH + pars.ToString();
        try
        {
            result = await _client.GetDataFromJsonAsync<ItemInfo>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return result;
    }

    /// <summary>
    /// Get Item.
    /// </summary>
    /// <param name="name">path name</param>
    /// <returns>return found Item by path name</returns>
    public ItemInfo GetItemByPath(string name)
    {
        ItemInfo? itm = null;
        Task<ItemInfo> result = GetItemByPathAsync(name);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            itm = result.Result;
        }
        return itm;
    }

    /// <summary>
    /// Delete Item Async.
    /// </summary>
    /// <param name="id">id (Guid) of item to delete</param>
    /// <returns>request status is returned</returns>
    public async Task<RequestStatus> DeleteItemAsync(Guid itemId)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_ID, itemId.ToString());
        RequestStatus result = RequestStatus.Unknown;

        var req = URI_ITEM_ID + pars.ToString();
        try
        {
            var response = await _client.DeleteAsync<RequestResponseInfo>(req);
            if (response != null && response.Success)
            {
                result = response.Status;
            }
            else
            {
                result = RequestStatus.Failed;
            }
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return result;
    }

    /// <summary>
    /// Delete Item.
    /// </summary>
    /// <remarks>check for LastLog results</remarks>
    /// <param name="id">id (Guid) of item to delete</param>
    public RequestStatus DeleteItem(Guid id)
    {
        RequestStatus response = RequestStatus.Unknown;
        Task<RequestStatus> result = DeleteItemAsync(id);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            response = result.Result;
            if (response == RequestStatus.Failed)
            {
                _resultsLog.Failed(RequestStatus.Failed.ToString());
            }
            else
            {
                _resultsLog.Succeeded();
            }
        }
        return response;
    }

    #endregion
    #region -- 4.00 - Manage Branches and Leafs

    /// <summary>
    /// Create a Branch using given path.
    /// </summary>
    /// <remarks>note that the path may really have a leaf pattern such as one
    /// that include a file and an extension at the end, if so a Leaf will be
    /// created instead</remarks>
    /// <param name="path">full path</param>
    /// <param name="description">description</param>
    /// <param name="containerId">target container</param>
    /// <returns>found or created branch is returned</returns>
    public async Task<ItemInfo> CreateBranchAsync(
       string path, string? description = null, Guid? containerId = null)
    {
        ItemInfo item = new ItemInfo();
        item.FullPath = path;
        item.Description = description;
        item.ContainerId = containerId.HasValue ? containerId.Value :
           CurrentContainer.Id;
        item.ItemType = DataObjects.Trees.TreeItemType.Branch;

        CatalogPathItem pitem = new CatalogPathItem(item);

        ItemInfo ritem = await AddItemAsync(pitem.Item);

        return ritem;
    }

    /// <summary>
    /// Add Data Item.
    /// </summary>
    /// <param name="item">item to ask</param>
    /// <returns>created item is returned, else null</returns>
    public ItemInfo? CreateBranch(
       string path, string? description = null, Guid? containerId = null)
    {
        ItemInfo? item = null;
        Task<ItemInfo?> result =
           CreateBranchAsync(path, description, containerId.Value);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            item = result.Result;
        }
        return item;
    }

    /// <summary>
    /// Create a Leaf using given path.
    /// </summary>
    /// <remarks>note that the path may really have a branch pattern such as one
    /// that do not include a file and an extension at the end, if so a Branch 
    /// will be created instead</remarks>
    /// <param name="path">full path</param>
    /// <param name="description">description</param>
    /// <param name="containerId">target container</param>
    /// <returns>found or created branch is returned</returns>
    public ItemInfo CreateLeaf(
       string path, string name, Guid? id = null, string? description = null,
       string? dataValue = null)
    {
        ItemInfo item = new ItemInfo();
        item.FullPath = path;
        item.Description = description;
        item.ContainerId = CurrentContainer.Id;
        item.ItemType = DataObjects.Trees.TreeItemType.Branch;

        CatalogPathItem pitem = new CatalogPathItem(item);

        ItemInfo ritem = AddItem(pitem.Item);

        return ritem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<ItemInfo?>> GetBranchAsync(string? path = null)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_PATH, path);
        List<ItemInfo> item = null;

        var req = URI_BRANCH_ITEMS + pars.ToString();
        try
        {
            item = await _client.GetDataFromJsonAsync<List<ItemInfo>>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return item;
    }

    /// <summary>
    /// Add Data Item.
    /// </summary>
    /// <param name="item">item to ask</param>
    /// <returns>created item is returned, else null</returns>
    public List<ItemInfo?> GetBranch(string? path)
    {
        List<ItemInfo?>? items = null;
        Task<List<ItemInfo?>> result = GetBranchAsync(path);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            items = result.Result;
        }
        return items;
    }

    #endregion
    #region -- 4.00 - Item Data Management

    /// <summary>
    /// Add Data Item Async.
    /// </summary>
    /// <param name="item">item to ask</param>
    /// <returns>created item is returned, else null</returns>
    public async Task<ItemDataInfo> AddItemAsync(ItemDataInfo item)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        ItemDataInfo result = null;

        var req = URI_ITEM_ADD + pars.ToString();
        try
        {
            result = await _client.PostAsync<ItemDataInfo>(req, item);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return result;
    }

    /// <summary>
    /// Add Data Item.
    /// </summary>
    /// <param name="item">item to ask</param>
    /// <returns>created item is returned, else null</returns>
    public ItemDataInfo AddItem(ItemDataInfo item)
    {
        ItemDataInfo? itm = null;
        Task<ItemDataInfo> result = AddItemAsync(item);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            itm = result.Result;
        }
        return itm;
    }

    /// <summary>
    /// Get Item Data Async.
    /// </summary>
    /// <param name="itemId">item ID whose data items are requested</param>
    /// <returns>List of item data instances are returned</returns>
    public async Task<List<ItemDataInfo>> GetItemDataAsync(Guid itemId)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_ID, itemId.ToString());
        List<ItemDataInfo> items = null;

        var req = URI_ITEM_DATA_ITEM_ID + pars.ToString();
        try
        {
            items = await _client.GetDataFromJsonAsync<List<ItemDataInfo>>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return items;
    }

    /// <summary>
    /// Get Item Data.
    /// </summary>
    /// <param name="itemId">item ID whose data items are requested</param>
    /// <returns>List of item data instances are returned</returns>
    public List<ItemDataInfo> GetItemData(Guid itemId)
    {
        List<ItemDataInfo?> items = null;
        Task<List<ItemDataInfo>> result = GetItemDataAsync(itemId);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            items = result.Result;
        }
        return items;
    }

    /// <summary>
    /// Delete Item Data (and all related data leafs) By Item ID Async.
    /// </summary>
    /// <param name="itemId">(guid) id</param>
    /// <returns>request status (code) is returned</returns>
    public async Task<RequestStatus> DeleteItemDataAsync(Guid itemId)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_ID, itemId.ToString());
        RequestStatus result = RequestStatus.Unknown;

        var req = URI_ITEM_DATA_ITEM_ID + pars.ToString();
        try
        {
            var response = await _client.DeleteAsync<RequestResponseInfo>(req);
            if (response != null && response.Success)
            {
                result = response.Status;
            }
            else
            {
                result = RequestStatus.Failed;
            }
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return result;
    }

    /// <summary>
    /// Delete Item Data (and all related data leafs) By Item ID.
    /// </summary>
    /// <remarks>(inner) ResultsLog contains the result</remarks>
    /// <param name="itemId">(guid) id</param>
    public RequestStatus DeleteItemData(Guid itemId)
    {
        RequestStatus response = RequestStatus.Unknown;
        Task<RequestStatus> result = DeleteItemDataAsync(itemId);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            response = result.Result;
            if (response == RequestStatus.Failed)
            {
                _resultsLog.Failed(RequestStatus.Failed.ToString());
            }
            else
            {
                _resultsLog.Succeeded();
            }
        }
        return response;
    }

    /// <summary>
    /// Delete Data By Id Async.
    /// </summary>
    /// <param name="dataId">data (blob) Id</param>
    /// <returns>Request Status is returned</returns>
    public async Task<RequestStatus> DeleteDataAsync(Guid dataId)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_ID, dataId.ToString());
        RequestStatus result = RequestStatus.Unknown;

        var req = URI_DATA_ID + pars.ToString();
        try
        {
            var response = await _client.DeleteAsync<RequestResponseInfo>(req);
            if (response != null && response.Success)
            {
                result = response.Status;
            }
            else
            {
                result = RequestStatus.Failed;
            }
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return result;
    }

    /// <summary>
    /// Delete Data By Id.
    /// </summary>
    /// <param name="dataId">data (blob) Id</param>
    /// <returns>Request Status is returned</returns>
    public RequestStatus DeleteData(Guid dataId)
    {
        RequestStatus response = RequestStatus.Unknown;
        Task<RequestStatus> result = DeleteDataAsync(dataId);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            response = result.Result;
            if (response == RequestStatus.Failed)
            {
                _resultsLog.Failed(RequestStatus.Failed.ToString());
            }
            else
            {
                _resultsLog.Succeeded();
            }
        }
        return response;
    }

    /// <summary>
    /// Create Data Leaf.
    /// </summary>
    /// <remarks>Item data is not created, call AddItem to do so</remarks>
    /// <param name="item">parent item</param>
    /// <param name="name">data item name</param>
    /// <param name="dataId">(optional) data id</param>
    /// <param name="dataValue"(blob) data value</param>
    /// <returns>instance of ItemData is returned</returns>
    public ItemDataInfo CreateDataLeaf(
       ItemInfo item, string name, Guid? dataId = null, byte[] dataValue = null)
    {
        var data = ItemDataInfo.CreateDataLeaf(item.Id, name, dataId);
        var pitem = new CatalogPathItem(item);
        data.ContentType = pitem.GetContentType();
        return data;
    }

    /// <summary>
    /// Create Data Leaf.
    /// </summary>
    /// <remarks>Item data is not created, call AddItem to do so</remarks>
    /// <param name="item">parent item</param>
    /// <param name="name">data item name</param>
    /// <param name="dataId">(optional) data id</param>
    /// <param name="dataValue"(blob) data value</param>
    /// <returns>instance of ItemData is returned</returns>
    public ItemDataInfo CreateDataLeaf(
       ItemInfo item, string name, Guid? dataId = null, string dataValue = null)
    {
        var data = ItemDataInfo.CreateDataLeaf(item.Id, name, dataId);
        data.DataText = dataValue;
        var pitem = new CatalogPathItem(item);
        data.ContentType = pitem.GetContentType();
        return data;
    }

    /// <summary>
    /// Get Data By Name Async.
    /// </summary>
    /// <param name="itemId">Guid of item Id</param>
    /// <param name="name">data (blob) name</param>
    /// <returns>instance of ItemDataInfo is returned</returns>
    public async Task<ItemDataInfo> GetDataByNameAsync(Guid itemId, string name)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_ITEM_ID, itemId.ToString());
        pars.Add(TAG_ITEM_NAME, itemId.ToString());
        ItemDataInfo item = null;

        var req = URI_ITEM_DATA_ITEM_NAME + pars.ToString();
        try
        {
            item = await _client.GetDataFromJsonAsync<ItemDataInfo>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return item;
    }

    /// <summary>
    /// Get Data By Name.
    /// </summary>
    /// <param name="itemId">Guid of item Id</param>
    /// <param name="name">data (blob) name</param>
    /// <returns>instance of ItemDataInfo is returned</returns>
    public ItemDataInfo GetDataByName(Guid itemId, string name)
    {
        ItemDataInfo? item = null;
        Task<ItemDataInfo> result = GetDataByNameAsync(itemId, name);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            item = result.Result;
        }
        return item;
    }

    /// <summary>
    /// Get Data Async.
    /// </summary>
    /// <param name="dataId">Guid Data ID</param>
    /// <returns>Instance of ItemDataInfo is returned</returns>
    public async Task<ItemDataInfo> GetDataAsync(Guid dataId)
    {
        _resultsLog.Clear();

        QueryStringBuilder pars = new QueryStringBuilder();
        pars.Add(QueryStringTag.SessionId, _lastSessionId);
        pars.Add(TAG_DATA_ID, dataId.ToString());
        ItemDataInfo item = null;

        var req = URI_ITEM_DATA_ID + pars.ToString();
        try
        {
            item = await _client.GetDataFromJsonAsync<ItemDataInfo>(req);
        }
        catch (Exception ex)
        {
            _resultsLog.Failed(ex);
        }

        return item;
    }

    /// <summary>
    /// Get Data Async.
    /// </summary>
    /// <param name="dataId">Guid Data ID</param>
    /// <returns>Instance of ItemDataInfo is returned</returns>
    public ItemDataInfo GetData(Guid dataId)
    {
        ItemDataInfo? item = null;
        Task<ItemDataInfo> result = GetDataAsync(dataId);
        result.Wait();
        if (result.Status == TaskStatus.RanToCompletion)
        {
            item = result.Result;
        }
        return item;
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
