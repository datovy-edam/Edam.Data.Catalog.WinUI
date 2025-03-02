using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
using Monaco;

namespace Edam.UI.Catalog.Controls;

public sealed partial class MonacoEditorControl : UserControl
{

    private MonacoEditorViewModel _ViewModel = new MonacoEditorViewModel();
    public MonacoEditorViewModel ViewModel
    {
        get { return _ViewModel; }
    }

    public MonacoEditorControl()
    {
        this.InitializeComponent();
        this.DataContext = _ViewModel;
    }

    /// <summary>
    /// Initialize Editor Control by creating an instance if it don't exists,
    /// setting some properties and its view model that contains the content to
    /// be edited, and finally setup the Frame place holder with the instance.
    /// </summary>
    /// <param name="model">editor view model</param>
    /// <returns>task is returned</returns>
    public async Task InitializeEditorControlAsync(MonacoEditorViewModel model)
    {
        if (ViewModel.EditorInstance == null)
        {
            ViewModel.EditorInstance = await EditorPool.GetEditorInstance();
            ViewModel.EditorInstance.SetEditorMiniMapVisible(false);
            ViewModel.EditorInstance.MonacoEditorLoaded += EditorLoadedAsync;
            ViewModel.EditorInstance.Tag = model;

            model.EditorInstance = ViewModel.EditorInstance;
            EditorFrame.Content = ViewModel.EditorInstance as Control;
        }
    }

    /// <summary>
    /// User Control (Editor) Loaded event to set the content to be edited.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event args</param>
    public async void EditorLoadedAsync(object sender, EventArgs e)
    {
        ViewModel.EditorInstance.CreateModelAsync(
            _ViewModel.Content, _ViewModel.CurrentLanguage);
    }

    /// <summary>
    /// Set the content to be edited.
    /// </summary>
    /// <param name="content">content to be edited</param>
    /// <param name="language">content language</param>
    /// <returns>task is returned</returns>
    public async Task SetEditorAsync(string content, string language)
    {
        _ViewModel.CurrentLanguage = language;
        ViewModel.EditorInstance.CreateModelAsync(content, language);
    }

    /// <summary>
    /// Get edited content.
    /// </summary>
    /// <returns>task with the string that was edited is returned</returns>
    public async Task<string> GetContentAsync()
    {
        return await _ViewModel.EditorInstance.GetEditorContentAsync();
    }

}
