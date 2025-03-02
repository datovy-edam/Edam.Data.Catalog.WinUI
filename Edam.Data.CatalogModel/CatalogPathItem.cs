using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Edam.DataObjects.Medias;
using Edam.DataObjects.Trees;
using Edam.InOut;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Edam.Data.CatalogModel
{

   /// <summary>
   /// Manage a Catalog Path and related information...
   /// </summary>
   public class CatalogPathItem : FolderFileItemInfo
   {
      public MediaFormat MediaFormat { get; set; }
      public string ContentType { get; set; }
      public ItemInfo Item { get; private set; }
      public CatalogItemInfo? TreeItem { get; set; } = null;

      public new CatalogPathItem? Parent { get; set; } = null;

      /// <summary>
      /// Initialize a catalog path based on a file item.
      /// </summary>
      /// <param name="item"></param>
      public CatalogPathItem(ItemInfo item)
      {
         Item = item;
         FromFullPath(Item.FullPath);
      }

      /// <summary>
      /// To Item Data...
      /// </summary>
      /// <param name="data">data</param>
      /// <returns>an instance of ItemData is returned</returns>
      public ItemDataInfo ToItemData(byte[] data)
      {
         ItemDataInfo item = new ItemDataInfo();
         item.ContentTypeId = ContentType;
         //item.Item = Item;
         item.ItemId = Item.Id;
         item.Name = Item.Name;
         item.Data = data;

         return item;
      }

      /// <summary>
      /// Get Content Type based on MediaId and Content TypeId.
      /// </summary>
      /// <returns>instance of ContentType is returned</returns>
      public ContentTypeInfo GetContentType()
      {
         var contentType = new ContentTypeInfo();
         contentType.TypeId = ContentType;
         contentType.Description = MediaFormat.ToString();
         return contentType;
      }

      /// <summary>
      /// Setup Full Path details...
      /// </summary>
      /// <param name="path">path</param>
      public void FromFullPath(string path)
      {
         base.FromFullPath(path, null);
         Name = NameFull;

         if (Path == null)
         {
            Path = path;
            Full = Path;
         }
         else
         { 
            Path = Path.Replace("\\", "/").
               Substring(DriverName.Length).Replace("//", "/");
            Full = (Path + "/" + NameFull).Replace("//", "/");
         }

         MediaFormat = MediaContentTypeHelper.GetMediaFormat(Extension);
         if (!string.IsNullOrWhiteSpace(Extension) &&
            MediaFormat == MediaFormat.Unknown &&
            Item.ItemType != TreeItemType.Branch)
         {
            MediaFormat = MediaFormat.TextFile;
         }

         ContentType = MediaFormat == MediaFormat.Unknown ? string.Empty :
            MediaContentTypeHelper.ToContentTypeText(MediaFormat);

         if (MediaFormat == MediaFormat.Unknown ||
            string.IsNullOrWhiteSpace(Extension))
         {
            Extension = string.Empty;
            ExtensionName = string.Empty;
            Type = ItemType.Folder;
            Item.ItemType = TreeItemType.Branch;
         }
         else
         {
            Type = ItemType.File;
            Item.ItemType = TreeItemType.Leaf;
         }

      }

   }

}
