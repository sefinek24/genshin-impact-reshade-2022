using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace StellaLauncher.Forms.Other
{
    public partial class WebView2Shake : Form
    {
        private static Random _random;
        private static Timer _shakeTimer;
        private static Timer _stopShakeTimer;

        public WebView2Shake()
        {
            InitializeComponent();
        }

        private void WebViewWindow_Load(object sender, EventArgs e)
        {
            _shakeTimer = new Timer { Interval = 99 };
            _shakeTimer.Tick += ShakeTimer_Tick;

            _stopShakeTimer = new Timer { Interval = 10000 };
            _stopShakeTimer.Tick += StopShakeTimer_Tick;

            _random = new Random();
        }

        private async void WebView2(string webView)
        {
            try
            {
                CoreWebView2Environment coreWebView2Env = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
                await webView21.EnsureCoreWebView2Async(coreWebView2Env);

                webView21.CoreWebView2.Navigate(webView);
            }
            catch (Exception ex)
            {
                Scripts.Forms.WebView2.HandleError(ex);
            }
        }

        public void Navigate(string url)
        {
            WebView2(url);
        }

        private static void StartShaking()
        {
            _shakeTimer.Start();
            _stopShakeTimer.Start();
        }

        private void ShakeTimer_Tick(object sender, EventArgs e)
        {
            int deltaX = _random.Next(-6, 7);
            int deltaY = _random.Next(-6, 7);
            Left += deltaX;
            Top += deltaY;
        }

        private void StopShakeTimer_Tick(object sender, EventArgs e)
        {
            _shakeTimer.Stop();
            _stopShakeTimer.Stop();
        }

        private void WebViewWindow_Shown(object sender, EventArgs e)
        {
            StartShaking();
        }
    }
}