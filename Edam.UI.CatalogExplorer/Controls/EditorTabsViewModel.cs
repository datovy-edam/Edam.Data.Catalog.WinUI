using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edam.Data.CatalogModel;
using Edam.Diagnostics;

namespace Edam.UI.Catalog.Controls;

public class EditorTabsViewModel
{
    public CatalogViewModel? CatalogBase { get; set; } = null;
    public string DocumentName { get; set; }
    public CatalogPathItem CurrentPathItem { get; set; }
    public string CurrentLanguage { get; set; }
    public ObservableCollection<MonacoEditorViewModel> 
        EditorTabs { get; set; } = new 
           ObservableCollection<MonacoEditorViewModel>();

    /// <summary>
    /// Update Model.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>true if model was updated</returns>
    public async Task<bool> UpdateModel(MonacoEditorViewModel model)
    {
        bool done = false;
        string tbLabel = "Content [" + model.CurrentPathItem.Full + "]";
        string func = nameof(UpdateModel);
        string feedback = " was not saved...";
        SeverityLevel severity = SeverityLevel.Info;

        string content = await model.GetContentAsync();
        if (model.Content != content)
        {
            if (CatalogBase != null)
            {
                var itm = await CatalogBase.PostItemAsync(
                    CurrentPathItem.Full, model.Content);
                if (itm != null)
                {
                    feedback = " was saved...";
                }
                else
                {
                    feedback = " Catalog PostItemAsync failed...";
                    severity = SeverityLevel.Fatal;
                }
            }
            else
            {
                feedback = " Catalog Services interface was not found...";
                severity = SeverityLevel.Fatal;
            }
        }
        else
        {
            feedback = " not changed...";
        }

        done = severity != SeverityLevel.Info;
        ResultLog.Trace(tbLabel + feedback, func, severity);

        return done;
    }

}
