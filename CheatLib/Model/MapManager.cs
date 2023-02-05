using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public class MapManager
    {
        public static MapManager INSTANCE = new MapManager();
        private readonly List<Point> _points;
        private readonly string _filePath;

        public IReadOnlyList<Point> Points => _points.AsReadOnly();

        private MapManager()
        {
            _points = new List<Point>();
            _filePath = InjectorUtils.Relative("settings", "map.json");
            LoadAll();
        }

        public async Task LoadAll()
        {
            _points.Clear();
            var lang = Thread.CurrentThread.CurrentUICulture;
            var tasks = Enum.GetValues(typeof(World)).OfType<World>().Select(x => Load(x, lang)).ToList();
            await Task.WhenAll(tasks);
            _points.AddRange(tasks.SelectMany(x => x.Result));
        }

        private async Task<List<Point>> Load(World world, CultureInfo cultureInfo)
        {
            try
            {
                var url = "https://sg-public-api-static.hoyolab.com/common/map_user/ys_obc/v1/map/point/list?map_id="
                          + $"{(int)world}&app_sn=ys_obc&lang={cultureInfo.ToString().ToLower()}";

                var json = await CookieManager.INSTANCE.GetJsonAsync(url);
                if (json["retcode"].Value<int>() != 0)
                    throw new Exception(json["message"].Value<string>());

                var langMap = json.SelectToken("data.label_list").AsJEnumerable()
                    .ToDictionary(label => label["name"].Value<int>());

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
                    _points.Add(p);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot get points for {world} world");
                Console.WriteLine(e);
            }

            return _points;
        }
    }
}