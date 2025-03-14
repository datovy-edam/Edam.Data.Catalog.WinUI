// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace CommunityToolkit.WinUI.Controls.Automation.Peers;

/// <summary>
/// Defines a framework element automation peer for the <see cref="SizerBase"/> controls.
/// </summary>
public partial class SizerAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SizerAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="SizerBase" /> that is associated with this <see cref="SizerAutomationPeer" />.
    /// </param>
    public SizerAutomationPeer(SizerBase owner)
        : base(owner)
    {
    }

    private SizerBase OwningSizer
    {
        get
        {
            return (Owner as SizerBase)!;
        }
    }

    /// <summary>
    /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
    /// differentiates the control represented by this AutomationPeer.
    /// </summary>
    /// <returns>The string that contains the name.</returns>
    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    /// <summary>
    /// Called by GetName.
    /// </summary>
    /// <returns>
    /// Returns the first of these that is not null or empty:
    /// - Value returned by the base implementation
    /// - Name of the owning ContentSizer
    /// - ContentSizer class name
    /// </returns>
    protected override string GetNameCore()
    {
        string name = AutomationProperties.GetName(this.OwningSizer);
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        name = this.OwningSizer.Name;
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        name = base.GetNameCore();
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        return string.Empty;
    }
}
