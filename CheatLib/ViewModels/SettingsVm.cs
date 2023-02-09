using CheatLib.Properties;
using Mvvm;

namespace CheatLib.ViewModels;

public class SettingsVm : BindableBase
{
    private bool _enableAutoLoot;
    private bool _enableAutoTreasure;
    private bool _enableMaxZoom;
    private double _maxZoom;
    private bool _isEnableEsp;
    public string Header => Resources.settings;

    public bool EnableAutoLoot
    {
        get => _enableAutoLoot;
        set => SetProperty(ref _enableAutoLoot, value);
    }

    public bool EnableAutoTreasure
    {
        get => _enableAutoTreasure;
        set => SetProperty(ref _enableAutoTreasure, value);
    }

    public bool EnableMaxZoom
    {
        get => _enableMaxZoom;
        set => SetProperty(ref _enableMaxZoom, value);
    }

    public double MaxZoom
    {
        get => _maxZoom;
        set => SetProperty(ref _maxZoom, value);
    }

    public bool IsEnableEsp
    {
        get => _isEnableEsp;
        set => SetProperty(ref _isEnableEsp, value);
    }
}