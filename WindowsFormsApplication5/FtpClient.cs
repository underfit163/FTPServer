using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    public class FtpClient
    {
        private string _UserName;
        private string _Password;
        private string _Host;
        private readonly StringBuilder _log;
        public string Log { get { return _log.ToString(); } }
        public TreeNode Directories { get; set; }
        public FtpClient(string userName, string password, string host)
        {
            _UserName = userName;
            _Password = password;
            _Host = host;
            _log = new StringBuilder();
        }
        
        public List<string> ListDirectory(string path)
        {
            List<string> directories = new List<string>();
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + _Host + "/" + path); 
            ftpRequest.Credentials = new NetworkCredential(_UserName, _Password);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails; 
            FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse(); 
            Stream responseStream = ftpResponse.GetResponseStream(); 
            if (responseStream != null)
            {
                _log.Append("ftp://" + _Host + "/" + path + Environment.NewLine); 
                StreamReader readStream = new StreamReader(responseStream, Encoding.ASCII);
                string content;
                while ((content = readStream.ReadLine()) != null) 
                {
                    _log.Append(content + Environment.NewLine);
                    string[] words = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string extension = words.Last();
                    if (words[0].StartsWith("d"))
                    {
                        directories.Add(extension);
                    }
                }
                responseStream.Close();
                readStream.Close();
                ftpResponse.Close();
            }
            return directories;
        }

        public void ListAllDirectories()
        {
            Directories = new TreeNode { Text = _Host, ImageIndex = 0, SelectedImageIndex = 0 };
            List<string> directoryList = ListDirectory(""); 
            for (int i = 0; i < directoryList.Count; i++)
            {
                Directories.Nodes.Add(List(directoryList[i] + "/"/*, directoryList[i]*/)); 
            }
        }

        private TreeNode List(string dir/*, string name*/)
        {
            TreeNode currentDir = new TreeNode(dir);
            List<string> directoryList = ListDirectory(dir);
            for (int i = 0; i < directoryList.Count; i++)
            {
                currentDir.Nodes.Add(List(dir + directoryList[i] + "/"/*, directoryList[i]*/));
            }
            return currentDir;
        }
    }
}