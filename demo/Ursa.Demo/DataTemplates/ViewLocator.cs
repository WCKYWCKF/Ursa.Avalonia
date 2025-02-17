using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ursa.Demo.Pages;
using Ursa.Demo.ViewModels;

namespace Ursa.Demo.Converters;

public class ViewLocator : IDataTemplate
{
    bool _isfirst = true;

    public Control? Build(object? param)
    {
        if (param is null) return null;
        var name = param.GetType().Name.Replace("ViewModel", "");
        try
        {
            var type = Type.GetType("Ursa.Demo.Pages." + name);
            if (type != null)
            {
                return Activator.CreateInstance(type) as Control;
            }

            return GetViewByVM(param) ?? new TextBlock { Text = "Not Found : " + name };
        }
        catch (Exception e)
        {
            var reslut = _isfirst
                ? new TextBlock
                {
                    Text = $"Activator.CreateInstance failure ::{e}", TextWrapping = Avalonia.Media.TextWrapping.Wrap
                }
                : GetViewByVM(param) ?? new TextBlock { Text = "Not Found : " + name };
            _isfirst = false;
            return reslut;
        }
    }

    public bool Match(object? data)
    {
        return true;
    }

    private Control? GetViewByVM(object vm)
    {
        return vm switch
        {
            IntroductionDemoViewModel => new IntroductionDemo(),
            AutoCompleteBoxDemoViewModel => new AutoCompleteBoxDemo(),
            AvatarDemoViewModel => new AvatarDemo(),
            BadgeDemoViewModel => new BadgeDemo(),
            BannerDemoViewModel => new BannerDemo(),
            BreadcrumbDemoViewModel => new BreadcrumbDemo(),
            ButtonGroupDemoViewModel => new ButtonGroupDemo(),
            ClassInputDemoViewModel => new ClassInputDemo(),
            ClockDemoViewModel => new ClockDemo(),
            DatePickerDemoViewModel => new DatePickerDemo(),
            DateTimePickerDemoViewModel => new DateTimePickerDemo(),
            DialogDemoViewModel => new DialogDemo(),
            DisableContainerDemoViewModel => new DisableContainerDemo(),
            DividerDemoViewModel => new DividerDemo(),
            DrawerDemoViewModel => new DrawerDemo(),
            DualBadgeDemoViewModel => new DualBadgeDemo(),
            ElasticWrapPanelDemoViewModel => new ElasticWrapPanelDemo(),
            EnumSelectorDemoViewModel => new EnumSelectorDemo(),
            FormDemoViewModel => new FormDemo(),
            IconButtonDemoViewModel => new IconButtonDemo(),
            ImageViewerDemoViewModel => new ImageViewerDemo(),
            IPv4BoxDemoViewModel => new IPv4BoxDemo(),
            KeyGestureInputDemoViewModel => new KeyGestureInputDemo(),
            LoadingDemoViewModel => new LoadingDemo(),
            MarqueeDemoViewModel => new MarqueeDemo(),
            MessageBoxDemoViewModel => new MessageBoxDemo(),
            MultiComboBoxDemoViewModel => new MultiComboBoxDemo(),
            NavMenuDemoViewModel => new NavMenuDemo(),
            NotificationDemoViewModel => new NotificationDemo(),
            NumberDisplayerDemoViewModel => new NumberDisplayerDemo(),
            NumericUpDownDemoViewModel => new NumericUpDownDemo(),
            NumPadDemoViewModel => new NumPadDemo(),
            PaginationDemoViewModel => new PaginationDemo(),
            PinCodeDemoViewModel => new PinCodeDemo(),
            RangeSliderDemoViewModel => new RangeSliderDemo(),
            RatingDemoViewModel => new RatingDemo(),
            ScrollToButtonDemoViewModel => new ScrollToButtonDemo(),
            SelectionListDemoViewModel => new SelectionListDemo(),
            SkeletonDemoViewModel => new SkeletonDemo(),
            TagInputDemoViewModel => new TagInputDemo(),
            ThemeTogglerDemoViewModel => new ThemeTogglerDemo(),
            TimeBoxDemoViewModel => new TimeBoxDemo(),
            TimelineDemoViewModel => new TimelineDemo(),
            TimePickerDemoViewModel => new TimePickerDemo(),
            ToastDemoViewModel => new ToastDemo(),
            ToolBarDemoViewModel => new ToolBarDemo(),
            TreeComboBoxDemoViewModel => new TreeComboBoxDemo(),
            TwoTonePathIconDemoViewModel => new TwoTonePathIconDemo(),
            AspectRatioLayoutDemoViewModel => new AspectRatioLayoutDemo(),
            PathPickerDemoViewModel => new PathPickerDemo(),
            _ => null
        };
    }
}