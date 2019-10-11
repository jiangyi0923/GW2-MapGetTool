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
            赋值参数();
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
        public bool 下载地图信息中 = false;

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
                dataGridView1.Rows.Remove(dataGridView1.Rows[dataGridView1.CurrentRow.Index]);
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
            //dataTable.Rows.Add(new object[] { "迷城峭壁", 1, 5, 922 });
            dataTable.Rows.Add(new object[] { "灵魂山谷", 1, 10, 1147 });
            dataTable.Rows.Add(new object[] { "救赎小道", 1, 10, 1149 });
            dataTable.Rows.Add(new object[] { "狮子拱门机场", 1, 4, 1155 });
            dataTable.Rows.Add(new object[] { "信仰要塞", 1, 10, 1156 });
            dataTable.Rows.Add(new object[] { "忏悔者堡垒", 1, 10, 1188 });
            dataTable.Rows.Add(new object[] { "阿达西姆之钥", 49, 12, 1323 });
            dataTable.Rows.Add(new object[] { "镀金山洞", 0, 10, 1068 });
            dataTable.Rows.Add(new object[] { "失落悬崖", 1, 10, 1069 });
            dataTable.Rows.Add(new object[] { "迎风庇护所", 49, 12, 1214 });
            dataTable.Rows.Add(new object[] { "法兰努尔，第一座城市", 1, 12, 1268 });
            //https://api.guildwars2.com/v2/continents/1/floors/1/regions/12/maps/1268?wiki=1&lang=zh 法兰努尔，第一座城市
            dataTable.Rows.Add(new object[] { "卡尔段直升", 1, 5, 34 });
            dataTable.Rows.Add(new object[] { "度量领域", 1, 5, 35 });
            dataTable.Rows.Add(new object[] { "阿斯福德平原", 1, 2, 19 });
            dataTable.Rows.Add(new object[] { "女王谷", 1, 4, 15 });
            dataTable.Rows.Add(new object[] { "旅者丘陵", 1, 1, 28 });
            dataTable.Rows.Add(new object[] { "布里斯班野地", 1, 5, 54 });
            dataTable.Rows.Add(new object[] { "底耶沙高地", 1, 2, 32 });
            dataTable.Rows.Add(new object[] { "凯席斯山", 1, 4, 23 });
            dataTable.Rows.Add(new object[] { "漂流雪境", 1, 1, 31 });
            dataTable.Rows.Add(new object[] { "甘达拉战区", 1, 4, 24 });
            dataTable.Rows.Add(new object[] { "罗拉通道", 1, 1, 27 });
            dataTable.Rows.Add(new object[] { "废墟原野", 1, 2, 21 });
            dataTable.Rows.Add(new object[] { "哈拉希腹地", 1, 4, 17 });
            dataTable.Rows.Add(new object[] { "裂脊草原", 1, 2, 20 });
            dataTable.Rows.Add(new object[] { "掘洞悬崖", 1, 1, 26 });
            dataTable.Rows.Add(new object[] { "血潮海岸", 1, 4, 73 });
            dataTable.Rows.Add(new object[] { "钢铁平原", 1, 2, 25 });
            dataTable.Rows.Add(new object[] { "林线瀑布", 1, 1, 29 });
            dataTable.Rows.Add(new object[] { "闪萤沼泽", 1, 8, 53 });
            dataTable.Rows.Add(new object[] { "炎心高地", 1, 2, 22 });
            dataTable.Rows.Add(new object[] { "漩涡山", 1, 8, 39 });
            dataTable.Rows.Add(new object[] { "浩劫海峡", 1, 3, 51 });
            dataTable.Rows.Add(new object[] { "霜谷之音", 1, 1, 30 });
            dataTable.Rows.Add(new object[] { "马尔科之跃", 1, 3, 65 });
            dataTable.Rows.Add(new object[] { "诅咒海岸", 1, 3, 62 });
            dataTable.Rows.Add(new object[] { "南阳海湾", 1, 4, 873 });
            dataTable.Rows.Add(new object[] { "干凅高地", 1, 11, 988 });
            dataTable.Rows.Add(new object[] { "白银荒地", 1, 11, 1015 });
            dataTable.Rows.Add(new object[] { "苍翠边界", 1, 10, 1052 });
            dataTable.Rows.Add(new object[] { "赤金盆地", 1, 10, 1043 });
            dataTable.Rows.Add(new object[] { "缠藤深渊", 1, 10, 1045 });
            dataTable.Rows.Add(new object[] { "巨龙阵地", 1, 10, 1041 });
            dataTable.Rows.Add(new object[] { "血石沼泽", 1, 10, 1165 });
            dataTable.Rows.Add(new object[] { "余烬海湾", 1, 20, 1175 });
            dataTable.Rows.Add(new object[] { "酷寒前线", 1, 1, 1178 });
            dataTable.Rows.Add(new object[] { "多里克湖", 1, 4, 1185 });
            dataTable.Rows.Add(new object[] { "巨龙之山", 1, 20, 1195 });
            dataTable.Rows.Add(new object[] { "海妖平台", 1, 3, 1203 });
            dataTable.Rows.Add(new object[] { "沙漠高地", 49, 12, 1211 });
            dataTable.Rows.Add(new object[] { "水晶绿洲", 49, 12, 1210 });
            dataTable.Rows.Add(new object[] { "伊伦河湾", 49, 12, 1228 });
            dataTable.Rows.Add(new object[] { "荒芜之地", 49, 12, 1226 });
            dataTable.Rows.Add(new object[] { "瓦比领域", 49, 12, 1248 });
            dataTable.Rows.Add(new object[] { "伊斯坦领域", 1, 12, 1263 });
            dataTable.Rows.Add(new object[] { "沙掠群岛", 49, 12, 1271 });
            dataTable.Rows.Add(new object[] { "克南领域", 49, 12, 1288 });
            dataTable.Rows.Add(new object[] { "亚哈悬崖", 49, 12, 1301 });
            dataTable.Rows.Add(new object[] { "雷云高峰", 1, 1, 1310 });
            dataTable.Rows.Add(new object[] { "坠龙之地", 49, 12, 1317 });
            dataTable.Rows.Add(new object[] { "戈瑟玛山谷", 1, 2, 1330 });
            dataTable.WriteXml(".\\地图参数设置保存.xml", XmlWriteMode.WriteSchema, true);
            dataGridView1.DataSource = dataTable;
        }

        public void 初始化数据文件() 
        {
            if (!Directory.Exists(".\\data"))
            {
                Directory.CreateDirectory(".\\data");
            }
            string sl1 =  "var mapzone = ["+ System.Environment.NewLine;
            string sl2 =  "var mapsasterypoints = [" + System.Environment.NewLine;
            string sl3 =  "var mappoints = [" + System.Environment.NewLine;
            string sl4 =  "var mapsectors = [" + System.Environment.NewLine;
            string sl5 =  "var mapadventure = [" + System.Environment.NewLine;
            File.WriteAllText(".\\data\\dboMapZone.js", sl1, Encoding.UTF8);
            File.WriteAllText(".\\data\\dboMapMasteryPoints.js", sl2, Encoding.UTF8);
            File.WriteAllText(".\\data\\dboMapPoints.js", sl3, Encoding.UTF8);
            File.WriteAllText(".\\data\\dboMapSectors.js", sl4, Encoding.UTF8);
            //File.WriteAllText(".\\data\\dboMapAdventures.js", sl5, Encoding.UTF8);
        }



        public void 结束数据文件()
        {
            string[] sl1 = { "];" };

            string TMP1 = File.ReadAllText(".\\data\\dboMapZone.js", Encoding.UTF8);
            string TMP2 = File.ReadAllText(".\\data\\dboMapMasteryPoints.js", Encoding.UTF8);
            string TMP3 = File.ReadAllText(".\\data\\dboMapPoints.js", Encoding.UTF8);
            string TMP4 = File.ReadAllText(".\\data\\dboMapSectors.js", Encoding.UTF8);
            //string TMP5 = File.ReadAllText(".\\data\\dboMapAdventures.js", Encoding.UTF8);

            TMP1 = TMP1.Substring(0, TMP1.Length - 3) + System.Environment.NewLine; 
            TMP2 = TMP2.Substring(0, TMP2.Length - 3) + System.Environment.NewLine;
            TMP3 = TMP3.Substring(0, TMP3.Length - 3) + System.Environment.NewLine;
            TMP4 = TMP4.Substring(0, TMP4.Length - 3) + System.Environment.NewLine;
            //TMP5 = TMP5.Substring(0, TMP5.Length - 3) + System.Environment.NewLine;

            File.WriteAllText(".\\data\\dboMapZone.js", TMP1, Encoding.UTF8);
            File.WriteAllText(".\\data\\dboMapMasteryPoints.js", TMP2, Encoding.UTF8);
            File.WriteAllText(".\\data\\dboMapPoints.js", TMP3, Encoding.UTF8);
            File.WriteAllText(".\\data\\dboMapSectors.js", TMP4, Encoding.UTF8);
            //File.WriteAllText(".\\data\\dboMapAdventures.js", TMP5, Encoding.UTF8);
            //
            File.AppendAllLines(".\\data\\dboMapZone.js", sl1, Encoding.UTF8);
            File.AppendAllLines(".\\data\\dboMapMasteryPoints.js", sl1, Encoding.UTF8);
            File.AppendAllLines(".\\data\\dboMapPoints.js", sl1, Encoding.UTF8);
            File.AppendAllLines(".\\data\\dboMapSectors.js", sl1, Encoding.UTF8);
            //File.AppendAllLines(".\\data\\dboMapAdventures.js", sl1, Encoding.UTF8);


            //byte[] Save = Properties.Resources.arcdps;
            //FileStream fsObj = new FileStream(".\\data\\arcdps.ini", FileMode.CreateNew);
            //fsObj.Write(Save, 0, Save.Length);
            //fsObj.Close();


            textBox2.AppendText("全部完成并保存文件"+回车);
        }

        //开始
        private void button8_Click(object sender, EventArgs e)
        {
            初始化数据文件();
            获取();
            if (Properties.Settings.Default.自动获取信息)
            {
                timer2.Enabled = true;
            }
            else
            {
                下载数据();
            }
            
        }
        //增加地图
        private void button9_Click(object sender, EventArgs e)
        {
            if (地板集合.Length-1 > 当前项目排序)
            {
                当前项目排序++;
                label20.Text = "当前地图ID:" + 地图ID集合[当前项目排序].ToString();
                label21.Text = "当前地图排序:" + 当前项目排序.ToString()+"/"+ (地板集合.Length - 1);
            }
        }
        //减少地图
        private void button13_Click(object sender, EventArgs e)
        {
            if (当前项目排序 > 0)
            {
                textBox2.AppendText(当前项目排序.ToString());
                当前项目排序--;
                label20.Text = "当前地图ID:" + 地图ID集合[当前项目排序].ToString();
                label21.Text = "当前地图排序:" + 当前项目排序.ToString() + "/" + (地板集合.Length - 1);
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
            textBox2.AppendText("开始赋值参数" + 回车);
            地板集合 = new int[dataTable.Rows.Count];
            地区集合 = new int[dataTable.Rows.Count];
            地图ID集合 = new int[dataTable.Rows.Count];
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                地板集合[i] = int.Parse(dataTable.Rows[i][1].ToString());
                地区集合[i] = int.Parse(dataTable.Rows[i][2].ToString());
                地图ID集合[i] = int.Parse(dataTable.Rows[i][3].ToString());
                //textBox2.AppendText(dataTable.Rows[i][0].ToString() + "-地板:" + 地板集合[i] + ",地区:" + 地区集合[i] + ",地图ID:" + 地图ID集合[i] + "\r\n");
            }
            textBox2.AppendText("赋值参数完成"+回车);
            label20.Text = "当前地图ID:" + 地图ID集合[当前项目排序].ToString();
            label21.Text = "当前地图排序:" + 当前项目排序.ToString() + "/" + (地板集合.Length - 1);
        }

        public void 开始获取地图数据()
        {
            Thread thread = new Thread(new ParameterizedThreadStart(delegate { 下载数据(); }))
            {
                IsBackground = true
            };
            thread.Start();
        }

        public string 回车 = " \r\n";
        public void 下载数据()
        {
            //根据当前下载数据
            textBox2.AppendText("尝试获取:排序- "+ 当前项目排序 + " 地图ID:" + 地图ID集合[当前项目排序] + 回车);
            string jsonString = "";
            try
            {
                string ull = "https://api.guildwars2.com/v2/continents/1/floors/";
                var wc3 = new WebClient();
                wc3.Encoding = System.Text.Encoding.UTF8;
                Uri dws1 = new Uri(ull + 地板集合[当前项目排序] + "/regions/" + 地区集合[当前项目排序] + "/maps/" + 地图ID集合[当前项目排序] + "?wiki=1&lang=zh");
                jsonString = wc3.DownloadString(dws1);

                textBox2.AppendText("当前排序:" + 当前项目排序 + "获取成功" + 回车);
            }
            catch (Exception e)
            {
                textBox2.AppendText("当前排序:" + 当前项目排序 + "获取失败" + 回车);
                textBox2.BackColor = Color.Red;
            }

            textBox2.AppendText("当前排序:" + 当前项目排序 + "开始解析" + 回车);
            //赋值数据
            var mapI =  MapinfosJs.FromJson(jsonString);
            //解析数据---
            //解析地图
            textBox2.AppendText("当前排序:" + 当前项目排序 + " - " + mapI.Name + " 解析完成" + 回车);
            textBox2.AppendText("dboMapZone.js 地图数据:" + 回车);

            if (mapI.Id == 988)
            {
                mapI.ContinentRect[0][1] = 15744;
            }


            textBox2.AppendText("{\"zoneid\": \""+ mapI.Id+ "\", \"name\": \""+ mapI.Name + "\", \"level\": { \"min\": "+ mapI.MinLevel+ ",\"max\": "+ mapI.MaxLevel+ "},\"area\": { \"top\": "+ mapI.ContinentRect[0][1]+ ",\"left\": "+ mapI.ContinentRect[0][0] + ",\"bottom\": "+ mapI.ContinentRect[1][1] + ",\"right\": "+ mapI.ContinentRect[1][0] + "}}," + 回车);
            string[] sl1 = { "  {\"zoneid\": \"" + mapI.Id + "\", \"name\": \"" + mapI.Name + "\", \"level\": { \"min\": " + mapI.MinLevel + ",\"max\": " + mapI.MaxLevel + "},\"area\": { \"top\": " + mapI.ContinentRect[0][1] + ",\"left\": " + mapI.ContinentRect[0][0] + ",\"bottom\": " + mapI.ContinentRect[1][1] + ",\"right\": " + mapI.ContinentRect[1][0] + "}}," };
            File.AppendAllLines(".\\data\\dboMapZone.js", sl1, Encoding.UTF8);
            //解析各种点
            textBox2.AppendText("dboMapPoints.js 点数据:" + 回车);
            string[] 所有1 = new string[mapI.PointsOfInterest.Count];
            int tmp1 = 0;
            foreach (var item in mapI.PointsOfInterest)
            {
                switch (item.Value.Type)
                {
                    case "landmark":
                    case "waypoint":
                    case "unlock":
                        if (item.Value.Id == 2344)
                        {
                            item.Value.Name = "狮子拱门机场";
                        }
                        if (item.Value.Id == 2850)
                        {
                            item.Value.Name = "大型地下城——雾锁殿堂";
                        }
                        if (item.Value.Id == 3053)
                        {
                            item.Value.Name = "大型地下城——阿达西姆之钥";
                        }
                        textBox2.AppendText("{ \"zoneid\": \""+ mapI.Id + "\",\"itemid\": \"" + item.Value.Id + "\",\"type\": \"" + item.Value.Type + "\",\"name\": \""+item.Value.Name+"\",\"pos\": { \"x\": "+ item.Value.Coord[0] + ", \"y\": "+ item.Value.Coord[1] + "},\"chat_link\": \""+ item.Value.ChatLink+ "\"}," + 回车);

                        所有1[tmp1] = "   { \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Value.Id + "\",\"type\": \"" + item.Value.Type + "\",\"name\": \"" + item.Value.Name + "\",\"pos\": { \"x\": " + item.Value.Coord[0] + ", \"y\": " + item.Value.Coord[1] + "},\"chat_link\": \"" + item.Value.ChatLink + "\"},";
                        tmp1++;

                        break;
                    case "vista":
                        textBox2.AppendText("{ \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Value.Id + "\",\"type\": \"" + item.Value.Type + "\",\"name\": \"观景点\",\"pos\": { \"x\": " + item.Value.Coord[0] + ", \"y\": " + item.Value.Coord[1] + "},\"chat_link\": \"" + item.Value.ChatLink + "\"}," + 回车);
                        所有1[tmp1] = "   { \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Value.Id + "\",\"type\": \"" + item.Value.Type + "\",\"name\": \"观景点\",\"pos\": { \"x\": " + item.Value.Coord[0] + ", \"y\": " + item.Value.Coord[1] + "},\"chat_link\": \"" + item.Value.ChatLink + "\"},";
                        tmp1++;
                        break;
                        break;
                    default:
                        break;
                }
            }
            
            if (tmp1 == mapI.PointsOfInterest.Count)
            {
                if (mapI.Id == 50)
                {
                    string[] assd = {
                        "   { \"zoneid\": \"50\",\"itemid\": \"2336\",\"type\": \"waypoint\",\"name\": \"机场传送点\",\"pos\": { \"x\": 16563.4, \"y\": 15752.9},\"chat_link\": \"[&BCAJAAA=]\"},",
                        "   { \"zoneid\": \"50\",\"itemid\": \"2970\",\"type\": \"unlock\",\"name\": \"特种部队训练场\",\"pos\": { \"x\": 16618.9, \"y\": 15829.7},\"chat_link\": \"[&BJoLAAA=]\"},"
                    };
                    File.AppendAllLines(".\\data\\dboMapPoints.js", assd, Encoding.UTF8);
                }
                File.AppendAllLines(".\\data\\dboMapPoints.js", 所有1, Encoding.UTF8);
                tmp1 = 0;
            }

            //解析爱心任务
            //textBox2.AppendText("dboMapTask.js 任务数据:" + 回车);
            string[] 所有2 = new string[mapI.Tasks.Values.Count];
            foreach (var item in mapI.Tasks.Values)
            {
                if (item.Id == 114)
                {
                    item.Objective = "帮助学者奥伦唬骗星神现世附近的穴居人。";
                }
                textBox2.AppendText("{ \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"task\",\"name\": \"" + item.Objective + "\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "},\"chat_link\": \"" + item.ChatLink + "\"}," + 回车);
                所有2[tmp1] = "   { \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"task\",\"name\": \"" + item.Objective + "\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "},\"chat_link\": \"" + item.ChatLink + "\"},";
                tmp1++;
            }
            if (tmp1 == mapI.Tasks.Values.Count)
            {
                File.AppendAllLines(".\\data\\dboMapPoints.js", 所有2, Encoding.UTF8);
                tmp1 = 0;
            }
            //解析技能挑战
            //textBox2.AppendText("dboMapSklls.js 地区数据:" + 回车);
            string[] 所有3 = new string[mapI.SkillChallenges.Length];
            foreach (var item in mapI.SkillChallenges)
            {
                textBox2.AppendText("{ \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"skill\",\"name\": \"技能点\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "}}," + 回车);
                所有3[tmp1] = "   { \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"skill\",\"name\": \"技能点\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "}},";
                tmp1++;
            }
            if (tmp1 == mapI.SkillChallenges.Length)
            {
                File.AppendAllLines(".\\data\\dboMapPoints.js", 所有3, Encoding.UTF8);
                tmp1 = 0;
            }
            //解析专精
            //textBox2.AppendText("dboMapMasteryPoints.js 专精数据:" + 回车);
            string[] 所有4 = new string[mapI.MasteryPoints.Length];
            foreach (var item in mapI.MasteryPoints)
            {
                textBox2.AppendText("{ \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"region\": \"" + item.Region + "\",\"type\": \"masteryPoints\",\"name\": \"专精点\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "}}," + 回车);//region
                所有4[tmp1] = "   { \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"region\": \"" + item.Region + "\",\"type\": \"masteryPoints\",\"name\": \"专精点\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "}},";
                tmp1++;
            }
            if (tmp1 == mapI.MasteryPoints.Length)
            {
                File.AppendAllLines(".\\data\\dboMapMasteryPoints.js", 所有4, Encoding.UTF8);
                tmp1 = 0;
            }

            //解析挑战
            //textBox2.AppendText("dboMapAdventures.js 挑战数据:" + 回车);
            //string[] 所有5 = new string[mapI.Adventures.Length];
            //foreach (var item in mapI.Adventures)
            //{
            //    textBox2.AppendText("{ \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"adventures\",\"name\": \"" + item.Name + "\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "}}," + 回车);
            //    所有5[tmp1] = "   { \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"adventures\",\"name\": \"" + item.Name + "\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "}},";
            //    tmp1++;
            //}
            //if (tmp1 == mapI.Adventures.Length)
            //{
            //    File.AppendAllLines(".\\data\\dboMapAdventures.js", 所有5, Encoding.UTF8);
            //    tmp1 = 0;
            //}

            //解析地区
            textBox2.AppendText("dboMapSectors.js 地区数据:" + 回车);
            string[] 所有6 = new string[mapI.Sectors.Values.Count];
            foreach (var item in mapI.Sectors.Values)
            {
                
                textBox2.AppendText("{ \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"sectors\",\"name\": \"" + item.Name + "\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "},\"chat_link\": \"" + item.ChatLink + "\",\"bounds\":"+ getbounds(item.Bounds) + "}," + 回车);
                所有6[tmp1] = "   { \"zoneid\": \"" + mapI.Id + "\",\"itemid\": \"" + item.Id + "\",\"type\": \"sectors\",\"name\": \"" + item.Name + "\",\"pos\": { \"x\": " + item.Coord[0] + ", \"y\": " + item.Coord[1] + "},\"chat_link\": \"" + item.ChatLink + "\",\"bounds\":" + getbounds(item.Bounds) + "},";
                tmp1++;
            }
            if (tmp1 == mapI.Sectors.Values.Count)
            {
                File.AppendAllLines(".\\data\\dboMapSectors.js", 所有6, Encoding.UTF8);
                tmp1 = 0;
            }

            //解析完成---
            textBox2.AppendText("解构完成 - - 可以开始下一个地图" + 回车);
            if (Properties.Settings.Default.自动获取信息)
            {
                if (地板集合.Length - 1 > 当前项目排序)
                {
                    当前项目排序++;
                    label20.Text = "当前地图ID:" + 地图ID集合[当前项目排序].ToString();
                    label21.Text = "当前地图排序:" + 当前项目排序.ToString() + "/" + (地板集合.Length - 1);
                }
                else
                {
                    timer2.Stop();
                    timer2.Enabled = false;
                    结束数据文件();
                    当前项目排序 = 0;
                }
                下载地图信息中 = false;
            }
            else
            {
                if (地板集合.Length - 1 > 当前项目排序)
                {

                }
                else
                {
                    结束数据文件();
                    当前项目排序 = 0;
                }
            }
        }

        public string getbounds(double[][] bounds)
        {
            string tmp = "[";
            for (int i = 0; i < bounds.Length; i++)
            {
                tmp += "["+ bounds[i][0]+","+ bounds[i][1] ;
                if (i != bounds.Length-1)
                {
                    tmp += "],";
                }
                else
                {
                    tmp += "]";
                }
            }
            tmp += "]";
            return tmp;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.自动获取信息 && !下载地图信息中&& 当前项目排序 < 地图ID集合.Length)
            {
                下载地图信息中 = true;
                开始获取地图数据();
            }
        }
    }
}
