using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Genshin_Stella_Mod.Forms.Other;
using Genshin_Stella_Mod.Properties;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod.Forms
{
    public partial class Tools : Form
    {
        private bool _mouseDown;
        private Point _offset;

        public Tools()
        {
            InitializeComponent();
        }

        private void Tools_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            int randomInt = random.Next(1, 50 + 1);
            if (randomInt == 25)
            {
                string pathStr = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", "forms", "tools", "2.png");
                if (!Utils.CheckFileExists(pathStr))
                {
                    Log.SaveErrorLog(new Exception($"Special background file was not found in '{pathStr}' for Tools."));
                    return;
                }

                string pathCombine = Path.Combine(pathStr);
                BackgroundImage = new Bitmap(pathCombine);
                panel3.Visible = false;
            }


            Version.Text = $@"v{Program.AppVersion}";

            MusicLabel_Set();
            RPCLabel_Set();
        }

        private void Utils_Shown(object sender, EventArgs e)
        {
            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 1)
            {
                Discord.Presence.Details = "Browsing tools 🔧";
                Discord.Client.SetPresence(Discord.Presence);
            }

            Log.Output($"Loaded form '{Text}'.");
        }

        private void MouseDown_Event(object sender, MouseEventArgs e)
        {
            _offset.X = e.X;
            _offset.Y = e.Y;
            _mouseDown = true;
        }

        private void MouseMove_Event(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;
            Point currentScreenPos = PointToScreen(e.Location);
            Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
        }

        private void MouseUp_Event(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
            Close();

            Discord.Home();
        }


        // -------------------------------- Launcher --------------------------------
        private void CreateShortcut_Button(object sender, EventArgs e)
        {
            bool success = Utils.CreateShortcut();
            if (success) MessageBox.Show(@"The shortcut has been successfully created.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MuteMusic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int data = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);
            switch (data)
            {
                case 0:
                    Program.Settings.WriteInt("Launcher", "EnableMusic", 1);
                    break;
                case 1:
                    Program.Settings.WriteInt("Launcher", "EnableMusic", 0);
                    break;
            }

            MusicLabel_Set();
        }

        private void MusicLabel_Set()
        {
            int data = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Tools));

            switch (data)
            {
                case 0:
                    MuteMusicOnStart.Text = resources.GetString("UnmuteMusicOnStart.Text");
                    break;
                case 1:
                    MuteMusicOnStart.Text = resources.GetString("MuteMusicOnStart.Text");
                    break;
                default:
                    Log.SaveErrorLog(new Exception("Wrong EnableMusic value."));
                    MessageBox.Show("Wrong EnableMusic value.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void DisableRPC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int iniData = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            switch (iniData)
            {
                case 0:
                    Program.Settings.WriteInt("Launcher", "DiscordRPC", 1);
                    Discord.InitRpc();
                    if (Discord.Username.Length > 0) // Fix
                        MessageBox.Show($"You're connected as {Discord.Username}. Hello!", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case 1:
                    Program.Settings.WriteInt("Launcher", "DiscordRPC", 0);
                    Discord.Client.Dispose();
                    break;
            }

            RPCLabel_Set();
        }

        private void RPCLabel_Set()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Tools));

            int iniData = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            switch (iniData)
            {
                case 0:
                    DisableDiscordRPC.Text = resources.GetString("EnableDiscordRPC.Text");
                    break;
                case 1:
                    DisableDiscordRPC.Text = resources.GetString("DisableDiscordRPC.Text");
                    break;
                default:
                    Log.SaveErrorLog(new Exception("Wrong DiscordRPC value."));
                    MessageBox.Show("Wrong DiscordRPC value.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }


        // ---------------------------------- Misc ----------------------------------
        private void ScanSysFiles_Click(object sender, EventArgs e)
        {
            Cmd.Execute("wt.exe", Path.Combine(Program.AppPath, "data", "cmd", "scan_sys_files.cmd"), Program.AppPath, true, false);
        }


        // ------------------------------ Config files ------------------------------
        private async void ReShadeConfig_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Program.ReShadePath))
                MessageBox.Show($@"ReShade config file was not found in {Program.ReShadePath}.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                await Cmd.CliWrap("notepad", Program.ReShadePath, null, true, false);
        }

        private async void UnlockerConfig_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("notepad", Path.Combine(Program.AppPath, "data", "unlocker", "unlocker.config.json"), null, true, false);
        }


        // --------------------------------- Cache ---------------------------------
        private void DeleteCache_Button(object sender, EventArgs e)
        {
            string resources = File.ReadAllText(Path.Combine(Program.AppData, "resources-path.sfn"));
            string cache = Path.Combine(resources, "Cache");
            string webViewCache = Path.Combine(Program.AppData, "EBWebView");
            string reShadeLog = Path.Combine(Utils.GetGame("giGameDir"), "ReShade.log");
            string logs = Path.Combine(Log.Folder);

            Cmd.Execute(
                "wt.exe",
                $"{Path.Combine(Program.AppPath, "data", "cmd", "delete_cache.cmd")} \"{Path.Combine(Program.AppData, "game-path.sfn")}\" \"{cache}\" \"{webViewCache}\" \"{reShadeLog}\" \"{logs}\"",
                Program.AppPath, true, false);
        }

        private void DeleteWebViewCache_Click(object sender, EventArgs e)
        {
            Cmd.Execute("wt.exe", Path.Combine(Program.AppPath, "data", "cmd", "delete_webview_cache.cmd"), Program.AppPath, true, false);
        }


        // ---------------------------------- Logs ---------------------------------
        private async void LauncherLogs_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("notepad", $@"{Log.Folder}\launcher.output.log", null, true, false);
        }

        private async void InnoSetup_Button(object sender, EventArgs e)
        {
            await Cmd.CliWrap("notepad", $@"{Log.Folder}\innosetup-logs.install.log", null, true, false);
        }

        private async void ReShadeLogs_Button(object sender, EventArgs e)
        {
            string gameDir = Utils.GetGame("giGameDir");
            string logFile = $@"{gameDir}\ReShade.log";

            if (!Directory.Exists(gameDir) || !File.Exists(logFile))
                MessageBox.Show($"ReShade log file was not found in:\n{logFile}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                await Cmd.CliWrap("notepad", logFile, null, true, false);
        }

        private async void InstallationLog_Button(object sender, EventArgs e)
        {
            await Cmd.CliWrap("notepad", $@"{Log.Folder}\setup.output.log", null, true, false);
        }


        // -------------------------- Nothing special ((: ---------------------------
        private void Notepad_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start($@"{Program.AppPath}\data\videos\poland-strong.mp4");
        }

        private void ChangeLang_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Language { Icon = Resources.icon_52x52 }.ShowDialog();
        }
    }
}