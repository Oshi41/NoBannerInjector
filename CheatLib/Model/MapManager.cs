using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CheatLib.Extensions;

namespace CheatLib.Model;

public enum WorldType
{
    Tayvat = 2,
    Enkamomika = 7,
    Chasm = 9,
}

public class Category
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }
    public bool Checked { get; set; }
    public List<Category> Children { get; set; } = new List<Category>();
}

public class Point
{
    public int Id { get; set; }
    public WorldType World { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string Label { get; set; }
    public string ImageUrl { get; set; }
}

public static class MapManager
{
    private static readonly string _points_file;
    private static readonly string _categories_file;
    private static readonly string _images_folder;

    static MapManager()
    {
        _points_file = LibExtensions.Relative("settings", "points.json");
        _categories_file = LibExtensions.Relative("settings", "categories.json");
        _images_folder = LibExtensions.Relative("data", "images");
    }

    public static async Task<List<Category>> LoadCategories()
    {
    }

    #region Categories

    private static async Task<List<Category>> LoadCategories(WorldType type, CultureInfo info)
    {
        var url = "https://sg-public-api-static.hoyolab.com/common/map_user/ys_obc/v1/map/label/tree?map_id="
                  + (int)type + "&app_sn=ys_obc&lang=" + info.ToString().ToLower();
        var (success, result) = await WebExtensions.GetJson(url);
        return null
    }

    #endregion
}