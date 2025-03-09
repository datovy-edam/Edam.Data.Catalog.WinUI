using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogModel;

public interface ICatalogContainer
{

   Task<ContainerInfo> GetContainerAsync(string? containerId, bool checkId = true);
   ContainerInfo GetContainer(string? containerId, bool checkId = true);
   ContainerInfo GetContainer(Guid containerId);

   ContainerInfo SetContainer(string sessionId, string containerId);
   ContainerInfo EnlistContainer(string containerId, string description);
   ContainerInfo DelistContainer(string containerId);

   Task<List<ContainerInfo>> GetContainersAsync();
   List<ContainerInfo> GetContainers();

}
