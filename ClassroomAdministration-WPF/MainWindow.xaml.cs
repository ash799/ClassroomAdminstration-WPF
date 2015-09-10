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
using System.Windows.Threading;

namespace ClassroomAdministration_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Status { errorId, errorPassword, success };
        private Status status;
        private WindowIndex winindex;
        private DispatcherTimer timer;
        bool timerRunning = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            TextBoxPId.Focus();
        }

        //DragMove
        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //Close Button
        private void CloseBorder_MouseEnter_1(object sender, MouseEventArgs e)
        {
            CloseBorder.Background = new SolidColorBrush(Color.FromArgb(100,255,255,255));
        }
        private void CloseBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseLabel.Margin = new Thickness(4, 0, 0.333, 0);
            CloseBorder.Background = null;
        }
        private void CloseBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseLabel.Margin = new Thickness(6, 0, 0.333, 0);
        }
        private void CloseBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        //Min Button
        private void MinBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            MinBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        }
        private void MinBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MinLabel.Margin = new Thickness(0, 0, 1, 0);
        }
        private void MinBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            MinLabel.Margin = new Thickness(0, 0, 3, 0);
            MinBorder.Background = null;
        }
        private void MinBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MinLabel.Margin = new Thickness(0, 0, 3, 0);
            this.WindowState = WindowState.Minimized;
        }

        //Login Button
        private void LoginBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            LoginBorder.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));
        }
        private void LoginBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            LoginBorder.Background = new SolidColorBrush(Color.FromArgb(153, 0, 0, 255));
            lockImage.Margin = new Thickness(42, 5, 0, 5);
        }
        private void LoginBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {

            lockImage.Margin = new Thickness(44, 5, 0, 5);
        }
        private void Border_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (timerRunning) return;
            lockImage.Margin = new Thickness(44, 5, 0, 5);
            TimerStart();
            login();
        }
        //Enter to Login, too
        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (timerRunning) return;
                lockImage.Margin = new Thickness(44, 5, 0, 5);
                TimerStart();
                login();
            }
        }

        //the timer to control the animation
        private void TimerStart()
        {
            LoginBorder.Visibility = Visibility.Hidden;
            TextBoxPId.Visibility = Visibility.Hidden;
            TextBoxPassword.Visibility = Visibility.Hidden;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 2, 300);
            timer.Tick += new EventHandler(timer_tick);
            Wait1.Begin(ellipse1);
            Wait2.Begin(ellipse2);
            Wait3.Begin(ellipse3);
            Wait4.Begin(ellipse4);
            Wait5.Begin(ellipse5);
            timer.Start();
            timerRunning = true;
        }

        private void timer_tick(object sender, EventArgs e)
        {
            lockImage.Margin = new Thickness(42, 5, 0, 5);

            timer.Stop();
            timerRunning = false;

            Wait1.SeekAlignedToLastTick(TimeSpan.FromSeconds(7));

            if (status == Status.errorId)
            {
                MessageBox.Show("ID输入有误，请重试。");
            }
            else if (status == Status.errorPassword)
            {
                MessageBox.Show("密码错误或未注册。");
            }
            else if (status == Status.success)
            {
                winindex.Show();
                this.Close();
                return;
            }

            LoginBorder.Visibility = Visibility.Visible;
            TextBoxPId.Visibility = Visibility.Visible;
            TextBoxPassword.Visibility = Visibility.Visible;

            TextBoxPId.Focus();
        }

        private void login()
        {
            int id;
            try { id = int.Parse(TextBoxPId.Text); }
            catch { status = Status.errorId; return; }

            Person p = DatabaseLinker.Login(id, TextBoxPassword.Password);

            if (null == p) 
                status = Status.errorPassword;
            else if (p is Person)
            {
                winindex = new WindowIndex(p);
                status = Status.success;
            }
            else 
                status = Status.errorPassword;
        }
    }
}
