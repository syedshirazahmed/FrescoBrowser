using MSHTML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fresco
{
    public partial class TheSBrowser : Form
    {
        public TheSBrowser()
        {
            InitializeComponent();
            Browser.ObjectForScripting = true;
            try
            {
                int versionCode=7000;
                String version = "";
                String appName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                object ieVersion = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Internet Explorer").GetValue("svcUpdateVersion");
                if (ieVersion == null)
                    ieVersion = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Internet Explorer").GetValue("Version");
                else
                {
                    version = ieVersion.ToString().Substring(0, ieVersion.ToString().IndexOf("."));
                    Console.WriteLine("Current Version of IE : " + version);
                    switch (version)
                    {
                        case "7":
                            versionCode = 7000;
                            break;
                        case "8":
                            versionCode = 8888;
                            break;
                        case "9":
                            versionCode = 9999;
                            break;
                        case "10":
                            versionCode = 10001;
                            break;
                        default:
                            if (int.Parse(version) >= 11)
                                versionCode = 11001;
                            else
                                throw new Exception("IE Version Not Supported");
                            break;
                    }
                }
                Console.WriteLine("Set Version Code : " + versionCode);
                String root = "HKEY_CURRENT_USER\\";
                String key = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
                object currentSetting = (Microsoft.Win32.Registry.CurrentUser.OpenSubKey(key).GetValue(appName + ".exe"));
                if ((currentSetting == null) || ((int.Parse(currentSetting.ToString())) != versionCode))
                {
                    Microsoft.Win32.Registry.SetValue(root + key, appName + ".exe", versionCode);
                    Microsoft.Win32.Registry.SetValue(root + key, appName + ".vshost.exe", versionCode);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void Navigate()
        {
            String url = txtUrl.Text;
            Browser.Navigate(url);
        }

        private void InjectJavaScript()
        {
            HtmlElement head = Browser.Document.GetElementsByTagName("head")[0];
            HtmlElement script = Browser.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)script.DomElement;
            element.text = "function hello() {alert(\"Yeah boy you did it\")}";
            head.AppendChild(script);
            Browser.Document.InvokeScript("hello");
        }

        private void btnNavigate_Click(object sender, EventArgs e)
        {
            Navigate();
        }

        private void btn_InjectJS_Click(object sender, EventArgs e)
        {
            InjectJavaScript();
        }
    }
}
