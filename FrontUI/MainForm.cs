using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrontUI
{
    public partial class MainForm : Form, IFront
    {
        public string Status
        {
            set { toolStripStatusLabel1.Text = value; }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        public event Action<string> PortSelected;
        public event Action Draw;

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshPortNames(cbPorts);
        }

        private void RefreshPortNames(object sender = null, EventArgs e = null)
        {
            using (new UiControlBusyScope(sender))
            {
                cbPorts.Items.Clear();
                cbPorts.Items.AddRange(ComHelper.GetPortNames());
            }
        }

        private void cbPorts_SelectedValueChanged(object sender, EventArgs e)
        {
            PortSelected?.Invoke(cbPorts.SelectedItem as string);
        }

        public void OutcomingData(string str)
        {
            tbOutcoming.Text += str;
        }

        public void IncomingData(string str)
        {
            tbIncoming.Text += str;
        }

        private void bDraw_Click(object sender, EventArgs e)
        {
            Draw?.Invoke();
        }
    }
}
