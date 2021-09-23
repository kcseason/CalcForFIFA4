using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalcForFIFA4
{
    public partial class MainForm : Form
    {
        private double rate = 0.72;
        private bool init = true;

        private DataTable dt1 = new DataTable();
        private DataTable dt2 = new DataTable();

        private List<int> lastMatchedRowIndex = new List<int>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rate = Convert.ToDouble(textBox3.Text);
            dt1 = CreateDataTable1();
            dt2 = CreateDataTable2();

            ShowMarket();
            init = false;
        }

        private void ShowMarket()
        {
            dt1.Rows.Clear();
            // 创建一个 StreamReader 的实例来读取文件 
            // using 语句也能关闭 StreamReader
            var path = AppDomain.CurrentDomain.BaseDirectory + "/MarketData.txt";
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    var dr = dt1.NewRow();
                    var strArr = line.Split(',');
                    var i = 0;
                    foreach (string str in strArr)
                    {
                        if (!String.IsNullOrEmpty(str))
                        {
                            dr[i] = str;
                            i++;
                        }
                    }
                    dt1.Rows.Add(dr);
                }
            }

            dataGridView1.DataSource = dt1;
        }

        private void ShowBuyHistory()
        {
            dt2.Rows.Clear();
            // 创建一个 StreamReader 的实例来读取文件 
            // using 语句也能关闭 StreamReader
            var path = AppDomain.CurrentDomain.BaseDirectory + "/BuyHistory.txt";
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    var dr = dt2.NewRow();
                    var strArr = line.Split(',');
                    var i = 0;
                    foreach (string str in strArr)
                    {
                        if (i == 3 && String.IsNullOrEmpty(str))
                        {
                            dr[i] = "2021-01-01";
                            i++;
                            continue;
                        }
                        if (i == 4)
                        {
                            var sellPrice = Math.Ceiling(Convert.ToDouble(dr[2]) / rate);
                            dr[i] = sellPrice.ToString();
                            i++;
                            continue;
                        }
                        if (!String.IsNullOrEmpty(str))
                            dr[i] = str;
                        i++;
                    }
                    dt2.Rows.Add(dr);
                }
            }

            dataGridView2.DataSource = dt2;
        }

        private DataTable CreateDataTable1()
        {
            var dt = new DataTable();
            DataColumn dc1 = new DataColumn("ColLevel", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("ColName1", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ColPrice1", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("ColName2", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("ColPrice2", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("ColName3", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("ColPrice3", Type.GetType("System.String"));
            DataColumn dc8 = new DataColumn("ColName4", Type.GetType("System.String"));
            DataColumn dc9 = new DataColumn("ColPrice4", Type.GetType("System.String"));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            dt.Columns.Add(dc7);
            dt.Columns.Add(dc8);
            dt.Columns.Add(dc9);

            return dt;
        }
        private DataTable CreateDataTable2()
        {
            var dt = new DataTable();
            DataColumn dc1 = new DataColumn("ColPlayerName", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("ColPlayerType", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ColBuyPrice", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("ColBuyTime", Type.GetType("System.DateTime"));
            DataColumn dc5 = new DataColumn("ColSellPrice", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("ColFinalPrice", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("ColSellTime", Type.GetType("System.DateTime"));
            DataColumn dc8 = new DataColumn("ColSellPlayer", Type.GetType("System.String"));
            DataColumn dc9 = new DataColumn("ColLevelupCost", Type.GetType("System.String"));
            DataColumn dc10 = new DataColumn("ColBenefit", Type.GetType("System.String"));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            dt.Columns.Add(dc7);
            dt.Columns.Add(dc8);
            dt.Columns.Add(dc9);
            dt.Columns.Add(dc10);
            return dt;
        }

        private void SaveData()
        {
            tabControl1.Focus();
            var fileName = "";
            DataTable dt = new DataTable();
            if (tabControl1.SelectedIndex == 0)
            {
                fileName = "/MarketData.txt";
                dt = dt1.Copy();
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                fileName = "/BuyHistory.txt";
                dt = dt2.Copy();
            }
            var path = AppDomain.CurrentDomain.BaseDirectory + fileName;
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (var str in dr.ItemArray)
                    {
                        if (str != null && !String.IsNullOrEmpty(str.ToString().Trim()))
                            sw.Write(str.ToString().Trim() + ",");
                        else
                        {
                            sw.Write(str.ToString().Trim() + ",");
                            continue;
                        }
                    }
                    sw.Write("\n");
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 3)
            {
                ShowBuyHistory();
                textBox1_TextChanged(null, null);
            }
            if (tabControl1.SelectedIndex == 0)
                ShowMarket();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var sellPrice = Math.Ceiling(Convert.ToDouble(textBox1.Text) / rate);
                textBox2.Text = sellPrice.ToString();
            }
            catch { textBox2.Text = "0"; }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                rate = Convert.ToDouble(textBox3.Text);
                textBox1_TextChanged(null, null);
            }
            catch { textBox3.Text = "0.72"; }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (init)
                return;

            var currentValue = dataGridView2.CurrentCell.Value == null ? "" : dataGridView2.CurrentCell.Value.ToString();
            if (e.ColumnIndex == dataGridView2.Columns["dataGridViewTextBoxColumn3"].Index)
            {
                var sellPrice = Math.Ceiling(Convert.ToDouble(currentValue) / rate);

                dt2.Rows[e.RowIndex]["ColSellPrice"] = sellPrice.ToString();
            }
            if (e.ColumnIndex == dataGridView2.Columns["ColFinalPrice"].Index ||
                e.ColumnIndex == dataGridView2.Columns["ColLevelupCost"].Index)
            {
                var cost = dt2.Rows[e.RowIndex]["ColLevelupCost"] is DBNull ?
                    0 : Convert.ToDouble(dt2.Rows[e.RowIndex]["ColLevelupCost"]);
                if (dt2.Rows[e.RowIndex]["ColFinalPrice"] is DBNull)
                {
                    dt2.Rows[e.RowIndex]["ColBenefit"] = null;
                }
                else
                    dt2.Rows[e.RowIndex]["ColBenefit"] = Convert.ToDouble(dt2.Rows[e.RowIndex]["ColFinalPrice"]) -
                        Convert.ToDouble(dt2.Rows[e.RowIndex]["ColSellPrice"]) - cost;
            }
        }

        private void TsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TsbNew_Click(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex == 0)
            {
                var dr = dt1.NewRow();
                dt1.Rows.InsertAt(dr, 0);
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                var dr = dt2.NewRow();
                dt2.Rows.InsertAt(dr, 0);
            }
        }

        private void TsbDelete_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                dt1.Rows.RemoveAt(dataGridView1.CurrentCell.RowIndex);
            else if (tabControl1.SelectedIndex == 3)
                dt2.Rows.RemoveAt(dataGridView2.CurrentCell.RowIndex);

            SaveData();
            MessageBox.Show("删除成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TsbRefresh_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                ShowMarket();
            else if (tabControl1.SelectedIndex == 3)
                ShowBuyHistory();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            // Linq模糊查询 
            IEnumerable<DataGridViewRow> enumerableList = this.dataGridView2.Rows.Cast<DataGridViewRow>();
            List<DataGridViewRow> list = (from item in enumerableList
                                          where (item.Cells[0].Value.ToString().IndexOf(this.tbSearch.Text.Trim()) >= 0 ||
                                          item.Cells[1].Value.ToString().IndexOf(this.tbSearch.Text.Trim()) >= 0) &&
                                          this.tbSearch.Text.Trim() != ""
                                          select item).ToList();

            // 恢复之前行的背景颜色为默认的白色背景 
            foreach (var i in lastMatchedRowIndex)
                this.dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.White;

            if (list.Count > 0)
            {
                lastMatchedRowIndex.Clear();
                foreach (var i in list)
                {
                    // 查找匹配行高亮显示 
                    this.dataGridView2.Rows[i.Index].DefaultCellStyle.BackColor = Color.Yellow;
                    dataGridView2.FirstDisplayedScrollingRowIndex = i.Index;
                    lastMatchedRowIndex.Add(i.Index);
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tbSearch.Text = string.Empty;
        }
    }
}
