using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace CheatLib
{
    public enum World
    {
        Tayvat = 2,
        Enkamomika = 7,
        Chasm = 9,
    }

    public class Point
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// Point label
        /// </summary>
        public string Label { get; set; }

        public string ImageUrl { get; set; }
        public World World { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }

        public List<Category> Children { get; } = new List<Category>();
    }

    public class MapManager
    {
        public static MapManager INSTANCE = new MapManager();
        private readonly ConcurrentBag<Point> _points;
        private readonly ConcurrentBag<Category> _categories;
        private readonly string _filePath;

        public IReadOnlyList<Point> Points => _points.ToList().AsReadOnly();
        public IReadOnlyList<Category> Categories => _categories.ToList().AsReadOnly();

        private MapManager()
        {
            _points = new ConcurrentBag<Point>();
            _categories = new ConcurrentBag<Category>();
            _filePath = InjectorUtils.Relative("settings", "map.json");
            LoadAll();
        }

        public async Task LoadAll()
        {
            var lang = Thread.CurrentThread.CurrentUICulture;
            var tasks = new List<Task>();

            foreach (var world in Enum.GetValues(typeof(World)).OfType<World>())
            {
                tasks.Add(LoadPoints(world, lang).ContinueWith(task => task.Result.ForEach(x => _points.Add(x))));
                tasks.Add(
                    LoadCategories(world, lang).ContinueWith(task => MargeCategories(task.Result)));
            }

            await Task.WhenAll(tasks);
        }

        private async Task<List<Point>> LoadPoints(World world, CultureInfo cultureInfo)
        {
            var result = new List<Point>();
            JToken errored = null;
            try
            {
                var url = "https://sg-public-api-static.hoyolab.com/common/map_user/ys_obc/v1/map/point/list?map_id="
                          + $"{(int)world}&app_sn=ys_obc&lang={cultureInfo.ToString().ToLower()}";

                var json = await CookieManager.INSTANCE.GetJsonAsync(url);
                if (json["retcode"].Value<int>() != 0)
                    throw new Exception(json["message"].Value<string>());

                var langMap = json.SelectToken("data.label_list").AsJEnumerable()
                    .ToDictionary(label => label["id"].Value<int>());

                foreach (var point in json.SelectToken("data.point_list").AsJEnumerable())
                {
                    var labelToken = langMap[point["label_id"].Value<int>()];
                    var p = new Point
                    {
                        Id = point["id"].Value<int>(),
                        X = point["x_pos"].Value<double>(),
                        Y = point["y_pos"].Value<double>(),
                        Label = labelToken["name"].Value<string>(),
                        ImageUrl = labelToken["icon"].Value<string>()
                    };
                    result.Add(p);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot get points for {world} world");
                if (errored != null) Console.WriteLine($"Error here = {errored}");
                Console.WriteLine(e);
            }

            return result;
        }

        private async Task<List<Category>> LoadCategories(World world, CultureInfo cultureInfo)
        {
            var result = new List<Category>();

            try
            {
                var url = "https://sg-public-api-static.hoyolab.com/common/map_user/ys_obc/v1/map/label/tree?map_id="
                          + (int)world + "&app_sn=ys_obc&lang=" + cultureInfo.ToString().ToLower();

                var json = await CookieManager.INSTANCE.GetJsonAsync(url);
                if (json["retcode"].Value<int>() != 0)
                    throw new Exception(json["message"].Value<string>());

                foreach (var jToken in json.SelectToken("data.tree").AsJEnumerable())
                {
                    result.Add(Parse(jToken));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot get categories for {world} world");
                Console.WriteLine(e);
            }

            return result;
        }

        private Category Parse(JToken token)
        {
            var cat = new Category
            {
                Id = token["id"].Value<int>(),
                Name = token["name"].Value<string>(),
                ImageUrl = token["image"]?.Value<string>(),
            };

            var children = token["children"]?.AsEnumerable().Select(Parse)?.ToList();
            if (children?.Any() == true)
                cat.Children.AddRange(children);

            return cat;
        }

        private void MargeCategories(List<Category> resp, Category root = null)
        {
            foreach (var category in resp)
            {
                var source = root?.Children ?? _categories.AsEnumerable();
                var other = source?.FirstOrDefault(x => x.Id == category.Id);
                if (other == null)
                {
                    other = new Category
                    {
                        Name = category.Name,
                        Id = category.Id,
                        ImageUrl = category.ImageUrl
                    };

                    if (root?.Children != null)
                        root.Children.Add(other);
                    else _categories.Add(other);
                }

                if (category.Children.Any())
                {
                    MargeCategories(category.Children, other);
                }
            }
        }
    }
}