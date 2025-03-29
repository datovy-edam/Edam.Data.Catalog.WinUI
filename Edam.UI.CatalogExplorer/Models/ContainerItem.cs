using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Edam.Data.CatalogModel;


// -----------------------------------------------------------------------------
namespace Edam.UI.Catalog.Models;

public class ContainerItem : ObservableObject
{

   public CatalogInfo Catalog { get; set; }
   public ICatalogService Client { get; set; }
   public ContainerInfo Container { get; set; }

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
      Client = client;
   }

}

