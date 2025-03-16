using Edam.Data.CatalogModel;
using Edam.Data.CatalogService;
using Edam.Test.TestCatalogLibrary;

namespace Edam.Test.TestBuilder;

[TestClass]
public sealed class TestFileSystemClient
{

   [TestInitialize]
   public void InitializeInstances()
   {
      AppHelper.InitializeTest();
   }

   [TestMethod]
   public void TestCatalogClientInitialization()
   {
      string fileSystemPath = "";
      var container = AppHelper.CatalogInstance.Container;
      CatalogFileSystemClient client = new CatalogFileSystemClient(
         Guid.NewGuid().ToString(), fileSystemPath, container);
   }

}
