using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CheatLib.Extensions;
using Microsoft.Xaml.Behaviors;

namespace CheatLib.Behaviors;

public class TabSwitchBehavior : Behavior<FrameworkElement>
{
    private static ConditionalWeakTable<TabControl, object> _queue = new();

    private TabControl _tabControl;

    private TabControl TabControl
    {
        get => _tabControl;
        set
        {
            if (TabControl != null)
            {
                TabControl.SelectionChanged -= OnSelection;
            }

            _tabControl = value;

            if (TabControl != null)
            {
                TabControl.SelectionChanged += OnSelection;
            }
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        this.AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.Loaded -= OnLoaded;
        base.OnDetaching();
        TabControl = null;
    }

    public static readonly DependencyProperty UpdateCommandProperty = DependencyProperty.Register(
        nameof(UpdateCommand), typeof(ICommand), typeof(TabSwitchBehavior), new PropertyMetadata(default(ICommand)));

    public ICommand UpdateCommand
    {
        get => (ICommand)GetValue(UpdateCommandProperty);
        set => SetValue(UpdateCommandProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TabControl = AssociatedObject.GetParent<TabControl>();
    }

    private void OnSelection(object sender, SelectionChangedEventArgs e)
    {
        if (TabControl is { SelectedContent: { } })
        {
            _queue.Remove(TabControl);
            _queue.Add(TabControl, TabControl.SelectedContent);
        }
    }
}