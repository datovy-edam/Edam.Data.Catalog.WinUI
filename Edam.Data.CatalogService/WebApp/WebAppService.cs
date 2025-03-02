using Edam.Application;
using System.Diagnostics;

namespace Edam.Data.CatalogService;

public class WebAppService : IDisposable
{
   public WebApplication? Application { get; }
   public CatalogSystem CatalogSystem { get; } = new();

   private static string? _sessionId;
   public static string SessionId
   {
      get { return _sessionId; }
   }

   public static bool IsInitialized { get; private set; }

   public WebAppService(WebApplication? application)
   {
      if (application == null) 
         throw new ArgumentNullException(nameof(application));
      Application = application;
   }

   /// <summary>
   /// For the meanwhile all sessionId's will be accepted. At a later time a
   /// Session will be registered, monitored, and its lifecycle managed until is
   /// not used, retired, or expire.
   /// </summary>
   /// <param name="sessionId">session Id</param>
   /// <returns>true if the session is valid</returns>
   public static bool VerifySessionId(string sessionId)
   {
      return true;
      //return _sessionId != sessionId;
   }

   /// <summary>
   /// If needed setup session ID.
   /// </summary>
   /// <param name="sessionId"></param>
   public static void SetupSession(string sessionId)
   {
      if (!IsInitialized)
      {
         _sessionId = sessionId;
         IsInitialized = true;
      }
   }

   /// <summary>
   /// Release resources.
   /// </summary>
   public void Dispose()
   {
      if (CatalogSystem != null)
      {
      }
   }

}
