using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monaco;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Edam.Application;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Windows.ApplicationModel;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

// -----------------------------------------------------------------------------
namespace Edam.UI.Catalog;

public class ApplicationStateChangeArgs : EventArgs
{
   public AppModelState State { get; set; }
   public ApplicationStateChangeArgs(AppModelState state) : base()
   {
      State = state;
   }
}
public delegate void ApplicationStateChangeEvent(
    object sender, ApplicationStateChangeArgs args);

public class AppSession
{

   private const string CONFIG_APP_SECTION = "AppConfig";
   public static Window Window { get; set; }

   public static ApplicationStateChangeEvent
       ApplicationStateChangeNotify
   { get; set; }
   private static AppModelState _applicationState = null;
   public static AppModelState ApplicationState
   {
      get { return _applicationState; }
      set
      {
         _applicationState = value;
         if (ApplicationStateChangeNotify != null)
         {
            var args = new ApplicationStateChangeArgs(value);
            ApplicationStateChangeNotify(Window, args);
         }
      }
   }

   /// <summary>
   /// Get Default App Configuration.
   /// </summary>
   /// <returns></returns>
   public static AppConfig GetDefaultConfiguration()
   {
      var cstring = AppSettings.GetSectionString(
         "DefaultConnectionString", CONFIG_APP_SECTION);
      var buri = AppSettings.GetSectionString(
         "CatalogServiceBaseUri", CONFIG_APP_SECTION);

      AppConfig config = new AppConfig("DEBUG", cstring, buri);

      return config;
   }

   /// <summary>
   /// Application Initialization.
   /// </summary>
   /// <param name="mainWindow">Main Window instance</param>
   /// <param name="localizer"></param>
   /// <param name=""></param>
   public static void AppInitialization(Window mainWindow, IStringLocalizer localizer)
   {
      Window = mainWindow;
      AppConfig config = AppSession.GetDefaultConfiguration();
      AppModelState state = new AppModelState()
      {
         Localizer = localizer,
         //Navigator = navigator,
         AppOptions = config
      };

      AppSession.ApplicationState = state;
   }

   /// <summary>
   /// Get File.
   /// </summary>
   /// <param name="extensions">(optional) file extensions</param>
   /// <returns></returns>
   public static async Task<StorageFile?> GetFileAsync(string extensions = "*")
   {
      var filePicker = new FileOpenPicker();
      var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Window);
      WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);
      filePicker.FileTypeFilter.Add(extensions);
      var file = await filePicker.PickSingleFileAsync();
      return file;
   }

   /// <summary>
   /// Get Folder.
   /// </summary>
   /// <param name="extensions">(optional) file extensions</param>
   /// <returns></returns>
   public static async Task<StorageFolder?> GetFolderAsync(
       string extensions = "*")
   {
      var folderPicker = new FolderPicker();
      var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Window);
      WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
      var folder = await folderPicker.PickSingleFolderAsync();
      return folder;
   }

}
