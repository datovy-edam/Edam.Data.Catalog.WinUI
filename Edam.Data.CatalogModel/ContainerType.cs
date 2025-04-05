using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogModel;


/// <summary>
/// Container Type.
/// </summary>
/// <remarks>
/// Data Context client 
/// </remarks>
public enum ContainerType
{
   Unknown = 0,
   DataContext = 1,
   FileSystem = 2
}

