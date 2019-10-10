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
using System.Xml;

namespace GW2MapGetTool
{
    public partial class Form1 : Form
    {
        #region 地图图片获取模块公共参数
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
        int 完成数 = 0;
        int 项目数 = 0;
        int 错误数 = 0;
        #endregion

        #region 地图图片获取
        private void Button1_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.autoget && !timer1.Enabled)
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
            if (!Directory.Exists(Application.StartupPath + "/1/" + tx + "/" + t))
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
                        Text = t.ToString() + "-" + i.ToString(),
                        Size = new Size(58, 20)
                    };
                    flowLayoutPanel1.Controls.Add(button);
                    Thread thread = new Thread(new ParameterizedThreadStart(delegate { Start(t, i, button); }))
                    {
                        IsBackground = true
                    };
                    thread.Start();
                    项目数++;
                    Application.DoEvents();

                }
            }
        }

        private void Start(int ii, int iie, Button button)
        {

            System.Threading.Thread.Sleep(300);
            if (!File.Exists(Application.StartupPath + "/1/" + tx + "/" + ii + "/" + iie + ".jpg") || File.ReadAllBytes(Application.StartupPath + "/1/" + tx + "/" + ii + "/" + iie + ".jpg").Length < 2)
            {
                var wc2 = new WebClient();

                try
                {
                    Uri dws = new Uri("http://tiles.guildwars2.com/1/1/" + tx + "/" + ii + "/" + iie + ".jpg");
                    wc2.DownloadFile(dws, Application.StartupPath + "/1/" + tx + "/" + ii + "/" + iie + ".jpg");
                    button.BackColor = Color.Green;
                    textBox1.AppendText(tx.ToString() + "-" + ii.ToString() + "-" + iie.ToString() + "完成\r\n");
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
            if (t < txm)
            {
                t++;
            }
            else
            {
                textBox1.AppendText(t.ToString() + "最大值了\r\n");
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
            if (完成数 == 项目数 && 错误数 == 0 && 项目数 > 0)
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
            if (完成数 + 错误数 == 项目数 && 错误数 > 0 && 完成数 > 0)
            {
                xiazai();
            }
            else
            {
                if (错误数 == 项目数 && 项目数 == 层级X数目[tx])
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
        private void Button5_Click(object sender, EventArgs e)
        {
            tx = 1;
            label8.Text = tx.ToString();
            Properties.Settings.Default.层级数据 = tx;
            Properties.Settings.Default.Save();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (tx < 7)
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
            Properties.Settings.Default.最大x获取 = txm = (int)numericUpDown8.Value;
            Properties.Settings.Default.Save();
        }
        #endregion



        #region 窗口
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            checkBox1.Checked = Properties.Settings.Default.autoget;
            checkBox2.Checked = Properties.Settings.Default.自动获取信息;
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
            //加载xml文件
            dataTable.Columns.Add(new DataColumn("地图名", typeof(System.String)));
            dataTable.Columns.Add(new DataColumn("地板", typeof(System.Int32)));
            dataTable.Columns.Add(new DataColumn("地域", typeof(System.Int32)));
            dataTable.Columns.Add(new DataColumn("地图ID", typeof(System.Int32)));
            if (File.Exists(".\\地图参数设置保存.xml"))
            {
                载入();
            }
            else
            {
                重新载入();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {


        }
        #endregion

        #region 地图数据获取公共参数
        DataTable dataTable = new DataTable("maps");
        public int[] 地板集合;
        public int[] 地区集合;
        public int[] 地图ID集合;
        public int 当前项目排序 = 0;
        #endregion

        //重载默认
        private void button10_Click(object sender, EventArgs e)
        {
            重新载入();
        }
        //地图删除
        private void button11_Click(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count >= 1)
            {
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
            }
        }
        //地图保存
        private void button12_Click(object sender, EventArgs e)
        {
            dataTable.WriteXml(".\\地图参数设置保存.xml", XmlWriteMode.WriteSchema, true);
        }

        private void 载入()
        {
            dataTable.ReadXml(".\\地图参数设置保存.xml");
            dataGridView1.DataSource = dataTable;
        }

        private void 重新载入()
        {
            dataTable.Rows.Clear();
            dataTable.Rows.Add(new object[] { "神佑之城", 1, 4, 18 });
            dataTable.Rows.Add(new object[] { "狮子拱门", 1, 4, 50 });
            dataTable.Rows.Add(new object[] { "圣林之地", 1, 5, 91 });
            dataTable.Rows.Add(new object[] { "拉塔索姆", 1, 5, 139 });
            dataTable.Rows.Add(new object[] { "黑烟壁垒", 1, 2, 218 });
            dataTable.Rows.Add(new object[] { "霍布雷克", 1, 1, 326 });
            dataTable.Rows.Add(new object[] { "利爪岛", 1, 4, 335 });
            dataTable.Rows.Add(new object[] { "北方之眼", 1, 1, 807 });
            dataTable.Rows.Add(new object[] { "迷城峭壁", 1, 5, 922 });
            dataTable.Rows.Add(new object[] { "灵魂山谷", 1, 10, 1147 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1147?wiki=1&lang=zh 灵魂山谷
            dataTable.Rows.Add(new object[] { "救赎小道", 1, 10, 1149 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1149?wiki=1&lang=zh 救赎小道
            dataTable.Rows.Add(new object[] { "狮子拱门机场", 1, 4, 1155 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/1155?wiki=1&lang=zh 机场
            dataTable.Rows.Add(new object[] { "信仰要塞", 1, 10, 1156 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1156?wiki=1&lang=zh 信仰要塞
            dataTable.Rows.Add(new object[] { "忏悔者堡垒", 1, 10, 1188 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1188?wiki=1&lang=zh 忏悔者堡垒
            dataTable.Rows.Add(new object[] { "阿达西姆之钥", 49, 12, 1323 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1323?wiki=1&lang=zh 阿达西姆之钥
            dataTable.Rows.Add(new object[] { "镀金山洞", 0, 10, 1068 });
            //https://api.guildwars2.com/v2/continents/1/floors/0/regions/10/maps/1068?wiki=1&lang=zh 镀金山洞
            dataTable.Rows.Add(new object[] { "失落悬崖", 1, 10, 1069 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1069?wiki=1&lang=zh 失落悬崖
            dataTable.Rows.Add(new object[] { "迎风庇护所", 49, 12, 1214 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1214?wiki=1&lang=zh 迎风庇护所
            dataTable.Rows.Add(new object[] { "法兰努尔，第一座城市", 1, 12, 1268 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/12/maps/1268?wiki=1&lang=zh 法兰努尔，第一座城市
            dataTable.Rows.Add(new object[] { "卡尔段直升", 1, 5, 34 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/5/maps/34?wiki=1&lang=en 卡尔段直升
            dataTable.Rows.Add(new object[] { "度量领域", 1, 5, 35 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/5/maps/35?wiki=1&lang=en 度量领域
            dataTable.Rows.Add(new object[] { "阿斯福德平原", 1, 2, 19 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/2/maps/19?wiki=1&lang=en 阿斯福德平原
            dataTable.Rows.Add(new object[] { "女王谷", 1, 4, 15 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/15?wiki=1&lang=en 女王谷
            dataTable.Rows.Add(new object[] { "旅者丘陵", 1, 1, 28 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/28?wiki=1&lang=en 旅者丘陵
            dataTable.Rows.Add(new object[] { "布里斯班野地", 1, 5, 54 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/5/maps/54?wiki=1&lang=en 布里斯班野地
            dataTable.Rows.Add(new object[] { "底耶沙高地", 1, 2, 32 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/2/maps/32?wiki=1&lang=en 底液儍高地
            dataTable.Rows.Add(new object[] { "凯席斯山", 1, 4, 23 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/23?wiki=1&lang=en 凯席斯山
            dataTable.Rows.Add(new object[] { "漂流雪境", 1, 1, 31 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/31?wiki=1&lang=en 漂流雪境
            dataTable.Rows.Add(new object[] { "甘达拉战区", 1, 4, 24 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/24?wiki=1&lang=en 嘎达那咱去
            dataTable.Rows.Add(new object[] { "罗拉通道", 1, 1, 27 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/27?wiki=1&lang=en 罗拉通道
            dataTable.Rows.Add(new object[] { "废墟原野", 1, 2, 21 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/2/maps/21?wiki=1&lang=en 废墟原野
            dataTable.Rows.Add(new object[] { "哈拉希腹地", 1, 4, 17 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/17?wiki=1&lang=en 哈拉希腹地
            dataTable.Rows.Add(new object[] { "裂脊草原", 1, 2, 20 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/2/maps/20?wiki=1&lang=en 劣迹草原
            dataTable.Rows.Add(new object[] { "掘洞悬崖", 1, 1, 26 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/26?wiki=1&lang=en 掘洞悬崖
            dataTable.Rows.Add(new object[] { "血潮海岸", 1, 4, 73 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/73?wiki=1&lang=en 血槽海岸
            dataTable.Rows.Add(new object[] { "钢铁平原", 1, 2, 25 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/2/maps/25?wiki=1&lang=en 钢铁平原
            dataTable.Rows.Add(new object[] { "林线瀑布", 1, 1, 29 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/29?wiki=1&lang=en 林线瀑布
            dataTable.Rows.Add(new object[] { "闪萤沼泽", 1, 8, 53 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/8/maps/53?wiki=1&lang=en 闪萤沼泽
            dataTable.Rows.Add(new object[] { "炎心高地", 1, 2, 22 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/2/maps/22?wiki=1&lang=en 岩心高地
            dataTable.Rows.Add(new object[] { "漩涡山", 1, 8, 39 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/8/maps/39?wiki=1&lang=en 漩涡上
            dataTable.Rows.Add(new object[] { "浩劫海峡", 1, 3, 51 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/3/maps/51?wiki=1&lang=en 浩劫海啸
            dataTable.Rows.Add(new object[] { "霜谷之音", 1, 1, 30 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/30?wiki=1&lang=en 霜谷之音
            dataTable.Rows.Add(new object[] { "马尔科之跃", 1, 3, 65 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/3/maps/65?wiki=1&lang=en 马尔科之跃
            dataTable.Rows.Add(new object[] { "诅咒海岸", 1, 3, 62 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/3/maps/62?wiki=1&lang=en 诅咒海岸
            dataTable.Rows.Add(new object[] { "南阳海湾", 1, 4, 873 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/873?wiki=1&lang=en 南阳海湾
            dataTable.Rows.Add(new object[] { "干凅高地", 1, 11, 988 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/11/maps/988?wiki=1&lang=en 干枯高地
            dataTable.Rows.Add(new object[] { "白银荒地", 1, 11, 1015 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/11/maps/1015?wiki=1&lang=en 白银荒地
            dataTable.Rows.Add(new object[] { "苍翠边界", 1, 10, 1052 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1052?wiki=1&lang=en 苍翠边界
            dataTable.Rows.Add(new object[] { "赤金盆地", 1, 10, 1043 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1043?wiki=1&lang=en 赤金盆地
            dataTable.Rows.Add(new object[] { "缠藤深渊", 1, 10, 1045 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1045?wiki=1&lang=en 缠藤深渊
            dataTable.Rows.Add(new object[] { "巨龙阵地", 1, 10, 1041 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1041?wiki=1&lang=en 巨龙阵地
            dataTable.Rows.Add(new object[] { "血石沼泽", 1, 10, 1165 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/10/maps/1165?wiki=1&lang=en 血石沼泽
            dataTable.Rows.Add(new object[] { "余烬海湾", 1, 20, 1175 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/20/maps/1175?wiki=1&lang=en 余烬海湾
            dataTable.Rows.Add(new object[] { "酷寒前线", 1, 1, 1178 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/1178?wiki=1&lang=en 酷寒前线
            dataTable.Rows.Add(new object[] { "多里克湖", 1, 4, 1185 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/4/maps/1185?wiki=1&lang=en 多里克湖
            dataTable.Rows.Add(new object[] { "巨龙之山", 1, 20, 1195 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/20/maps/1195?wiki=1&lang=en 巨龙之山
            dataTable.Rows.Add(new object[] { "海妖平台", 1, 3, 1203 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/3/maps/1203?wiki=1&lang=en 海妖平台
            dataTable.Rows.Add(new object[] { "沙漠高地", 49, 12, 1211 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1211?wiki=1&lang=en 沙漠高地
            dataTable.Rows.Add(new object[] { "水晶绿洲", 49, 12, 1210 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1210?wiki=1&lang=en 水晶沙漠
            dataTable.Rows.Add(new object[] { "伊伦河湾", 49, 12, 1228 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1228?wiki=1&lang=en 伊伦河湾
            dataTable.Rows.Add(new object[] { "荒芜之地", 49, 12, 1226 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1226?wiki=1&lang=en 荒芜之地
            dataTable.Rows.Add(new object[] { "瓦比领域", 49, 12, 1248 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1248?wiki=1&lang=en 瓦比领域
            dataTable.Rows.Add(new object[] { "伊斯坦领域", 1, 12, 1263 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/12/maps/1263?wiki=1&lang=en 伊斯坦领域
            dataTable.Rows.Add(new object[] { "沙掠群岛", 49, 12, 1271 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1271?wiki=1&lang=zh 沙掠群岛
            dataTable.Rows.Add(new object[] { "克南领域", 49, 12, 1288 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1288?wiki=1&lang=en 克南领域
            dataTable.Rows.Add(new object[] { "亚哈悬崖", 49, 12, 1301 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1301?wiki=1&lang=en 亚哈悬崖
            dataTable.Rows.Add(new object[] { "雷云高峰", 1, 1, 1310 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/1/maps/1310?wiki=1&lang=en 雷云高峰
            dataTable.Rows.Add(new object[] { "坠龙之地", 49, 12, 1317 });
            //https://api.guildwars2.com/v2/continents/1/floors/49/regions/12/maps/1317?wiki=1&lang=en 坠龙之地
            dataTable.Rows.Add(new object[] { "戈瑟玛山谷", 1, 2, 1330 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/2/maps/1330?wiki=1&lang=en 戈瑟玛山谷
            dataTable.WriteXml(".\\地图参数设置保存.xml", XmlWriteMode.WriteSchema, true);
            dataGridView1.DataSource = dataTable;
        }
        //开始
        private void button8_Click(object sender, EventArgs e)
        {
            获取();
            if (Properties.Settings.Default.自动获取信息)
            {
                下载数据();
            }
            else
            {
                自动下载数据();
            }
            
        }
        //增加地图
        private void button9_Click(object sender, EventArgs e)
        {
            if (地板集合.Length < 当前项目排序)
            {
                当前项目排序++;
                label20.Text = "当前地图ID:" + 地图ID集合[当前项目排序].ToString();
                label21.Text = "当前地图排序:" + 当前项目排序.ToString();
            }
        }
        //减少地图
        private void button13_Click(object sender, EventArgs e)
        {
            if (当前项目排序 > 0)
            {
                当前项目排序--;
                label20.Text = "当前地图ID:" + 地图ID集合[当前项目排序].ToString();
                label21.Text = "当前地图排序:" + 当前项目排序.ToString();
            }
        }
        //重新获取
        private void button14_Click(object sender, EventArgs e)
        {

        }
        //保存文件
        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.自动获取信息 = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }

        public void 获取()
        {
            赋值参数();
        }

        public void 赋值参数()
        {
            地板集合 = new int[dataTable.Rows.Count];
            地区集合 = new int[dataTable.Rows.Count];
            地图ID集合 = new int[dataTable.Rows.Count];
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {

                地板集合[i] = int.Parse(dataTable.Rows[i][1].ToString());
                地区集合[i] = int.Parse(dataTable.Rows[i][2].ToString());
                地图ID集合[i] = int.Parse(dataTable.Rows[i][3].ToString());
                textBox2.AppendText(dataTable.Rows[i][0].ToString() + "-地板:" + 地板集合[i] + ",地区:" + 地区集合[i] + ",地图ID:" + 地图ID集合[i] + "\r\n");
            }
            label20.Text = "当前地图ID:" + 地图ID集合[当前项目排序].ToString();
            label21.Text = "当前地图排序:" + 当前项目排序.ToString();
        }

        public void 下载数据()
        {
            //根据当前下载数据
            string jsonString = "";
            //赋值数据
            var mapI = MapinfosJs.FromJson(jsonString);
            //解析数据---
            //解析地图

            //解析各种点
            //解析挑战
            //解析专精
            //解析爱心任务
            //解析地区
            //解析完成---
        }

        public void 自动下载数据()
        {

        }
    }
}
