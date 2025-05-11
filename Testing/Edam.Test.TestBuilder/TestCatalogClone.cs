using Edam.Data.CatalogModel;
using Edam.Data.CatalogService;
using Edam.Test.TestCatalogLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Test.TestBuilder;

[TestClass]
public sealed class TestCatalogClone
{

   [TestInitialize]
   public void InitializeInstances()
   {
      AppHelper.InitializeTest();
   }

   [TestMethod]
   public void TestClone()
   {
      // Note that we are not passing any file-system path and then it will use
      // the one specified in the appsettings file (see DefaultRootFileFolder).
      // it could be something like:
      //    "C:/Users/esobr/Documents/Edam.Studio/Edam.App.Other/Projects/"
      var client = CatalogFileSystemClient.GetClient(
         defaultContainerId: "remote-projects", 
         fileSystemPath: null); // fileSystemPath);

      CatalogInfo catalog = client.Catalog;
      ICatalogService service = catalog.CatalogService;

      // Where the source items will be placed within the target catalog?
      // Note that the default catalog may or may not contain this path and
      // we need to make sure is created then move all its content.
      string targetPath = "/Projects/PSJ.Courts/";

      // Within the source catalog look-up the source path...
      // Note that "/PSJ.Courts/" is the Project then parent folder of the
      // file (or leaf)...
      string sourcePath = 
         "/PSJ.Courts/Arguments/0001.Courts.ToDictionary.Args.json";
      var item = service.Item.GetItemByPath(sourcePath);

      // Finally... clone the data
      CatalogClone.CloneProjectLeaf(targetPath, item, 
         catalog.CatalogService, catalog.DefaultCatalogService);
   }

}

