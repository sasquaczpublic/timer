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
using System.IO;
using System.Windows.Threading;

namespace Czasomierz_projektu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Start, Pause;
        string Path;
        int ProjectTime, SessionTime, PauseTime, SessionCount, PauseAllTime;
        DispatcherTimer _timer, _pauser;
        TimeSpan _projectTime, _sessionTime, _pauseTime;
        
        
        public MainWindow()
        {            
            InitializeComponent();
            Path = @"Project.time";
            
            if (File.Exists(Path))
            {
                using (StreamReader sr = new StreamReader(Path))
                {
                    ProjectTime = Convert.ToInt32(sr.ReadLine());
                    SessionCount = Convert.ToInt32(sr.ReadLine());
                    PauseAllTime = Convert.ToInt32(sr.ReadLine());
                    SessionTime = PauseTime = 0;
                }                
            }
            else
            {
                File.Create(Path);
                ProjectTime = SessionTime = PauseTime = 0;
            }
            _projectTime = TimeSpan.FromSeconds(ProjectTime);
            _sessionTime = TimeSpan.FromSeconds(SessionTime);
            txtSessionCount.Text = ++SessionCount + ":";
            SessionTimer();
            PauseTimer();
            Start = Pause = false;
        }
        
        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if(!Start)
            {
                Start = true;
                SessionTimer();
                btPause.IsEnabled = true;
                btStart.Content = "SKOŃCZ SESJĘ";
            }
            else
            {
                Start = false;
                _timer.Stop();
                using (StreamWriter sw = new StreamWriter(Path))
                {
                    sw.WriteLine(Convert.ToString(ProjectTime));
                    sw.WriteLine(Convert.ToString(SessionCount));
                    sw.WriteLine(Convert.ToString(PauseTime+PauseAllTime));

                }
                Application.Current.Shutdown();
            }
        }

        private void SessionTimer()
        {
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                txtProjectTime.Text = _projectTime.ToString("c");
                txtSessionTime.Text = _sessionTime.ToString("c");
                if (_sessionTime == TimeSpan.Zero) _timer.Stop();
                _projectTime = _projectTime.Add(TimeSpan.FromSeconds(1));
                _sessionTime = _sessionTime.Add(TimeSpan.FromSeconds(1));
                SessionTime++;
                ProjectTime++;
            }, Application.Current.Dispatcher);
            _timer.Start();
        }
               
            
          private void PauseTimer()
        {
            _pauser = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                txtPauseTime.Text = _pauseTime.ToString("c");
                if (_pauseTime == TimeSpan.Zero) _pauser.Stop();
                _pauseTime = _pauseTime.Add(TimeSpan.FromSeconds(1));
                PauseTime++;
            }, Application.Current.Dispatcher);
            _pauser.Start();
        }    
        
        private void btPause_Click(object sender, RoutedEventArgs e)
        {
            if (!Pause)
            {
                Pause = true;
                _timer.Stop();
                PauseTimer();
                btPause.Content = "WZNÓW";
            }
            else
            {
                Pause = false;
                _pauser.Stop();
                SessionTimer();
                btPause.Content = "PRZERWA";
            }
        }
    }
}
