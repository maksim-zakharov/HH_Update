using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.Collections.Generic;

namespace HH_Update
{
    public partial class Form1 : Form
    {
        const string name = "HeadHunter";
        private ContextMenu m_menu;
        List<string> collection;
        string fullPath;
        string path;
        string fileName = "ResumeUpdate.txt";
        public Form1()
        {
            InitializeComponent();
            path = Path.GetTempPath();
            fullPath = path + fileName;
            m_menu = new ContextMenu();
            m_menu.MenuItems.Add(new MenuItem("Exit", new System.EventHandler(Exit_Click)));
            notifyIcon1.ContextMenu = m_menu;   
            timer1.Interval = (int)new TimeSpan(0,1,0).TotalMilliseconds;
            SetAutorunValue(true);
            notifyIcon1.Icon = Properties.Resources.hh;
            notifyIcon1.Visible = true;

            if (File.Exists(fullPath))
            {
                string file = File.ReadAllText(fullPath);
                string login = file.Split(' ')[0];
                string password = file.Split(' ')[1];
                Authentication(login, password);
                collection = new List<string>();
                collection = GetAllId();
            }

        }
        protected void Exit_Click(Object sender, System.EventArgs e)
        {
            Close();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (var resume in collection)
                {
                    try
                    {
                        UpdateResumeById(resume);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch(Exception)
            {

            }
        }
        public void UpdateResumeById(string id)
        {
            string url = "https://hh.ru/applicant/resumes/touch";
            string data = "resume=" + id + "&undirectable=true";
            Post(url, data);
        }
        internal void Post(string url, string data)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookies;
            req.Accept = "*/*";
            req.Method = "POST";
            req.UserAgent = UserAgent;
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Headers.Add("X-Xsrftoken", token);
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //req.Headers.Add("Cookie","hhtoken=jQIfoWxrw5tVoEM3Me3YmaWUTNx9; hhuid=rQB30lemh_RZnFdT5_oyMg--; _ym_uid=1465116651560788141; display=desktop; hhref='https://hh.ru/applicant/negotiations'; _ga=GA1.2.414017207.1465366693; _ym_isad=1; regions=1; remember=0; auth_user=268e7a1cf3fde8ed76af69bc92192326; JSESSIONID=1k15vw5qvtve21w0o81s5yng2c; _xsrf=35277a3bcc2dbbf3603d3db1c2720e8e; _xsrf=35277a3bcc2dbbf3603d3db1c2720e8e; crypted_id=DC314D43D3B9BE3A5B191F6B840B1C82A5471C6F722104504734B32F0D03E9F8; crypted_id=DC314D43D3B9BE3A5B191F6B840B1C82A5471C6F722104504734B32F0D03E9F8; hhrole=applicant; unique_banner_user=1465488755.0590142639887; GMT=3; _ym_visorc_156828=w; __utmt_main=1; __utmt_reg=1; __utma=1.149964434.1465116651.1465474440.1465484620.17; __utmb=1.18.10.1465484620; __utmc=1; __utmz=1.1465206946.6.2.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)");
            byte[] sentData = Encoding.ASCII.GetBytes(data);
            req.ContentLength = sentData.Length;
            using (Stream stream = req.GetRequestStream())
            {
                stream.Write(sentData, 0, sentData.Length);
            }
            using (WebResponse GetResponse = req.GetResponse()) ;
        }
        public List<string> GetAllId()
        {
            List<string> collection = new List<string>();
            string url = "http://hh.ru/applicant/resumes";
            using (StreamReader reader = Get(url))
            {
                {
                    string html = reader.ReadToEnd();
                    while (html.IndexOf("b-resumelist-vacancyname b-marker-link") > -1)
                    {
                        html = html.Remove(0, html.IndexOf("b-resumelist-vacancyname b-marker-link"));
                        html = html.Remove(0, html.IndexOf("resume/")+7);
                        collection.Add((html.Remove(html.IndexOf('"'))));
                    }
                }
            }
            return collection;
        }
        public string UserName;
        public CookieContainer cookies = new CookieContainer();
        public string token;
        private string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.84 Safari/537.36";
        internal StreamReader Get(string url)
        {
            var Get = WebRequest.Create(url) as HttpWebRequest;
            Get.CookieContainer = cookies;
            Get.UserAgent = UserAgent;
            WebResponse GetResponse = Get.GetResponse();
            Stream stream = GetResponse.GetResponseStream();
            return new StreamReader(stream);
        }
        public void Authentication(string email, string pass)
        {
            var GetRequest = WebRequest.Create("http://hh.ru/") as HttpWebRequest;
            GetRequest.CookieContainer = cookies;
            GetRequest.UserAgent = UserAgent;
            using (WebResponse GetResponse = GetRequest.GetResponse())
                token = cookies.GetCookieHeader(new Uri("http://hh.ru/")).Split(';')[6].Split('=')[1];

            var PostRequest = WebRequest.Create("https://hh.ru/account/login") as HttpWebRequest;
            PostRequest.CookieContainer = cookies;
            PostRequest.Method = "POST";
            PostRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] queryArr = Encoding.UTF8.GetBytes("backUrl=https%3A%2F%2Fhh.ru%2F&failUrl=%2Faccount%2Flogin%3Fbackurl%3D%252F%26role%3D&username=" + Uri.EscapeDataString(email) + "&password=" + Uri.EscapeDataString(pass) + "&_xsrf=" + token);
            PostRequest.ContentLength = queryArr.Length;
            using (Stream stream = PostRequest.GetRequestStream())
            {
                stream.Write(queryArr, 0, queryArr.Length);
            }
            using (WebResponse PostResponse = PostRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();
                    string tag = "\"mainmenu_userName\">";
                    html = html.Remove(0, html.IndexOf(tag) + tag.Length);
                    html = html.Remove(0, html.IndexOf("</span>") + 7);
                    UserName = html.Remove(html.IndexOf("</span>"));
                }
            }
        }
        public bool SetAutorunValue(bool autorun)
        {
            string ExePath = System.Windows.Forms.Application.ExecutablePath;
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                if (autorun)
                    reg.SetValue(name, ExePath);
                else
                    reg.DeleteValue(name);

                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (File.Exists(fullPath))
            {
                Hide();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(fullPath))
            {
                this.WindowState = FormWindowState.Minimized;
                Hide();
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (loginBox.Text!="" && passBox.Text!="")
            {
                string login = loginBox.Text;
                string password = passBox.Text;
                try
                {
                    Authentication(login, password);
                    collection = new List<string>();
                    collection = GetAllId();
                    status.Text = "Обновляем резюме";
                    File.WriteAllText(fullPath, login + " " + password);
                    this.Enabled = false;
                    Hide();
                }
                catch(Exception)
                {
                    status.Text = "Ошибка";
                }
            }
        }
    }
}
