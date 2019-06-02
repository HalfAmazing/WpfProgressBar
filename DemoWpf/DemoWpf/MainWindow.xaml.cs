using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;

namespace DemoWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker _BgwUpload;
         
        public MainWindow()
        {
            InitializeComponent();

            _BgwUpload = new BackgroundWorker();
            _BgwUpload.WorkerReportsProgress = true;
            _BgwUpload.WorkerSupportsCancellation = true;
            _BgwUpload.DoWork += DoWork_Handler;
            _BgwUpload.ProgressChanged += ProgressChanged_Handler;
            _BgwUpload.RunWorkerCompleted += RunWorkerCompleted_Handler;
        }

        private void DoWork_Handler(object sender, DoWorkEventArgs args)
        {
            args.Result = Download(sender, args);
        }

        private static string Download(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BackgroundWorker w = sender as BackgroundWorker;
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(10);
                w.ReportProgress(i + 1);
                if (w.CancellationPending)
                {
                    e.Cancel = true; return "";
                }
            }
            return "上传完成，耗时：" + sw.ElapsedMilliseconds / 1000 + " 秒";
        }

        private void ProgressChanged_Handler(object sender, ProgressChangedEventArgs args)
        {
            pgbUpload.Value = args.ProgressPercentage;
            lbResult.Content = "文件上传中，请稍等...";
        }

        private void RunWorkerCompleted_Handler(object sender, RunWorkerCompletedEventArgs args)
        {
            Action<string> act = delegate (string str) { lbResult.Content = str; };
            if (args.Cancelled)
            {
                this.Dispatcher.BeginInvoke(act, "取消文件上传");
            }
            else
            {
                this.Dispatcher.BeginInvoke(act, args.Result.ToString());
                btStart.IsEnabled = true;
                btCancle.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbResult.Content = "请上传文件";
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            btStart.IsEnabled = false;
            btCancle.IsEnabled = true;
            if (!_BgwUpload.IsBusy)
            {
                _BgwUpload.RunWorkerAsync();
            }
        }

        private void btCancle_Click(object sender, RoutedEventArgs e)
        {
            btStart.IsEnabled = true;
            btCancle.IsEnabled = false;
            _BgwUpload.CancelAsync();
            lbResult.Content = "请上传文件";
            pgbUpload.Value = 0;
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            //btStart.IsEnabled = true;
            //_BgwUpload.CancelAsync();
            //lbResult.Content = "请上传文件";
        }
    }
}
