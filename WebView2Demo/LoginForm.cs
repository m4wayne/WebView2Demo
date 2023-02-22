#define FIXED_RUNTIME

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics;

namespace WebView2Demo
{
    public partial class LoginForm : Form
    {
        WebView2 webView2 = new WebView2();

        public LoginForm()
        {
            InitializeAsync();
            InitializeComponent();
        }

        private async void InitializeAsync()
        {
            string userDataDir = $"{Application.StartupPath}cache";
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions();
            //options.AdditionalBrowserArguments = "--no-proxy-server";

#if FIXED_RUNTIME
            string runtimeDir = @$"{Application.StartupPath}\runtimes\win-x64\fixed.109.0.1518.78.x64";
            Debug.Assert(!string.IsNullOrEmpty(runtimeDir), "You need to set the WebView2 Runtime fixed version path.");
            Debug.Assert(Directory.Exists(runtimeDir), "WebView2 Runtime fixed version path not exists.");

            var env = await CoreWebView2Environment.CreateAsync(browserExecutableFolder: runtimeDir, userDataFolder: userDataDir, options: options);
#else
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataDir, options: options);
#endif
            await webView2.EnsureCoreWebView2Async(env);
            Debug.WriteLine($"--> WebView2 runtime version:{webView2.CoreWebView2.Environment.BrowserVersionString}");
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            webView2.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
            //WebSimulator project applicationUrl value.
            webView2.Source = new Uri("https://localhost:7018");
            webView2.Dock = DockStyle.Fill;
            this.Controls.Add(webView2);
            webView2.BringToFront();
        }

        private void WebView2_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            webView2.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
        }

        private async void CoreWebView2_WebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            Uri uri = new Uri(e.Request.Uri);
            string path = "/Home";

            if (uri.LocalPath.StartsWith(path, StringComparison.OrdinalIgnoreCase)
                && e.Response.StatusCode == 200)
            {
                string rawText = String.Empty;
                try
                {
                    //Exception position. use runtime for 110.0.1587.x
                    Stream content = await e.Response.GetContentAsync();
                    byte[] data = new byte[content.Length];
                    content.Read(data, 0, data.Length);
                    rawText = Encoding.UTF8.GetString(data);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"-->{ex.Message}");
                    Debug.WriteLine($"-->{ex.StackTrace}");
                    return;
                }

                if (!string.IsNullOrEmpty(rawText))
                {
                    Match nameMach = Regex.Match(rawText, @"Hello,\s*.*(?=<)");
                    if (nameMach.Success)
                    {
                        Debug.WriteLine("--> Login success.");
                    }
                    else
                    {
                        Debug.WriteLine("--> Login fail.");
                    }
                }
            }
        }
    }
}