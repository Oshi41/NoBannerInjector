using System;
using System.Windows.Forms;

namespace example
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            webView21.Source = new Uri(AuthServiceExtensions.AuthDomain);
            webView21.ContentLoading += Reload;
            webView21.CoreWebView2InitializationCompleted += Reload;
        }

        private void Reload(object sender = null, EventArgs e = null)
        {
            AuthServiceExtensions.Save(webView21.CoreWebView2?.CookieManager);
            if (!AuthServiceExtensions.HasAuth)
                webView21.Source = new Uri(AuthServiceExtensions.AuthDomain);
            else
                this.Close();
        }
    }
}