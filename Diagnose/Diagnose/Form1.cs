using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Backprop;
using System.IO;
using System.Globalization;

namespace Diagnose
{
    public partial class Form1 : Form
    {
        private NeuralNet nn;
        private double[,] data;
        private int dataRow;
        private int dataCol;
        private Boolean result;
        double[,] testing;

        DataGridView my_datagridview = new DataGridView();
        DataTable my_datatable = new DataTable();

        public Form1()
        {
            InitializeComponent();
            nn = new NeuralNet(20, 60, 1);
            this.AutoSize = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i;
            for (i = 0; i < symptoms.Items.Count; i++)
            {
                if (symptoms.GetItemChecked(i)) nn.setInputs(i, 1.0);
                else nn.setInputs(i, 0.0);
            }

            for (int k = 0; k < external.Items.Count; k++, i++)
            {
                if (external.GetItemChecked(k)) nn.setInputs(i, 1.0);
                else nn.setInputs(i, 0.0);
            }

            nn.run();
            result = true;
            if (nn.getOuputData(0) < 0.5) result = false;

            //textBox1.Text = "" + nn.getOuputData(0);

            //richTextBox1.Text = "" + nn.getOuputData(0);

            panel3.Visible = true;

            if (result)
            {
                output.ForeColor = Color.FromArgb(231, 64, 56);
                output.Text = "High Risk of COVID19";
                richTextBox1.Text = "It is advised that you self-isolate and contact your medical provider or a " +
                    "COVID-19 information line for advice. Don't forget to drink plenty of fluid, and eat nutritious food. " +
                    "Seek medical care if you have a fever, cough, and difficulty breathing. Call in advance.";
            }
            else
            {
                output.ForeColor = Color.Green;
                output.Text = "Low Risk of COVID19";
                richTextBox1.Text = "Still you have stay at home to be safe. Don't forget to drink plenty of fluid, and eat nutritious food. " +
                    "Seek medical care if you have a fever, cough, and difficulty breathing. Call in advance.";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            nn.loadWeights(openFileDialog1.FileName);
        }


        private void readFile(string file)
        {
            var lines = File.ReadAllLines(file);
            int i = 0, j = 0;

            dataRow = lines.Length - 1;
            dataCol = 21;
            data = new double[dataRow, dataCol];

            foreach (var line in lines)
            {
                if (i != 0)
                {
                    var values = line.Split(',');
                    foreach (var value in values)
                    {
                        data[i - 1, j] = Convert.ToDouble(value);
                        j++;
                    }
                }
                i++;
                j = 0;
            }

            /*
           for(int k=0; k < data.GetLength(0); k++)
           {
               for(int l = 0; l < data.GetLength(1); l++)
               {
                   richTextBox1.Text += "" + data[k, l] + " ";
               }
               richTextBox1.Text += "\n";
           }
            */
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel4.Visible = false;
            button2.Visible = true; ;

            button5.BackColor = Color.FromArgb(231, 64, 56);
            button5.ForeColor = Color.White;
            button6.BackColor = Color.White;
            button6.ForeColor = Color.FromArgb(231, 64, 56);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel4.Visible = true;
            button2.Visible = false;

            button6.BackColor = Color.FromArgb(231, 64, 56);
            button6.ForeColor = Color.White;
            button5.BackColor = Color.White;
            button5.ForeColor = Color.FromArgb(231, 64, 56);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            readFile(openFileDialog2.FileName);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < dataRow; j++)
                {
                    for (int k = 0; k < dataCol - 1; k++)
                    {
                        nn.setInputs(k, data[j, k]);
                    }
                    nn.setDesiredOutput(0, data[j, dataCol - 1]);
                    nn.learn();
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            my_datatable = new DataTable();
            readFile(openFileDialog3.FileName);
            var lines = File.ReadAllLines(openFileDialog3.FileName);
            string[] data_col = null;
            int x = 0;

            foreach (string text_line in lines)
            {

                data_col = text_line.Split(',');

                if (x == 0)
                {
                    int k = 0;
                    for (; k < data_col.Count(); k++)
                    {
                        my_datatable.Columns.Add(data_col[k]);
                    }
                    my_datatable.Columns.Add("N.N. Result");
                    x++;
                }
                else
                {
                    my_datatable.Rows.Add(data_col);
                }
            }

            dataGridView1.DataSource = my_datatable;


            richTextBox1.Text = "";
            for (int j = 0; j < dataRow; j++)
            {
                for (int k = 0; k < dataCol - 1; k++)
                {
                    nn.setInputs(k, data[j, k]);
                }
                nn.run();

                DataRow dr = my_datatable.Rows[j];
                dr[21] = Convert.ToDecimal(nn.getOuputData(0));

                //NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                //nfi.PercentDecimalDigits = 4;
                //nn.getOuputData(0).ToString("P", nfi)
                //richTextBox2.Text += "" +(j+1) + ". Expected: "+data[j,dataCol-1] +" Result: " + Convert.ToDecimal(nn.getOuputData(0)) + "\n";
            }

            
        }


        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            nn.saveWeights(saveFileDialog1.FileName);
        }
    }
}
