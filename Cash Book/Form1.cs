using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Gazi_Mobilya
{
    public partial class Form1 : Form
    {
        public OleDbConnection baglan = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "/cashbook.accdb");
        public OleDbCommand komut = new OleDbCommand();
        public DataSet tablo = new DataSet();
        Decimal bakiye = 0, borc = 0, alacak = 0, gider = 0, verilen = 0;
        public void yukle_genel()
        {
            //---General Expenses---
            Decimal bakiye2 = 0;
            dataGridView1.Columns.Clear();
            tablo.Tables.Clear();
            baglan.Open();
            OleDbDataAdapter ac = new OleDbDataAdapter("Select * From gider", baglan);
            ac.Fill(tablo, "gider");
            dataGridView1.DataSource = tablo.Tables["gider"];
            baglan.Close();

            //---Toplam....
            DataTable dt = new DataTable();
            ac.Fill(dt);
            ac.Dispose();
            foreach (DataRow item in dt.Rows)
            {
                if (item["fiyat"].ToString() != "")
                {
                    bakiye2 -= Convert.ToDecimal(item["fiyat"].ToString());
                }
            }
            bakiye = bakiye2;
            gider = 0-bakiye2;
            //***Total****
            //***General Expenses***
        }
        public void yukle_musteri()
        {
            Decimal alacak2 = 0;
            //---Customer Transactions---
            dataGridView2.Columns.Clear();
            tablo.Tables.Clear();
            baglan.Open();
            OleDbDataAdapter ac2 = new OleDbDataAdapter("Select * From musteri", baglan);
            ac2.Fill(tablo, "musteri");
            dataGridView2.DataSource = tablo.Tables["musteri"];
            baglan.Close();

            //---Total....
            DataTable dt = new DataTable();
            ac2.Fill(dt);
            ac2.Dispose();
            foreach (DataRow item in dt.Rows)
            {
                if (item["m_borc"].ToString() != "")
                {
                    alacak2 += Convert.ToDecimal(item["m_borc"].ToString());
                }
            }
            alacak = alacak2;
            //***Total****
            //***Customer Transactions***
        }

        public void yukle_firma()
        {
            Decimal borc2 = 0;
            //---Company Transactions---
            dataGridView3.Columns.Clear();
            tablo.Tables.Clear();
            baglan.Open();
            OleDbDataAdapter ac3 = new OleDbDataAdapter("Select * From firma", baglan);
            ac3.Fill(tablo, "firma");
            dataGridView3.DataSource = tablo.Tables["firma"];
            baglan.Close();

            //---Total....
            DataTable dt = new DataTable();
            ac3.Fill(dt);
            ac3.Dispose();
            foreach (DataRow item in dt.Rows)
            {
                if (item["f_borc"].ToString() != "")
                {
                    borc2 += Convert.ToDecimal(item["f_borc"].ToString());
                }
            }
            borc = borc2;
            //***Total****
            //***Company Transactions***
        }

        public void yukle_kasa()
        {
            //---Cash Transactions---
            Decimal bakiye2 = 0, verilen2 = 0;
            dataGridView4.Columns.Clear();
            tablo.Tables.Clear();
            baglan.Open();
            OleDbDataAdapter ac4 = new OleDbDataAdapter("Select tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan From kasa ORDER BY t_no ASC", baglan);
            ac4.Fill(tablo, "kasa");
            dataGridView4.DataSource = tablo.Tables["kasa"];
            //---Total....
            DataTable dt = new DataTable();
            ac4.Fill(dt);
            ac4.Dispose();
            foreach (DataRow item in dt.Rows)
            {
                if (item["alinan"].ToString() != "")
                {
                    bakiye2 += Convert.ToDecimal(item["alinan"].ToString());
                }
                if (item["verilen"].ToString() != "")
                {
                    verilen2 += Convert.ToDecimal(item["verilen"].ToString());
                }
            }
            baglan.Close();
            bakiye += bakiye2;
            verilen = verilen2 - gider;
            //***Total****
            //***Cash Transactions***
        }

        public Form1()
        {
            InitializeComponent();
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            yukle_genel();
            yukle_musteri();
            yukle_firma();
            yukle_kasa();
            dateTimePicker1.Text = Convert.ToString(DateTime.Today);
            dateTimePicker2.Text = Convert.ToString(DateTime.Today);
            dateTimePicker3.Text = Convert.ToString(DateTime.Today);
            label33.Text = Convert.ToString(bakiye - verilen);
            label35.Text = Convert.ToString(borc);
            label37.Text = Convert.ToString(alacak);
        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
            {
                MessageBox.Show("Please fill in the fields completely...", "Error");
            }
            else
            {
                if (MessageBox.Show("Register customer?", "Register", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    MessageBox.Show("Registration has been canceled...", "Register");
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                }
                else
                {
                    tablo.Tables.Clear();
                    baglan.Open();
                    OleDbDataAdapter ac = new OleDbDataAdapter("Insert Into musteri (m_ad, m_tel, m_borc) Values ('" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox5.Text + "')", baglan);
                    ac.Fill(tablo, "musteri");
                    baglan.Close();
                    yukle_musteri();
                    //--Transfer to Cash Account---
                    tablo.Tables.Clear();
                    baglan.Open();
                    OleDbDataAdapter ac2 = new OleDbDataAdapter("Insert Into kasa (tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan) Values ('" + DateTime.Today + "', '" + textBox3.Text + "', '" + textBox5.Text + "', '0', '0', '0', '" + textBox5.Text + "')", baglan);
                    ac2.Fill(tablo, "kasa");
                    baglan.Close();
                    yukle_genel();
                    yukle_musteri();
                    yukle_firma();
                    yukle_kasa();
                    label33.Text = Convert.ToString(bakiye - verilen);
                    label35.Text = Convert.ToString(borc);
                    label37.Text = Convert.ToString(alacak);
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    //**Transfer to Cash Account***
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.Columns.Clear();
            tablo.Tables.Clear();
            baglan.Open();
            OleDbDataAdapter ac = new OleDbDataAdapter("Select m_no, m_ad, m_tel, m_borc from musteri Where m_ad = '" + textBox6.Text + "' ORDER BY m_no ASC ", baglan);
            ac.Fill(tablo, "musteri");
            dataGridView2.DataSource = tablo.Tables["musteri"];
            baglan.Close();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            yukle_musteri();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int satir;
            satir = dataGridView2.CurrentRow.Index;
            textBox7.Text = dataGridView2.Rows[satir].Cells[0].Value.ToString();
            textBox8.Text = dataGridView2.Rows[satir].Cells[1].Value.ToString();
            textBox9.Text = dataGridView2.Rows[satir].Cells[2].Value.ToString();
            textBox10.Text = dataGridView2.Rows[satir].Cells[3].Value.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox11.Text == "")
            {
                textBox11.Text = "0";
            }
            if (textBox22.Text == "")
            {
                textBox22.Text = "0";
            }
            int sonuc;
            sonuc = Convert.ToInt32(textBox10.Text) - Convert.ToInt32(textBox11.Text) + Convert.ToInt32(textBox22.Text);
            string sorgu = "update musteri set m_ad='" + textBox8.Text + "',m_tel='" + textBox9.Text + "',m_borc='" + sonuc + "' where m_no=" + textBox7.Text + "";
            OleDbCommand komut = new OleDbCommand(sorgu, baglan);
            baglan.Open();
            komut.ExecuteNonQuery();
            baglan.Close();
            yukle_musteri();
            //--Transfer to Cash Account---
            tablo.Tables.Clear();
            baglan.Open();
            decimal toplam, kalan;
            toplam = (Convert.ToInt32(textBox10.Text) + Convert.ToInt32(textBox22.Text));
            kalan = toplam - Convert.ToInt32(textBox11.Text);
            OleDbDataAdapter ac2 = new OleDbDataAdapter("Insert Into kasa (tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan) Values ('" + DateTime.Today + "', '" + textBox8.Text + "', '" + toplam + "', '0', '" + textBox11.Text + "', '0', '" + kalan + "')", baglan);
            ac2.Fill(tablo, "kasa");
            baglan.Close();
            yukle_genel();
            yukle_musteri();
            yukle_firma();
            yukle_kasa();
            label33.Text = Convert.ToString(bakiye - verilen);
            label35.Text = Convert.ToString(borc);
            label37.Text = Convert.ToString(alacak);
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Text = "0";
            textBox22.Text = "0";
            //**Transfer to Cash Account***
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if (textBox11.Text == "")
            {
                textBox11.Text = "0";
            }
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            if (textBox22.Text == "")
            {
                textBox22.Text = "0";
            }
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox7_DoubleClick(object sender, EventArgs e)
        {
            
            
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (textBox8.Enabled == false)
            {
                textBox8.Enabled = true;
            }
            else
            {
                textBox8.Enabled = false;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (textBox9.Enabled == false)
            {
                textBox9.Enabled = true;
            }
            else
            {
                textBox9.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox12.Clear();
            textBox13.Clear();
            textBox14.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox12.Text == "" || textBox13.Text == "" || textBox14.Text == "")
            {
                MessageBox.Show("Please fill in the fields completely...", "Error");
            }
            else
            {
                if (MessageBox.Show("Register company?", "Register", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    MessageBox.Show("Registration has been canceled...", "Register");
                    textBox12.Clear();
                    textBox13.Clear();
                    textBox14.Clear();
                }
                else
                {
                    tablo.Tables.Clear();
                    baglan.Open();
                    OleDbDataAdapter ac = new OleDbDataAdapter("Insert Into firma (f_ad, f_tel, f_borc) Values ('" + textBox12.Text + "', '" + textBox13.Text + "', '" + textBox14.Text + "')", baglan);
                    ac.Fill(tablo, "firma");
                    baglan.Close();
                    yukle_firma();
                    //--Transfer to Cash Account---
                    tablo.Tables.Clear();
                    baglan.Open();
                    OleDbDataAdapter ac2 = new OleDbDataAdapter("Insert Into kasa (tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan) Values ('" + DateTime.Today + "', '" + textBox12.Text + "', '0', '" + textBox14.Text + "', '0', '0', '" + textBox14.Text + "')", baglan);
                    ac2.Fill(tablo, "kasa");
                    baglan.Close();
                    yukle_genel();
                    yukle_musteri();
                    yukle_firma();
                    yukle_kasa();
                    label33.Text = Convert.ToString(bakiye - verilen);
                    label35.Text = Convert.ToString(borc);
                    label37.Text = Convert.ToString(alacak);
                    textBox12.Clear();
                    textBox13.Clear();
                    textBox14.Clear();
                    //**Transfer to Cash Account***
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView3.Columns.Clear();
            tablo.Tables.Clear();
            baglan.Open();
            OleDbDataAdapter ac = new OleDbDataAdapter("Select * from firma Where f_ad = '" + textBox15.Text + "' ORDER BY f_no ASC ", baglan);
            ac.Fill(tablo, "firma");
            dataGridView3.DataSource = tablo.Tables["firma"];
            baglan.Close();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            yukle_firma();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int satir;
            satir = dataGridView3.CurrentRow.Index;
            textBox16.Text = dataGridView3.Rows[satir].Cells[0].Value.ToString();
            textBox17.Text = dataGridView3.Rows[satir].Cells[1].Value.ToString();
            textBox18.Text = dataGridView3.Rows[satir].Cells[2].Value.ToString();
            textBox19.Text = dataGridView3.Rows[satir].Cells[3].Value.ToString();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (textBox17.Enabled == false)
            {
                textBox17.Enabled = true;
            }
            else
            {
                textBox17.Enabled = false;
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (textBox18.Enabled == false)
            {
                textBox18.Enabled = true;
            }
            else
            {
                textBox18.Enabled = false;
            }
        }

        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox20.Text == "")
            {
                textBox20.Text = "0";
            }
            if (textBox21.Text == "")
            {
                textBox21.Text = "0";
            }
            int sonuc;
            sonuc = Convert.ToInt32(textBox19.Text) - Convert.ToInt32(textBox20.Text) + Convert.ToInt32(textBox21.Text);
            string sorgu = "update firma set f_ad='" + textBox17.Text + "',f_tel='" + textBox18.Text + "',f_borc='" + sonuc + "' where f_no=" + textBox16.Text;
            OleDbCommand komut = new OleDbCommand(sorgu, baglan);
            baglan.Open();
            komut.ExecuteNonQuery();
            baglan.Close();
            yukle_firma();
            //--Transfer to Cash Account (Total)---
            tablo.Tables.Clear();
            baglan.Open();
            decimal toplam, kalan;
            toplam = (Convert.ToInt32(textBox19.Text) + Convert.ToInt32(textBox21.Text));
            kalan = toplam - Convert.ToInt32(textBox20.Text);
            OleDbDataAdapter ac2 = new OleDbDataAdapter("Insert Into kasa (tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan) Values ('" + DateTime.Today + "', '" + textBox17.Text + "', '0', '" + toplam + "', '0', '" + textBox20.Text + "', '" + kalan + "')", baglan);
            ac2.Fill(tablo, "kasa");
            baglan.Close();
            yukle_genel();
            yukle_musteri();
            yukle_firma();
            yukle_kasa();
            label33.Text = Convert.ToString(bakiye - verilen);
            label35.Text = Convert.ToString(borc);
            label37.Text = Convert.ToString(alacak);
            textBox16.Clear();
            textBox17.Clear();
            textBox18.Clear();
            textBox19.Clear();
            textBox20.Text = "0";
            textBox21.Text = "0";
            //**Transfer to Cash Account (Total)***
        }

        private void button14_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Please fill in the fields completely...", "Error");
            }
            else
            {
                if (MessageBox.Show("Save the expense?", "Register", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    MessageBox.Show("Registration has been canceled...", "Register");
                    textBox1.Clear();
                    textBox2.Clear();
                }
                else
                {
                    tablo.Tables.Clear();
                    baglan.Open();
                    OleDbDataAdapter ac = new OleDbDataAdapter("Insert Into gider (tarih, fiyat, aciklama) Values ('" + dateTimePicker1.Text + "', " + textBox1.Text + ", '" + textBox2.Text + "')", baglan);
                    ac.Fill(tablo, "gider");
                    baglan.Close();
                    yukle_genel();
                    //--Transfer to Cash Account---
                    tablo.Tables.Clear();
                    baglan.Open();
                    OleDbDataAdapter ac2 = new OleDbDataAdapter("Insert Into kasa (tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan) Values ('" + dateTimePicker1.Text + "', '" + textBox2.Text + "', '0', '0', '0', '" + textBox1.Text + "', '0')", baglan);
                    ac2.Fill(tablo, "kasa");
                    baglan.Close();
                    yukle_genel();
                    yukle_musteri();
                    yukle_firma();
                    yukle_kasa();
                    label33.Text = Convert.ToString(bakiye - verilen);
                    label35.Text = Convert.ToString(borc);
                    label37.Text = Convert.ToString(alacak);
                    textBox1.Clear();
                    textBox2.Clear();
                    //**Transfer to Cash Account***
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button22_Click(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int satir;
            satir = dataGridView1.CurrentRow.Index;
            textBox23.Text = dataGridView1.Rows[satir].Cells[0].Value.ToString();
            textBox26.Text = dataGridView1.Rows[satir].Cells[1].Value.ToString();
            textBox24.Text = dataGridView1.Rows[satir].Cells[2].Value.ToString();
            textBox25.Text = dataGridView1.Rows[satir].Cells[3].Value.ToString();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (textBox23.Text == "")
            {
                MessageBox.Show("Please select the record you want to delete...", ",Error");
            }
            else
            {
                if (MessageBox.Show("Delete Registration?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    MessageBox.Show("Deletion aborted...", "Delete");
                }
                else
                {
                    baglan.Open();
                    OleDbDataAdapter ac = new OleDbDataAdapter("Delete From gider Where g_no = " + textBox23.Text, baglan);
                    ac.Fill(tablo, "gider");
                    baglan.Close();
                    //--Transfer to Cash Account---
                    tablo.Tables.Clear();
                    baglan.Open();
                    OleDbDataAdapter ac2 = new OleDbDataAdapter("Insert Into kasa (tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan) Values ('" + textBox26.Text + "', 'Silme - " + textBox25.Text + "', '0', '0', '" + textBox24.Text + "', '0', '0')", baglan);
                    ac2.Fill(tablo, "kasa");
                    baglan.Close();
                    yukle_genel();
                    yukle_musteri();
                    yukle_firma();
                    yukle_kasa();
                    label33.Text = Convert.ToString(bakiye - verilen);
                    label35.Text = Convert.ToString(borc);
                    label37.Text = Convert.ToString(alacak);
                    textBox23.Clear();
                    textBox24.Clear();
                    textBox25.Clear();
                    textBox26.Clear();
                    //**Transfer to Cash Account***
                }
            }  
        }

        private void button23_Click(object sender, EventArgs e)
        {
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            yukle_genel();
            yukle_musteri();
            yukle_firma();
            yukle_kasa();
            label33.Text = Convert.ToString(bakiye - verilen);
            label35.Text = Convert.ToString(borc);
            label37.Text = Convert.ToString(alacak);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Enabled = true;
            dateTimePicker3.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Enabled = false;
            dateTimePicker3.Enabled = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox24_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox18_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox19_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Decimal hesap = 0;
            if (radioButton1.Checked)
            {
                dataGridView4.Columns.Clear();
                tablo.Tables.Clear();
                baglan.Open();
                OleDbDataAdapter ac = new OleDbDataAdapter("select tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan from kasa where tarih >= #" + dateTimePicker2.Value.Month + "/" + dateTimePicker2.Value.Day + "/" + dateTimePicker2.Value.Year + "# and tarih <= #" + dateTimePicker3.Value.Month + "/" + dateTimePicker3.Value.Day + "/" + dateTimePicker3.Value.Year + "# ORDER BY t_no ASC", baglan);
                ac.Fill(tablo, "kasa");
                dataGridView4.DataSource = tablo.Tables["kasa"];

                //---Total....
                DataTable dt = new DataTable();
                ac.Fill(dt);
                ac.Dispose();
                foreach (DataRow item in dt.Rows)
                {
                    if (item["verilen"].ToString() != "")
                    {
                        hesap += Convert.ToDecimal(item["verilen"].ToString());
                    }
                }
                label44.Text = Convert.ToString(hesap);
                hesap = 0;
                foreach (DataRow item in dt.Rows)
                {
                    if (item["alinan"].ToString() != "")
                    {
                        hesap += Convert.ToDecimal(item["alinan"].ToString());
                    }
                }
                label42.Text = Convert.ToString(hesap);
                hesap = 0;
                //***Total****

                baglan.Close();
            }
            else
            {
                dataGridView4.Columns.Clear();
                tablo.Tables.Clear();
                baglan.Open();
                OleDbDataAdapter ac = new OleDbDataAdapter("select tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan from kasa ORDER BY t_no ASC", baglan);
                ac.Fill(tablo, "kasa");
                dataGridView4.DataSource = tablo.Tables["kasa"];
                //---Total....
                DataTable dt2 = new DataTable();
                ac.Fill(dt2);
                ac.Dispose();
                foreach (DataRow item in dt2.Rows)
                {
                    if (item["verilen"].ToString() != "")
                    {
                        hesap += Convert.ToDecimal(item["verilen"].ToString());
                    }
                }
                label44.Text = Convert.ToString(hesap);
                hesap = 0;
                foreach (DataRow item in dt2.Rows)
                {
                    if (item["alinan"].ToString() != "")
                    {
                        hesap += Convert.ToDecimal(item["alinan"].ToString());
                    }
                }
                label42.Text = Convert.ToString(hesap);
                hesap = 0;
                baglan.Close();
                //***Total****
            }
        }

        private void label44_Click(object sender, EventArgs e)
        {
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            int para = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("Enter the amount you want to add ($):", "", "", -1, -1));
            dataGridView4.Columns.Clear();
            tablo.Tables.Clear();
            baglan.Open();
            OleDbDataAdapter ac2 = new OleDbDataAdapter("Insert Into kasa (tarih, kisi_firma, alacak, verecek, alinan, verilen, kalan) Values ('" + DateTime.Today + "', 'Kasa Hesabına Yatırma', '0', '0', '" + para + "', '0', '0')", baglan);
            ac2.Fill(tablo, "kasa");
            baglan.Close();
            yukle_genel();
            yukle_musteri();
            yukle_firma();
            yukle_kasa();
            label33.Text = Convert.ToString(bakiye - verilen);
            label35.Text = Convert.ToString(borc);
            label37.Text = Convert.ToString(alacak);
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            if (textBox20.Text == "")
            {
                textBox20.Text = "0";
            }
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            if (textBox21.Text == "")
            {
                textBox21.Text = "0";
            }
        }
    }
}
