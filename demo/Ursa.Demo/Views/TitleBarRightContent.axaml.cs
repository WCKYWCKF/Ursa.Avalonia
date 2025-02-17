using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Ursa.Demo.Views;

public partial class TitleBarRightContent : UserControl
{
    public TitleBarRightContent()
    {
        InitializeComponent();
        this.ActualThemeVariantChanged += TitleBarRightContent_ActualThemeVariantChanged;
    }

    private void TitleBarRightContent_ActualThemeVariantChanged(object? sender, System.EventArgs e)
    {
        text.Text = this?.ActualThemeVariant is null ? "ActualThemeVariant is null" : this.ActualThemeVariant?.Key as string ?? "ActualThemeVariant.Key is null";
    }

    private async void OpenRepository(object? sender, RoutedEventArgs e)
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null) return;
        var launcher = top.Launcher;
        await launcher.LaunchUriAsync(new Uri("https://github.com/irihitech/Ursa.Avalonia"));
    }

}