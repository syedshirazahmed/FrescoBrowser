using CefSharp;
using CefSharp.WinForms;
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
            SetRegistryValuesToWorkForIE11();
            Cef.EnableHighDPISupport();
            InitBrowser();
            
        }

        private void Navigate()
        {
            String url = txtUrl.Text;
            browser.Load(url);
        }

        //private void InjectJavaScript()
        //{
        //    HtmlElement head = Browser.Document.GetElementsByTagName("head")[0];
        //    HtmlElement script = Browser.Document.CreateElement("script");
        //    IHTMLScriptElement element = (IHTMLScriptElement)script.DomElement;
        //    element.text = "function hello() {alert(\"Yeah boy you did it\")}";
        //    head.AppendChild(script);
        //    Browser.Document.InvokeScript("hello");
        //}

        public void InjectJavaScript()
        {
            browser.ExecuteScriptAsyncWhenPageLoaded("var fileContents; ");
            browser.ExecuteScriptAsyncWhenPageLoaded("function startRead(){            var file = document.getElementById('file').files[0];                if (file)                {                    getAsText(file);                }            }            ");
            browser.ExecuteScriptAsyncWhenPageLoaded("function getAsText(readFile){                var reader;                try                {                    reader = new FileReader();                }                catch (e)                {                    alert(\"Error: seems File API is not supported on your browser\");                    return;                }               reader.readAsText(readFile, \"UTF-8\");             reader.onprogress = updateProgress;                reader.onload = loaded;                reader.onerror = errorHandler;            }            ");
            browser.ExecuteScriptAsyncWhenPageLoaded("function updateProgress(evt){                if (evt.lengthComputable)                {               var loaded = (evt.loaded / evt.total);                    if (loaded < 1)                    {                                  }                }            }            ");
            browser.ExecuteScriptAsyncWhenPageLoaded("function loaded(evt){            var fileString = evt.target.result;                fileContents = fileString;                alert(\"File loaded in Memory Successfully\");            }            ");
            browser.ExecuteScriptAsyncWhenPageLoaded("function errorHandler(evt){                if (evt.target.error.code == evt.target.error.NOT_READABLE_ERR)                {                    alert(\"Error reading file...\");                }            }            ");
            browser.ExecuteScriptAsyncWhenPageLoaded("function getTheAnswer(){                try                {                    var ques = document.getElementsByClassName(\"question\");                    var textInQues = (ques[0].getElementsByTagName(\"p\"))[0];                    var isFound = fileContents.search(textInQues.innerHTML.substring(0, 26));                    if (isFound == -1)                    {                        alert(\"Sorry! This question is not available in the document.\");                    }                    else                    {                        alert(fileContents.substring(isFound, isFound + 200));                    }                }                catch (e)                {                    alert(\"Somthing went wrong\");                }            }            ");
            browser.ExecuteScriptAsyncWhenPageLoaded("function uploadDumpsTxtFile(){                var x = document.createElement(\"INPUT\");                x.setAttribute(\"id\", \"file\");                x.setAttribute(\"type\", \"file\");                x.setAttribute(\"onchange\", \"startRead()\");                x.setAttribute(\"hidden\", \"true\");                document.body.appendChild(x);                x.click();            }            ");
            browser.ExecuteScriptAsyncWhenPageLoaded("uploadDumpsTxtFile();");
            browser.ExecuteScriptAsyncWhenPageLoaded("function yo() {alert(\"Hello\")}");
            browser.ExecuteScriptAsyncWhenPageLoaded("yo();");
        }

        private void btnNavigate_Click(object sender, EventArgs e)
        {
             Navigate();
        }

        private void btn_InjectJS_Click(object sender, EventArgs e)
        {
            InjectJavaScript();
        }

        public ChromiumWebBrowser browser;
        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("www.google.com");
            panel3.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
      
        }

        public void SetRegistryValuesToWorkForIE11()
        {
            try
            {
                int versionCode = 7000;
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
    }
}
