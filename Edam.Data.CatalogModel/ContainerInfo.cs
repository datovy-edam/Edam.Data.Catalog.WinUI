using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.DataObjects.Trees;
using System.Text.Json.Serialization;

// -----------------------------------------------------------------------------

namespace Edam.Data.CatalogModel;

/// <summary>
/// Similar to a FileSystem drive.
/// </summary>
[Table("Container"), Index(nameof(ContainerId),IsUnique = true)]
public class ContainerInfo: ITreeContainer
{
   public const string CONTAINER_ID_DEFAULT = "default";
   public const string SCHEMA_CONTENT_TYPE_JSON = "application/json";
   public const string TEXT_CONTENT_TYPE = "text/plain";

   [Key]
   public Guid Id { get; set; } = Guid.NewGuid();

   [MaxLength(80)]
   public string ContainerId { get; set; } = CONTAINER_ID_DEFAULT;

   [MaxLength(1024)]
   public string Description { get; set; } = "Default";

   public ContainerType ContainerType { get; set; } = ContainerType.WebApi;

   [MaxLength(1024)]
   public string ContainerURI { get; set; } = "";

   public string ContentType { get; set; } = SCHEMA_CONTENT_TYPE_JSON;
   public string Catalog { get; set; } = "{}";

   [JsonIgnore]
   public virtual ICollection<ItemInfo> Items { get; set; }

   public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset RecordStatusDate { get; set; } = DateTimeOffset.Now;

   [MaxLength(20)]
   public string RecordStatusCode { get; set; } = "Active";

   [JsonIgnore, NotMapped]
   public string StatusCode
   {
      get { return RecordStatusCode; }
      set { RecordStatusCode = value; }
   }
}
