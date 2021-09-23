using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalcForFIFA4
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        int GW_CHILD = 5;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        public const int EM_SETREADONLY = 0xcf;

        private double rate = 0.72;
        private bool init = true;

        private DataTable dt1 = new DataTable();
        private DataTable dt2 = new DataTable();

        private List<int> lastMatchedRowIndex = new List<int>();

        public MainForm()
        {
            InitializeComponent();

            IntPtr editHandle = GetWindow(comboBox1.Handle, GW_CHILD);
            IntPtr editHandle2 = GetWindow(comboBox2.Handle, GW_CHILD);
            SendMessage(editHandle, EM_SETREADONLY, 1, 0);
            SendMessage(editHandle2, EM_SETREADONLY, 1, 0);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitDefaultValue();

            rate = Convert.ToDouble(textBox3.Text);
            dt1 = CreateDataTable1();
            dt2 = CreateDataTable2();

            ShowMarket();
            init = false;
        }

        private void InitDefaultValue()
        {
            foreach (string key in ConfigurationManager.AppSettings)
                if (key.StartsWith("showFilter"))
                    comboBox1.Items.Add(ConfigurationManager.AppSettings[key]);
                else if (key.StartsWith("discount"))
                    textBox3.Text = ConfigurationManager.AppSettings[key];

            comboBox1.Text = comboBox1.Items[0].ToString();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DataBackup();
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
            dataGridView1.SelectedCells[0].Selected = false;
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
                        if (i == 4 && String.IsNullOrEmpty(str))
                        {
                            dr[i] = "2021/01/01";
                            i++;
                            continue;
                        }
                        if (i == 5)
                        {
                            if (dr[3] is DBNull)
                                dr[3] = 0;
                            var sellPrice = Math.Ceiling(Convert.ToDouble(dr[3]) / rate);
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
            comboBox1_SelectedIndexChanged(null, null);
            dataGridView2.SelectedCells[0].Selected = false;
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
            DataColumn dc11 = new DataColumn("ColPlayerState", Type.GetType("System.String"));
            DataColumn dc1 = new DataColumn("ColPlayerName", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("ColPlayerType", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ColBuyPrice", Type.GetType("System.Double"));
            DataColumn dc4 = new DataColumn("ColBuyTime", Type.GetType("System.DateTime"));
            DataColumn dc5 = new DataColumn("ColSellPrice", Type.GetType("System.Double"));
            DataColumn dc6 = new DataColumn("ColFinalPrice", Type.GetType("System.Double"));
            DataColumn dc7 = new DataColumn("ColSellTime", Type.GetType("System.DateTime"));
            DataColumn dc8 = new DataColumn("ColSellPlayer", Type.GetType("System.String"));
            DataColumn dc9 = new DataColumn("ColLevelupCost", Type.GetType("System.Double"));
            DataColumn dc10 = new DataColumn("ColBenefit", Type.GetType("System.Double"));
            dt.Columns.Add(dc11);
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

        private void DataBackup()
        {
            // 当前路径
            var path = AppDomain.CurrentDomain.BaseDirectory;

            // 判断目标目录是否存在如果不存在则新建
            var destinationPath = path + "/" + DateTime.Now.ToString("yyyyMM");
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            var fileName1 = "MarketData.txt";
            var fileName2 = "BuyHistory.txt";
            var sourceFile1 = path + "/" + fileName1;
            var sourceFil2 = path + "/" + fileName2;
            var destinationFile1 = destinationPath + "/MarketData" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            var destinationFile2 = destinationPath + "/BuyHistory" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            File.Copy(sourceFile1, destinationFile1, true);
            File.Copy(sourceFil2, destinationFile2, true);
        }

        private DataTable SwapRow(int index1, int index2, DataTable dt)
        {
            DataRow dr = dt.NewRow();
            dr.ItemArray = dt.Rows[index1].ItemArray;
            dt.Rows[index1].ItemArray = dt.Rows[index2].ItemArray;
            dt.Rows[index2].ItemArray = dr.ItemArray;
            return dt;
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
            if (e.ColumnIndex == dataGridView2.Columns["ColBuyPrice"].Index)
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
                    dt2.Rows[e.RowIndex]["ColBenefit"] = DBNull.Value;
                }
                else
                    dt2.Rows[e.RowIndex]["ColBenefit"] = Convert.ToDouble(dt2.Rows[e.RowIndex]["ColFinalPrice"]) * rate -
                        Convert.ToDouble(dt2.Rows[e.RowIndex]["ColBuyPrice"]) - cost;
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

        private void TsbUp_Click(object sender, EventArgs e)
        {
            var dgv = new DataGridView();
            if (tabControl1.SelectedIndex == 0)
                dgv = dataGridView1;
            else if (tabControl1.SelectedIndex == 3)
                dgv = dataGridView2;
            else
                return;

            if (dgv.CurrentRow == null)
                return;
            var index1 = dgv.CurrentRow.Index;
            if (index1 == 0)
                return;
            SwapRow(index1, index1 - 1, dgv.DataSource as DataTable);
            SaveData();
            dgv.CurrentCell = dgv.Rows[index1 - 1].Cells[0];
        }

        private void TsbDown_Click(object sender, EventArgs e)
        {
            var dgv = new DataGridView();
            if (tabControl1.SelectedIndex == 0)
                dgv = dataGridView1;
            else if (tabControl1.SelectedIndex == 3)
                dgv = dataGridView2;
            else
                return;

            if (dgv.CurrentRow == null)
                return;
            var index1 = dgv.CurrentRow.Index;
            if (index1 == dgv.Rows.Count - 1)
                return;
            SwapRow(index1, index1 + 1, dgv.DataSource as DataTable);
            SaveData();
            dgv.CurrentCell = dgv.Rows[index1 + 1].Cells[0];
        }

        private void tsbBackup_Click(object sender, EventArgs e)
        {
            DataBackup();
            MessageBox.Show("备份成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TsbOpen_Click(object sender, EventArgs e)
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void TsbSetting_Click(object sender, EventArgs e)
        {
            SettingForm frm = new SettingForm();
            frm.ShowDialog(this);
        }

        private void TsbAbout_Click(object sender, EventArgs e)
        {
            AboutForm frm = new AboutForm();
            frm.ShowDialog(this);
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            // Linq模糊查询 
            IEnumerable<DataGridViewRow> enumerableList = this.dataGridView2.Rows.Cast<DataGridViewRow>();
            List<DataGridViewRow> list = (from item in enumerableList
                                          where (item.Cells[0].Value.ToString().IndexOf(this.tbSearch.Text.Trim()) >= 0 ||
                                          item.Cells[1].Value.ToString().IndexOf(this.tbSearch.Text.Trim()) >= 0) &&
                                          this.tbSearch.Text.Trim() != "" &&
                                          item.Visible == true
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (init || tabControl1.SelectedIndex != 3)
                return;

            foreach (DataGridViewRow row in dataGridView2.Rows)
                row.Visible = true;

            if (!comboBox1.Text.Equals("全部"))
            {
                // Linq模糊查询 
                IEnumerable<DataGridViewRow> enumerableList = this.dataGridView2.Rows.Cast<DataGridViewRow>();
                List<DataGridViewRow> list = (from item in enumerableList
                                              where item.Cells[0].Value.ToString() != comboBox1.Text.Trim()
                                              select item).ToList();
                if (list.Count > 0)
                {
                    CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView2.DataSource];
                    cm.SuspendBinding();

                    foreach (var row in list)
                        row.Visible = false;

                    cm.ResumeBinding();
                }
            }

            tbSearch_TextChanged(null, null);
            ChbSum_CheckedChanged(null, null);
        }

        private void ChbSum_CheckedChanged(object sender, EventArgs e)
        {
            if (ChbSum.Checked)
            {
                splitContainer1.Panel2Collapsed = false;
                // Linq模糊查询 
                IEnumerable<DataGridViewRow> enumerableList = this.dataGridView2.Rows.Cast<DataGridViewRow>();
                List<DataGridViewRow> list = (from item in enumerableList
                                              where item.Visible == true
                                              select item).ToList();

                double buyPriceSum = 0;
                double benefitSum = 0;
                double sellPriceSum = 0;
                double levelupCostSum = 0;
                foreach (var i in list)
                {
                    if (!(dataGridView2.Rows[i.Index].Cells["ColBuyPrice"].Value is DBNull))
                        buyPriceSum += Convert.ToDouble(dataGridView2.Rows[i.Index].Cells["ColBuyPrice"].Value);
                    if (!(dataGridView2.Rows[i.Index].Cells["ColBenefit"].Value is DBNull))
                        benefitSum += Convert.ToDouble(dataGridView2.Rows[i.Index].Cells["ColBenefit"].Value);
                    if (!(dataGridView2.Rows[i.Index].Cells["ColLevelupCost"].Value is DBNull))
                        sellPriceSum += Convert.ToDouble(dataGridView2.Rows[i.Index].Cells["ColLevelupCost"].Value);
                    if (!(dataGridView2.Rows[i.Index].Cells["ColFinalPrice"].Value is DBNull))
                        levelupCostSum += Convert.ToDouble(dataGridView2.Rows[i.Index].Cells["ColFinalPrice"].Value);
                }

                dataGridView3.Rows.Clear();
                dataGridView3.Rows.Add();
                dataGridView3.Rows[0].Cells["ColBuyPriceSum"].Value = buyPriceSum.ToString();
                dataGridView3.Rows[0].Cells["ColBuyPriceSum"].Value = buyPriceSum.ToString();
                dataGridView3.Rows[0].Cells["ColBenefitSum"].Value = benefitSum.ToString();
                dataGridView3.Rows[0].Cells["ColLevelupCostSum"].Value = sellPriceSum.ToString();
                dataGridView3.Rows[0].Cells["ColFinalPriceSum"].Value = levelupCostSum.ToString();
                dataGridView3.SelectedCells[0].Selected = false;
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
            }
        }

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.Automatic)
                comboBox1_SelectedIndexChanged(null, null);
        }
    }
}
