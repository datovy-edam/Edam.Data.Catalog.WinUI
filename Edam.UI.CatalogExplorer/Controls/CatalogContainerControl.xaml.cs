using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Edam.UI.Catalog.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Converters;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Edam.UI.Catalog.Controls;

public sealed partial class CatalogContainerControl : UserControl
{
    public CatalogContainerViewModel ViewModel { get; set; } = 
        new CatalogContainerViewModel();

    public CatalogContainerControl()
    {
        this.InitializeComponent();
        this.DataContext = ViewModel;
        
    }

    #region -- 4.00 - Container Support

    private void ToggleContenerEditor()
    {
        ContainerEditor.Visibility =
            ContainerEditor.Visibility == Visibility.Visible ?
               Visibility.Collapsed : Visibility.Visible;
    }

    private void ContainerIdTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        var econtrol = sender as TextBox;
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var item = econtrol.DataContext as ContainerItem;
        }
    }

    private void ToggleContainerEditor_Click(object sender, PointerRoutedEventArgs e)
    {
        ToggleContenerEditor();
    }

    private void CancelContainerEditor_Click(object sender, PointerRoutedEventArgs e)
    {
        ContainerId.Text = string.Empty;
        ToggleContenerEditor();
    }

    private void SaveContainer_Click(object sender, PointerRoutedEventArgs e)
    {
        var results = ViewModel.AddContainer(ContainerId.Text);
        if (results == Diagnostics.EventCode.Success)
        {
            ToggleContenerEditor();
            ContainerId.Text = String.Empty;
        }
    }

    #endregion

}
