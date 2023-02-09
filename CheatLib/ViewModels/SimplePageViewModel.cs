using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using CheatLib.Extensions;
using CheatLib.Model;
using CheatLib.Properties;
using Microsoft.Xaml.Behaviors.Core;
using Mvvm;

namespace CheatLib.ViewModels;

public abstract class WebPageViewModelBase : BindableBase
{
    private string _address;

    public WebPageViewModelBase()
    {
        RefreshViewCommand = new ActionCommand(() =>
        {
            Address = OrigLink;
            this.OnPropertyChanged(nameof(ChangeLangScript));
            this.OnPropertyChanged(nameof(Header));
        });

        RefreshViewCommand.Execute(null);
    }

    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public virtual string ChangeLangScript { get; }

    public virtual string Header { get; }

    protected virtual string OrigLink { get; }

    public ICommand RefreshViewCommand { get; }

    protected string LoadDefaultScript()
    {
        var text = LibExtensions.ScriptText("lang_switch.js")
            .Replace("{0}", (Resources.Culture ?? CultureInfo.CurrentUICulture).TwoLetterISOLanguageName.ToLower());
        return text;
    }
}

class EnkaVm : WebPageViewModelBase
{
    protected override string OrigLink => "https://enka.network/u/" + Cookies.UUID;
    public override string Header => Resources.Enka;
    public override string ChangeLangScript => LoadDefaultScript();
}

class AbyssVm : WebPageViewModelBase
{
    protected override string OrigLink =>
        "https://act.hoyolab.com/app/community-game-records-sea/index.html?bbs_presentation_style=fullscreen&bbs_auth_required=true&v=330&gid=2&utm_source=hoyolab&utm_medium=tools";

    public override string Header => Resources.Abyss;
    public override string ChangeLangScript => LoadDefaultScript();
}