using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.CatalogModel;

[Table("ContentType")]
public class ContentTypeInfo
{

   [Key, MaxLength(80)]
   public string TypeId { get; set; }
   [MaxLength(128)]
   public string? Description { get; set; }

   public ContentTypeInfo()
   {
   }

   public ContentTypeInfo(string contentTypeCode, string? description)
   {
      TypeId = contentTypeCode;
      Description = description;
   }  

}
