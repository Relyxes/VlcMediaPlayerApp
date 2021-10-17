using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Forms;

namespace VlcMediaPlayerApp
{
    public partial class MainForm : Form
    {
        FileInfo _filePath;

        public MainForm()
        {
            InitializeComponent();
            textBoxUrl.Text = $@"rtsp://guest:guest@demo.domination.one:7005/camera=2&stream=0";
            vlcControl.VlcMediaPlayer.Dialogs.UseDialogManager(new MyDialogManager());
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            string uri = textBoxUrl.Text;
            string[] options =
            {
                ":network-caching=2000"
            };
            vlcControl.Play(new Uri(uri));

            //if (!vlcControl.VlcMediaPlayer.CouldPlay && uri == string.Empty)
            //    OpenBtn_Click(this, new EventArgs());


            //if (vlcControl.IsPlaying)
            //{
            //    vlcControl.Pause();
            //    buttonPlay.Text = "Play";
            //}
            //else
            //{
            //    if (uri == string.Empty && _filePath != null)
            //        vlcControl.Play(_filePath);
            //    else
            //        vlcControl.Play(uri);

            //    buttonPlay.Text = "Pause";
        
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            vlcControl.Stop();
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    _filePath = new FileInfo(ofd.FileName);
            }
            if (_filePath != null)
                vlcControl.VlcMediaPlayer.Play(_filePath);
        }

        /// <summary>
        /// Get library path to vlcmediaplayer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vlcControl_VlcLibDirectoryNeeded(object sender, VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (currentDirectory == null)
                return;
            if (IntPtr.Size == 4)
                e.VlcLibDirectory = new DirectoryInfo(Path.GetFullPath(@".\libvlc\win-x86\"));
            else
                e.VlcLibDirectory = new DirectoryInfo(Path.GetFullPath(@".\libvlc\win-x64\"));

            if (!e.VlcLibDirectory.Exists)
            {
                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = "Select path to vlclib.dll";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    e.VlcLibDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                }
                else
                {
                    MessageBox.Show("App can't be runned. Don't minimal resources - vlclib.dll.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void vlcControl_Stopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
            buttonPlay.BeginInvoke((MethodInvoker)(() => buttonPlay.Text = "Play"));            
        }

        public class MyDialogManager : IVlcDialogManager
        {
            public MyDialogManager()
            { }

            public async Task DisplayErrorAsync(IntPtr userdata, string title, string text)
            {
                MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            public async Task<LoginResult> DisplayLoginAsync(IntPtr userdata, IntPtr dialogId, string title, string text, string username, bool askstore, CancellationToken cancellationToken)
            {
                var loginResult = new LoginResult
                {
                    Username = "guest",
                    Password = "guest",
                    StoreCredentials = false
                };
                return loginResult;
            }

            public Task DisplayProgressAsync(IntPtr userdata, IntPtr dialogId, string title, string text, bool indeterminate, float position, string cancelButton, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task<QuestionAction?> DisplayQuestionAsync(IntPtr userdata, IntPtr dialogId, string title, string text, DialogQuestionType questionType, string cancelButton, string action1Button, string action2Button, CancellationToken cancellationToken)
            {
                return Task.FromResult<QuestionAction?>(null);
            }

            public void UpdateProgress(IntPtr userdata, IntPtr dialogId, float position, string text)
            {
            }
        }
    }
}
