﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Irihi.Avalonia.Shared.Helpers;

namespace Ursa.Controls;

[TemplatePart(PART_PickerContainer, typeof(Grid))]
[TemplatePart(PART_HourSelector, typeof(DateTimePickerPanel))]
[TemplatePart(PART_MinuteSelector, typeof(DateTimePickerPanel))]
[TemplatePart(PART_SecondSelector, typeof(DateTimePickerPanel))]
[TemplatePart(PART_AmPmSelector, typeof(DateTimePickerPanel))]
[TemplatePart(PART_HourScrollPanel, typeof(Control))]
[TemplatePart(PART_MinuteScrollPanel, typeof(Control))]
[TemplatePart(PART_SecondScrollPanel, typeof(Control))]
[TemplatePart(PART_AmPmScrollPanel, typeof(Control))]
[TemplatePart(PART_FirstSeparator, typeof(Control))]
[TemplatePart(PART_SecondSeparator, typeof(Control))]
[TemplatePart(PART_ThirdSeparator, typeof(Control))]
public class TimePickerPresenter: TemplatedControl
{
    public const string PART_HourSelector = "PART_HourSelector";
    public const string PART_MinuteSelector = "PART_MinuteSelector";
    public const string PART_SecondSelector = "PART_SecondSelector";
    public const string PART_AmPmSelector = "PART_AmPmSelector";
    public const string PART_PickerContainer = "PART_PickerContainer";
    
    public const string PART_HourScrollPanel = "PART_HourScrollPanel";
    public const string PART_MinuteScrollPanel = "PART_MinuteScrollPanel";
    public const string PART_SecondScrollPanel = "PART_SecondScrollPanel";
    public const string PART_AmPmScrollPanel = "PART_AmPmScrollPanel";
    
    public const string PART_FirstSeparator = "PART_FirstSeparator";
    public const string PART_SecondSeparator = "PART_SecondSeparator";
    public const string PART_ThirdSeparator = "PART_ThirdSeparator";
    
    private DateTimePickerPanel? _hourSelector;
    private DateTimePickerPanel? _minuteSelector;
    private DateTimePickerPanel? _secondSelector;
    private DateTimePickerPanel? _ampmSelector;
    private Grid? _pickerContainer;
    private Control? _hourScrollPanel;
    private Control? _minuteScrollPanel;
    private Control? _secondScrollPanel;
    private Control? _ampmScrollPanel;
    private Control? _firstSeparator;
    private Control? _secondSeparator;
    private Control? _thirdSeparator;
    private bool _use12Clock;
    private bool _updateFromTimeChange;
    internal TimeSpan _timeHolder;
    
    
    public static readonly StyledProperty<bool> NeedsConfirmationProperty = AvaloniaProperty.Register<TimePickerPresenter, bool>(
        nameof(NeedsConfirmation));

    public bool NeedsConfirmation
    {
        get => GetValue(NeedsConfirmationProperty);
        set => SetValue(NeedsConfirmationProperty, value);
    }

    public static readonly StyledProperty<int> MinuteIncrementProperty = AvaloniaProperty.Register<TimePickerPresenter, int>(
        nameof(MinuteIncrement));

    public int MinuteIncrement
    {
        get => GetValue(MinuteIncrementProperty);
        set => SetValue(MinuteIncrementProperty, value);
    }

    public static readonly StyledProperty<int> SecondIncrementProperty = AvaloniaProperty.Register<TimePickerPresenter, int>(
        nameof(SecondIncrement));

    public int SecondIncrement
    {
        get => GetValue(SecondIncrementProperty);
        set => SetValue(SecondIncrementProperty, value);
    }

    public static readonly StyledProperty<TimeSpan?> TimeProperty = AvaloniaProperty.Register<TimePickerPresenter, TimeSpan?>(
        nameof(Time));

    public TimeSpan? Time
    {
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public static readonly StyledProperty<string> PanelFormatProperty = AvaloniaProperty.Register<TimePickerPresenter, string>(
        nameof(PanelFormat), defaultValue: "HH mm ss t");

    public string PanelFormat
    {
        get => GetValue(PanelFormatProperty);
        set => SetValue(PanelFormatProperty, value);
    }

    public event EventHandler<TimePickerSelectedValueChangedEventArgs>? SelectedTimeChanged; 

    static TimePickerPresenter()
    {
        PanelFormatProperty.Changed.AddClassHandler<TimePickerPresenter, string>((presenter, args) => presenter.OnPanelFormatChanged(args));
        TimeProperty.Changed.AddClassHandler<TimePickerPresenter, TimeSpan?>((presenter, args) => presenter.OnTimeChanged(args));
    }

    private void OnTimeChanged(AvaloniaPropertyChangedEventArgs<TimeSpan?> args)
    {
        _updateFromTimeChange = true;
        UpdatePanelsFromSelectedTime();
        _updateFromTimeChange = false;
        SelectedTimeChanged?.Invoke(this,
            new TimePickerSelectedValueChangedEventArgs(args.OldValue.Value, args.NewValue.Value));
    }

    private void OnPanelFormatChanged(AvaloniaPropertyChangedEventArgs<string> args)
    {
        var format = args.NewValue.Value;
        
        UpdatePanelLayout(format);
    }

    private void UpdatePanelLayout(string panelFormat)
    {
        var parts = panelFormat.Split(new[] { ' ', '-', ':' }, StringSplitOptions.RemoveEmptyEntries);
        var panels = new List<Control?>();
        foreach (var part in parts)
        {
            if (part.Length < 1) continue;
            if ((part.Contains('h') || part.Contains('H')) && !panels.Contains(_hourScrollPanel))
            {
                panels.Add(_hourScrollPanel);
                _use12Clock = part.Contains('h');
                _hourSelector?.SetValue(DateTimePickerPanel.ItemFormatProperty, part.ToLower());
                if (_hourSelector is not null)
                {
                    _hourSelector.MaximumValue = _use12Clock ? 12 : 23;
                    _hourSelector.MinimumValue = _use12Clock ? 1 : 0;
                }
            }
            else if (part[0] == 'm' && !panels.Contains(_minuteSelector))
            {
                panels.Add(_minuteScrollPanel);
                _minuteSelector?.SetValue(DateTimePickerPanel.ItemFormatProperty, part);
            }
            else if (part[0] == 's' && !panels.Contains(_secondScrollPanel))
            {
                panels.Add(_secondScrollPanel);
                _secondSelector?.SetValue(DateTimePickerPanel.ItemFormatProperty, part.Replace('s', 'm'));
            }
            else if (part[0] == 't' && !panels.Contains(_ampmScrollPanel))
            {
                panels.Add(_ampmScrollPanel);
                _ampmSelector?.SetValue(DateTimePickerPanel.ItemFormatProperty, part);
            }
        }
        if (panels.Count < 1) return;
        IsVisibleProperty.SetValue(false, _hourScrollPanel, _minuteScrollPanel, _secondScrollPanel, _ampmScrollPanel,
            _firstSeparator, _secondSeparator, _thirdSeparator);
        for(var i = 0; i< panels.Count; i++)
        {
            var panel = panels[i];
            if (panel is null) continue;
            panel.IsVisible = true;
            Grid.SetColumn(panel, 2 * i);
            var separator = i switch
            {
                0 => _firstSeparator,
                1 => _secondSeparator,
                2 => _thirdSeparator,
                _ => null,
            };
            if (i != panels.Count - 1) IsVisibleProperty.SetValue(true, separator);
        }
    }

    public TimePickerPresenter()
    {
        SetCurrentValue(TimeProperty, DateTime.Now.TimeOfDay);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_hourSelector is not null)
        {
            _hourSelector.SelectionChanged -= OnPanelSelectionChanged;
        }
        if (_minuteSelector is not null)
        {
            _minuteSelector.SelectionChanged -= OnPanelSelectionChanged;
        }
        if (_secondSelector is not null)
        {
            _secondSelector.SelectionChanged -= OnPanelSelectionChanged;
        }
        if (_ampmSelector is not null)
        {
            _ampmSelector.SelectionChanged -= OnPanelSelectionChanged;
        }
        _hourSelector = e.NameScope.Find<DateTimePickerPanel>(PART_HourSelector);
        _minuteSelector = e.NameScope.Find<DateTimePickerPanel>(PART_MinuteSelector);
        _secondSelector = e.NameScope.Find<DateTimePickerPanel>(PART_SecondSelector);
        _ampmSelector = e.NameScope.Find<DateTimePickerPanel>(PART_AmPmSelector);
        if(_hourSelector is not null)
        {
            _hourSelector.SelectionChanged += OnPanelSelectionChanged;
        }
        if(_minuteSelector is not null)
        {
            _minuteSelector.SelectionChanged += OnPanelSelectionChanged;
        }
        if(_secondSelector is not null)
        {
            _secondSelector.SelectionChanged += OnPanelSelectionChanged;
        }
        if(_ampmSelector is not null)
        {
            _ampmSelector.SelectionChanged += OnPanelSelectionChanged;
        }
        _pickerContainer = e.NameScope.Find<Grid>(PART_PickerContainer);
        _hourScrollPanel = e.NameScope.Find<Control>(PART_HourScrollPanel);
        _minuteScrollPanel = e.NameScope.Find<Control>(PART_MinuteScrollPanel);
        _secondScrollPanel = e.NameScope.Find<Control>(PART_SecondScrollPanel);
        _ampmScrollPanel = e.NameScope.Find<Control>(PART_AmPmScrollPanel);
        _firstSeparator = e.NameScope.Find<Control>(PART_FirstSeparator);
        _secondSeparator = e.NameScope.Find<Control>(PART_SecondSeparator);
        _thirdSeparator = e.NameScope.Find<Control>(PART_ThirdSeparator);
        Initialize();
        UpdatePanelLayout(PanelFormat);
        UpdatePanelsFromSelectedTime();
    }

    private void OnPanelSelectionChanged(object sender, System.EventArgs e)
    {
        if (_updateFromTimeChange) return;
        TimeSpan time = NeedsConfirmation ? _timeHolder : Time ?? DateTime.Now.TimeOfDay;
        int hour = _hourSelector?.SelectedValue ?? time.Hours;
        int minute = _minuteSelector?.SelectedValue ?? time.Minutes;
        int second = _secondSelector?.SelectedValue ?? time.Seconds;
        int ampm = _ampmSelector?.SelectedValue ?? (time.Hours >= 12 ? 1 : 0);
        if (_use12Clock)
        {
            hour = ampm switch
            {
                0 when hour == 12 => 0,
                1 when hour < 12 => hour + 12,
                _ => hour
            };
        }
        var newTime = new TimeSpan(hour, minute, second);
        if (NeedsConfirmation)
        {
            _timeHolder = newTime;
        }
        else
        {
            SetCurrentValue(TimeProperty, newTime);
        }
    }

    private void UpdatePanelsFromSelectedTime()
    {
        if (Time is null) return;
        var time = Time ?? DateTime.Now.TimeOfDay;
        if (_hourSelector is not null)
        {
            var index = _use12Clock ? time.Hours % 12 : time.Hours;
            if (index == 0) index = 12;
            _hourSelector.SelectedValue = index;
        }
        if (_minuteSelector is not null)
        {
            _minuteSelector.SelectedValue = time.Minutes;
        }
        if (_secondSelector is not null)
        {
            _secondSelector.SelectedValue = time.Seconds;
        }
        if (_ampmSelector is not null)
        {
            _ampmSelector.SelectedValue = time.Hours switch
            {
                >= 12 => 1,
                _ => 0
            };
            _ampmSelector.IsEnabled = _use12Clock;
        }
    }

    private void Initialize()
    {
        if (_pickerContainer is null) return;
        if (_hourSelector is not null)
        {
            _hourSelector.ItemFormat = "hh";
            _hourSelector.MaximumValue = _use12Clock ? 12 : 23;
            _hourSelector.MinimumValue = _use12Clock ? 1 : 0;
            
        }
        if(_minuteSelector is not null)
        {
            _minuteSelector.ItemFormat = "mm";
            _minuteSelector.MaximumValue = 59;
            _minuteSelector.MinimumValue = 0;
            
        }
        if(_secondSelector is not null)
        {
            _secondSelector.ItemFormat = "mm";
            _secondSelector.MaximumValue = 59;
            _secondSelector.MinimumValue = 0;
            
        }
        if(_ampmSelector is not null)
        {
            _ampmSelector.ItemFormat = "t";
            _ampmSelector.MaximumValue = 1;
            _ampmSelector.MinimumValue = 0;
        }
    }

    public void Confirm()
    {
        if (NeedsConfirmation)
        {
            SetCurrentValue(TimeProperty, _timeHolder);
        }
    }
}