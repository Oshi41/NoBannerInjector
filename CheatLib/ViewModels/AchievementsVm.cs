using System.Globalization;
using CheatLib.Extensions;
using CheatLib.Properties;

namespace CheatLib.ViewModels;

public class AchievementsVm : WebPageViewModelBase
{
    public override string Header => Resources.achievements;

    protected override string OrigLink =>
        $"https://genshin-center.com/{(Resources.Culture ?? CultureInfo.CurrentUICulture).TwoLetterISOLanguageName.ToLower()}/achievements";

    public override string ChangeLangScript { get; } = LibExtensions.ScriptText("hide_tabs.js");
}