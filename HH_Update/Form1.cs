using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;

namespace HH_Update
{
    public partial class Form1 : Form
    {
        HeadHunter hh;
        const string name = "HeadHunter";
        private ContextMenu m_menu; 
        public Form1()
        {
            InitializeComponent();
            m_menu = new ContextMenu();
            m_menu.MenuItems.Add(new MenuItem("Exit", new System.EventHandler(Exit_Click)));
            notifyIcon1.ContextMenu = m_menu;   
            timer1.Interval = (int)new TimeSpan(0,1,0).TotalMilliseconds;
            SetAutorunValue(true);
            notifyIcon1.Icon = Properties.Resources.hh;
            notifyIcon1.Visible = true;
            hh = new HeadHunter();
            Auth();
        }
        public void Auth()
        {
            try
            {
                hh.Authentication("Login","Pass");
            }
            catch (Exception)
            {
                Thread.Sleep(30000);
                Auth();
            }
        }
        protected void Exit_Click(Object sender, System.EventArgs e)
        {
            Close();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                await hh.UpdateAllAsync();
            }
            catch(Exception)
            {

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
            Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Hide();
        }
    }
}
