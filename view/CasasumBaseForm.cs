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
            dataGridView2.Columns[ 0 ].HeaderText = "Název modelu";
            dataGridView2.Columns[ 1 ].HeaderText = "Datum prodeje";
            dataGridView2.Columns[ 2 ].HeaderText = "Cena";
            dataGridView2.Columns[ 3 ].HeaderText = "DPH";
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.RowHeadersVisible = false;

            comboBox1.Text = "Modely prodané o víkendu (default)";
            comboBox1.Items.Insert( 0, "Modely prodané o víkendu (default)" );
            comboBox1.Items.Insert( 1, "Modely prodané přes pacovní týden" );
            comboBox1.Items.Insert( 2, "Součet všech prodaných modelů" );

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            resetTable( dataGridView1 );
            resetTable( dataGridView2 );
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if( openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                textBox1.Text = openFileDialog1.FileName;
                label1.Text = "Vybraný soubor: " + openFileDialog1.SafeFileName;
            }
            separator.processXmlFile( openFileDialog1.FileName );
            processAppMessages();
            if( separator.SeparatorOutput.ValidInputData )
            {
                foreach( var row in separator.SeparatorOutput.SaleCasesList.SaleCaseList )
                {
                    dataGridView2.Rows.Add( row.Model, row.Date.ToString( "d.M.yyyy" ), row.PriceWoVat.ToString( "N0" ) + ",-", row.Vat.ToString() );
                }
                foreach( string row in separator.SeparatorOutput.WeekendSumPrintQueue )
                {
                    dataGridView1.Rows.Add( row );
                }
            }
            else
            {
                MessageBox.Show( "Neplatné vstupní data!", "Chybové správy", MessageBoxButtons.OK, MessageBoxIcon.Stop );
                dataGridView2.Rows.Add( "Neplatné data", "Neplatné data", "Neplatné data", "Neplatné data" );
                dataGridView1.Rows.Add( "Neplatné vstupní data" );
            }
        }
        private controller.AppLogicSeparator separator = new();

        private void comboBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( !separator.SeparatorOutput.ValidInputData ) { MessageBox.Show("Neplatné vstupní data!", "Chybové správy", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if( comboBox1.SelectedIndex == (int) controller.Constants.SaleTime.WeekendSale )       // Modely prodané o víkendu (default)
            {
                resetTable( dataGridView1 );
                foreach( string row in separator.SeparatorOutput.WeekendSumPrintQueue ) 
                {
                    dataGridView1.Rows.Add( row );
                }
            }
            else if( comboBox1.SelectedIndex == (int) controller.Constants.SaleTime.WorkWeekSale )  // Modely prodané přes pacovní týden
            {
                resetTable(dataGridView1);
                separator.summarize( controller.Constants.SaleTime.WorkWeekSale );
                foreach( string row in separator.SeparatorOutput.WorkWeekSumPrintQueue )
                {
                    dataGridView1.Rows.Add( row );
                }
            }
            else if( comboBox1.SelectedIndex == (int) controller.Constants.SaleTime.AllSales )  // Celkový součet prodaných modelů
            {
                resetTable(dataGridView1);
                separator.summarize( controller.Constants.SaleTime.AllSales );
                foreach( string row in separator.SeparatorOutput.AllDaysSumPrintQueue )
                {
                    dataGridView1.Rows.Add(row);
                }
            }
        }

        private void resetTable( DataGridView table )
        {
            if ( table.Rows.Count > 0 )
            {
                table.Rows.Clear();
            }
        }

        private void processAppMessages()
        {
            List< List< string>> messagesLists = new List< List< string > >();
            messagesLists.Add( separator.SeparatorOutput.WarningMessagesList );
            messagesLists.Add( separator.SeparatorOutput.ErrorMessagesList );
            List< MessageBoxIcon > messageIcons = new List< MessageBoxIcon > { MessageBoxIcon.Warning, MessageBoxIcon.Error };
            List< string > messageLabels = new List< string > { "Varovné zprávy", "Chybové zprávy" };

            byte iteration = 0;
            foreach( var messagesList in messagesLists )
            {
                if (messagesList.Count > 0)
                {
                    StringBuilder message = new();
                    foreach (var msg in separator.SeparatorOutput.WarningMessagesList)
                    {
                        message.Append(msg + "\n");
                    }
                    MessageBox.Show( message.ToString(), messageLabels[ iteration ], MessageBoxButtons.OK, messageIcons[ iteration ]);
                }
                iteration++;
            }
        }
    }
}
