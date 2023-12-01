using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class Settings : UserControl
    {
        private ServerComponent serverComponent = null;

        public ushort Port { get; set; }

        public string LocalIP { get; set; }

        public bool AutoStartServer { get; set; }

        public string GetIP()
        {
            IPAddress[] ipv4Addresses = Array.FindAll(
                Dns.GetHostEntry(string.Empty).AddressList,
                a => a.AddressFamily == AddressFamily.InterNetwork);
            
            return String.Join(",", Array.ConvertAll(ipv4Addresses, x => x.ToString()));
        }

        public string PortString
        {
            get { return Port.ToString(); }
            set { Port = ushort.Parse(value); }
        }

        public Settings(ServerComponent serverComponent)
        {
            InitializeComponent();

            this.serverComponent = serverComponent;

            Port = 16834;

            LocalIP = GetIP();
            label3.Text = LocalIP;

            txtPort.DataBindings.Add("Text", this, "PortString", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Port", PortString) ^
                SettingsHelper.CreateSetting(document, parent, "AutoStartServer", AutoStartServer);
        }

        public void SetSettings(XmlNode settings)
        {
            PortString = SettingsHelper.ParseString(settings["Port"]);

            AutoStartServer = SettingsHelper.ParseBool(settings["AutoStartServer"]);
            checkBoxAutoStart.Checked = AutoStartServer;

            if (AutoStartServer && serverComponent != null && serverComponent.Server == null)
                serverComponent.Start();
        }

        private void checkBoxAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartServer = checkBoxAutoStart.Checked;
        }
    }
}
