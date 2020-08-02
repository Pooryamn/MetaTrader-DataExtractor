using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CSV_View
{
    public partial class Form1 : Form
    {

        private int oldRowIndex;
        DataTable dt;
        string path;
        

        public Form1()
        {
            // RUN CLINET SILDE PROGRAM :
            path = AppDomain.CurrentDomain.BaseDirectory;
            path += "\\Data\\Client.exe";
            Process.Start(path);

            InitializeComponent();
            oldRowIndex = 0;
            BindCsv();
            timer1.Start();
        }

        private void BindCsv()
        {


            //DataTable dt = new DataTable();
            dt = new DataTable();

            string[] lines = System.IO.File.ReadAllLines("Data/DATA_FILE.csv");
            if (lines.Length > 0)
            {
                if (lines.Length < oldRowIndex)
                {
                    oldRowIndex = 0;
                }
                string[] HeaderLabels;
                HeaderLabels = new String[] {"ردیف","Name","تاریخ","ساعت","عرضه","تقاضا","آخرین قیمت","بیشترین قیمت","کمترین قیمت","تغییر روزانه","تعداد معاملات","حجم معاملات","قیمت باز","قیمت بسته"};
                foreach (string headerWord in HeaderLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }

                // DATA :

                System.IO.StreamReader Symbol = new System.IO.StreamReader("Data/Symbols.txt");

                for (int r = 1; r < lines.Length; r++)
                {

                    string[] dataWords = lines[r].Split(',');

                    DataRow dr = dt.NewRow();

                    // Data :
                    String Date = dataWords[0];
                    dr[2] = Date;

                    String Time = dataWords[1];
                    dr[3] = Time;

                    String BID = dataWords[2].Split('.')[0];
                    dr[4] = BID;

                    String ASK = dataWords[3].Split('.')[0];
                    dr[5] = ASK;

                    String Last = dataWords[4].Split('.')[0];
                    dr[6] = Last;

                    String Last_H = dataWords[5].Split('.')[0];
                    dr[7] = Last_H;

                    String Last_L = dataWords[6].Split('.')[0];
                    dr[8] = Last_L;

                    String Change = dataWords[7];
                    if (Change.Length >= 5)
                    {
                        Change = Change.Remove(5);
                        Change += " %";
                    }
                    dr[9] = Change;

                    String Deals = dataWords[8].Split('.')[0];
                    dr[10] = Deals;

                    String Deals_V = dataWords[9].Split('.')[0];
                    dr[11] = Deals_V;

                    String Opens = dataWords[10].Split('.')[0];
                    dr[12] = Opens;

                    String Closes = dataWords[11].Split('.')[0];
                    dr[13] = Closes;

                    string SymName;
                    SymName = Symbol.ReadLine();
                    dr[1] = SymName;
                    dr[0] = r.ToString();

                    dt.Rows.Add(dr);

                }
                Symbol.Close();

            }


            if (txt_Search.Text.Length != 0)
            {
                show_search();
            }
            else
            {

                if (dt.Rows.Count > 0)
                {

                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[4].DefaultCellStyle.ForeColor = Color.Blue;
                    dataGridView1.Columns[5].DefaultCellStyle.ForeColor = Color.Red;
                    dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(255, 128, 192);
                    dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.FromArgb(145, 145, 255);
                    dataGridView1.Columns[6].DefaultCellStyle.BackColor = Color.FromArgb(181, 230, 29);
                    //dataGridView1.Rows[oldRowIndex].Selected = true;
                    //dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.SelectedRows[oldRowIndex].Index;
                    //if (oldRowIndex >= 11)
                    //{
                        dataGridView1.FirstDisplayedScrollingRowIndex = oldRowIndex;
                    //}
                    //else
                    //{
                        //dataGridView1.FirstDisplayedScrollingRowIndex = 0;
                    //}
                }
            }
            
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BindCsv();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            oldRowIndex = e.RowIndex;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //dataGridView1.ClearSelection();
        }

        private void txt_IP_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("POORYA MOHAMMADI","ALERT");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // open file dialog
            // save dt data to a csv
            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = "CSV (*.csv) | *.csv";
            SaveFile.FileName = "Output.csv";

            bool fileErr = false;

            if (SaveFile.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(SaveFile.FileName))
                {
                    try
                    {
                        File.Delete(SaveFile.FileName);
                    }
                    catch(IOException ex)
                    {
                        fileErr = true;
                        MessageBox.Show("Can't write data to file" + ex.Message);
                    }
                }
                if (!fileErr)
                {
                    try
                    {
                        int columnCount = dataGridView1.Columns.Count;
                        string ColNames = "";
                        string[] outputCSV = new string[dataGridView1.Rows.Count + 1];

                        for (int i = 0; i < columnCount; i++)
                        {
                            ColNames += dataGridView1.Columns[i].HeaderText.ToString() + ",";
                        }

                        outputCSV[0] += ColNames;

                        for (int i = 1; (i - 1) < dataGridView1.Rows.Count; i++)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                outputCSV[i] += dataGridView1.Rows[i - 1].Cells[j].Value.ToString() + ",";
                            }
                        }
                        File.WriteAllLines(SaveFile.FileName, outputCSV, Encoding.UTF8); // very important part
                        MessageBox.Show("Data exported !!", "Info");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error : " + ex.Message);
                    }
                }
            }
        }

        private void aboutCSVViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CSV Viewer Version 1.0.6\n\nPoorya Mohammadi", "About software");
        }

        private void webSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.github.com/Pooryamn");
        }

        private void supportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Support info", "Support");
        }

        private void rowCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowCount = dataGridView1.Rows.Count;
            MessageBox.Show("Row Count :" + rowCount.ToString(), "Rows");
        }

        private void Form1_MaximumSizeChanged(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.Width = this.Width;
            //dataGridView1.Height = this.Height;
            dataGridView1.Size = new Size(this.Width, this.Height);
        }

        private void rowCountToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            int rowCount = dataGridView1.Rows.Count;
            MessageBox.Show("Row Count :" + rowCount.ToString(), "Rows");
        }

        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            show_search();   
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.IO.File.Copy("RawData/DATA_FILE.csv", "Data/DATA_FILE.csv", true);
            System.IO.File.Copy("RawData/Symbols.txt", "Data/Symbols.txt", true);
        }

        private void show_search()
        {
            DataView dv = new DataView(dt);
            dv.RowFilter = string.Format("Name LIKE '%{0}%'", txt_Search.Text);
            dataGridView1.DataSource = dv;
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            //if (e.NewValue == 0)
            //{
                //e.NewValue = e.OldValue;
            //}
            oldRowIndex = e.NewValue;

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            //oldRowIndex++;
            //if (oldRowIndex > 100)
            //{
                //oldRowIndex = 100;
           // }
            //dataGridView1.Rows[oldRowIndex].Selected = true;
        }

    } 

}
