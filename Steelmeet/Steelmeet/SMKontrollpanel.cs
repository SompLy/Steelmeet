﻿
using System.Data;

using SteelMeet;
using DataTable = System.Data.DataTable;
using SpreadsheetLight;
using System.Globalization;
using DocumentFormat.OpenXml.EMMA;

namespace Powermeet2
{

    public partial class SMKontrollpanel : Form
    {
        public SMKontrollpanel()
        {
            InitializeComponent();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            tabControl1.TabPages[0].ForeColor = Color.FromArgb(187, 225, 250);
        }

        private void ChangeTabColor(object sender, DrawItemEventArgs e)
        {
            Font TabFont;
            Brush BackBrush = new SolidBrush(Color.Green); //Set background color
            Brush ForeBrush = new SolidBrush(Color.Yellow);//Set foreground color
            if (e.Index == this.tabControl1.SelectedIndex)
            {
                TabFont = new Font(e.Font, FontStyle.Regular);
            }
            else
            {
                TabFont = e.Font;
            }
            string TabName = this.tabControl1.TabPages[e.Index].Text;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.FillRectangle(BackBrush, e.Bounds);
            Rectangle r = e.Bounds;
            r = new Rectangle(r.X, r.Y + 3, r.Width, r.Height - 3);
            e.Graphics.DrawString(TabName, TabFont, ForeBrush, r, sf);
            //Dispose objects
            sf.Dispose();
            if (e.Index == this.tabControl1.SelectedIndex)
            {
                TabFont.Dispose();
                BackBrush.Dispose();
            }
            else
            {
                BackBrush.Dispose();
                ForeBrush.Dispose();
            }
        }

        System.Data.DataTable dt = new();
        System.Data.DataTable dt2 = new();

        bool a = true;
        bool b = true;
        public bool IsExcelFile;
        bool DebugModeExcelImport = true;
        bool IsRecord = false;
        bool restartLiftingOrderList = true;     //LiftingOderList

        public string BrowsedFilePath;
        public string BrowsedFile;
        public string recordType; //Klubb, Distrikt, Svenskt rekord, Europa rekord, World record!!!

        public int SelectedRowIndex;
        public int SelectedColumnIndex;
        int platesCount = 0;
        int secondsLapp;
        int minutesLapp;
        int secondsLyft;
        int minutesLyft;
        int groupIndexCurrent;
        int groupIndexCount = 1;    //Antal grupper
        int group1Count;            //Antal lyftare i grupp
        int group2Count;            //Antal lyftare i grupp
        int group3Count;            //Antal lyftare i grupp
        int groupRowFixer;          //Ändars beronde på grupp så att LifterID[SelectedRowIndex + groupRowFixer] blir rätt
        int firstLftdatagridviewColumn = 11;

        public Dictionary<int, Lifter> LifterID = new();

        public List<int> usedPlatesList = new List<int>();     //Hur många plates calculatorn har använt.
        List<int> totalPlatesList = new List<int>();    //Antalet paltes som användaren anvivit
        List<float> weightsList = new List<float>();    //Vikter

        List<System.Windows.Forms.Label> LiftingOrderListLabels = new List<System.Windows.Forms.Label>(); //Order med lyftare och vikt de ska ta i rätt ordning.
        List<float> LiftingOrderList = new List<float>(); //För att sortera viktera
        List<float> LiftingOrderList2 = new List<float>(); //För att sortera viktera

        List<Lifter> LiftingOrderListNew = new List<Lifter>(); //För att sortera
        List<Lifter> LiftingOrderList2New = new List<Lifter>(); //För att sortera

        List<System.Windows.Forms.Label> GroupLiftingOrderListLabels = new List<System.Windows.Forms.Label>(); //Order med lyftare och vikt de ska ta i rätt ordning.
        List<float> GroupLiftingOrderList = new List<float>(); //För att sortera viktera

        MouseEventArgs mouseEvent = new MouseEventArgs(Control.MouseButtons, 0, 0, 0, 0);

        //Default Plate setup 16x25kg
        public PlateInfo plateInfo = new PlateInfo(0, 16, 2, 2, 2, 2, 2, 2, 2, 2, Color.ForestGreen, Color.Red, Color.Blue, Color.Yellow, Color.LimeGreen, Color.WhiteSmoke, Color.Black, Color.Silver, Color.Gainsboro, Color.Gainsboro);

        public CultureInfo customCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

        public class Lifter
        {
            public Lifter(
                string groupNumber,
                string name,
                string lotNumber,
                string weightClass,
                string Kategory,
                string licenceNumber,
                string accossiation,
                string bodyWeight,
                string squatHeight,
                string tilted,
                string s1,
                string benchHeight,
                string benchRack,
                string liftoff,
                string b1,
                string d1)
            {
                this.groupNumber = Int16.Parse(groupNumber);
                this.name = name;
                this.lotNumber = float.Parse(lotNumber);
                this.weightClass = weightClass;
                this.Kategory = Kategory;
                this.licenceNumber = licenceNumber;
                this.accossiation = accossiation;
                this.bodyWeight = float.Parse(bodyWeight);
                this.squatHeight = Int16.Parse(squatHeight);
                this.tilted = tilted;
                this.s1 = float.Parse(s1);
                this.benchHeight = Int16.Parse(benchHeight);
                this.benchRack = Int16.Parse(benchRack);
                this.liftoff = liftoff;
                this.b1 = float.Parse(b1);
                this.d1 = float.Parse(d1);
                CurrentLift = 11;       //Väljer vilken column som första böjen börjar på
                                        //Du måsta ändra en sak i tabcontrol långt ner
                string emptystring = " ";
                LiftRecord = new List<bool>();
                sbdList = new List<float>() { this.s1, s2, s3, this.b1, b2, b3, this.d1, d2, d3 };
            }

            public int place { get; set; }
            public int groupNumber { get; set; }
            public string name { get; set; }
            public float lotNumber { get; set; }
            public string weightClass { get; set; }
            public string Kategory { get; set; }
            public enum eCategory
            {
                MenEquipped,
                MenClassic,
                MenEquippedBench,
                MenClassicBench,
                WomenEquipped,
                WomenClassic,
                WomenEquippedBench,
                WomenClassicBench

            }
            public eCategory CategoryEnum { get; set; }
            public string licenceNumber { get; set; }
            public string accossiation { get; set; }

            public float bodyWeight { get; set; }
            public int squatHeight { get; set; }
            public string tilted { get; set; }
            public float s1 { get; set; }
            public float s2 { get; set; }
            public float s3 { get; set; }
            public int benchHeight { get; set; }
            public int benchRack { get; set; }
            public string liftoff { get; set; }
            public float b1 { get; set; }
            public float b2 { get; set; }
            public float b3 { get; set; }
            public float d1 { get; set; }
            public float d2 { get; set; }
            public float d3 { get; set; }
            public float total { get; set; }
            public int pointsWilks { get; set; }
            public int pointsGL { get; set; }

            public int CurrentLift { get; set; }
            public float bestS { get; set; }
            public float bestB { get; set; }
            public float bestD { get; set; }
            public List<bool> LiftRecord { get; set; } //en lista med true eller false beroende på om lyftaren fick godkänt eller inte
            public List<float> sbdList { get; set; }

        }
        public class LifterComparer : IComparer<Lifter>
        {
            public int Compare(Lifter x, Lifter y)
            {
                int indexX = x.CurrentLift - 11;
                int indexY = y.CurrentLift - 11;

                if (indexX >= 0 && indexX < x.sbdList.Count && indexY >= 0 && indexY < y.sbdList.Count)
                {
                    float valueX = x.sbdList[indexX];
                    float valueY = y.sbdList[indexY];

                    // Compare the values and return the result.
                    return valueX.CompareTo(valueY);
                }

                // Handle cases where indices are out of bounds.
                return 0; // Or any other appropriate value.
            }
        }
        public class LifterComparerTotal : IComparer<Lifter>
        {
            public int Compare(Lifter x, Lifter y)
            {
                // Compare lifters based on their total
                return x.total.CompareTo(y.total);
            }
        }
        public class PlateInfo
        {
            public PlateInfo(int plate50, int plate25, int plate20, int plate15, int plate10, int plate5, int plate25small, int plate05, int plate125, int plate025
            , Color col_plate50, Color col_plate25, Color col_plate20, Color col_plate15, Color col_plate10, Color col_plate5, Color col_plate25small, Color col_plate05, Color col_plate125, Color col_plate025)
            {
                this.plate50 = plate50 / 2;
                this.plate25 = plate25 / 2;
                this.plate20 = plate20 / 2;
                this.plate15 = plate15 / 2;
                this.plate10 = plate10 / 2;
                this.plate5 = plate5 / 2;
                this.plate25small = plate25small / 2;
                this.plate125 = plate125 / 2;
                this.plate05 = plate05 / 2;
                this.plate025 = plate025 / 2;

                this.col_plate50 = col_plate50;
                this.col_plate25 = col_plate25;
                this.col_plate20 = col_plate20;
                this.col_plate15 = col_plate15;
                this.col_plate10 = col_plate10;
                this.col_plate5 = col_plate5;
                this.col_plate25small = col_plate25small;
                this.col_plate05 = col_plate05;
                this.col_plate125 = col_plate125;
                this.col_plate025 = col_plate025;
            }
            public int plate50 { get; set; }
            public int plate25 { get; set; }
            public int plate20 { get; set; }
            public int plate15 { get; set; }
            public int plate10 { get; set; }
            public int plate5 { get; set; }
            public int plate25small { get; set; }
            public int plate05 { get; set; }
            public int plate125 { get; set; }
            public int plate025 { get; set; }
            //Colors
            public Color col_plate50 { get; set; }
            public Color col_plate25 { get; set; }
            public Color col_plate20 { get; set; }
            public Color col_plate15 { get; set; }
            public Color col_plate10 { get; set; }
            public Color col_plate5 { get; set; }
            public Color col_plate25small { get; set; }
            public Color col_plate05 { get; set; }
            public Color col_plate125 { get; set; }
            public Color col_plate025 { get; set; }

        }


        //Invägning
        //Invägning
        //Invägning
        //Invägning
        //Invägning

        private void dataGridViewWeighIn_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (Enumerable.Range(0, dataGridViewWeighIn.RowCount).Contains(e.RowIndex))
            {
                dataGridViewWeighIn.Rows[e.RowIndex].Selected = true;
            }
        }
        private void dataGridViewWeighIn_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            WeighInInfoUpdate();
        }
        private void btn_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "C:\\Users\\ninja\\source\\repos\\Powermeet2\\Powermeet2\\";
            openFileDialog1.Title = "Powermeet Impoertera fil :)";
            openFileDialog1.Filter =
                "Excel and txt files|*.txt; *.xlsx; *.xls|" +                                                                                   //Filformat som man kan välja
                "All files (*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();                                                                                  //Öppnar dialog
            if (result == DialogResult.OK)                                                                                                       //Testar om man klckat på ok i dialog
            {
                if (".txt" == Path.GetExtension(openFileDialog1.FileName))                                                                      //Om man väljer text fil
                {
                    BrowsedFile = openFileDialog1.FileName;
                    IsExcelFile = false;
                    try
                    {
                        FileInfo finfo = new FileInfo(BrowsedFile);
                        BrowsedFilePath = finfo.DirectoryName + "\\" + finfo.Name;
                        lbl_ImportedfilePath.Text = "dir: " + BrowsedFilePath;                                                             //Ändrar grafisk text

                        dt.Rows.Clear();
                        string text = File.ReadAllText(BrowsedFile);
                        TxtImportHandler(text);
                    }
                    catch (IOException)
                    {
                    }

                }
                else if (".xls" == Path.GetExtension(openFileDialog1.FileName) || ".xlsx" == Path.GetExtension(openFileDialog1.FileName))       //Om man väljer en excel fil
                {
                    BrowsedFile = openFileDialog1.FileName;
                    IsExcelFile = true;
                    try
                    {
                        System.IO.FileInfo finfo = new System.IO.FileInfo(BrowsedFile);
                        BrowsedFilePath = finfo.DirectoryName + "\\" + finfo.Name;
                        lbl_ImportedfilePath.Text = "dir: " + BrowsedFilePath;

                        dt.Rows.Clear();
                        ExcelImportHandler();

                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        private void btn_Refresh_Click(object sender, EventArgs e)                                                                              //En uppdateringsknapp
        {
            dt.Rows.Clear();
            if (IsExcelFile)
            {
                ExcelImportHandler();
            }
            else
            {
                string text = File.ReadAllText(BrowsedFilePath);
                TxtImportHandler(text);
            }
        }

        public void TxtImportHandler(string text)                                                                                               //Hanterar text impoteringen av text
        {
            List<string> LyftarID = new List<string>();

            string s = text;

            foreach (string line in s.Split(':'))
            {
                if (!line.Contains("//"))
                {
                    LyftarID.Add(line);
                }
            }

            for (int i = 0; i < LyftarID.Count; i++)
            {
                string[] data = LyftarID[i].Split(',');
                //Display(data[0], data[1], data[2], data[3], data[4]);
            }

        }

        public void ExcelImportHandler()                                                                                               //Hanterar text impoteringen av excel
        {
            //Excel.Workbook excelbook = excelApp.Workbooks.BrowsedFile);
            //Excel._Worksheet excelsheet = excelbook.Sheets[1];
            //Excel.Range excelRange = excelsheet.UsedRange;
            using SLDocument sl = new SLDocument(BrowsedFile);
            SLWorksheetStatistics stats = sl.GetWorksheetStatistics();

            int rowCount = stats.NumberOfRows;
            int realRowCount = 0;
            int columnCount = stats.NumberOfColumns;

            List<string> data = new List<string>();
            for (int i = 1; i < 1000; i++) //Hittar antal rader som är ifyllda
            {
                if (string.IsNullOrWhiteSpace(sl.GetCellValueAsString(i, 1)))
                {
                    realRowCount = i;
                    i = 2000;
                }
            }

            if (!DebugModeExcelImport) //När man inte är i testarläge
            {
                for (int i = 0; i < realRowCount - 1; i++)
                {
                    //if (excelRange.Cells[i + 1, 1].Value2?.ToString() != "Grupp")
                    if (sl.GetCellValueAsString(i, 1) != "Grupp")
                    {
                        //Display(
                        //    sl.GetCellValueAsString(i, 1),
                        //    sl.GetCellValueAsString(i, 2),
                        //    sl.GetCellValueAsString(i, 3),
                        //    sl.GetCellValueAsString(i, 4),
                        //    sl.GetCellValueAsString(i, 5));
                        //excelRange.Cells[i + 1, 1].Value2.ToString(),
                        //excelRange.Cells[i + 1, 2].Value2.ToString(),
                        //excelRange.Cells[i + 1, 3].Value2.ToString(),
                        //excelRange.Cells[i + 1, 4].Value2.ToString(),
                        //excelRange.Cells[i + 1, 5].Value2.ToString());
                    }
                }
            }
            else
            {

                for (int i = 1; i < realRowCount; i++)
                {
                    if (sl.GetCellValueAsString(i, 1) != "Grupp")
                    {
                        DisplayDebug(
                            sl.GetCellValueAsString(i, 1),
                            sl.GetCellValueAsString(i, 2),
                            sl.GetCellValueAsString(i, 3),
                            sl.GetCellValueAsString(i, 4),
                            sl.GetCellValueAsString(i, 5),
                            sl.GetCellValueAsString(i, 6),
                            sl.GetCellValueAsString(i, 7),
                            sl.GetCellValueAsString(i, 8),
                            sl.GetCellValueAsString(i, 9),
                            sl.GetCellValueAsString(i, 10),
                            sl.GetCellValueAsString(i, 11),
                            sl.GetCellValueAsString(i, 12),
                            sl.GetCellValueAsString(i, 13),
                            sl.GetCellValueAsString(i, 14),
                            sl.GetCellValueAsString(i, 15),
                            sl.GetCellValueAsString(i, 16));
                    }
                }

                WeighInInfoUpdate();
            }
            for (int i = 0; i < dataGridViewWeighIn.ColumnCount; i++)
            {
                dataGridViewWeighIn.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //excelApp.Quit();

            //https://www.youtube.com/watch?v=kF2PGCl-rXU&ab_channel=AzharTechnoCoder

        }
        //public void Display(string Namn, string Viktklass, string Kategori, string Licensnummer, string Förening)
        //{
        //    if (a)
        //    {
        //        dt.Columns.Add("Namn");                 //
        //        dt.Columns.Add("Viktklass");            //1
        //        dt.Columns.Add("Kategori");             //2
        //        dt.Columns.Add("Licensnummer");         //3
        //        dt.Columns.Add("Förening");             //4
        //        dt.Columns.Add("Kroppsvikt");           //5
        //        dt.Columns.Add("Höjd Bänk");            //6
        //        dt.Columns.Add("Rack Bänk");            //6
        //        dt.Columns.Add("Ingång Bänk");          //7
        //        dt.Columns.Add("Höjd Böj");             //8
        //        dt.Columns.Add("Ingång Böj");           //9
        //        dt.Columns.Add("Ingång Mark");          //10    hemilga koden blir 0 4 1 2 5 6 8 9 7 10


        //        a = false;
        //    }
        //    DataRow dr = dt.NewRow();

        //    dr[0] = Namn;
        //    dr[1] = Kategori;
        //    dr[2] = Viktklass;
        //    dr[3] = Licensnummer;
        //    dr[4] = Förening;

        //    dt.Rows.Add(dr);
        //    dataGridViewWeighIn.DataSource = dt;

        //}
        public void DisplayDebug(
            string Gruppnummer,
            string Namn, //Testerläge då hela filen är ifylld med random värden så man inte behöver skriva in
            string Lotnummer,
            string Viktklass,
            string Kategori,
            string Licensnummer,
            string Förening,
            string Kroppsvikt,
            string HöjdBöj,
            string Infällt,
            string IngångBöj,
            string HöjdBänk,
            string RackBänk,
            string Avlyft,
            string IngångBänk,
            string IngångMark)
        {
            if (a)
            {
                dt.Columns.Add("Grupp");                 //0
                dt.Columns.Add("Namn");                  //1
                dt.Columns.Add("Lot");                   //2
                dt.Columns.Add("Klass");                 //3
                dt.Columns.Add("Kategori");              //4
                dt.Columns.Add("Licensnr.");             //5
                dt.Columns.Add("Förening");              //6
                dt.Columns.Add("Kropps\nvikt");          //7
                dt.Columns.Add("Höjd\nBöj");             //8
                dt.Columns.Add("Infällt");               //9
                dt.Columns.Add("Böj");                   //10
                dt.Columns.Add("Höjd\nBänk");            //11
                dt.Columns.Add("Rack\nBänk");            //12
                dt.Columns.Add("Avlyft");                //13
                dt.Columns.Add("Bänk");                  //14
                dt.Columns.Add("Mark");                  //15

                a = false;
            }
            DataRow dr = dt.NewRow();

            dr[0] = Gruppnummer;
            dr[1] = Namn;
            dr[2] = Lotnummer;
            dr[3] = Viktklass;
            dr[4] = Kategori;
            dr[5] = Licensnummer;
            dr[6] = Förening;
            dr[7] = Kroppsvikt;
            dr[8] = HöjdBöj;
            dr[9] = Infällt;
            dr[10] = IngångBöj;
            dr[11] = HöjdBänk;
            dr[12] = RackBänk;
            dr[13] = Avlyft;
            dr[14] = IngångBänk;
            dr[15] = IngångMark;

            dt.Rows.Add(dr);
            dataGridViewWeighIn.DataSource = dt;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                ofd.Title = "Steelmeet Impoertera fil :)";
                ofd.Filter = "Excel file |*.xlsx";
                ofd.FileName = "Steelmeet_lyftare_\"Start_XX:XX\"";
                DialogResult result = ofd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    DataTable dt = MoreExcel.DataGridView_To_Datatable(dataGridViewWeighIn);
                    dt.exportToExcel(ofd.FileName);

                    MessageBox.Show("Excel fil sparad! :)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        public void ExcelExport()
        {

        }
        private void button2_Click(object sender, EventArgs e) //Skicka till tävlings knappen lol
        {
            List<string> list = new List<string>();
            LifterID.Clear();
            dt2.Rows.Clear();

            for (int o = 0; o < dataGridViewWeighIn.RowCount - 1; o++)
            {
                for (int i = 0; i < dataGridViewWeighIn.ColumnCount; i++) //antal columner som inte är lyft
                {
                    list.Add(dataGridViewWeighIn[i, o].Value.ToString());
                }

                if (list[4].ToLower().Contains("herr"))                      //kollar om viktklassen är giltig för dam och herr
                {

                    if (list[3].ToLower().Contains("120+") || list[3].ToLower().Contains("+120"))
                    {
                        list[3] = "+120";
                    }
                    else if (list[3].ToLower().Contains("120"))
                    {
                        list[3] = "-120";
                    }
                    else if (list[3].ToLower().Contains("105"))
                    {
                        list[3] = "-105";
                    }
                    else if (list[3].ToLower().Contains("93"))
                    {
                        list[3] = "-93";
                    }
                    else if (list[3].ToLower().Contains("83"))
                    {
                        list[3] = "-83";
                    }
                    else if (list[3].ToLower().Contains("74"))
                    {
                        list[3] = "-74";
                    }
                    else if (list[3].ToLower().Contains("66"))
                    {
                        list[3] = "-66";
                    }
                    else if (list[3].ToLower().Contains("59"))
                    {
                        list[3] = "-59";
                    }
                    else if (list[3].ToLower().Contains("53"))
                    {
                        list[3] = "-53";
                    }
                    else if (list[3].ToLower().Contains("koeffhk"))          //Herr Klassiskt
                    {
                        list[3] = "koeffHK";
                    }
                    else if (list[3].ToLower().Contains("koeffhu"))          //Herr Utrustat
                    {
                        list[3] = "koeffHU";
                    }
                    else
                    {
                        MessageBox.Show("Ogiltig viktklass", "⚠Varning!⚠"); //Varning 
                        list[3] = "Ange klass!!";
                    }
                }
                else if (list[4].ToLower().Contains("dam")) // dam viktklass
                {
                    if (list[3].ToLower().Contains("84+") || list[3].ToLower().Contains("+84"))
                    {
                        list[3] = "+84";
                    }
                    else if (list[3].ToLower().Contains("84"))
                    {
                        list[3] = "-84";
                    }
                    else if (list[3].ToLower().Contains("76"))
                    {
                        list[3] = "-76";
                    }
                    else if (list[3].ToLower().Contains("69"))
                    {
                        list[3] = "-69";
                    }
                    else if (list[3].ToLower().Contains("63"))
                    {
                        list[3] = "-63";
                    }
                    else if (list[3].ToLower().Contains("57"))
                    {
                        list[3] = "-57";
                    }
                    else if (list[3].ToLower().Contains("52"))
                    {
                        list[3] = "-52";
                    }
                    else if (list[3].ToLower().Contains("47"))
                    {
                        list[3] = "-47";
                    }
                    else if (list[3].ToLower().Contains("43"))
                    {
                        list[3] = "-43";
                    }
                    else if (list[3].ToLower().Contains("koeffdk"))      //Dam Klassiskt
                    {
                        list[3] = "koeffDK";
                    }
                    else if (list[3].ToLower().Contains("koeffdu"))      //Dam Utrustat
                    {
                        list[3] = "koeffDU";
                    }
                    else
                    {
                        MessageBox.Show("Ogiltig viktklass, \"⚠Varning!⚠\""); //Varning 
                        list[3] = "Ange klass!!";
                    }
                }
                else
                {
                    MessageBox.Show("Ogiltig viktklass, \"⚠Varning!⚠\""); //Varning 
                    list[3] = "Ange klass!!";
                }


                dataGridViewWeighIn.Rows[o].Cells[3].Value = list[3];

                //Lägger till lyftare adderar lyftare ny lyftare
                LifterID.Add(o, new Lifter(list[0], list[1], list[2], list[3], list[4], list[5], list[6], list[7], list[8], list[9], list[10], list[11], list[12], list[13], list[14], list[15]));

                SetCategoryEnum(list[4]);

                list.Clear();
            }

            for (int i = 0; i < dataGridViewControlPanel.ColumnCount; i++) //sorterar
            {
                dataGridViewControlPanel.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        void WeighInInfoUpdate()
        {
            string gindex = dataGridViewWeighIn.Rows[dataGridViewWeighIn.RowCount - 2].Cells[0].Value.ToString();                          //Tar den sista lyftarens grupp
            dataGridViewWeighIn.Rows[0].Selected = false;
            lbl_WeightInData.Text = "Antal Lyftare : " + (dataGridViewWeighIn.RowCount - 1).ToString() + "\nAntal Grupper : " + gindex; //Uppdaterar data för invägning
        }

        void SetCategoryEnum(string Category)
        {
            string[] wholeThing;

            string sex;
            string yearclass;
            bool Equipped;
            bool BenchOnly;

            wholeThing = Category.Split(' ');
            sex = wholeThing[0].ToLower();
            yearclass = wholeThing[1].ToLower();
            if (wholeThing[2].ToLower() == "utrustat")
            {
                Equipped = true;
            }
            else
            {
                Equipped = false;
            }
            if (wholeThing[3].ToLower() == "bänk")
            {
                BenchOnly = true;
            }
            else
            {
                BenchOnly = false;
            }

            if (sex == "herr")
            {
                if (BenchOnly)
                {
                    if (Equipped == true)
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.MenEquippedBench;
                    }
                    else
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.MenClassicBench;
                    }
                }
                else
                {
                    if (Equipped == true)
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.MenEquipped;
                    }
                    else
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.MenClassic;
                    }
                }
            }
            else
            {
                if (BenchOnly)
                {
                    if (Equipped == true)
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.WomenEquippedBench;
                    }
                    else
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.WomenClassicBench;
                    }
                }
                else
                {
                    if (Equipped == true)
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.WomenEquipped;
                    }
                    else
                    {
                        LifterID[LifterID.Count - 1].CategoryEnum = Lifter.eCategory.WomenClassic;
                    }
                }
            }
        }

        //Invägning
        //Invägning
        //Invägning
        //Invägning
        //Invägning






        //Intällningar
        //Intällningar
        //Intällningar
        //Intällningar
        //Intällningar

        private void btn_Weightplates_Click(object sender, EventArgs e)
        {
            //Updaterar PlateInfo med antal viktskivor och färger
            plateInfo = new PlateInfo(Int16.Parse(txtb50.Text), Int16.Parse(txtb25.Text), Int16.Parse(txtb20.Text), Int16.Parse(txtb15.Text), Int16.Parse(txtb10.Text), Int16.Parse(txtb5.Text), Int16.Parse(txtb25small.Text), Int16.Parse(txtb125small.Text), Int16.Parse(txtb05small.Text), Int16.Parse(txtb025small.Text)
                                        , btn50.BackColor, btn25.BackColor, btn20.BackColor, btn15.BackColor, btn10.BackColor, btn5.BackColor, btn25small.BackColor, btn05small.BackColor, btn125small.BackColor, btn025small.BackColor);
        }
        public void ColorPicker(System.Windows.Forms.Button button)
        {
            ColorDialog colorpicker = new ColorDialog();

            if (colorpicker.ShowDialog() == DialogResult.OK)
            {
                button.BackColor = colorpicker.Color;
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            ColorPicker(btn50);
        }

        private void btn25_Click(object sender, EventArgs e)
        {
            ColorPicker(btn25);
        }

        private void btn20_Click(object sender, EventArgs e)
        {
            ColorPicker(btn20);
        }

        private void btn15_Click(object sender, EventArgs e)
        {
            ColorPicker(btn15);
        }

        private void btn10_Click(object sender, EventArgs e)
        {
            ColorPicker(btn10);
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            ColorPicker(btn5);
        }

        private void btn25small_Click(object sender, EventArgs e)
        {
            ColorPicker(btn25small);
        }

        private void btn125small_Click(object sender, EventArgs e)
        {
            ColorPicker(btn125small);
        }

        private void btn05small_Click(object sender, EventArgs e)
        {
            ColorPicker(btn05small);
        }

        private void btn025small_Click(object sender, EventArgs e)
        {
            ColorPicker(btn025small);
        }
        //Intällningar
        //Intällningar
        //Intällningar
        //Intällningar
        //Intällningar



        //Tävling
        //Tävling
        //Tävling
        //Tävling
        //Tävling
        public void infopanel_Controlpanel_Paint(object sender, PaintEventArgs e)
        {

            int x1 = 10, y1 = 40, x2 = 10, y2 = 100;
            Pen p = new Pen(Color.Red, 10);
            int offset = 12;
            Graphics g = e.Graphics;

            List<Color> plateColorList = new List<Color>();
            plateColorList.AddRange(new Color[] {plateInfo.col_plate50, plateInfo.col_plate25, plateInfo.col_plate20, plateInfo.col_plate15, plateInfo.col_plate10,
            plateInfo.col_plate5, plateInfo.col_plate25small, plateInfo.col_plate125, plateInfo.col_plate05, plateInfo.col_plate025});

            List<int> paintedPlatesList = new List<int>();
            paintedPlatesList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            for (int i = 0; i < 10;)
            {
                if (Enumerable.Any(usedPlatesList) && usedPlatesList[i] > paintedPlatesList[i])
                {
                    p.Color = plateColorList[i];
                    g.DrawLine(p, x1 + offset, y1, x2 + offset, y2);
                    offset += 12;

                    paintedPlatesList[i]++;
                }
                else { i++; }
            }
            p.Color = Color.DarkGray;
            g.DrawLine(p, x1 + offset, 60, x2 + offset, 80);
        }
        private void infopanel_Controlpanel_Paint2(object sender, PaintEventArgs e)
        {
            int x1 = 10, y1 = 40, x2 = 10, y2 = 100;
            Pen p = new Pen(Color.Red, 10);
            int offset = 12;
            Graphics g = e.Graphics;

            List<Color> plateColorList = new List<Color>();
            plateColorList.AddRange(new Color[] {plateInfo.col_plate50, plateInfo.col_plate25, plateInfo.col_plate20, plateInfo.col_plate15, plateInfo.col_plate10,
            plateInfo.col_plate5, plateInfo.col_plate25small, plateInfo.col_plate125, plateInfo.col_plate05, plateInfo.col_plate025});

            List<int> paintedPlatesList = new List<int>();
            paintedPlatesList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            for (int i = 0; i < 10;)
            {
                if (Enumerable.Any(usedPlatesList) && usedPlatesList[i] > paintedPlatesList[i])
                {
                    p.Color = plateColorList[i];
                    g.DrawLine(p, x1 + offset, y1, x2 + offset, y2);
                    offset += 12;

                    paintedPlatesList[i]++;
                }
                else { i++; }
            }
            p.Color = Color.DarkGray;
            g.DrawLine(p, x1 + offset, 60, x2 + offset, 80);
        }


        private void dataGridViewControlPanel_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridViewControlPanel_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //dataGridViewControlPanel.Rows[e.RowIndex].Cells[1].;
        }
        private void dataGridViewControlPanel_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (Enumerable.Range(0, dataGridViewControlPanel.RowCount).Contains(e.RowIndex))
            {
                //dataGridViewControlPanel.MultiSelect = false;
                if (dataGridViewControlPanel.Rows[e.RowIndex].Cells[1].Selected != true)
                {
                    //MessageBox.Show("Simulerar click på första columnen");
                    //dataGridViewControlPanel.Rows[e.RowIndex].Cells[1].Selected = true;
                    dataGridViewControlPanel_CellMouseClick(dataGridViewControlPanel, new DataGridViewCellMouseEventArgs(1, e.RowIndex, 0, 0, mouseEvent));

                }
                //dataGridViewControlPanel.ClearSelection();

                //for (int i = 0; i < 8; i++) //Antal selected cells i raden när man clickar med musen
                //{
                //    dataGridViewControlPanel.Rows[e.RowIndex].Cells[i].Selected = true;
                //}
                SelectedRowIndex = e.RowIndex;
                SelectedColumnIndex = e.ColumnIndex;
                if (Enumerable.Any(LifterID) && dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value != DBNull.Value) //Kollar om det finns något i LifterID listan annars blir det error
                {
                    //Visar Info om nuvarande lyftare i informationsrutan
                    //lbl_Name.Text = LifterID[SelectedRowIndex + groupRowFixer].name;
                    //PlateCalculator(float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString()), plateInfo);
                    //lbl_Placement.Text = LifterID[SelectedRowIndex + groupRowFixer].place.ToString();
                    //lbl_Infällt.Text = LifterID[SelectedRowIndex + groupRowFixer].tilted.ToString();
                    //lbl_Avlyft.Text = LifterID[SelectedRowIndex + groupRowFixer].liftoff.ToString();
                    //lbl_Grupp_control.Text = LifterID[SelectedRowIndex + groupRowFixer].groupNumber.ToString();
                    //lbl_Lot_control.Text = LifterID[SelectedRowIndex + groupRowFixer].lotNumber.ToString();
                    //lbl_GLPoints_control.Text = GLPointsCalculator(LifterID[SelectedRowIndex + groupRowFixer]).ToString();
                    if (LiftingOrderListNew.Count > 0)
                    {
                        //PlateCalculator(LiftingOrderListNew[0].sbdList[LiftingOrderListNew[0].CurrentLift + firstLftdatagridviewColumn], plateInfo);

                        lbl_Name.Text = LiftingOrderListNew[0].name;
                        lbl_Placement.Text = LiftingOrderListNew[0].place.ToString();
                        lbl_Infällt.Text = LiftingOrderListNew[0].tilted.ToString();
                        lbl_Avlyft.Text = LiftingOrderListNew[0].liftoff.ToString();
                        lbl_Grupp_control.Text = LiftingOrderListNew[0].groupNumber.ToString();
                        lbl_Lot_control.Text = LiftingOrderListNew[0].lotNumber.ToString();
                        lbl_GLPoints_control.Text = GLPointsCalculator(LiftingOrderListNew[0]).ToString();

                        if (LiftingOrderListNew.Count > 1)
                        {
                            lbl_Name2.Text = LiftingOrderListNew[1].name;
                            PlateCalculator(float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LiftingOrderListNew[1].CurrentLift].Value.ToString()), plateInfo);
                            lbl_Placement2.Text = LiftingOrderListNew[1].place.ToString();
                            lbl_Infällt2.Text = LiftingOrderListNew[1].tilted.ToString();
                            lbl_Avlyft2.Text = LiftingOrderListNew[1].liftoff.ToString();
                            lbl_Grupp_control2.Text = LiftingOrderListNew[1].groupNumber.ToString();
                            lbl_Lot_control2.Text = LiftingOrderListNew[1].lotNumber.ToString();
                            lbl_GLPoints_control2.Text = GLPointsCalculator(LiftingOrderListNew[1]).ToString();

                        }
                    }

                }
            }
        }
        private void dataGridViewControlPanel_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //resetta plateshit


            dataGridViewControlPanel.EndEdit();
            RankUpdate();
            if (dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value != DBNull.Value)
            {

                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[20].Value = LifterID[SelectedRowIndex + groupRowFixer].total;
                string s = dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString();
                //string s = (Math.Round(float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString().Replace(".", ",2,5")) / .5f) * .5f).ToString();
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = s; //Ändrar punkt till komman

                if (!s.Any(char.IsLetter))
                {
                    s = (Math.Round(float.Parse(s.Replace(",", ".")) / .5f) * .5f).ToString();
                    dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = s;

                    if (SelectedColumnIndex < 20)
                    {
                        LifterID[SelectedRowIndex + groupRowFixer].sbdList[LifterID[SelectedRowIndex + groupRowFixer].LiftRecord.Count] =
                            float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString()); // Sätter vikten till sbdlist

                    }
                    LiftOrderUpdate();//Updaterar lyftar ordning
                    GroupLiftOrderUpdate();//Updaterar nästa grupps lyftar ordning

                    if (float.Parse(s) <= float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift - 1].Value.ToString()) &&
                        SelectedColumnIndex < 14)
                    {
                        MessageBox.Show("Föregående lyft \"" + dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift - 1].Value.ToString() + "\" Är Högre än \"" + s + "\"", "⚠Varning!⚠");
                    }
                    float f = 0; //gör bara så att tryparse några rader under har något o lägga en variabel i lol

                    float totalWeightAllPlates = 0f;

                    if (float.TryParse(s, out f) || s == 0.ToString())
                    {
                        for (int i = 0; i < weightsList.Count; i++)
                        {
                            totalWeightAllPlates = (totalPlatesList[i] * 2) * weightsList[i];
                        }
                        if (float.Parse(s) < 25 && float.Parse(s) > totalWeightAllPlates)
                        {
                            dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = 25f;
                        }
                    }
                }
                else
                {

                    MessageBox.Show(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString() + " Är inte ett nummer", "⚠Varning!⚠");
                    dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = 25;
                    if (SelectedColumnIndex < 14)
                    {
                        LiftingOrderListNew.Add(LifterID[SelectedRowIndex + groupRowFixer]);
                        dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Style.BackColor = Color.Empty;
                        dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = 0;
                        dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift - 1].Style.BackColor = Color.FromArgb(255, 127, 80);
                        LifterID[SelectedRowIndex + groupRowFixer].CurrentLift -= 1;
                    }

                }
                usedPlatesList.Clear();
            }
            else
            {

                MessageBox.Show("det var en string");
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = 25f;
            }

        }
        private void dataGridViewControlPanel_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            //    if (!string.IsNullOrWhiteSpace(dataGridViewControlPanel.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) &&
            //    float.Parse(dataGridViewControlPanel.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) >= 25)
            //    {
            //        LiftingOrderList2New.Add(float.Parse(dataGridViewControlPanel.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()));
            //    }


            //dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = 25f;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) //Hanterar all input från tagentbord
        {
            if (tabControl1.SelectedIndex == 2 &&
                keyData == Keys.Enter
                //om man är på sista raden 
                )
            {
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[SelectedColumnIndex - 1].Selected = true;
            }
            if (tabControl1.SelectedIndex == 2 &&
                keyData == Keys.G && LifterID[SelectedRowIndex + groupRowFixer].CurrentLift <= 19 &&
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value != DBNull.Value &&
                !dataGridViewControlPanel.IsCurrentCellInEditMode)            //Godkänt lyft
            {
                goodLift();

                return true;
            }
            if (tabControl1.SelectedIndex == 2 &&
                keyData == Keys.U && LifterID[SelectedRowIndex + groupRowFixer].CurrentLift <= 19 &&
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value != DBNull.Value &&
                !dataGridViewControlPanel.IsCurrentCellInEditMode)       //Underkänt lyft
            {
                badLift();

                return true;
            }
            if (tabControl1.SelectedIndex == 2 && keyData == Keys.R && LifterID[SelectedRowIndex + groupRowFixer].CurrentLift >= 12 &&
                !dataGridViewControlPanel.IsCurrentCellInEditMode)       //Ångra lyft
            {
                undoLift();

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void goodLift()
        {
            //Tar bort rätt lyftare
            if (!(LiftingOrderListNew.Count <= 0))
            {
                // Medelande om lyftaren redan lyft funkar inte ?!?!?!?!?
                if (!LiftingOrderListNew.Contains(LifterID[SelectedRowIndex + groupRowFixer]))
                {
                    MessageBox.Show("Denna lyftare har redan lyft denna omgång");
                    return;
                }
                for (int i = 0; i < LiftingOrderListNew.Count; i++)
                {

                    if (LifterID[SelectedRowIndex + groupRowFixer] == LiftingOrderListNew[i])
                    {
                        LiftingOrderListNew.RemoveAt(i);
                    }

                }
            }
            //Updaterar lyftar ordning
            LiftOrderUpdate();

            TimerController(8); //Startar lapp timern på 1 minut
            TimerController(9); //Stoppar lyft timern och sätter timern på 00:00

            dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Style.BackColor = Color.Green;

            //BestS,b,d sätts BestSBDUpdate(); för att den ska uppdatera när man ångrar lyft också
            BestSBDUpdate();
            //Uppdaterar placering
            RankUpdate();
            //Totalen sätts i comboboxgruppselecteditem switch satsen och här eftersom den måste uppdateras hela tiden
            LifterID[SelectedRowIndex + groupRowFixer].total = LifterID[SelectedRowIndex + groupRowFixer].bestS +
            LifterID[SelectedRowIndex + groupRowFixer].bestB + LifterID[SelectedRowIndex + groupRowFixer].bestD;


            if (SelectedColumnIndex < 19)
            {

                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift + 1].Style.BackColor = Color.FromArgb(255, 127, 80);
                dataGridViewControlPanel.CurrentCell = dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift + 1];

                if (SelectedColumnIndex != 14 && SelectedColumnIndex != 17)
                {
                    dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift + 1].Value = //Lägger till 2,5 automatiskt när man godkänner ett lyft
                        2.5f + float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString());

                }

                dataGridViewControlPanel.BeginEdit(true);
            }

            if (SelectedColumnIndex < 20)
            {
                LifterID[SelectedRowIndex + groupRowFixer].LiftRecord.Add(true); //Registrerar ett godkänt lyft för lyftaren
                LifterID[SelectedRowIndex + groupRowFixer].sbdList[LifterID[SelectedRowIndex + groupRowFixer].LiftRecord.Count - 1] = 
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString());

                LifterID[SelectedRowIndex + groupRowFixer].CurrentLift += 1;
            }


        }
        public void badLift()
        {
            LiftOrderUpdate();//Updaterar lyftar ordning
            if (!(LiftingOrderListNew.Count <= 0))
            {
                LiftingOrderListNew.RemoveAt(0);
            }
            TimerController(8); //Startar lapp timern på 1 minut
            TimerController(9); //Stoppar lyft timern och sätter timern på 00:00

            dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Style.BackColor = Color.Red;
            dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Style.Font = new System.Drawing.Font("Trebuchet MS", 10f, FontStyle.Strikeout);

            LifterID[SelectedRowIndex + groupRowFixer].total = LifterID[SelectedRowIndex + groupRowFixer].bestS +
            LifterID[SelectedRowIndex + groupRowFixer].bestB + LifterID[SelectedRowIndex + groupRowFixer].bestD;

            if (SelectedColumnIndex < 19)
            {
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift + 1].Style.BackColor = Color.FromArgb(255, 127, 80);
                dataGridViewControlPanel.CurrentCell = dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift + 1];

                if (SelectedColumnIndex != 14 && SelectedColumnIndex != 17)
                {
                    dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift + 1].Value = //Upprepar samma lyft i nästa ruta för underkänt lyft
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString());
                }
                dataGridViewControlPanel.BeginEdit(true);
            }

            if (SelectedColumnIndex < 20)
            {
                LifterID[SelectedRowIndex + groupRowFixer].LiftRecord.Add(false); //Registrerar ett godkänt lyft för lyftaren
                LifterID[SelectedRowIndex + groupRowFixer].sbdList[LifterID[SelectedRowIndex + groupRowFixer].LiftRecord.Count - 1] = float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value.ToString());

                LifterID[SelectedRowIndex + groupRowFixer].CurrentLift += 1;
            }
        }
        public void undoLift()
        {
            if (SelectedColumnIndex > firstLftdatagridviewColumn)
            {
                LiftOrderUpdate();//Updaterar lyftar ordning

                LifterID[SelectedRowIndex + groupRowFixer].LiftRecord.RemoveAt(LifterID[SelectedRowIndex + groupRowFixer].LiftRecord.Count - 1); //Ångarar ett lyft för lyftaren

                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Style.BackColor = Color.Empty;
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift].Value = 0;
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift - 1].Style.BackColor = Color.FromArgb(255, 127, 80);
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[LifterID[SelectedRowIndex + groupRowFixer].CurrentLift - 1].Style.Font = new System.Drawing.Font("Trebuchet MS", 10f, FontStyle.Regular);
                LifterID[SelectedRowIndex + groupRowFixer].CurrentLift -= 1;

                BestSBDUpdate(); //Updaterar bästa lyften sedan sätter totalen
                RankUpdate();
                LifterID[SelectedRowIndex + groupRowFixer].total = LifterID[SelectedRowIndex + groupRowFixer].bestS +
                LifterID[SelectedRowIndex + groupRowFixer].bestB + LifterID[SelectedRowIndex + groupRowFixer].bestD;
                //Visuellt sätter totalen
                dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[20].Value = LifterID[SelectedRowIndex + groupRowFixer].total;
            }
        }

        public void redoLift()
        {
            undoLift();
            LiftOrderUpdate();//Updaterar lyftar ordning
            LiftingOrderListNew.Add(LifterID[SelectedRowIndex + groupRowFixer]);
        }


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    {
                        //Invägning tabben laddas

                        break;
                    }
                case 1:
                    {
                        //Inställningar tabben laddas
                        break;
                    }
                case 2:
                    {
                        //Tävling tabben laddas

                        //Uppdaterar information of viktsivor såsom antal och färg
                        plateInfo = new PlateInfo(Int16.Parse(txtb50.Text), Int16.Parse(txtb25.Text), Int16.Parse(txtb20.Text), Int16.Parse(txtb15.Text), Int16.Parse(txtb10.Text), Int16.Parse(txtb5.Text), Int16.Parse(txtb25small.Text), Int16.Parse(txtb125small.Text), Int16.Parse(txtb05small.Text), Int16.Parse(txtb025small.Text)
                                        , btn50.BackColor, btn25.BackColor, btn20.BackColor, btn15.BackColor, btn10.BackColor, btn5.BackColor, btn25small.BackColor, btn05small.BackColor, btn125small.BackColor, btn025small.BackColor);

                        //Uppdaterar hur många grupper som finns
                        for (int i = 0; i < LifterID.Count; i++)
                        {
                            if (LifterID[i].groupNumber > groupIndexCount)
                            {
                                groupIndexCount = LifterID[i].groupNumber;
                            }
                        }
                        for (int i = 0; i < groupIndexCount; i++)
                        {
                            if (!combo_Aktivgrupp.Items.Contains(i + 1))
                            {
                                combo_Aktivgrupp.Items.Add(i + 1);
                            }
                        }

                        combo_Aktivgrupp.SelectedItem = 1;

                        if (Enumerable.Any(LifterID))
                        {
                            //LiftOrderUpdate();//Updaterar lyftar ordning
                            // dataGridViewControlPanel.Rows[0].Selected = false;  //Gör så att inget är markerat när datagrdiviewn laddas
                        }
                        for (int i = 0; i < dataGridViewControlPanel.RowCount; i++)
                        {
                            dataGridViewControlPanel.Rows[i].Cells[firstLftdatagridviewColumn].Style.BackColor = Color.FromArgb(255, 127, 80); //Vilken column börjar första böjen på?
                            for (int o = 0; o < 8; o++)
                            {
                                dataGridViewControlPanel.Rows[i].Cells[o].ReadOnly = true;
                                dataGridViewControlPanel.Columns[20].ReadOnly = true; //total
                                dataGridViewControlPanel.Columns[21].ReadOnly = true; //poäng
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        //Resultat tabben laddas
                        break;
                    }
                default:
                    break;
            }
        }

        //public void Display1(
        //    string Namn,
        //    string Lotnummer,
        //    string Viktklass,
        //    string Kategori,
        //    string Licensnummer,
        //    string Förening,
        //    string Kroppsvikt,
        //    string HöjdBöj,
        //    string HöjdBänk,
        //    string RackBänk,
        //    string IngångBöj,
        //    string IngångBänk,
        //    string IngångMark)
        //{
        //    if (b)
        //    {
        //        dt2.Columns.Add("#");           //0
        //        dt2.Columns.Add("Namn");        //1
        //        dt2.Columns.Add("Lot");         //2 
        //        dt2.Columns.Add("Klass");       //3
        //        dt2.Columns.Add("Kategori");    //4
        //        dt2.Columns.Add("Licensnr");//5
        //        dt2.Columns.Add("Förening");    //6
        //        dt2.Columns.Add("Kv");          //7
        //        dt2.Columns.Add("H\nBöj");       //8
        //        dt2.Columns.Add("H\nBänk");      //9
        //        dt2.Columns.Add("R\nBänk");      //10
        //        dt2.Columns.Add("S1");          //11
        //        dt2.Columns.Add("S2");          //12
        //        dt2.Columns.Add("S3");          //13
        //        dt2.Columns.Add("B1");          //14
        //        dt2.Columns.Add("B2");          //15
        //        dt2.Columns.Add("B3");          //16
        //        dt2.Columns.Add("D1");          //17
        //        dt2.Columns.Add("D2");          //18
        //        dt2.Columns.Add("D3");          //19
        //        dt2.Columns.Add("Total");       //20
        //        dt2.Columns.Add("IPF GL\nPoäng");

        //        b = false;
        //    }
        //    DataRow dr2 = dt2.NewRow();

        //    dr2[1] = Namn;
        //    dr2[2] = Lotnummer;
        //    dr2[3] = Viktklass;
        //    dr2[4] = Kategori;
        //    dr2[5] = Licensnummer;
        //    dr2[6] = Förening;
        //    dr2[7] = Kroppsvikt;
        //    dr2[8] = HöjdBöj;
        //    dr2[9] = HöjdBänk;
        //    dr2[10] = RackBänk;
        //    dr2[11] = IngångBöj;
        //    dr2[14] = IngångBänk;
        //    dr2[17] = IngångMark;

        //    //Debug
        //    //dr2[1] = "Namn";
        //    //dr2[2] = "Lotnummer";
        //    //dr2[3] = "Viktklass";
        //    //dr2[4] = "Kategori";
        //    //dr2[5] = "Licensnummer";
        //    //dr2[6] = "Förening";
        //    //dr2[7] = "Kroppsvikt";
        //    //dr2[8] = "HöjdBöj";
        //    //dr2[9] = "HöjdBänk";
        //    //dr2[10] = "Rackbänk";
        //    //dr2[11] = "IngångBöj";
        //    //dr2[14] = "IngångBänk";
        //    //dr2[17] = "IngångMark";

        //    dt2.Rows.Add(dr2);
        //    dataGridViewControlPanel.DataSource = dt2;
        //}
        public void DisplayAll(
            string Place,
            string Namn,
            string Lotnummer,
            string Viktklass,
            string Kategori,
            string Licensnummer,
            string Förening,
            string Kroppsvikt,
            string HöjdBöj,
            string HöjdBänk,
            string RackBänk,
            string s1,
            string s2,
            string s3,
            string b1,
            string b2,
            string b3,
            string d1,
            string d2,
            string d3,
            string group)
        {
            if (b)
            {
                dt2.Columns.Add("#");           //0
                dt2.Columns.Add("Namn");        //1
                dt2.Columns.Add("Lot");         //2 
                dt2.Columns.Add("Klass");       //3
                dt2.Columns.Add("Kat.");        //4
                dt2.Columns.Add("Lic.nr");      //5
                dt2.Columns.Add("Förening");    //6
                dt2.Columns.Add("Kv");          //7
                dt2.Columns.Add("H\nBöj");      //8
                dt2.Columns.Add("H\nBänk");     //9
                dt2.Columns.Add("R\nBänk");     //10
                dt2.Columns.Add("S1");          //11
                dt2.Columns.Add("S2");          //12
                dt2.Columns.Add("S3");          //13
                dt2.Columns.Add("B1");          //14
                dt2.Columns.Add("B2");          //15
                dt2.Columns.Add("B3");          //16
                dt2.Columns.Add("D1");          //17
                dt2.Columns.Add("D2");          //18
                dt2.Columns.Add("D3");          //19
                dt2.Columns.Add("Total");       //20
                dt2.Columns.Add("IPF GL\nPoäng");

                b = false;
            }
            DataRow dr2 = dt2.NewRow();

            dr2[0] = Place;
            dr2[1] = Namn;
            dr2[2] = Lotnummer;
            dr2[3] = Viktklass;
            dr2[4] = Kategori;
            dr2[5] = Licensnummer;
            dr2[6] = Förening;
            dr2[7] = Kroppsvikt;
            dr2[8] = HöjdBöj;
            dr2[9] = HöjdBänk;
            dr2[10] = RackBänk;
            dr2[11] = s1;
            dr2[12] = s2;
            dr2[13] = s3;
            dr2[14] = b1;
            dr2[15] = b2;
            dr2[16] = b3;
            dr2[17] = d1;
            dr2[18] = d2;
            dr2[19] = d3;

            List<string> sbdlist = new List<string>();
            sbdlist.AddRange(new string[] { s1, s2, s3, b1, b2, b3, d1, d2, d3 });

            //Debug
            //MessageBox.Show("Namn : " + Namn +
            //              "\n Squat 1 : " + s1 +
            //              "\n Squat 2 : " + s2 +
            //              "\n Squat 3 : " + s3 +
            //              "\n Bench 1 : " + b1 +
            //              "\n Bench 2 : " + b2 +
            //              "\n Bench 3 : " + b3 +
            //              "\n Deadlift 1 : " + d1 +
            //              "\n Deadlift 2 : " + d2 +
            //              "\n Deadlift 3 : " + d3);

            dt2.Rows.Add(dr2);
            dataGridViewControlPanel.DataSource = dt2;

        }
        public void PlateCalculator(float targetWeight, PlateInfo plateInfo)
        {
            //targetWeight = 0;
            //weightSum = 0;
            //usedPlatesList.Clear();
            

            targetWeight = (targetWeight / 2);
            float weightSum = 12.5f; //Stång (20kg) + lås (5kg) delas på två eftersom target weight också är delat på två

            usedPlatesList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            totalPlatesList.AddRange(new int[] { plateInfo.plate50, plateInfo.plate25, plateInfo.plate20, plateInfo.plate15, plateInfo.plate10, plateInfo.plate5,
            plateInfo.plate25small, plateInfo.plate125, plateInfo.plate05, plateInfo.plate025,});

            weightsList.AddRange(new float[] { 50, 25, 20, 15, 10, 5, 2.5f, 1.25f, 0.5f, 0.25f });

            for (int i = 0; weightSum != targetWeight;)
            {
                if (weightSum + weightsList[i] <= targetWeight && totalPlatesList[i] > usedPlatesList[i])
                {
                    weightSum += weightsList[i];
                    usedPlatesList[i]++;
                    platesCount++;
                    infopanel_Controlpanel.Invalidate();
                }
                else { i++; }

            }

            if (weightSum == targetWeight) //Tar totala summan och kollar om det är samma som målsumman
            {
                lbl_currentWeight.Text = (targetWeight * 2).ToString() + " KG";

                // Debuggi'n Antal hur många av varera viktskiva har använts
                //lbl_currentWeight.Text =
                //    ((usedPlatesList[0] * weightsList[0] +
                //    usedPlatesList[1] * weightsList[1] +
                //    usedPlatesList[2] * weightsList[2] +
                //    usedPlatesList[3] * weightsList[3] +
                //    usedPlatesList[4] * weightsList[4] +
                //    usedPlatesList[5] * weightsList[5] +
                //    usedPlatesList[6] * weightsList[6] +
                //    usedPlatesList[7] * weightsList[7] +
                //    usedPlatesList[8] * weightsList[8] +
                //    usedPlatesList[9] * weightsList[9]) * 2) +

                //    "\n50 : " + usedPlatesList[0] + "| 25 : " + usedPlatesList[1]
                //    + "| 20 : " + usedPlatesList[2] + "| 15 : " + usedPlatesList[3] + "| 10 : " + usedPlatesList[4]
                //     + "\n 5 : " + usedPlatesList[5] + "| 2,5 : " + usedPlatesList[6] + "| 125 : " + usedPlatesList[7]
                //     + "| 0,5 : " + usedPlatesList[8] + "| 0,25 : " + usedPlatesList[9];

            }
            else { MessageBox.Show("Något blev fel med viktuträkning"); }

        }

        public void TimerController(int option)
        {
            switch (option)
            {
                case 0:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 1;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 1:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 2;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 2:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 3;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 3:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 4;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 4:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 10;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 5:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 20;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 6:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 30;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 7:         //Sätt klockan på 1 minut
                    {
                        minutesLyft = 60;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                case 8:         //Starta lapp timern på 1 minut
                    {
                        minutesLapp = 1;
                        secondsLapp = 0;
                        timerLapp.Start();
                        break;
                    }
                case 9:         //Stoppar och resettar lyft timern
                    {
                        minutesLyft = 0;
                        secondsLyft = 0;
                        timerLyft.Start();
                        break;
                    }
                default: { break; }

            }
        }
        private void TimerTickLyft(object sender, EventArgs e)
        {
            secondsLyft--;
            if (secondsLyft < 0)
            {
                secondsLyft = 59;
                minutesLyft--;
                if (minutesLyft < 0)
                {
                    minutesLyft = 0;
                    secondsLyft = 0;
                }
            }
            lbl_timerLyft.Text = (minutesLyft.ToString()).PadLeft(2, '0') + ":" + (secondsLyft.ToString()).PadLeft(2, '0');
        }
        private void TimerTickLapp(object sender, EventArgs e)
        {
            secondsLapp--;
            if (secondsLapp < 0)
            {
                secondsLapp = 59;
                minutesLapp--;
                if (minutesLapp < 0)
                {
                    minutesLapp = 0;
                    secondsLapp = 0;
                }
            }
            lbl_timerLapp.Text = (minutesLapp.ToString()).PadLeft(2, '0') + ":" + (secondsLapp.ToString()).PadLeft(2, '0');
        }

        public void LiftOrderUpdate()   //Uppdaerar nuvarandes grupps lyftarordning och den läggs till i när man klickar på go
        {                               //förta listan fylls och sen tas det bort lyftare allt eftersom och när den är tom så byts informationen från lista två ut mot informationen i lista 1 så det blir infinate loop,
                                        //man tar bara bort lyftare från lista 1 och lägger bara till i lista 2

            LiftingOrderListLabels.AddRange(new System.Windows.Forms.Label[] { lbl_liftOrder_control_1, lbl_liftOrder_control_2, lbl_liftOrder_control_3, lbl_liftOrder_control_4,
                                                        lbl_liftOrder_control_5, lbl_liftOrder_control_6, lbl_liftOrder_control_7, lbl_liftOrder_control_8,
                                                        lbl_liftOrder_control_9, lbl_liftOrder_control_10, lbl_liftOrder_control_11, lbl_liftOrder_control_12,
                                                        lbl_liftOrder_control_13, lbl_liftOrder_control_14, lbl_liftOrder_control_15, lbl_liftOrder_control_16,
                                                        lbl_liftOrder_control_17, lbl_liftOrder_control_18, lbl_liftOrder_control_19, lbl_liftOrder_control_20});
            if (restartLiftingOrderList == true)     //Fyller listan för första gången
            {
                LiftingOrderListNew.Clear();
                for (int i = 0; i < group1Count; i++)
                {
                    LiftingOrderListNew.Add(LifterID[i]);
                    restartLiftingOrderList = false;
                }
            }
            if (LiftingOrderListNew.Count == 0)       //Om andra listan är tom så fylls den tills den första listan är tom.
            {
                //fyll lista två med all lyftares currentlift
                for (int i = 0; i < group1Count; i++)
                {
                    LiftingOrderList2New.Add(LifterID[i]);
                }

                for (int i = 0; i < LiftingOrderList2New.Count; i++)
                {
                    LiftingOrderListNew.Add(LiftingOrderList2New[i]);
                }
                LiftingOrderList2New.Clear();
            }
            for (int i = 0; i < LiftingOrderListLabels.Count; i++)
            {
                LiftingOrderListLabels[i].Text = "";
            }
            LiftingOrderList.Sort();
            // Create an instance of the custom comparer.
            var comparer = new LifterComparer();

            // Use the custom comparer to sort LiftingOrderListNew.
            LiftingOrderListNew = LiftingOrderListNew.OrderBy(item => item, comparer).ToList();


            for (int i = 0; i < LiftingOrderListNew.Count; i++)
            {
                LiftingOrderListLabels[i].Text = LiftingOrderListNew[i].sbdList[LiftingOrderListNew[i].CurrentLift - 11] + " " + LiftingOrderListNew[i].name;
                if (LiftingOrderListLabels.Count <= 20 && LiftingOrderList2New.Any())
                {
                    MessageBox.Show(i + LiftingOrderListNew.Count.ToString());
                    LiftingOrderListLabels[i + LiftingOrderListNew.Count].Text = LiftingOrderListNew[i].sbdList[LiftingOrderList2New[i].CurrentLift - 11] + " " + LiftingOrderList2New[i].name;
                }
            }
        }
        public void GroupLiftOrderUpdate() //Updaterar nästa grupps ingångar
        {
            group2Count = 0;                        //Resettar så att den inte blir för mycket om man ändrar grupper
            for (int i = 0; i < LifterID.Count; i++) //Antal lyftare i grupp 1
            {
                if (LifterID[i].groupNumber == 1)
                {
                    group2Count += 1;
                }
            }

            GroupLiftingOrderListLabels.AddRange(new System.Windows.Forms.Label[] { lbl_groupLiftOrder_control_1, lbl_groupLiftOrder_control_2, lbl_groupLiftOrder_control_3, lbl_groupLiftOrder_control_4,
                                                        lbl_groupLiftOrder_control_5, lbl_groupLiftOrder_control_6, lbl_groupLiftOrder_control_7, lbl_groupLiftOrder_control_8,
                                                        lbl_groupLiftOrder_control_9, lbl_groupLiftOrder_control_10, lbl_groupLiftOrder_control_11, lbl_groupLiftOrder_control_12,
                                                        lbl_groupLiftOrder_control_13, lbl_groupLiftOrder_control_14, lbl_groupLiftOrder_control_15, lbl_groupLiftOrder_control_16,
                                                        lbl_groupLiftOrder_control_17, lbl_groupLiftOrder_control_18, lbl_groupLiftOrder_control_19, lbl_groupLiftOrder_control_20});
            for (int i = 0; i < GroupLiftingOrderListLabels.Count; i++)
            {
                GroupLiftingOrderListLabels[i].Text = "";
            }
            if (groupIndexCurrent == 0)     //Fyller listan, om den aktiva gruppen är grupp 1
            {
                GroupLiftingOrderList.Clear();
                //for (int i = group1Count; i < group2Count + group1Count; i++)
                //{
                //    GroupLiftingOrderList.Add(LifterID[i].sbdList[LifterID[i].CurrentLift - 11]);
                //}
            }
            GroupLiftingOrderList.Sort();
            for (int i = 0; i < GroupLiftingOrderList.Count; i++)
            {
                GroupLiftingOrderListLabels[i].Text = GroupLiftingOrderList[i].ToString();
            }
        }
        public void BestSBDUpdate()
        {
#pragma warning disable CA1305
            if (SelectedColumnIndex < 14)
            {
                LifterID[SelectedRowIndex + groupRowFixer].bestS = MoreMath.Max(
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[11].Value.ToString()),
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[12].Value.ToString()),
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[13].Value.ToString()));
            }
#pragma warning restore CA1305
            else if (SelectedColumnIndex > 13 && SelectedColumnIndex < 17)
            {
                LifterID[SelectedRowIndex + groupRowFixer].bestB = MoreMath.Max(
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[14].Value.ToString()),
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[15].Value.ToString()),
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[16].Value.ToString()));
            }
            else
            {
                LifterID[SelectedRowIndex + groupRowFixer].bestD = MoreMath.Max(
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[17].Value.ToString()),
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[18].Value.ToString()),
                    float.Parse(dataGridViewControlPanel.Rows[SelectedRowIndex].Cells[19].Value.ToString()));
            }
        }
        public void RankUpdate()
        {
            var groupedLifters = LifterID.Values.GroupBy(l => new { l.weightClass, l.CategoryEnum });

            // Iterate through each group
            foreach (var group in groupedLifters)
            {
                // Sort the lifters within the group based on their total in descending order
                var sortedLifters = group.OrderByDescending(l => l.total).ThenBy(l => l.bodyWeight).ToList();

                for (int i = 0; i < sortedLifters.Count; i++)
                {
                    var lifterToUpdate = LifterID.Values.FirstOrDefault(l => l.weightClass == group.Key.weightClass && l.CategoryEnum == group.Key.CategoryEnum && l.name == sortedLifters[i].name);

                    if (lifterToUpdate != null)
                    {
                        lifterToUpdate.place = i + 1;
                    }
                }
            }

            // Update the DataGridView
            for (int i = 0; i < dataGridViewControlPanel.Rows.Count; i++)
            {
                dataGridViewControlPanel.Rows[i].Cells[0].Value = LifterID[i + groupRowFixer].place;
            }
        }

        //CODE BELOW WORKS GOOD BUT DOES NOT CONSIDER BODYWEIGHT
        //public void RankUpdate()
        //{
        //    var groupedLifters = LifterID.Values.GroupBy(l => new { l.weightClass, l.CategoryEnum });

        //    // Iterate through each group
        //    foreach (var group in groupedLifters)
        //    {
        //        // Sort the lifters within the group based on their total in descending order
        //        var sortedLifters = group.OrderByDescending(l => l.total).ToList();

        //        for (int i = 0; i < sortedLifters.Count; i++)
        //        {
        //            var lifterToUpdate = LifterID.Values.FirstOrDefault(l => l.weightClass == group.Key.weightClass && l.CategoryEnum == group.Key.CategoryEnum && l.name == sortedLifters[i].name);

        //            if (lifterToUpdate != null)
        //            {
        //                lifterToUpdate.place = i + 1;
        //            }
        //        }
        //    }

        //    // Update the DataGridView
        //    for (int i = 0; i < dataGridViewControlPanel.Rows.Count; i++)
        //    {
        //        dataGridViewControlPanel.Rows[i].Cells[0].Value = LifterID[i + groupRowFixer].place;
        //    }
        //}



        private void TimerTickRekordAnimering(object sender, EventArgs e)
        {
            if (IsRecord)
            {
                //Till projector gör så det kommer upp text som blinkar som innehåller recordType

            }
        }

        private void btn_1min_Click(object sender, EventArgs e)
        {
            TimerController(0);
        }

        private void btn_2min_Click(object sender, EventArgs e)
        {
            TimerController(1);
        }

        private void btn_3min_Click(object sender, EventArgs e)
        {
            TimerController(2);
        }

        private void btn_4min_Click(object sender, EventArgs e)
        {
            TimerController(3);
        }

        private void btn_10min_Click(object sender, EventArgs e)
        {
            TimerController(4);
        }

        private void btn_20min_Click(object sender, EventArgs e)
        {
            TimerController(5);
        }

        private void btn_30min_Click(object sender, EventArgs e)
        {
            TimerController(6);
        }

        private void btn_60min_Click(object sender, EventArgs e)
        {
            TimerController(7);
        }

        private void btn_klovad_Click(object sender, EventArgs e)
        {
            TimerController(0);
        }

        private void btn_godkänt_Click(object sender, EventArgs e)
        {
            goodLift();
        }

        private void btn_underkänt_Click(object sender, EventArgs e)
        {
            badLift();
        }

        private void btn_ångralyft_Click(object sender, EventArgs e)
        {
            undoLift();
        }

        private void btn_Gåom_Click(object sender, EventArgs e)
        {
            redoLift();
        }
        private void dataGridViewControlPanel_KeyDown(object sender, KeyEventArgs e)    //Tar bort möjligheten att nagigera med höger och vänster piltagenter
        {                                                                               //Det var möjligt att nagigera höger väntster utan att rutn blev blå
            switch (e.KeyData & Keys.KeyCode)                                           //Men sen när man skrev så bled det i den rutan ändå även om den inte var blå
            {
                case Keys.Right:
                case Keys.Left:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void combo_Aktivgrupp_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupIndexCurrent = combo_Aktivgrupp.SelectedIndex;
            RankUpdate();
            switch (combo_Aktivgrupp.SelectedIndex)
            {
                case 0:
                    //ladda första gruppen
                    //1111111111111111
                    //1111111111111111
                    //1111111111111111
                    //1111111111111111
                    //1111111111111111
                    //1111111111111111
                    dt2.Rows.Clear();
                    groupRowFixer = 0;
                    group1Count = 0;                        //Resettar så att den inte blir för mycket om man ändrar grupper
                    for (int i = 0; i < LifterID.Count; i++) //Antal lyftare i grupp 1
                    {
                        if (LifterID[i].groupNumber == 1)
                        {
                            group1Count += 1;
                        }
                    }

                    for (int i = 0; i < group1Count; i++)
                    {
                        DisplayAll(LifterID[i].place.ToString(), LifterID[i].name, LifterID[i].lotNumber.ToString(), LifterID[i].weightClass, "Senior", LifterID[i].licenceNumber
                            , LifterID[i].accossiation, LifterID[i].bodyWeight.ToString(), LifterID[i].squatHeight.ToString(), LifterID[i].benchHeight.ToString()
                            , LifterID[i].benchRack.ToString()
                            , LifterID[i].sbdList[0].ToString(), LifterID[i].sbdList[1].ToString(), LifterID[i].sbdList[2].ToString()
                            , LifterID[i].sbdList[3].ToString(), LifterID[i].sbdList[4].ToString(), LifterID[i].sbdList[5].ToString()
                            , LifterID[i].sbdList[6].ToString(), LifterID[i].sbdList[7].ToString(), LifterID[i].sbdList[8].ToString()
                            , LifterID[i].groupNumber.ToString());
                    }

                    for (int i = 0; i < dataGridViewControlPanel.RowCount; i++)
                    {
                        //Totalen sätts
                        LifterID[i + groupRowFixer].total = LifterID[i + groupRowFixer].bestS + LifterID[i + groupRowFixer].bestB + LifterID[i + groupRowFixer].bestD; // Sätter totalen
                        dataGridViewControlPanel.Rows[i].Cells[20].Value = LifterID[i + groupRowFixer].total;

                        //firstLftdatagridviewColumn är första lyftets kolumn
                        for (int o = firstLftdatagridviewColumn; o < LifterID[i].CurrentLift; o++) //Fyller i lyft historik tills currentlift
                        {
                            for (int p = 0; p < LifterID[i].LiftRecord.Count; p++) //p räknar hur många lyft som vad godkända eller underkända
                            {
                                if (LifterID[i].LiftRecord[p] == true)
                                {
                                    dataGridViewControlPanel.Rows[i].Cells[o].Style.BackColor = Color.ForestGreen;
                                }
                                else if (LifterID[i].LiftRecord[p] == false)
                                {
                                    dataGridViewControlPanel.Rows[i].Cells[o].Style.BackColor = Color.Red;
                                }
                            }
                        }
                        if (SelectedColumnIndex < 19)
                        {
                            dataGridViewControlPanel.Rows[i].Cells[LifterID[i].CurrentLift].Style.BackColor = Color.FromArgb(255, 127, 80);
                        }
                        for (int o = 0; o < 8; o++)
                        {
                            dataGridViewControlPanel.Rows[i].Cells[o].ReadOnly = true;
                        }
                    }

                    for (int i = 0; i < dataGridViewControlPanel.ColumnCount; i++)
                    {
                        dataGridViewControlPanel.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }


                    break;
                case 1:
                    //ladda andra gruppen
                    //222222222222
                    //222222222222
                    //222222222222
                    //222222222222
                    //222222222222
                    dt2.Rows.Clear();
                    groupRowFixer = group1Count;
                    weightsList.Clear();
                    group2Count = 0;                        //Resettar så att den inte blir för mycket om man ändrar grupper
                    for (int i = 0; i < LifterID.Count; i++) //Antal lyftare i grupp 1
                    {
                        if (LifterID[i].groupNumber == 2)
                        {
                            group2Count += 1;
                        }
                    }

                    for (int i = group1Count; i < group1Count + group2Count; i++)
                    {

                        DisplayAll(LifterID[i].place.ToString(), LifterID[i].name, LifterID[i].lotNumber.ToString(), LifterID[i].weightClass, "Senior", LifterID[i].licenceNumber
                            , LifterID[i].accossiation, LifterID[i].bodyWeight.ToString(), LifterID[i].squatHeight.ToString(), LifterID[i].benchHeight.ToString()
                            , LifterID[i].benchRack.ToString()
                            , LifterID[i].sbdList[0].ToString(), LifterID[i].sbdList[1].ToString(), LifterID[i].sbdList[2].ToString()
                            , LifterID[i].sbdList[3].ToString(), LifterID[i].sbdList[4].ToString(), LifterID[i].sbdList[5].ToString()
                            , LifterID[i].sbdList[6].ToString(), LifterID[i].sbdList[7].ToString(), LifterID[i].sbdList[8].ToString()
                            , LifterID[i].groupNumber.ToString());
                    }

                    for (int i = 0; i < dataGridViewControlPanel.RowCount; i++)
                    {
                        //Totalen sätts
                        LifterID[i + groupRowFixer].total = LifterID[i + groupRowFixer].bestS + LifterID[i + groupRowFixer].bestB + LifterID[i + groupRowFixer].bestD; // Sätter totalen
                        dataGridViewControlPanel.Rows[i].Cells[20].Value = LifterID[i + groupRowFixer].total;

                        //firstLftdatagridviewColumn är första lyftets kolumn
                        for (int o = firstLftdatagridviewColumn; o < LifterID[i + group1Count].CurrentLift; o++) //Fyller i lyft historik tills currentlift
                        {
                            for (int p = 0; p < LifterID[i + group1Count].LiftRecord.Count; p++) //p räknar hur många lyft som vad godkända eller underkända
                            {
                                if (LifterID[i + group1Count].LiftRecord[p] == true)
                                {
                                    dataGridViewControlPanel.Rows[i].Cells[o].Style.BackColor = Color.ForestGreen;
                                }
                                else if (LifterID[i + group1Count].LiftRecord[p] == false)
                                {
                                    dataGridViewControlPanel.Rows[i].Cells[o].Style.BackColor = Color.Red;
                                }
                            }
                        }
                        if (SelectedColumnIndex < 19)
                        {
                            dataGridViewControlPanel.Rows[i].Cells[LifterID[i + group1Count].CurrentLift].Style.BackColor = Color.FromArgb(255, 127, 80);
                        }
                        for (int o = 0; o < 8; o++)
                        {
                            dataGridViewControlPanel.Rows[i].Cells[o].ReadOnly = true;
                        }
                    }
                    for (int i = 0; i < dataGridViewControlPanel.ColumnCount; i++)
                    {
                        dataGridViewControlPanel.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    break;
                case 2:
                    //ladda tredje gruppen
                    //333333333333333
                    //333333333333333
                    //333333333333333
                    //333333333333333
                    //333333333333333
                    dt2.Rows.Clear();
                    groupRowFixer = group1Count + group3Count;
                    group3Count = 0;                        //Resettar så att den inte blir för mycket om man ändrar grupper
                    for (int i = 0; i < LifterID.Count; i++) //Antal lyftare i grupp 1
                    {
                        if (LifterID[i].groupNumber == 3)
                        {
                            group3Count += 1;
                        }
                    }

                    for (int i = 0; i < group3Count - 1; i++)
                    {
                        DisplayAll(LifterID[i].place.ToString(), LifterID[i].name, LifterID[i].lotNumber.ToString(), LifterID[i].weightClass, "Senior", LifterID[i].licenceNumber
                            , LifterID[i].accossiation, LifterID[i].bodyWeight.ToString(), LifterID[i].squatHeight.ToString(), LifterID[i].benchHeight.ToString()
                            , LifterID[i].benchRack.ToString()
                            , LifterID[i].sbdList[0].ToString(), LifterID[i].sbdList[1].ToString(), LifterID[i].sbdList[2].ToString()
                            , LifterID[i].sbdList[3].ToString(), LifterID[i].sbdList[4].ToString(), LifterID[i].sbdList[5].ToString()
                            , LifterID[i].sbdList[6].ToString(), LifterID[i].sbdList[7].ToString(), LifterID[i].sbdList[8].ToString()
                            , LifterID[i].groupNumber.ToString());
                    }

                    for (int i = 0; i < dataGridViewControlPanel.RowCount; i++)
                    {
                        //Totalen sätts
                        LifterID[i + groupRowFixer].total = LifterID[i + groupRowFixer].bestS + LifterID[i + groupRowFixer].bestB + LifterID[i + groupRowFixer].bestD; // Sätter totalen
                        dataGridViewControlPanel.Rows[i].Cells[20].Value = LifterID[i + groupRowFixer].total;

                        //firstLftdatagridviewColumn är första lyftets kolumn
                        for (int o = firstLftdatagridviewColumn; o < LifterID[i + group1Count + group2Count].CurrentLift; o++) //Fyller i lyft historik tills currentlift
                        {
                            for (int p = 0; p < LifterID[i + group1Count + group2Count].LiftRecord.Count; p++) //p räknar hur många lyft som vad godkända eller underkända
                            {
                                if (LifterID[i + group1Count + group2Count].LiftRecord[p] == true)
                                {
                                    dataGridViewControlPanel.Rows[i].Cells[o].Style.BackColor = Color.ForestGreen;
                                }
                                else if (LifterID[i + group1Count + group2Count].LiftRecord[p] == false)
                                {
                                    dataGridViewControlPanel.Rows[i].Cells[o].Style.BackColor = Color.Red;
                                }
                            }
                        }
                        if (SelectedColumnIndex < 19)
                        {
                            dataGridViewControlPanel.Rows[i].Cells[LifterID[i + group1Count + group2Count].CurrentLift].Style.BackColor = Color.FromArgb(255, 127, 80);
                        }
                        for (int o = 0; o < 8; o++)
                        {
                            dataGridViewControlPanel.Rows[i].Cells[o].ReadOnly = true;
                        }
                    }

                    for (int i = 0; i < dataGridViewControlPanel.ColumnCount; i++)
                    {
                        dataGridViewControlPanel.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    break;
                default:
                    break;
            }
        }

        private void lbl_currentWeight_Click(object sender, EventArgs e)
        {

        }

        public double GLPointsCalculator(Lifter lifter)
        {

            //Men
            double MenEquippedA = 1236.25115;
            double MenEquippedB = 1449.21864;
            double MenEquippedC = 0.01644;
            double MenClassicA = 1199.72839;
            double MenClassicB = 1025.18162;
            double MenClassicC = 0.00921;
            double MenEquippedBenchA = 381.22073;
            double MenEquippedBenchB = 733.79378;
            double MenEquippedBenchC = 0.02398;
            double MenClassicBenchA = 320.98041;
            double MenClassicBenchB = 281.40258;
            double MenClassicBenchC = 0.01008;
            //Women
            double WomenEquippedA = 758.63878;
            double WomenEquippedB = 949.31382;
            double WomenEquippedC = 0.02435;
            double WomenClassicA = 610.32796;
            double WomenClassicB = 1045.59282;
            double WomenClassicC = 0.03048;
            double WomenEquippedBenchA = 221.82209;
            double WomenEquippedBenchB = 357.00377;
            double WomenEquippedBenchC = 0.02937;
            double WomenClassicBenchA = 142.40398;
            double WomenClassicBenchB = 442.52671;
            double WomenClassicBenchC = 0.04724;

            double A = 1;
            double B = 1;
            double C = 1;

            double GLPointsCoeff = 0;
            double GLPoints = 0;

            switch (lifter.CategoryEnum)
            {
                case Lifter.eCategory.MenEquipped:
                    A = MenEquippedA;
                    B = MenEquippedB;
                    C = MenEquippedC;
                    break;
                case Lifter.eCategory.MenClassic:
                    A = MenClassicA;
                    B = MenClassicB;
                    C = MenClassicC;
                    break;
                case Lifter.eCategory.MenEquippedBench:
                    A = MenEquippedBenchA;
                    B = MenEquippedBenchB;
                    C = MenEquippedBenchC;
                    break;
                case Lifter.eCategory.MenClassicBench:
                    A = MenClassicBenchA;
                    B = MenClassicBenchB;
                    C = MenClassicBenchC;
                    break;
                case Lifter.eCategory.WomenEquipped:
                    A = WomenEquippedA;
                    B = WomenEquippedB;
                    C = WomenEquippedC;
                    break;
                case Lifter.eCategory.WomenClassic:
                    A = WomenClassicA;
                    B = WomenClassicB;
                    C = WomenClassicC;
                    break;
                case Lifter.eCategory.WomenEquippedBench:
                    A = WomenEquippedBenchA;
                    B = WomenEquippedBenchB;
                    C = WomenEquippedBenchC;
                    break;
                case Lifter.eCategory.WomenClassicBench:
                    A = WomenClassicBenchA;
                    B = WomenClassicBenchB;
                    C = WomenClassicBenchC;
                    break;
                default:
                    break;
            }
            GLPointsCoeff = 100 / (A - B * Math.Pow(Math.E, -C * lifter.bodyWeight));
            GLPoints = lifter.total * GLPointsCoeff;

            return GLPoints;
        }

        bool active = false;
        private void btn_rekord_Click(object sender, EventArgs e)
        {
            //TODO 
            //fixa en klocka så bilderna kan blinka
            active = !active;
            if (active)
            {
                btn_rekord.Text = "Deaktivera rekord";

                if (cb_squat.Checked && !cb_bench.Checked && !cb_deadlift.Checked && !cb_total.Checked)
                {
                    //visa "Böj"
                }
                else if (!cb_squat.Checked && cb_bench.Checked && !cb_deadlift.Checked && !cb_total.Checked)
                {
                    //visa "Bänkpress"
                }
                else if (!cb_squat.Checked && !cb_bench.Checked && cb_deadlift.Checked && !cb_total.Checked)
                {
                    //visa "Marklyft"
                }
                else if (!cb_squat.Checked && !cb_bench.Checked && !cb_deadlift.Checked && cb_total.Checked)
                {
                    //visa "Total"
                }
                else if (!cb_squat.Checked && !cb_bench.Checked && cb_deadlift.Checked && cb_total.Checked)
                {
                    //visa "Marklyft & total"
                }
                else
                {
                    //visa "kontrollera att du har klickat i rätt rekord"
                }
            }
            else
            {
                btn_rekord.Text = "Aktivera rekord";
            }
            if (rb_club.Checked)
            {
                //visa "kubb rekord"
            }
            else if (rb_district.Checked)
            {
                //visa "distrikt rekord"
            }
            else if (rb_national.Checked)
            {
                //visa "nationellt rekord"
            }

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void lbl_Placement2_Click(object sender, EventArgs e)
        {

        }




        //Tävling
        //Tävling
        //Tävling
        //Tävling
        //Tävling




        //Resultat
        //Resultat
        //Resultat
        //Resultat
        //Resultat


        //Resultat
        //Resultat
        //Resultat
        //Resultat
        //Resultat
    }
}
