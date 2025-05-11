using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Threading.Tasks;

using Edam.Data.CatalogModel;
using Edam.Data.CatalogService;
using Edam.Test.TestCatalogLibrary;

// -----------------------------------------------------------------------------

namespace Edam.Test.TestBuilder;

[TestClass]
public sealed class TestFileSystemClient
{
   public const string TEMP_TEST_FOLDER = "d:/temp/test/";
   public const string FOLDER_NEW_BRANCH = "/folder 1/newBranch";
   public const string CHILD_FOLDER = "/folder 1/child 1";

   [TestInitialize]
   public void InitializeInstances()
   {
      AppHelper.InitializeTest();
   }

   [TestMethod]
   public void TestCatalogClientInitialization()
   {
      string fileSystemPath = TEMP_TEST_FOLDER;

      var client = CatalogFileSystemClient.GetClient(fileSystemPath);

      Assert.IsNotNull(client);
   }

   [TestMethod]
   public void TestBranchMethods()
   {
      string fileSystemPath = TEMP_TEST_FOLDER;

      var client = CatalogFileSystemClient.GetClient(fileSystemPath);

      // create branch
      var itm1 = client.Item.CreateBranch(FOLDER_NEW_BRANCH);
      var itm2 = client.Item.CreateBranch(CHILD_FOLDER + "/anotherNewBranch");

      var itm3 = client.Item.GetBranch(FOLDER_NEW_BRANCH);
      Assert.AreEqual(itm3.Count, 1);

      // delete folder
      var itm4 = client.Item.GetBranch(CHILD_FOLDER);
      Assert.AreEqual(itm4.Count, 1);
      client.Item.DeleteItem(itm4[0].Id);
   }

   [TestMethod]
   public void TestItemDataMethods()
   {
      string fileSystemPath = TEMP_TEST_FOLDER;

      var client = CatalogFileSystemClient.GetClient(fileSystemPath);

      var itm1 = client.Item.CreateBranch(
         FOLDER_NEW_BRANCH + "/testSample.txt");
      var pitem = new CatalogPathItem(itm1);
      var ditem = pitem.ToItemData("text sample");

      client.ItemData.AddItem(ditem);

      var itemData = client.ItemData.GetItemData(ditem.ItemId);
      var status = client.ItemData.DeleteItemData(ditem.ItemId);
   }

}
