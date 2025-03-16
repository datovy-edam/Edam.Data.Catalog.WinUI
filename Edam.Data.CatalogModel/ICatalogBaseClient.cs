using Edam.Diagnostics;
using Edam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogModel;

public interface ICatalogBaseClient
{
   CatalogTreeBuilder Cataloger { get; }

   string BaseURI { get; }
   string LastSessionId { get; }
   string ConnectionString { get; }
   IResultsLog ResultsLog { get; }
   WebApiClient Client { get; }

   ContainerInfo DefaultContainer { get; set; }
   ContainerInfo CurrentContainer { get; set; }

   ICatalogContainer Container { get; set; }
   ICatalogItem Item { get; set; }
   ICatalogItemData ItemData { get; set; }
}
