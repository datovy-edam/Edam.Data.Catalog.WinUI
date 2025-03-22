using Edam.Data.CatalogModel;
using Edam.Data.CatalogService;
using Edam.Test.TestCatalogLibrary;

// -----------------------------------------------------------------------------

namespace Edam.Test.TestBuilder;

[TestClass]
public sealed class TestFileSystemClient
{
   public const string TEMP_TEST_FOLDER = "d:/temp/test/";

   [TestInitialize]
   public void InitializeInstances()
   {
      AppHelper.InitializeTest();
   }

   [TestMethod]
   public void TestCatalogClientInitialization()
   {
      string fileSystemPath = TEMP_TEST_FOLDER;
      var client = CatalogFileFolderClient.GetClient(fileSystemPath);
      Assert.IsNotNull(client);
   }

   [TestMethod]
   public void TestGetItemData()
   {
      string fileSystemPath = TEMP_TEST_FOLDER;
      var client = CatalogFileFolderClient.GetClient(fileSystemPath);
      client.Item.GetBranch("");
   }

}
