using System.Windows;
using System.Windows.Controls;

namespace CheatLib.Controls;

public partial class HelpToggle : UserControl
{
    public HelpToggle()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        nameof(IsChecked), typeof(bool), typeof(HelpToggle), new PropertyMetadata(default(bool)));

    public bool IsChecked
    {
        get { return (bool)GetValue(IsCheckedProperty); }
        set { SetValue(IsCheckedProperty, value); }
    }

    public static readonly DependencyProperty HelpProperty = DependencyProperty.Register(
        nameof(Help), typeof(string), typeof(HelpToggle), new PropertyMetadata(default(string)));

    public string Help
    {
        get { return (string)GetValue(HelpProperty); }
        set { SetValue(HelpProperty, value); }
    }

    public static readonly DependencyProperty SettingsNameProperty = DependencyProperty.Register(
        nameof(SettingsName), typeof(string), typeof(HelpToggle), new PropertyMetadata(default(string)));

    public string SettingsName
    {
        get { return (string)GetValue(SettingsNameProperty); }
        set { SetValue(SettingsNameProperty, value); }
    }
}