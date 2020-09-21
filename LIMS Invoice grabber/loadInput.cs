using System.Windows.Forms;

namespace LIMS_Invoice_grabber {
    public partial class loadInput : Form
    {
        //public ProgressBar progressBar1;
        public loadInput()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.MarqueeAnimationSpeed = 20;
            progressBar1.Style = ProgressBarStyle.Marquee;
        }
    }
}
