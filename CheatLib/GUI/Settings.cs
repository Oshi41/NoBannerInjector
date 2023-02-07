using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheatLib.Properties;

namespace CheatLib
{
    public partial class Settings : UserControl, IRefresh
    {
        private readonly DelayWorker _maxZoomWorker;
        private readonly DelayWorker _espLoader;
        private readonly TreeView _shadowCopy = new TreeView();

        public Settings()
        {
            InitializeComponent();
            LanguageSwitcher.RegisterLanguageSwitcher(this);
            _maxZoomWorker = new DelayWorker(OnMaxZoomChanged);
            _espLoader = new DelayWorker(RefreshEsp);
        }

        public async void RefreshControl()
        {
            groupBox1.Text = Resources.auto_pick_up;

            auto_pickup.Text = Resources.auto_f;
            toolTip1.SetToolTip(auto_pickup, Resources.pickup_tooltip);

            auto_treassure.Text = Resources.auto_treasure;
            toolTip1.SetToolTip(auto_treassure, Resources.auto_treassure_tootip);

            groupBox2.Text = Resources.camera_zoom;
            max_zoom.Text = Resources.max_zoom;
            toolTip1.SetToolTip(max_zoom, Resources.max_zoom_tooltip);
            setMaxZoomValue(null, null);

            enable_esp.Text = Resources.enable;
            _espLoader.Schedule(TimeSpan.FromSeconds(1));
        }

        private void auto_pickup_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Auto pickup=" + auto_pickup.Checked);
        }

        private void auto_treassure_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Auto treassure=" + auto_treassure.Checked);
        }

        private void max_zoom_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Enabled = max_zoom.Checked;
            Console.WriteLine("Camera zoom=" + max_zoom.Checked);
        }

        private double FromTrackBar()
        {
            var str = (trackBar1.Value / 1000.0).ToString("0.000");
            var res = Double.Parse(str);
            return res;
        }

        private void setMaxZoomValue(object sender, EventArgs e)
        {
            label1.Text = string.Format(Resources.zoom_scale, FromTrackBar());
            trackBar1.Enabled = max_zoom.Checked;
            if (e != null)
                _maxZoomWorker.Schedule(TimeSpan.FromMilliseconds(500));
        }

        private void OnMaxZoomChanged()
        {
            Invoke((MethodInvoker)(() => { Console.WriteLine("Max zoom=" + FromTrackBar()); }));
        }

        private void enable_esp_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Esp enabled=" + enable_esp.Checked);
        }

        private async void RefreshEsp()
        {
            Invoke((MethodInvoker)(() =>
            {
                _shadowCopy.Nodes.Clear();
                treeView1.Nodes.Clear();
            }));

            await MapManager.INSTANCE.LoadAll();
            var categories = MapManager.INSTANCE.Categories.ToList();

            var source = new TaskCompletionSource<bool>();

            if (treeView1.ImageList?.Images?.Count == null || treeView1.ImageList.Images.Count < 1)
            {
                var tasks = new List<Task>();
                var images = new ConcurrentDictionary<int, Image>();
                foreach (var cat in InjectorUtils.Traverse(categories, x => x.Children)
                             .Where(x => Uri.IsWellFormedUriString(x.ImageUrl, UriKind.Absolute)))
                {
                    tasks.Add(Task.Factory.StartNew(async () =>
                    {
                        var resp = await WebRequest.CreateHttp(cat.ImageUrl).GetResponseAsync();
                        var stream = resp.GetResponseStream();
                        var image = Image.FromStream(stream);
                        images.TryAdd(cat.Id, image);
                    }));
                }

                await Task.WhenAll(tasks);
                Invoke((MethodInvoker)(() =>
                {
                    treeView1.BeginUpdate();
                    treeView1.ImageList = new ImageList();
                    foreach (var key in images.Keys.ToList())
                    {
                        var image = images[key];
                        treeView1.ImageList.Images.Add(key.ToString(), image);
                    }

                    treeView1.EndUpdate();
                }));
            }

            void Fill(TreeNodeCollection collection, IEnumerable<Category> list)
            {
                foreach (var category in list)
                {
                    var imageKey = category.Id.ToString();
                    var node = new TreeNode(category.Name)
                    {
                        ImageKey = imageKey,
                        SelectedImageKey = imageKey,
                        StateImageKey = imageKey,
                        BackColor = string.IsNullOrEmpty(category.ImageUrl) ? Color.Transparent : Color.Gray,
                        Tag = category,
                        ToolTipText = category.Name,
                    };
                    collection.Add(node);
                    Fill(node.Nodes, category.Children);
                }
            }

            Invoke((MethodInvoker)(() =>
            {
                Fill(_shadowCopy.Nodes, categories);
                esp_search_TextChanged(null, null);
                source.SetResult(true);
            }));

            await source.Task;
        }

        private void esp_search_TextChanged(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();

            TreeNode Clone(TreeNode source, IEnumerable<TreeNode> children = null)
            {
                var copy = new TreeNode
                {
                    Text = source.Text,
                    ImageKey = source.ImageKey,
                    SelectedImageKey = source.ImageKey,
                    StateImageKey = source.ImageKey,
                    BackColor = source.BackColor,
                    Tag = source.Tag,
                    ToolTipText = source.ToolTipText,
                };
                children?.ToList().ForEach(x => copy.Nodes.Add(Clone(x)));
                return copy;
            }

            List<TreeNode> FindByString(string search, TreeNodeCollection collection)
            {
                var result = new List<TreeNode>();
                foreach (TreeNode n in collection)
                {
                    var children = FindByString(search, n.Nodes);
                    if (children.Any() || string.IsNullOrEmpty(search) || search.Contains(n.Text) ||
                        n.Text.Contains(search))
                    {
                        var clone = Clone(n, children);
                        result.Add(clone);
                    }
                }

                return result;
            }

            var root = FindByString(esp_search.Text, _shadowCopy.Nodes);
            root.ForEach(x => treeView1.Nodes.Add(x));
        }
    }
}