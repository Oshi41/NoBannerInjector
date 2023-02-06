using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheatLib.Properties;

namespace CheatLib
{
    public partial class Settings : UserControl, IRefresh
    {
        private readonly DelayWorker _maxZoomWorker;
        private readonly DelayWorker _espLoader;

        public Settings()
        {
            InitializeComponent();
            LanguageSwitcher.RegisterLanguageSwitcher(this);
            _maxZoomWorker = new DelayWorker(OnMaxZoomChanged);
            _espLoader = new DelayWorker(RefreshEsp);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RefreshControl();
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
            await MapManager.INSTANCE.LoadAll();
            var points = MapManager.INSTANCE.Points;
            var categories = MapManager.INSTANCE.Categories;
        }
    }
}