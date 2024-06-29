using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Casasum.view
{
    public partial class casasumBaseForm : Form
    {
        public casasumBaseForm()
        {
            InitializeComponent();
        }

        private void casasumBaseForm_Load(object sender, EventArgs e)
        {
            label1.Text = "Info: Select XML file with input data";
            button1.Text = "Open";
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].HeaderText = "Název modelu\nCena bez DPH       Cena s DPH";
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                label1.Text = "Selected file: " + openFileDialog1.SafeFileName;
            }
        }
    }
}
