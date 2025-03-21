﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

using Edam.DataObjects.Requests;

namespace Edam.Data.CatalogModel;

public interface ICatalogService
{
   ContainerInfo DefaultContainer { get; set; }
   ContainerInfo CurrentContainer { get; set; }

   ICatalogContainer Container { get; set; }
   ICatalogItem Item { get; set; }
   ICatalogItemData ItemData { get; set; }
}
