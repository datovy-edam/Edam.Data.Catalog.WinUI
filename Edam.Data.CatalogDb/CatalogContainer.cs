using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.DataObjects.Medias;
using Edam.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogDb;

public class CatalogContainer : ICatalogContainer
{

   #region -- 4.00 - Properties and Definitions

   private CatalogContext? DbContext { get; set; } = null;

   private ICatalogBaseClient _Client;
   public ICatalogBaseClient BaseClient
   {
      get { return _Client; }
   }

   public CatalogContainer(ICatalogBaseClient client, 
      CatalogContext context)
   {
      _Client = client;
      DbContext = context;
   }

   #endregion
   #region -- 4.00 - Initialize Context

   /// <summary>
   /// Initialize Repository
   /// </summary>
   public void InitializeDbContext()
   {
      var connectionString =
         String.IsNullOrWhiteSpace(_Client.ConnectionString) ?
            AppSettings.GetConnectionString("catalogDb") :
            _Client.ConnectionString;

      DbContext = new CatalogContext(connectionString);
      if (!DbContext.Database.CanConnect())
      {
         try
         {
            DbContext.Database.EnsureCreated();
         }
         catch (Exception ex)
         {

         }
      }

      if (!DbContext.ContentTypes.Any())
      {
         var types = new ContentTypeInfo[]
         {
         new ContentTypeInfo(MediaContentTypeHelper.JSONLD, "json-ld document"),
         new ContentTypeInfo(MediaContentTypeHelper.JsonDocument,
            "json document"),
         new ContentTypeInfo(MediaContentTypeHelper.XmlDocument, "xml text"),
         new ContentTypeInfo(MediaContentTypeHelper.TextFile, "text document"),
         new ContentTypeInfo(MediaContentTypeHelper.OfficeExcelXmlFile,
            "excel open xml document"),
         new ContentTypeInfo(MediaContentTypeHelper.JAVASCRIPT,
            "javascript document")
         };
         foreach (var type in types)
         {
            DbContext.ContentTypes.Add(type);
         }
         DbContext.SaveChanges();
      }

      // define default container
      if (!DbContext.Containers.Any())
      {
         _Client.DefaultContainer = new ContainerInfo();
         DbContext.Containers.Add(_Client.DefaultContainer);
         DbContext.SaveChanges();
      }
      else
      {
         _Client.DefaultContainer = GetContainer(null);
      }
      _Client.CurrentContainer = _Client.DefaultContainer;

      // define default container root item
      if (!DbContext.Items.Any())
      {
         _Client.Item.CreateRootItem();
      }

   }
   #endregion
   #region -- 4.00 - Container Management

   /// <summary>
   /// Get Root Item of given container.
   /// </summary>
   /// <param name="containerId">container id</param>
   /// <returns>instance of File-Item is returned</returns>
   public ContainerInfo GetContainer(Guid id)
   {
      var items = from item in DbContext.Containers
                  where item.Id == id
                  select item;
      var l = items.ToList();
      return l.Count > 0 ? l[0] : null;
   }

   /// <summary>
   /// Get Containers.
   /// </summary>
   /// <returns>list of containers is returned</returns>
   public async Task<List<ContainerInfo>> GetContainersAsync()
   {
      return await DbContext.Containers.ToListAsync<ContainerInfo>();
   }

   /// <summary>
   /// Get Containers.
   /// </summary>
   /// <returns>list of containers is returned</returns>
   public List<ContainerInfo> GetContainers()
   {
      return DbContext.Containers.ToList<ContainerInfo>();
   }

   /// <summary>
   /// Get Container ID.
   /// </summary>
   /// <param name="containerId"></param>
   /// <returns></returns>
   protected string GetContainerId(string? containerId)
   {
      if (String.IsNullOrWhiteSpace(containerId))
      {
         containerId = ContainerInfo.CONTAINER_ID_DEFAULT;
      }
      return containerId;
   }

   /// <summary>
   /// Get Container.
   /// </summary>
   /// <param name="containerId"></param>
   /// <param name="checkId">true to check ID</param>
   /// <returns></returns>
   public async Task<ContainerInfo?> GetContainerAsync(
      string? containerId, bool checkId = true)
   {
      var cid = checkId ? GetContainerId(containerId) : containerId;
      ContainerInfo container = DbContext.Containers.Where(
         x => x.ContainerId == cid).FirstOrDefault();
      return container;
   }

   /// <summary>
   /// Get Container.
   /// </summary>
   /// <param name="containerId"></param>
   /// <param name="checkId">true to check ID</param>
   /// <returns></returns>
   public ContainerInfo? GetContainer(string? containerId, bool checkId = true)
   {
      var cid = checkId ? GetContainerId(containerId) : containerId;
      ContainerInfo container = DbContext.Containers.Where(
         x => x.ContainerId == cid).FirstOrDefault();
      return container;
   }

   /// <summary>
   /// Set Session and current container (by id).
   /// </summary>
   /// <param name="sessionId">(optional) session id</param>
   /// <param name="containerId">(optional) container id</param>
   /// <returns>found container is is returned</returns>
   public virtual ContainerInfo? SetContainer(
      string sessionId, string containerId)
   {
      var cid = GetContainerId(containerId);

      ContainerInfo container = _Client.CurrentContainer;

      if (DbContext == null)
      {
         InitializeDbContext();
         container = _Client.CurrentContainer;
         if (containerId == "\"\"")
         {
            cid = container.ContainerId;
         }
      }

      if (container == null || container.ContainerId != cid)
      {
         container = GetContainer(cid);
      }

      _Client.CurrentContainer = container;
      if (_Client.DefaultContainer == null)
      {
         _Client.DefaultContainer = container;
      }
      return container;
   }

   /// <summary>
   /// Enlist (Add or Update) container info.
   /// </summary>
   /// <param name="containerId">container id</param>
   /// <param name="description">description</param>
   /// <returns>container info is returned</returns>
   public ContainerInfo EnlistContainer(string containerId, string description)
   {
      ContainerInfo container = null;
      ResultLog results = new ResultLog();

      if (String.IsNullOrWhiteSpace(containerId))
      {
         results.Failed(EventCode.NameExpectedNoneFound);
         container = new ContainerInfo();
         container.ContainerId = String.Empty;
         return container;
      }

      try
      {
         container = GetContainer(containerId);
         if (container == null || container.ContainerId != containerId)
         {
            // prepare container
            container = new();
            container.ContainerId = containerId;
            container.Description = description;
            DbContext.Containers.Add(container);

            // add root item
            _Client.Item.CreateRootItem(container.Id);
         }
         else if (container.Description != description)
         {
            container.Description += description;
            DbContext.SaveChanges();
         }

         // new container is the current container...
         _Client.CurrentContainer = container;
         results.Succeeded();
      }
      catch (Exception ex)
      {
         container = new ContainerInfo();
         container.ContainerId = string.Empty;
         results.Failed(ex);
      }

      return container;
   }

   /// <summary>
   /// Delete Container Item-Data and Items.
   /// </summary>
   /// <param name="id">Container ID</param>
   protected void DeleteContainerData(Guid id)
   {
      var fitems = _Client.Item.GetContainerItems(id);
      foreach (var item in fitems)
      {
         var ditems = _Client.ItemData.GetItemData(item.Id);
         foreach (var ditem in ditems)
         {
            DbContext.DataItems.Remove(ditem);
         }
         DbContext.Items.Remove(item);
      }
      DbContext.SaveChanges();
   }

   /// <summary>
   /// Delist (Delete) container info.
   /// </summary>
   /// <param name="containerId">container id</param>
   /// <returns>container info is returned</returns>
   public ContainerInfo DelistContainer(string containerId)
   {
      if (String.IsNullOrWhiteSpace(containerId))
      {
         return null;
      }

      var container = GetContainer(containerId, checkId: false);
      if (container != null || container.ContainerId == containerId)
      {
         // delete all container items...
         DeleteContainerData(container.Id);

         // finally... delete container
         container.StatusCode = CatalogBaseClient.DELETED;
         DbContext.Remove(container);
         DbContext.SaveChanges();
      }
      return container;
   }

   /// <summary>
   /// Get Current Container.
   /// </summary>
   /// <returns>instance of container is returned</returns>
   public ContainerInfo GetCurrentContainer()
   {
      if (_Client.CurrentContainer == null)
      {
         if (_Client.DefaultContainer == null)
         {
            GetContainer(null);
         }
      }

      return _Client.CurrentContainer;
   }

   #endregion

}
