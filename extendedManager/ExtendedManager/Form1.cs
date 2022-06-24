using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.Runtime.InteropServices;
using System.Data.SQLite;

namespace SimpleFileManager
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
           (
               int nLeftRect,     // x-coordinate of upper-left corner
               int nTopRect,      // y-coordinate of upper-left corner
               int nRightRect,    // x-coordinate of lower-right corner
               int nBottomRect,   // y-coordinate of lower-right corner
               int nWidthEllipse, // width of ellipse
               int nHeightEllipse // height of ellipse
            );
        string filePath = Application.StartupPath;
        bool isFile = false;
        string currentlySelectedItemName = "";
        DirectoryInfo[] dirs;
        bool bIsFile = false;
        bool bIsDirectory = false;
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        int tempsayac = 0;
        Point lastLoc;
        string tempfp = "";
        bool btnclick = true;
        DirectoryInfo fileList;
        bool formTasi = false;
        int sayac = 175;
        bool butonTasi = false; 
        private const bool V = true;
        string[] catnames;
        bool formTasiniyor = false;
        Point baslangicNoktasi = new Point(0, 0);
        Point butonNoktasi = new Point(0, 0);
        int ts = 0;
        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, 1153, 585, 20, 20));
            #region ButonaEventEkleme
            //Tüm butonlara eventlarını dinamik olarak elkme
            foreach (var item in this.Controls)//Form a eklenen tüm controller
            {
                if (item is Button) // gelen kontrol buton ise
                {
                    var Btn = (Button)item;
                    Btn.MouseDown += Btn_MouseDown; //event ekle
                    Btn.MouseMove += Btn_MouseMove; //event ekle
                    Btn.MouseUp += Btn_MouseUp;//event ekle
                    buttonPoints.Add(Btn.Name, Btn.Location);//buton lokasyonunu ekle

                }
                ButtonIlkPoints = buttonPoints;//buton ilk lokasyonlarını sakla
            }
            #endregion
            InitializeComponent();
            for (int i = 0; i < allDrives.Length; i++)
            {
                listBox1.Items.Add(allDrives[i].ToString().Replace(@"\", "/"));
            }
        }
        #region ButonSurukleBirakIcinEventlar

        Dictionary<string, Point> ButtonIlkPoints = new Dictionary<string, Point>();//buton ilk lokasyonlarını saklamak için
        Dictionary<string, Point> buttonPoints = new Dictionary<string, Point>();//buton anlık lokasyonlarını saklamak için
        string btnText = "";
        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            if (butonTasi)
            {
                var Btn = (Button)sender;
                Btn.Text = btnText;
                btnclick = true;
            }
        }

        private void Btn_MouseMove(object sender, MouseEventArgs e)
        {
            if (butonTasi)
            {
                if (e.Button == MouseButtons.Left)
                {
                    var Btn = (Button)sender;
                    Btn.Left += e.X - buttonPoints[Btn.Text].X;
                    Btn.Top += e.Y - buttonPoints[Btn.Text].Y;
                }
            }
        }
        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            if (butonTasi)
            {
                var Btn = (Button)sender;
                btnText = Btn.Text;
                Btn.Text = "Taşı";
                buttonPoints[Btn.Text] = e.Location;
                btnclick = false;
            }
        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            string tempfilepath = filePath.Replace(@"\", "/");
            filePathTextBox.Text = tempfilepath;
            loadFilesAndDirectories();
            label5.Visible = false;
            play.Visible = false;
            axWindowsMediaPlayer1.Visible = false;
            axWindowsMediaPlayer1.uiMode = "None";
            CreateSqlDB();
            importDataB();

            List<Label> lbls = this.Controls.OfType<Label>().ToList();

            foreach (var lbl in lbls)
            {
                lbl.Click += lbl_Click;
            }


        }
        void lbl_Click(object sender, EventArgs e)
        {
            string asiriTempString = "";
            Label lbl = sender as Label;

            SQLiteConnection con;
            SQLiteCommand cmd;
            SQLiteDataReader dr;

            con = new SQLiteConnection($@"Data Source={allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite");
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM files";
            dr = cmd.ExecuteReader();
            
            while (dr.Read())
            {
                asiriTempString = asiriTempString + dr["catName"].ToString() + "/";
            }

            con.Close();

            for (int i = 0; i < asiriTempString.Split('/').Length; i++)
            {
                catnames = asiriTempString.Split('/');

            }

            KATEGORIYE GORE IMPORT

            //listView1.Clear();
            //listView1.Items.Add("C:/DataBase/asd.txt", "asd", 3);
            //listView1.Items.Add("C:/Users/Administrator/source/s.mp4", "s", 6);
        }
        private void importDataB()
        {
            SQLiteConnection con;
            SQLiteCommand cmd;
            SQLiteDataReader dr;

            con = new SQLiteConnection($@"Data Source={allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite");
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM locations";
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string tempstring = dr["location"].ToString().Replace("{", "");
                string tempstringname = dr["name"].ToString();
                tempstring = tempstring.Replace("}", "");
                tempstring = tempstring.Replace("X", "");
                tempstring = tempstring.Replace("Y", "");
                tempstring = tempstring.Replace("=", "");
                string[] tempsplit = tempstring.Split(',');
                int tempintx = Convert.ToInt32(tempsplit[0]);
                int tempinty = Convert.ToInt32(tempsplit[1]);
                Point localization = new Point(tempintx, tempinty);
                this.Controls.Add(new Label { Text = tempstringname, Location = new Point(12, 155 + tempinty), Name = tempstringname });
                tempsayac = Convert.ToInt32(tempsplit[1]);

            }
            sayac = tempsayac + 25;
            con.Close();
        }
        private void CreateSecTable()
        {
            string sql = @"CREATE TABLE files(
                               ID INTEGER PRIMARY KEY AUTOINCREMENT ,
                               favouriteName        TEXT       NOT NULL,
                               favouriteLink        TEXT       NOT NULL,
                               catName        TEXT       NOT NULL
                            );";
            SQLiteConnection con = new SQLiteConnection($@"Data Source={allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite");
            SQLiteCommand cmd = new SQLiteCommand();
            con.Open();
            cmd = new SQLiteCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        private void CreateSqlDB()
        {
            string tempStr = allDrives[0].Name.Replace(@"\", "/");
            if (!Directory.Exists(tempStr + "../DataBase"))
            {
                Directory.CreateDirectory(tempStr + "/DataBase");
                tempStr = tempStr + "../DataBase/";
                SQLiteConnection.CreateFile(tempStr + "DB.sqlite");
                    string sql = @"CREATE TABLE locations(
                               ID INTEGER PRIMARY KEY AUTOINCREMENT ,
                               location           TEXT      NOT NULL,
                               name            TEXT       NOT NULL,
                               lastsayac           TEXT      NOT NULL
                            );";
                    SQLiteConnection con = new SQLiteConnection($@"Data Source={allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite");
                    SQLiteCommand cmd = new SQLiteCommand();
                    con.Open();
                    cmd = new SQLiteCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    con.Close();

                CreateSecTable();
            }
            else
            {
                if (!File.Exists($@"{allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite"))
                {
                    SQLiteConnection.CreateFile(tempStr + "DB.sqlite");
                    string sql = @"CREATE TABLE locations(
                               ID INTEGER PRIMARY KEY AUTOINCREMENT ,
                               location           TEXT      NOT NULL,
                               name            TEXT       NOT NULL,
                               lastsayac           TEXT      NOT NULL
                            );";
                    SQLiteConnection con = new SQLiteConnection($@"Data Source={allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite");
                    SQLiteCommand cmd = new SQLiteCommand();
                    con.Open();
                    cmd = new SQLiteCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    CreateSecTable();
                }
            }
        }
        private void ForwardBack()
        {
            string[] fwb = filePathTextBox.Text.Split('/');
            int tempnumber = fwb.Length;
            string newpath = fwb[0];
            tempnumber--;
            for (int i = 1; i < tempnumber; i++)
            {
                newpath = newpath + "/" + fwb[i];
            }
            tempfp = filePathTextBox.Text;
            if (newpath.Split('/').Length == 1)
            {
                filePathTextBox.Text = newpath + "/";
            }
            else { filePathTextBox.Text = newpath; }
            loadButtonAction();
            loadFilesAndDirectories();
        }

        public void loadFilesAndDirectories()
        {
            string tempFilePath = "";
            FileAttributes fileAttr;
            try
            {

                if (isFile)
                {
                    tempFilePath = filePath + "/" + currentlySelectedItemName;
                    FileInfo fileDetails = new FileInfo(tempFilePath);
                    fileNameLabel.Text = fileDetails.Name;
                    fileTypeLabel.Text = fileDetails.Extension;
                    fileAttr = File.GetAttributes(tempFilePath);
                    Process.Start(tempFilePath);
                }
                else
                {
                    fileAttr = File.GetAttributes(filePath);
                    
                }

                if((fileAttr & FileAttributes.Directory ) == FileAttributes.Directory)
                {
                    fileList = new DirectoryInfo(filePath);
                    FileInfo[] files = fileList.GetFiles();
                    dirs = fileList.GetDirectories();
                    string fileExtension = "";
                    listView1.Items.Clear();

                    for (int i = 0; i < files.Length; i++)
                    {
                        fileExtension = files[i].Extension.ToUpper();
                        switch (fileExtension)
                        {
                            case ".MP3":
                            case ".MP2":
                                listView1.Items.Add(files[i].Name, 5);
                                break;
                            case ".EXE":
                            case ".COM":
                                listView1.Items.Add(files[i].Name, 7);
                                break;

                            case ".MP4":
                            case ".AVI":
                            case ".MKV":
                                listView1.Items.Add(files[i].Name, 6);
                                break;
                            case ".PDF":
                                listView1.Items.Add(files[i].Name, 4);
                                break;
                            case ".DOC":
                            case ".DOCX":
                                listView1.Items.Add(files[i].Name, 3);
                                break;
                            case ".PNG":
                            case ".JPG":
                            case ".JPEG":
                                listView1.Items.Add(files[i].Name, 9);
                                break;

                            default:
                                listView1.Items.Add(files[i].Name, 8);
                                break;
                        }

                    }

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        listView1.Items.Add(dirs[i].Name, 10);
                    }
                }
                else
                {
                    fileNameLabel.Text = this.currentlySelectedItemName;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "Bu klasöre erişim engellendi!");
                goButton.PerformClick();
            }
        }

        public void loadButtonAction()
        {
            removeBackSlash();
            filePath = filePathTextBox.Text;
            loadFilesAndDirectories();
            isFile = false;
        }

        public void removeBackSlash()
        {
            string path = filePathTextBox.Text;
            if(path.LastIndexOf("/") == path.Length - 1)
            {
                if (path.Substring(0, path.Length - 1).Length == 2)
                {
                    filePathTextBox.Text = path.Substring(0, path.Length - 1) + "/";
                }
                else
                {
                    filePathTextBox.Text = path.Substring(0, path.Length - 1);
                }
            }
        }

        public void goBack()
        {
            try
            {
                removeBackSlash();
                string path = filePathTextBox.Text;
                path = path.Substring(0, path.LastIndexOf("/"));
                this.isFile = false;
                filePathTextBox.Text = path;
                removeBackSlash();
            }
            catch(Exception)
            {

            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            filePathTextBox.Text = tempfp;
            loadButtonAction();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            currentlySelectedItemName = e.Item.Text;


            FileAttributes fileAttr = File.GetAttributes(filePath + "/" + currentlySelectedItemName);
            if((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                isFile = false;
                if (filePath.Length < 4 && filePath.Contains("/"))
                {
                    filePathTextBox.Text = filePath + currentlySelectedItemName;
                    filePathTextBox.Text = filePathTextBox.Text.Replace(@"\", "/");
                }
                else { filePathTextBox.Text = filePath + "/" + currentlySelectedItemName; filePathTextBox.Text = filePathTextBox.Text.Replace(@"\", "/"); }
            }
            else
            {
                isFile = true;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            loadButtonAction();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            ForwardBack();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tempfp = filePathTextBox.Text;
            filePathTextBox.Text = listBox1.SelectedItem.ToString();
            loadButtonAction();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlenabled = true;
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            label5.Visible = false;
            play.Visible = true;
        }

        private void play_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlenabled = true;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            label5.Visible = true;
            play.Visible = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tempFilePath = "";
            tempFilePath = filePath + "/" + currentlySelectedItemName;
            FileInfo fileDetails = new FileInfo(tempFilePath);
            fileNameLabel.Text = fileDetails.Name;
            fileTypeLabel.Text = fileDetails.Extension;
            label4.Text = fileDetails.LastWriteTime.ToString();

            if (fileDetails.Extension == ".mp3" || fileDetails.Extension == ".mp4" || fileDetails.Extension == ".wav" || fileDetails.Extension == ".avi" || fileDetails.Extension == ".mov" || fileDetails.Extension == ".gif")
            {
                axWindowsMediaPlayer1.Visible = true;
                try
                {
                    axWindowsMediaPlayer1.URL = tempFilePath;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    label5.Visible = true;
                }
                catch (Exception) { }
            }
            else { axWindowsMediaPlayer1.Visible = false; axWindowsMediaPlayer1.Ctlcontrols.stop(); axWindowsMediaPlayer1.URL = ""; label5.Visible = false; play.Visible = false; }
            if (fileDetails.Extension == ".png" || fileDetails.Extension == ".jpg" || fileDetails.Extension == ".jpeg" || fileDetails.Extension == ".tiff")
            {
                pictureBox1.Visible = true;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.ImageLocation = tempFilePath;
            }
            else { pictureBox1.Visible = false; pictureBox1.ImageLocation = ""; }
        }

        private void play_MouseDown(object sender, MouseEventArgs e)
        {
            play.ForeColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void play_MouseUp(object sender, MouseEventArgs e)
        {
            play.ForeColor = Color.FromArgb(0, 0, 0, 0);
        }

        private void label5_MouseDown(object sender, MouseEventArgs e)
        {
            label5.ForeColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void label5_MouseUp(object sender, MouseEventArgs e)
        {
            label5.ForeColor = Color.FromArgb(0, 0, 0, 0);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                formTasiniyor = true;
                baslangicNoktasi = new Point(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Right)
            {
            }
            else
            {
                
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                formTasiniyor = false;
            }
            if (e.Button == MouseButtons.Right)
            {

            }
            else
            {

            }
        }

        //private void Form1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (formTasiniyor)
        //    {
        //        Point p = PointToScreen(e.Location);
        //        Location = new Point(p.X - this.baslangicNoktasi.X, p.Y - this.baslangicNoktasi.Y);
        //    }
        //}

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(PointToScreen(e.Location));

            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(PointToScreen(e.Location));
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label6_MouseDown(object sender, MouseEventArgs e)
        {
            label6.ForeColor = Color.FromArgb(255, 255, 0, 0);
        }

        private void label6_MouseUp(object sender, MouseEventArgs e)
        {
            label6.ForeColor = Color.FromArgb(0, 0, 0, 0);
        }

        private void kategoriEkleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fr2 = new Form2();
            fr2.ShowDialog();
            
            if (fr2.Name == "")
            {
                MessageBox.Show("Boş bir kategori oluşturamazsınız!");
            }
            else
            {
                Point loclab;
                loclab = new Point(0, 0);
                loclab.X = Categories.Location.X;
                loclab.Y = sayac;
                this.Controls.Add(new Label { Text = fr2.Name, Location = new Point(12, 155 + sayac), Name = fr2.Name });
                string tempLocName = fr2.Name;
                SQLiteCommand cmd = new SQLiteCommand();
                SQLiteConnection con = new SQLiteConnection($@"Data Source={allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite");
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "insert into locations(location,name, lastsayac) values ('" + loclab.ToString() + "','" + fr2.Name.ToString() + "','" + loclab.Y +  "')";
                cmd.ExecuteNonQuery();
                con.Close();
                sayac += 25;
                (contextMenuStrip1.Items[0] as ToolStripMenuItem).DropDownItems.Add(fr2.Name.ToString());
                MessageBox.Show(loclab.Y.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void dosyayıVeyaKlasörüSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string templink = filePathTextBox.Text;
            MessageBox.Show(templink);

            FileAttributes attr = File.GetAttributes(templink);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                if (templink == currentlySelectedItemName)
                {
                    
                }
                else {
                    Directory.Delete(templink, true); backButton.PerformClick(); tempfp = "";
                }
            }
            else
            { MessageBox.Show("Its a file"); }
        }

        private void kategorilereEkleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void kategorilereEkleToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string templink = filePathTextBox.Text;
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                SQLiteCommand cmd = new SQLiteCommand();
                SQLiteConnection con = new SQLiteConnection($@"Data Source={allDrives[0].ToString().Replace(@"\", "/") + "/DataBase/"}DB.sqlite");
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "insert into files(favouriteName,favouriteLink, catName) values ('" + templink + "/" + listView1.SelectedItems[i].Text + "','" + listView1.SelectedItems[i].Text.Split('.')[0].ToString() + "','" + e.ClickedItem.Text + "')";
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}
