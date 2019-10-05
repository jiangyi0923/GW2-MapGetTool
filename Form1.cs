using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GW2MapGetTool
{
    public partial class Form1 : Form
    {
        public int t = Properties.Settings.Default.数据;
        public int tx = Properties.Settings.Default.层级数据;
        public int txm = Properties.Settings.Default.最大x获取;
        public bool autoget = Properties.Settings.Default.autoget;
        public int[] 层级X数目 = { 0,
            Properties.Settings.Default.设置1,
            Properties.Settings.Default.设置2,
            Properties.Settings.Default.设置3,
            Properties.Settings.Default.设置4,
            Properties.Settings.Default.设置5,
            Properties.Settings.Default.设置6,
            Properties.Settings.Default.设置7 };
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            checkBox1.Checked = Properties.Settings.Default.autoget;
            label1.Text = t.ToString();
            label2.Text = "0";
            label3.Text = "0";
            label7.Text = "0";
            label8.Text = tx.ToString();
            numericUpDown1.Value = 层级X数目[1];
            numericUpDown2.Value = 层级X数目[2];
            numericUpDown3.Value = 层级X数目[3];
            numericUpDown4.Value = 层级X数目[4];
            numericUpDown5.Value = 层级X数目[5];
            numericUpDown6.Value = 层级X数目[6];
            numericUpDown7.Value = 层级X数目[7];
            numericUpDown8.Value = txm;
        }
        int 完成数 = 0;
        int 项目数 = 0;
        int 错误数 = 0;
        private void Button1_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.autoget&& !timer1.Enabled)
            {
                timer1.Enabled = true;
            }
            else
            {
                if (timer1.Enabled)
                {
                    timer1.Enabled = false;
                }
            }
            xiazai();
        }

        public void xiazai()
        {
             完成数 = 0;
             项目数 = 0;
             错误数 = 0;
            flowLayoutPanel1.Controls.Clear();
            if (!Directory.Exists(Application.StartupPath + "/1/" + tx+"/" + t))
            {
                Directory.CreateDirectory(Application.StartupPath + "/1/" + tx + "/" + t);
                Application.DoEvents();
            }
            {
                //7层0-148,157, 
                //6层0-74,79, 
                //5层0-37,40,
                //4层0-18,20 
                //3层0-9-10,
                //2层0-4-5,
                //1层0-2-3
                
                for (int i = 0; i < 层级X数目[tx]; i++)
                {
                    Button button = new Button
                    {
                        Text = t.ToString()+"-"+i.ToString(),
                        Size = new Size(58, 20)
                    };
                    flowLayoutPanel1.Controls.Add(button);
                    Thread thread = new Thread(new ParameterizedThreadStart(delegate { Start(t,i, button); }))
                    {
                        IsBackground = true
                    };
                    thread.Start();
                    项目数++;
                    Application.DoEvents();
                    
                }
            }
        }

        private void Start(int ii,int iie, Button button)
        {

            System.Threading.Thread.Sleep(300);
            if (!File.Exists(Application.StartupPath + "/1/" + tx + "/" + ii + "/" + iie + ".jpg") || File.ReadAllBytes(Application.StartupPath + "/1/" + tx + "/" + ii + "/" + iie + ".jpg").Length < 2)
            {
                var wc2 = new WebClient();
                
                try
                {
                    Uri dws = new Uri("http://tiles.guildwars2.com/1/1/"+tx+"/" + ii + "/" + iie + ".jpg");
                    wc2.DownloadFile(dws, Application.StartupPath + "/1/" + tx + "/" + ii + "/" + iie + ".jpg");
                    button.BackColor = Color.Green;
                    textBox1.AppendText(tx.ToString()+"-"+ii.ToString() + "-" + iie.ToString() + "完成\r\n");
                    完成数++;
                }
                catch (Exception e)
                {
                    //string dw= e.Message;
                    //MessageBox.Show(e.Message);
                    //textBox1.AppendText(iie.ToString()+"失败" + dw + "\r\n");
                    textBox1.AppendText(tx.ToString() + "-" + ii.ToString() + "-" + iie.ToString() + "失败\r\n");
                    button.BackColor = Color.Red;
                    错误数++;
                    //throw;
                }
                finally
                {
                    wc2.Dispose();
                    
                }
            }
            else
            {
                button.BackColor = Color.Green;
                textBox1.AppendText(tx.ToString() + "-" + ii.ToString() + "-" + iie.ToString() + "已存在\r\n");
                完成数++;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (t< txm)
            {
                t++;
            }
            else
            {
                textBox1.AppendText(t.ToString()+ "最大值了\r\n");
            }
            label1.Text = t.ToString();
            Properties.Settings.Default.数据 = t;
            Properties.Settings.Default.Save();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (t > 0)
            {
                t--;
            }
            else
            {
                textBox1.AppendText(t.ToString() + "最大值了\r\n");
            }
            label1.Text = t.ToString();
            Properties.Settings.Default.数据 = t;
            Properties.Settings.Default.Save();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            label8.Text = tx.ToString();
            label3.Text = 完成数.ToString();
            label2.Text = 错误数.ToString();
            label7.Text = 项目数.ToString();
            if (完成数 == 项目数 && 错误数 ==0 && 项目数 > 0)
            {
                
                if (t < txm)
                {
                    t++;
                    label1.Text = t.ToString();
                    Properties.Settings.Default.数据 = t;
                    Properties.Settings.Default.Save();
                    xiazai();
                }
                else
                {
                    textBox1.AppendText("全部完成\r\n");
                    timer1.Stop();
                }

            }
            if (完成数+ 错误数 == 项目数 && 错误数 > 0 && 完成数 > 0)
            {
                xiazai();
            }
            else
            {
                if (错误数 == 项目数&& 项目数 == 层级X数目[tx])
                {
                    if (Directory.Exists(Application.StartupPath + "/1/" + tx + "/" + t))
                    {
                        Directory.Delete(Application.StartupPath + "/1/" + tx + "/" + t);
                    }
                    if (tx < 7)
                    {
                        tx++;
                        Properties.Settings.Default.层级数据 = tx;
                        Properties.Settings.Default.Save();
                        t = 0;
                        xiazai();
                    }
                    else
                    {
                        textBox1.AppendText("全部完成\r\n");
                        timer1.Stop();
                    }

                }
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            t = 0;
            label1.Text = t.ToString();
            Properties.Settings.Default.数据 = t;
            Properties.Settings.Default.Save();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.autoget = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button5_Click(object sender, EventArgs e)
        {
            tx = 1;
            label8.Text = tx.ToString();
            Properties.Settings.Default.层级数据 = tx;
            Properties.Settings.Default.Save();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (tx<7)
            {
                tx++;
                label8.Text = tx.ToString();
                Properties.Settings.Default.层级数据 = tx;
                Properties.Settings.Default.Save();
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (tx > 1)
            {
                tx--;
                label8.Text = tx.ToString();
                Properties.Settings.Default.层级数据 = tx;
                Properties.Settings.Default.Save();
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            
            Properties.Settings.Default.设置1 = 层级X数目[1] = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.设置2 = 层级X数目[2] = (int)numericUpDown2.Value;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.设置3 = 层级X数目[3] = (int)numericUpDown3.Value;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.设置4 = 层级X数目[4] = (int)numericUpDown4.Value;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.设置5 = 层级X数目[5] = (int)numericUpDown5.Value;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.设置6 = 层级X数目[6] = (int)numericUpDown6.Value;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.设置7 = 层级X数目[7] = (int)numericUpDown7.Value;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.最大x获取 = txm = (int)numericUpDown8.Value ;
            Properties.Settings.Default.Save();
        }
    }
}
