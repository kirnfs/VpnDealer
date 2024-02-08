using System.Windows.Forms;

namespace VpnDialer
{
    public partial class LogForm : Form
    {
        public static LogForm instance { get; set; }

        public LogForm()
        {
            InitializeComponent();
        }

        public static LogForm getInstance()
        {
            try
            {
                if (instance == null)
                {
                    instance = new LogForm();
                    return instance;
                }
                instance.Close(); instance = null; instance = new LogForm(); return instance;
            }
            catch { instance = null; instance = new LogForm(); return instance; }
        }

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }
    }
}
