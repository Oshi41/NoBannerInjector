using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheatLib.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CheatLib
{
    public class LanguageSwitcher
    {
        private static readonly string _path = InjectorUtils.Relative("settings", "cfg.json");
        private static WeakCollection<IRefresh> _collection = new WeakCollection<IRefresh>();
        public static event EventHandler OnLanguageChanged;

        public static void ChangeLanguage(CultureInfo info)
        {
            Resources.Culture = info;
            Thread.CurrentThread.CurrentUICulture = info;
            OnLanguageChanged?.Invoke(null, null);
            foreach (var view in _collection)
                view.RefreshControl();

            try
            {
                var jsonRaw = InjectorUtils.ReadFile(_path, () => Task.FromResult("{}")).Result;
                var json = JsonConvert.DeserializeObject<JObject>(jsonRaw);
                json["lang"] = info.Name;
                File.WriteAllText(_path, json.ToString(Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot save lang settings");
                Console.WriteLine(e);
            }
        }

        public static void RegisterLanguageSwitcher(IRefresh refresh)
        {
            _collection.Add(refresh);
            if (refresh is UserControl control)
            {
                control.Load += (sender, args) => { refresh.RefreshControl(); };
            }
            if (refresh is Form form)
            {
                form.Load += (sender, args) => { refresh.RefreshControl(); };
            }
        }

        public static void LoadLanguage()
        {
            try
            {
                var path = InjectorUtils.Relative("settings", "cfg.json");
                var file = InjectorUtils.ReadFile(path).Result;
                var value = JsonConvert.DeserializeObject<JObject>(file)?.SelectToken("lang")?.Value<string>();
                var info = CultureInfo.GetCultureInfo(value);
                ChangeLanguage(info);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public interface IRefresh
    {
        void RefreshControl();
    }

    internal sealed class WeakCollection<T> : IEnumerable<T> where T : class
    {
        private const int DefaultCapacity = 4; // default capacity of List<T>

        private WeakReference[] _items = new WeakReference[DefaultCapacity];
        private int _size;

        public void Add(T t)
        {
            EnsureCapacity(_size + 1);
            _items[_size++] = new WeakReference(t);
        }

        private void EnsureCapacity(int size)
        {
            if (size > _items.Length)
            {
                // Clear out dead entires first; we might not have to resize it
                Compact();
                if (size > _items.Length)
                {
                    // Need to expand the list
                    int newSize = _items.Length * 2;
                    if (newSize < size)
                    {
                        newSize = size;
                    }

                    var newList = new WeakReference[newSize];
                    _items.CopyTo(newList, 0);
                    _items = newList;
                }
            }
        }

        private void Compact()
        {
            int newSize = 0;
            for (int i = 0; i < _size; i++)
            {
                if (_items[i].IsAlive)
                {
                    if (newSize < i)
                    {
                        _items[newSize] = _items[i];
                    }

                    newSize++;
                }
            }

            for (int i = newSize; i < _size; i++)
            {
                _items[i] = null;
            }

            _size = newSize;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                T light = (T)_items[i].Target;
                if (light != null)
                {
                    yield return light;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}