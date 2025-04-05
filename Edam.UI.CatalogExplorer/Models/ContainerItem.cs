using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Edam.Data.CatalogModel;


// -----------------------------------------------------------------------------
namespace Edam.UI.Catalog.Models;

/// <summary>
/// Container Item Observable Object.
/// </summary>
public class ContainerItem : ObservableObject
{

   /// <summary>
   /// Catalog with info about the root item and related children, internally 
   /// will also contain the client instance, default and current containers,
   /// and searchable Catalog Dictionary... the last containing the key as 
   /// the full or partial tree paths, and their instances (or Item beign
   /// Branches, or Leafs).
   /// </summary>
   public CatalogInfo Catalog { get; set; }

   /// <summary>
   /// Client instance to support Containers, Items, and Item Data...
   /// </summary>
   public ICatalogService Client { get; set; }

   /// <summary>
   /// Container Instance with its details...
   /// </summary>
   public ContainerInfo Container { get; set; }

   /// <summary>
   /// Container ID that generaly shows the root element of the tree.  The
   /// "default" root represents the top of the tree represented by the
   /// foreward slash ("/").
   /// </summary>
   public string ContainerId
   {
      get { return Container.ContainerId; }
      set
      {
         if (Container.ContainerId != value)
         {
            Container.ContainerId = value;
            OnPropertyChanged(nameof(ContainerId));
         }
      }
   }

   /// <summary>
   /// Initialize Container Item and setup supporting Catalog Client
   /// that provides underlying services.
   /// </summary>
   /// <param name="client">Catalog Service instance</param>
   public ContainerItem(ICatalogService client)
   {
      if (client != null)
      {
         Client = client;
         Catalog = client.Catalog;
      }
   }

}

