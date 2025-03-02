using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Windows.ApplicationModel.Appointments;

// -----------------------------------------------------------------------------

namespace Edam.UI.Catalog;
public class AppModelState
{
   public IStringLocalizer Localizer { get; set; }
   public AppConfig AppOptions { get; set; }
   //public IOptions<AppConfig> AppOptions { get; set; }
   //public INavigator Navigator { get; set; }

   public string? GetDefaultConnectionString()
   {
      return AppOptions.DefaultConnectionString;
      //return AppOptions?.Value?.DefaultConnectionString;
   }

   public string? GetCatalogServiceBaseUri()
   {
      return AppOptions.CatalogServiceBaseUri;
      //return AppOptions?.Value?.CatalogServiceBaseUri;
   }

   public string? GetConnectionUri()
   {
      if (Environment.OSVersion.Platform == PlatformID.Other)
      {
         return GetCatalogServiceBaseUri();
      }
      else
      {
         return GetDefaultConnectionString();
      }
   }
}
