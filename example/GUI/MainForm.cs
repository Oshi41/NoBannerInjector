using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using HookManager;

namespace example
{
    public partial class Form1 : Form
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

            AuthChain();
        }

        public async Task AuthChain()
        {
            while (true)
            {
                if (AuthServiceExtensions.HasAuth)
                {
                    await Task.Delay(1000 * 20);
                    continue;
                }

                new AuthForm().ShowDialog(this);
            }
        }
    }
}