using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

using Edam.Data.CatalogModel;
using Edam.Diagnostics;
using Edam.UI.Catalog.Models;
using Edam.Data.CatalogService;
using Windows.Media.Protection.PlayReady;

// -----------------------------------------------------------------------------

namespace Edam.UI.Catalog.Controls;

public class CatalogContainerViewModel : ObservableObject
{

   public CatalogViewModel CatalogBase { get; set; }

   private Visibility _containerEditVisibility;
   public Visibility ContainerEditVisibility
   {
      get { return _containerEditVisibility; }
      set
      {
         if (_containerEditVisibility != value)
         {
            _containerEditVisibility = value;
            OnPropertyChanged(nameof(ContainerEditVisibility));
         }
      }
   }

   public ObservableCollection<ContainerItem> DataSource { get; set; } =
       new ObservableCollection<ContainerItem>();

   public NotificationEventHandler NotifyEvent { get; set; }

   public CatalogContainerViewModel()
   {
      ContainerEditVisibility = Visibility.Collapsed;
   }

   /// <summary>
   /// Notify Client Change Event.
   /// </summary>
   /// <param name="message">message with details about the client</param>
   public void NotifyClientChangedEvent(string message = null)
   {
      if (NotifyEvent != null)
      {
         var args = new NotificationEventArgs
         {
            EventID = Guid.NewGuid().ToString(),
            Message = message ?? "Change Container Event",
            Data = CatalogBase.Catalog.CatalogService
         };

         NotifyEvent(this, args);
      }
   }

   /// <summary>
   /// Add Container using submitted name.
   /// </summary>
   /// <param name="name">container name</param>
   /// <returns>EventCode is returned (Success or InsertUpdateFailed)</returns>
   public EventCode AddContainer(string name)
   {
      if (String.IsNullOrWhiteSpace(name))
      {
         return EventCode.NameExpectedNoneFound;
      }

      EventCode added = EventCode.Success;
      var description = Edam.Text.Convert.ToProperCase(name);
      var container = CatalogBase.Catalog.CatalogService.Container.
          EnlistContainer(name, description);
      if (container == null ||
          String.IsNullOrWhiteSpace(container.ContainerId))
      {
         var results = new ResultLog();
         results.Failed(EventCode.InsertUpdateFailed);
      }
      else
      {
         var client = container.ContainerType == ContainerType.WebApi ?
            CatalogBase.Catalog.CatalogService : null;

         var item = new ContainerItem(client);
         item.Container = container;
         DataSource.Add(item);
      }
      return added;
   }

   /// <summary>
   /// Initialize Catalog
   /// </summary>
   public async Task InitializeContainersAsync()
   {
      DataSource.Clear();
      var lst = await CatalogBase.Catalog.
         CatalogService.Container.GetContainersAsync();
      ICatalogService client = null;

      foreach (var item in lst)
      {
         // setup only the (default) container client, supporting client
         // should be set only when the container is selected the first time
         client = item.ContainerId == CatalogInfo.DEFAULT_CONTAINER_NAME ? 
            CatalogBase.Catalog.CatalogService : null;

         // add container
         var container = new ContainerItem(client);
         container.Container = item;
         DataSource.Add(container);
      }

      if (CatalogBase.Catalog.CatalogService != client)
      {
         CatalogBase.Catalog.CatalogService = client;
         NotifyClientChangedEvent();
      }
   }

   /// <summary>
   /// A container has been selected select it as needed...
   /// </summary>
   /// <param name="item">selected container-item</param>
   public void SelectedContainer(ContainerItem item)
   {
      if (item.Client == null)
      {
         item.Client = CatalogBase.GetClient(item.Container);
      }

      // this should never happend!
      if (item.Client == null)
      {
         ResultLog.Trace(
            "CatalogContainerViewModel::SelectedContainer: " +
            "Up's switching to default catalog services");
         item.Client = CatalogBase.Catalog.DefaultCatalogService;
         CatalogBase.Catalog.CatalogService = item.Client;

         NotifyClientChangedEvent();
         return;
      }

      if (CatalogBase.Catalog.CatalogService != item.Client)
      {
         NotifyClientChangedEvent();
      }
   }

}

