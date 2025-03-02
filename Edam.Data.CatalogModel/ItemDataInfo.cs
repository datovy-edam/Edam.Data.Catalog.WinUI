using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Edam.Data.CatalogModel;

[Table("ItemData")]
public class ItemDataInfo
{
   public const string PARTITION_DEFAULT = "default";

   [Key]
   public Guid Id { get; set; } = Guid.NewGuid();

   [ForeignKey(nameof(Item))]
   public Guid ItemId { get; set; }

   [JsonIgnore]
   public ItemInfo Item { get; set; }

   [MaxLength(80)]
   public string PartitionId { get; set; } = PARTITION_DEFAULT;

   [MaxLength(128)]
   public string Name { get; set; }

   [ForeignKey(nameof(ContentType))]
   public string ContentTypeId { get; set; }

   public virtual ContentTypeInfo ContentType { get; set; } =
      new ContentTypeInfo()
      {
         TypeId = ContainerInfo.TEXT_CONTENT_TYPE,
         Description = ContainerInfo.TEXT_CONTENT_TYPE.Replace("/", " ")
      };

   public byte[] Data { get; set; }

   /// <summary>
   /// String representation of the data. Check content type to ensure that you
   /// could call this method for binary arrays that are string encoded data.
   /// </summary>
   [NotMapped]
   public string DataText
   {
      get
      {
         return Encoding.UTF8.GetString(Data);
      }
      set { Data = Encoding.UTF8.GetBytes(value); }
   }

   public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset RecordStatusDate { get; set; } = DateTimeOffset.Now;

   [MaxLength(20)]
   public string RecordStatusCode { get; set; } = "Active";

   /// <summary>
   /// Create/Update file item data.
   /// </summary>
   /// <param name="itemId">Guid of parent item</param>
   /// <param name="name">unique data item name</param>
   /// <param name="dataId">data id</param>
   /// <param name="contentType">content type ID</param>
   /// <returns>file item instance is returned</returns>
   public static ItemDataInfo CreateDataLeaf(Guid itemId, string name,
      Guid? dataId = null, ContentTypeInfo? contentType = null)
   {
      var data = new ItemDataInfo();

      data.Id = dataId ?? data.Id;
      data.ItemId = itemId;
      data.Name = name;

      data.ContentType = contentType ?? null;
      data.ContentTypeId = contentType == null ? null : contentType.TypeId;

      return data;
   }

}
