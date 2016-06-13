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
        HeadHunter hh = null;
        const string name = "HeadHunter";
        private ContextMenu m_menu;
        string fullPath;
        string path;
        string fileName = "ResumeUpdate.txt";
        public Form1()
        {
            InitializeComponent();
            hh = new HeadHunter();
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
                hh.Authentication(login, password);
            }

        }
        protected void Exit_Click(Object sender, System.EventArgs e)
        {
            Close();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            hh.UpdateAll();
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
                    hh.Authentication(login, password);
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
