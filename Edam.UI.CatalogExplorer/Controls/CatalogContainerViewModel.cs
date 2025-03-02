using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

using Edam.Data.CatalogModel;
using Edam.Diagnostics;
using Edam.UI.Catalog.Models;

namespace Edam.UI.Catalog.Controls;

public class CatalogContainerViewModel : ObservableObject
{

    public CatalogViewModel CatalogBase { get; set; }

    private Visibility _containerEditVisibility;
    public Visibility ContainerEditVisibility
    {
        get { return _containerEditVisibility; }
        set
        {
            if (_containerEditVisibility != value)
            {
                _containerEditVisibility = value;
                OnPropertyChanged(nameof(ContainerEditVisibility));
            }
        }
    }

    public ObservableCollection<ContainerItem> DataSource { get; set; } =
        new ObservableCollection<ContainerItem>();

    public CatalogContainerViewModel()
    {
        ContainerEditVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Add Container using submitted name.
    /// </summary>
    /// <param name="name">container name</param>
    /// <returns>EventCode is returned (Success or InsertUpdateFailed)</returns>
    public EventCode AddContainer(string name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            return EventCode.NameExpectedNoneFound;
        }

        EventCode added = EventCode.Success;
        var description = Edam.Text.Convert.ToProperCase(name);
        var container = CatalogBase.Catalog.CatalogService.
            EnlistContainer(name, description);
        if (container == null || 
            String.IsNullOrWhiteSpace(container.ContainerId))
        {
            var results = new ResultLog();
            results.Failed(EventCode.InsertUpdateFailed);
        }
        else
        {
            var item = new ContainerItem();
            item.Container = container;
            DataSource.Add(item);
        }
        return added;
    }

    /// <summary>
    /// Initialize Catalog
    /// </summary>
    public async Task InitializeContainersAsync()
    {
        DataSource.Clear();
        var lst = await CatalogBase.Catalog.CatalogService.GetContainersAsync();

        foreach(var item in lst)
        {
            var container = new ContainerItem();
            container.Container = item;
            DataSource.Add(container);
        }
    }

}



