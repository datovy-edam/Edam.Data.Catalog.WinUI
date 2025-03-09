using Edam.Data.CatalogModel;
using Edam.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------
using Edam.Data.CatalogDb;

namespace Edam.Data.CatalogService;

public class ClientCatalogContainer : ICatalogContainer
{

   #region -- 4.00 - Properties and Definitions

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public ClientCatalogContainer(ICatalogBaseClient client)
   {
      _Client = client;
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
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_CONTAINER_ID, containerId);
      pars.Add(QueryStringTag.Description, description);
      pars.Add(QueryStringTag.Status, status);
      ContainerInfo container = null;

      var req = CatalogBaseClient.URI_CONTAINER_DELIST + pars.ToString();
      try
      {
         container = await _Client.Client.GetDataFromJsonAsync<ContainerInfo>(
            req);
         _Client.DefaultContainer = _Client.CurrentContainer = container;
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
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
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_CONTAINER_ID, containerId);
      pars.Add(QueryStringTag.Description, description);
      ContainerInfo container = null;

      var req = CatalogBaseClient.URI_CONTAINER_ENLIST + pars.ToString();
      try
      {
         container = await _Client.Client.GetDataFromJsonAsync<ContainerInfo>(
            req);
         _Client.DefaultContainer = _Client.CurrentContainer = container;
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
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
      if (_Client == null)
      {

      }

      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      pars.Add(CatalogBaseClient.TAG_CONTAINER_ID, containerId);
      ContainerInfo container = null;

      var req = CatalogBaseClient.URI_CONTAINER_INFO + pars.ToString();
      try
      {
         container = await _Client.Client.GetDataFromJsonAsync<ContainerInfo>(
            req);
         _Client.DefaultContainer = _Client.CurrentContainer = container;
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
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
   /// Get Container.
   /// </summary>
   /// <param name="id">Guid value</param>
   /// <returns>if called and exception is thrown</returns>
   public ContainerInfo GetContainer(Guid id)
   {
      new Exception("ClientCatalogContainer::GetContainer - Should never be called.");
      return null;
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
      _Client.ResultsLog.Clear();

      QueryStringBuilder pars = new QueryStringBuilder();
      pars.Add(QueryStringTag.SessionId, _Client.LastSessionId);
      List<ContainerInfo> list = null;

      var req = CatalogBaseClient.URI_CONTAINER_LIST + pars.ToString();
      try
      {
         list = await _Client.Client.GetDataFromJsonAsync<List<ContainerInfo>>(
            req);
      }
      catch (Exception ex)
      {
         _Client.ResultsLog.Failed(ex);
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

   #endregion

}
