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

        public ConcurrentBag<Category> Children { get; set; } = new ConcurrentBag<Category>();
    }

    public class MapManager
    {
        public static MapManager INSTANCE = new MapManager();
        private ConcurrentBag<Point> _points;
        private ConcurrentBag<Category> _categories;
        private readonly string[] _imageNames = { "icon", "image" };

        public IReadOnlyList<Point> Points => _points.ToList().AsReadOnly();
        public IReadOnlyList<Category> Categories => _categories.ToList().AsReadOnly();

        private MapManager()
        {
            _points = new ConcurrentBag<Point>();
            _categories = new ConcurrentBag<Category>();
            LoadAll();
        }

        public async Task LoadAll()
        {
            var lang = Thread.CurrentThread.CurrentUICulture;
            _categories = new ConcurrentBag<Category>();
            _points = new ConcurrentBag<Point>();
            var all_worlds = Enum.GetValues(typeof(World)).OfType<World>().ToList();
            var points = all_worlds.Select(x => LoadPoints(x, lang)).ToList();
            var categories = all_worlds.Select(x => LoadCategories(x, lang)).ToList();

            await Task.WhenAll(points.OfType<Task>().Union(categories.OfType<Task>()));

            foreach (var point in points.SelectMany(x => x.Result))
                _points.Add(point);

            MargeCategories(categories.SelectMany(x => x.Result), _categories);
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
                ImageUrl = _imageNames.Select(x => token[x]?.Value<string>())
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? ""
            };

            var children = token["children"]?.AsEnumerable().Select(Parse)?.ToList();
            children?.ForEach(x => cat.Children.Add(x));

            return cat;
        }

        private void MargeCategories(IEnumerable<Category> resp, ConcurrentBag<Category> source)
        {
            foreach (var category in resp)
            {
                var other = source?.FirstOrDefault(x => x.Id == category.Id);
                if (other == null)
                {
                    other = new Category
                    {
                        Name = category.Name,
                        Id = category.Id,
                        ImageUrl = category.ImageUrl
                    };
                    source.Add(other);
                }

                if (category.Children.Any())
                {
                    MargeCategories(category.Children, other.Children);
                }
            }
        }
    }
}