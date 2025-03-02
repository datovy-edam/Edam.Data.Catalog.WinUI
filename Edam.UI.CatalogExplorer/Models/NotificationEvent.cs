using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Diagnostics;

namespace Edam.UI.Catalog.Models;

public class NotificationEventArgs : EventArgs
{
   public IResultsLog Results { get; set; } = null;
   public bool Managed { get; set; } = false;
   public string EventID { get; set; }
   public string Message { get; set; }
   public object Data { get; set; }
}

public delegate void NotificationEventHandler(
   object sender, NotificationEventArgs e);
