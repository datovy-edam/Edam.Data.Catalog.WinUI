using Edam.Data.CatalogDb;
using Edam.Data.CatalogService;
using Edam.Data.CatalogModel;
using szer = Edam.Serialization;
using Edam.DataObjects.Requests;
using Edam.DataObjects.Objects;
using Newtonsoft;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddCors();

//builder.Services.AddControllersWithViews()
//    .AddNewtonsoftJson(options =>
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//);


//builder.AddSqlServerDbContext<CatalogContext>("CatalogDb");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// setup service container 
WebAppService appService = new WebAppService(app);

#region -- 1.50 - Initialization and Session Management

// this should be called first...
app.MapGet("/catalogservice/session/info", (
   string sessionId, string containerId) =>
{
   var container = appService.CatalogSystem.SetContainer(sessionId, containerId);
   return container;
});

#endregion
#region -- 4.00 - Container Support

// get container info
app.MapGet("/catalogservice/container/info", (
   string sessionId, string containerId) =>
{
   var container = appService.CatalogSystem.SetContainer(sessionId, containerId);
   return container;
});

// get container info
app.MapGet("/catalogservice/container/id", (
   string sessionId, string id) =>
{
   var container = appService.CatalogSystem.Instance.GetContainer(id);
   return container;
});

// get container items
app.MapGet("/catalogservice/container/items/id", (
   string sessionId, Guid id) =>
{
   var item = appService.CatalogSystem.Instance.GetContainerItems(id);
   return item;
});

// get container info
app.MapGet("/catalogservice/container/item/root/id", (
   string sessionId, Guid id) =>
{
   var container = appService.CatalogSystem.Instance.GetContainerRootItem(id);
   return container;
});

// get container list
app.MapGet("/catalogservice/container/list", (string sessionId) =>
{
   WebAppService.SetupSession(sessionId);
   if (!WebAppService.VerifySessionId(sessionId))
   {
      return new List<ContainerInfo>();
   }
   return appService.CatalogSystem.GetContainerList();
});

// get container enlisted details
app.MapGet("/catalogservice/container/enlist", (
   string sessionId, string containerId, string description) =>
{
   WebAppService.SetupSession(sessionId);
   if (!WebAppService.VerifySessionId(sessionId))
   {
      return new ContainerInfo();
   }
   return appService.CatalogSystem.Instance.EnlistContainer(
      containerId, description);
});

// get container delisted details
app.MapGet("/catalogservice/container/delist", (
   string sessionId, string containerId, string description, 
   string? status = null) =>
{
   WebAppService.SetupSession(sessionId);
   if (!WebAppService.VerifySessionId(sessionId))
   {
      return new ContainerInfo();
   }
   return appService.CatalogSystem.Instance.DelistContainer(containerId);
});

#endregion
#region -- 4.00 - Catalog Item Support

// post catalog item
app.MapPost("/catalogservice/catalog/item", (
   string sessionId, ItemInfo item) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   var aitem = appService.CatalogSystem.Instance.AddItem(item);
   ritem = szer.JsonSerializer.Serialize<ItemInfo>(aitem);
   return ritem;
});

// get catalog item
app.MapGet("/catalogservice/catalog/item/id", (
   string sessionId, Guid id) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   ItemInfo item = appService.CatalogSystem.Instance.GetItem(id);

   return item;
});

// get catalog item
app.MapGet("/catalogservice/catalog/item/path", (
   string sessionId, string path) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   ItemInfo item = appService.CatalogSystem.Instance.GetItemByPath(path);

   return item;
});

// get catalog item
app.MapDelete("/catalogservice/catalog/item/id", (
   string sessionId, Guid id) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   RequestResponseInfo response = new RequestResponseInfo();
   try
   {
      appService.CatalogSystem.Instance.DeleteItem(id);
      response.Success = true;
      response.Status = RequestStatus.Completed;
   }
   catch(Exception ex)
   {
      response.Success = false;
      response.SessionId = sessionId;
      response.Status = RequestStatus.Failed;
   }
   //ritem = szer.JsonSerializer.Serialize<ItemInfo>(aitem);
   return response;
});

#endregion
#region -- 4.00 - Manage Branches and Leafs

// get banch
app.MapGet("/catalogservice/catalog/branch/items", (
   string sessionId, string path) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   List<ItemInfo> item = appService.CatalogSystem.Instance.GetBranch(path);

   return item;
});

#endregion
#region -- 4.00 - Catalog Data Item Support

// post catalog item
app.MapPost("/catalogservice/catalog/data/item", (
   string sessionId, ItemDataInfo itemData) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   var aitem = appService.CatalogSystem.Instance.AddItem(itemData);
   ritem = szer.JsonSerializer.Serialize<ItemDataInfo>(aitem);
   return ritem;
});

// get catalog item
app.MapGet("/catalogservice/catalog/data/items/id", (
   string sessionId, Guid id) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   List<ItemDataInfo> items = appService.CatalogSystem.Instance.GetItemData(id);

   return items;
});

// get catalog item
app.MapGet("/catalogservice/catalog/data/item/name", (
   string sessionId, Guid itemId, string name) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   ItemDataInfo items = 
      appService.CatalogSystem.Instance.GetDataByName(itemId, name);

   return items;
});

// get catalog item
app.MapGet("/catalogservice/catalog/data/item/id", (
   string sessionId, Guid id) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   List<ItemDataInfo> items =
      appService.CatalogSystem.Instance.GetItemData(id);

   return items;
});

// get catalog item
app.MapDelete("/catalogservice/catalog/data/item/id", (
   string sessionId, Guid id) =>
{
   string ritem = null;
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   RequestResponseInfo response = new RequestResponseInfo();
   try
   {
      appService.CatalogSystem.Instance.DeleteItemData(id);
      response.Success = true;
      response.Status = RequestStatus.Completed;
   }
   catch (Exception ex)
   {
      response.Success = false;
      response.SessionId = sessionId;
      response.Status = RequestStatus.Failed;
   }
   //ritem = szer.JsonSerializer.Serialize<ItemInfo>(aitem);
   return response;
});

// get catalog item
app.MapDelete("/catalogservice/catalog/data/id", (
   string sessionId, Guid id) =>
{
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   RequestResponseInfo response = new RequestResponseInfo();
   try
   {
      appService.CatalogSystem.Instance.DeleteData(id);
      response.Success = true;
      response.Status = RequestStatus.Completed;
   }
   catch (Exception ex)
   {
      response.Success = false;
      response.SessionId = sessionId;
      response.Status = RequestStatus.Failed;
   }
   //ritem = szer.JsonSerializer.Serialize<ItemInfo>(aitem);
   return response;
});

#endregion
#region -- 4.00 - Manage Other Requests...

// get catalog item
app.MapGet("/catalogservice/catalog/content/type/id", (
   string sessionId, string contentTypeId) =>
{
   WebAppService.SetupSession(sessionId);
   //if (!WebAppService.VerifySessionId(sessionId))
   //{
   //   return null;
   //}

   ContentTypeInfo contentType =
      appService.CatalogSystem.Instance.GetContentType(contentTypeId);

   return contentType;
});

#endregion

app.UseCors(cors => cors
   .AllowAnyMethod()
   .AllowAnyHeader()
   .SetIsOriginAllowed(origin => true)
   .AllowCredentials()
);

app.UseDeveloperExceptionPage();
//app.MapDefaultEndpoints();
app.Run();
