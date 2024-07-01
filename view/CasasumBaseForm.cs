using Casasum.controller;
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

        private void casasumBaseForm_Load( object sender, EventArgs e )
        {
            label1.Text = "Info: Vyberte XML soubor se vtupními daty";

            button1.Text = "Otevřít";

            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].HeaderText = "Název modelu\nCena bez DPH       Cena s DPH";
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;

            dataGridView2.ColumnCount = 4;
            dataGridView2.Columns[0].HeaderText = "Název modelu";
            dataGridView2.Columns[1].HeaderText = "Datum prodeje";
            dataGridView2.Columns[2].HeaderText = "Cena";
            dataGridView2.Columns[3].HeaderText = "DPH";
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.RowHeadersVisible = false;

            comboBox1.Text = "Modely prodané o víkendu (default)";
            comboBox1.Items.Insert( 0, "Modely prodané o víkendu (default)" );
            comboBox1.Items.Insert( 1, "Modely prodané přes pacovní týden" );
            comboBox1.Items.Insert( 2, "Celkový součet prodaných modelů" );
        }

        private void button1_Click( object sender, EventArgs e )
        {
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if ( openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                textBox1.Text = openFileDialog1.FileName;
                label1.Text = "Vybraný soubor: " + openFileDialog1.SafeFileName;
            }
            separator.processXmlFile( openFileDialog1.FileName );
            foreach ( var row in separator.SeparatorOutput.SaleCasesList.SaleCaseList )
            {
                dataGridView2.Rows.Add( row.Model, row.Date.ToString( "d.M.yyyy" ), row.PriceWoVat.ToString(), row.Vat.ToString() );
            }
            foreach ( string row in separator.SeparatorOutput.SumPrintQueue )
            {
                dataGridView1.Rows.Add( row );
            }
        }
        private controller.AppLogicSeparator separator = new();

        private void comboBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( comboBox1.SelectedIndex == 0 )       // Modely prodané o víkendu (default)
            {

            }
            else if( comboBox1.SelectedIndex == 1 )  // Modely prodané přes pacovní týden
            {
                
            }
            else if( comboBox1.SelectedIndex == 2 )  // Celkový součet prodaných modelů
            {

            }
        }
    }
}
