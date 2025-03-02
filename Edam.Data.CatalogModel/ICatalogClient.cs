using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogModel
{

   public interface ICatalogClient : ICatalogService
   {
      Task<ContainerInfo> InitializeClientAsync(
         string sessionId, string? containerId = null);
      ContainerInfo? InitializeClient(
         string sessionId, string? containerId = null);
   }

}
