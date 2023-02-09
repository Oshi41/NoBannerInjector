using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Mvvm;

namespace CheatLib.ViewModels;

public class MainViewModel : BindableBase
{
    public ObservableCollection<BindableBase> Tabs { get; private set; } = new ObservableCollection<BindableBase>();

    public ICommand UpdateTabsCommand { get; }

    public MainViewModel()
    {
        UpdateTabsCommand = new ActionCommand(o =>
        {
            Tabs.Clear();
            Tabs.Add(new SettingsVm());
            Tabs.Add(new EnkaVm());
            Tabs.Add(new AbyssVm());
            Tabs.Add(new AchievementsVm());
        });
    }
}