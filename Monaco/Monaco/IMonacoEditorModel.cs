using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monaco
{
   public interface IMonacoEditorModel
   {
      string CurrentLanguage { get; set; }
      string Content { get; set; }
   }
}
