using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edam.Application;
using Edam.Data.CatalogModel;
using Edam.DataObjects.Medias;
using Edam.DataObjects.Requests;
using Edam.Diagnostics;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogDb;

/// <summary>
/// Support for Catalog/File System repository inqueries and requests.
/// </summary>
public class CatalogServiceInstance : 
   CatalogBaseClient, ICatalogService, IDisposable
{

   #region -- 1.00 - Fields and Properties

   protected CatalogContext? DbContext { get; set; } = null;

   #endregion
   #region -- 1.50 - Initialization and Disposition

   public CatalogServiceInstance(string? defaultConnectionString) : 
      base(defaultConnectionString)
   {
      _defaultConnectionString = defaultConnectionString;
   }

   /// <summary>
   /// Release resources.
   /// </summary>
   public void Dispose()
   {
      if (DbContext != null)
      {
         DbContext.Dispose();
         DbContext = null;
      }
   }

   #endregion

}
