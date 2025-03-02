using Edam.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monaco;

/// <summary>
/// Manage the Editor Pool allowing to pre-instantiate editor controls and 
/// initialize and setup as needed before being used/fetched.
/// </summary>
public class EditorPool
{

   public static int POOL_SIZE = 5;
   private static List<Monaco.MonacoEditor> Pool { get; set; } =
       new List<Monaco.MonacoEditor>();

   #region -- 4.00 - Manage Editor Pool

   /// <summary>
   /// Initialize Editor Pool.
   /// </summary>
   private static async Task InitializePool()
   {
      try
      {
         for (var i = Pool.Count; i < POOL_SIZE; i++)
         {
            var editor = new Monaco.MonacoEditor();
            Pool.Add(editor);
            await editor.InitializeControlAsync();
         }
      }
      catch (Exception ex)
      {
         ResultLog.Trace(ex, "EditorPool::InitializePool");
      }
   }

   public static int AvailablePools()
   {
      return Pool.Count;
   }

   /// <summary>
   /// Get an Editor instance.
   /// </summary>
   /// <returns>return an Editor instance</returns>
   public static async Task<Monaco.MonacoEditor> GetEditorInstance()
   {
      // initialize pool if it is empty
      if (Pool.Count == 0)
      {
         await InitializePool();
      }

      var item = Pool[0];
      Pool.RemoveAt(0);

      // replenish the pool
      await InitializePool();

      return item;
   }

   #endregion

}
