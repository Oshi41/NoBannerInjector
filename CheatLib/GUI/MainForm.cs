using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CheatLib.Properties;
using HookManager;
using Microsoft.Scripting.Utils;

namespace CheatLib
{
    public partial class Form1 : Form, IRefresh
    {
        private readonly SystemHotKey _manager;

        public Form1()
        {
            InitializeComponent();

            _manager = new SystemHotKey(this.Handle);
            _manager.AddHotKey(0, 0, Keys.F1, () =>
            {
                var opactity = this.Opacity > 0 ? 0 : 100;
                this.Opacity = opactity;
                if (opactity > 0)
                {
                    this.Focus();
                    this.TopLevel = true;
                }
            });

            LanguageSwitcher.RegisterLanguageSwitcher(this);
            enToolStripMenuItem.Click += ChangeLanguage;
            ruToolStripMenuItem.Click += ChangeLanguage;
        }

        private void ChangeLanguage(object sender, EventArgs e)
        {
            if (sender is ToolStripItem item && item.Tag is CultureInfo info)
            {
                LanguageSwitcher.ChangeLanguage(info);
            }
        }

        public void RefreshControl()
        {
            tabPage1.Text = Resources.enka;
            tabPage2.Text = Resources.abyss;
            tabPage3.Text = Resources.debug;
            tabPage4.Text = Resources.achievements;
            tabPage5.Text = Resources.settings;
            toolStripMenuItem1.Text = Resources.file;
            toolStripMenuItem3.Text = Resources.language;
            toolStripMenuItem4.Text = Resources.refresh_tab;

            enToolStripMenuItem.Tag = CultureInfo.GetCultureInfo("en-us");
            enToolStripMenuItem.Text = (enToolStripMenuItem.Tag as CultureInfo).NativeName;

            ruToolStripMenuItem.Tag = CultureInfo.GetCultureInfo("ru-ru");
            ruToolStripMenuItem.Text = (ruToolStripMenuItem.Tag as CultureInfo).NativeName;

            foreach (var menuItem in toolStripMenuItem3.DropDownItems.OfType<ToolStripMenuItem>())
            {
                menuItem.Checked = Equals(menuItem.Tag, Thread.CurrentThread.CurrentUICulture);
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            foreach (IRefresh page in tabControl1.TabPages.OfType<TabPage>().Select(x => x.Controls[0]).OfType<IRefresh>())
            {
                page.RefreshControl();
            }
        }
    }
}