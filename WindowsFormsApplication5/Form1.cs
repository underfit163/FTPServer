using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    public struct FileStruct
    {
       public string Flags;
       public string Owner;
       public string CreateTime;
       public string Name;
       public Int64 Size;
       public string Directory;
       public Int64 SizeAll;
    }

    public partial class Form1 : Form
    {
        private FtpClient _client;   
        private DataTable Files;
        private DataTable Types;
        private List<string> fileList;
        private List<FileStruct> myList;
        public Form1()
        {
            InitializeComponent();
            Files = new DataTable();
            Types = new DataTable();
            fileList = new List<string>();       
        }
        protected static Regex m_FtpListingRegex = new Regex(@"^([d-])((?:[rwxt-]{3}){3})\s+(\d{1,})\s+(\w+)?\s+(\w+)?\s+(\d{1,})\s+(\w+)\s+(\d{1,2})\s+(\d{4})?(\d{1,2}:\d{2})?\s+(.+?)\s?$",
RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

       // protected static readonly String Timeformat = "MMM dd yyyy HH:mm";
        public void AllFiles()
        {
            FileStruct f = new FileStruct();
            Types = new DataTable();
            string[] Lines = _client.Log.Split('\n');
            myList = new List<FileStruct>();
            List<string> derictoriesList = new List<string>();
            fileList = new List<string>();
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].StartsWith("ftp"))
                {
                    derictoriesList.Add(Lines[i]);
                    listBox3.Items.Add(Lines[i]);
                }

                else if (!Lines[i].StartsWith("d"))
                {
                   fileList.Add(Lines[i]);
                    listBox4.Items.Add(Lines[i]);
                }
            }
            int g = 1;
            for (var j = 0; j < derictoriesList.Count - 1; j++)
            {
                while (Lines[g] != derictoriesList[j + 1] && g < Lines.Length)
                {
                    if (!Lines[g].StartsWith("d"))
                    {
                        MatchCollection matches = m_FtpListingRegex.Matches(Lines[g]); 
                        foreach (Match match in matches)
                        {
                          
                            f.Flags = match.Groups[2].Value;
                            f.Directory = derictoriesList[j];
                            f.Owner = match.Groups[4].Success ? match.Groups[4].Value : null;
                            Int64 fileSize;
                            Int64.TryParse(match.Groups[6].Value, out fileSize);
                            f.Size = fileSize;
                            f.SizeAll += f.Size;
                            string month = match.Groups[7].Value;
                            String day = match.Groups[8].Value.PadLeft(2, '0');
                            String year = match.Groups[9].Success ? match.Groups[9].Value : DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                            String timeString = month + " " + day + " " + year;
                            f.CreateTime = timeString;
                            f.Name = match.Groups[11].Value;
                            myList.Add(f);
                        }
                    }
                    g++;
                }
            }
            label8.Text = f.SizeAll.ToString()+" Байт";
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            _client = new FtpClient(loginTextBox.Text, passwordTextBox.Text, server_adressTextBox.Text);
            _client.ListAllDirectories();
            AllFiles();
            DataTable table2 = Types.Copy();
            List<string> types = new List<string>();
            for (int i = 0; i < myList.Count; i++)
            {
                try
                {
                    String temp = myList[i].Name;
                    FileInfo file = new FileInfo(temp);
                    bool contains = false;
                    foreach (string type in types)
                    {
                        if (file.Extension == type)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        types.Add(file.Extension);
                    }
                }
                catch
                {
                }
            }
            Int64[] sizes = new Int64[types.Count];
            for (int i = 0; i < sizes.Length; i++)
            {
                try
                {
                    for (int j = 0; j < myList.Count; j++)
                    {
                        FileInfo file = new FileInfo(myList[j].Name);
                        if (file.Extension == types[i])

                        {
                            sizes[i] += myList[j].Size;
                        }
                    }
                }
                catch { }
                listBox13.Items.Add(types[i] + " = " + sizes[i] + " байт"); 
                
            }
            
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

