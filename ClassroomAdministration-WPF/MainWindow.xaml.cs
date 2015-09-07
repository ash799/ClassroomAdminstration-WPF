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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }



        private void CloseBorder_MouseEnter_1(object sender, MouseEventArgs e)
        {
            CloseBorder.Background = new SolidColorBrush(Color.FromArgb(20,255,255,255));
        }

        private void CloseBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBorder.Background = null;
        }

        private void CloseBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseLabel.Margin = new Thickness(5, 0, 0.333, 0);
        }
        private void CloseLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseLabel.Margin = new Thickness(5, 0, 0.333, 0);
        }

        private void CloseBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CloseLabel.Margin = new Thickness(4, 0, 0.333, 0);
            this.Close();
        }

        private void MinBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            MinBorder.Background = new SolidColorBrush(Color.FromArgb(20, 255, 255, 255));
        }

        private void MinBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MinLabel.Margin = new Thickness(5, 0, 0.333, 0);
        }

        private void MinBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            MinBorder.Background = null;
        }

        private void MinBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MinLabel.Margin = new Thickness(4, 0, 0.333, 0);
            this.WindowState = WindowState.Minimized;
        }

        private void LoginBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            LoginBorder.Background = new SolidColorBrush(Color.FromArgb(240, 30, 30, 255));
        }

        private void LoginBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            LoginBorder.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
        }

        private void LoginBorder_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            lockImage.Margin = new Thickness(32, 5, 0, 5);
        }
        private void Stackpanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lockImage.Margin = new Thickness(32, 5, 0, 5);
        }

        private void Loginlabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lockImage.Margin = new Thickness(32, 5, 0, 5);
        }

        private void lockImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lockImage.Margin = new Thickness(32, 5, 0, 5);
        }

        private enum Status{errorId,errorPassword,success};
        private Status status;
        private WindowIndex winindex;
        private DispatcherTimer timer=new DispatcherTimer();

        private void Border_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            lockImage.Margin = new Thickness(30, 5, 0, 5);

            LoginBorder.Visibility = Visibility.Hidden;
            TextBoxPId.Visibility = Visibility.Hidden;
            TextBoxPassword.Visibility = Visibility.Hidden;


            
            timer=new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 4);
            timer.Tick += new EventHandler(timer_tick);
            Wait1.Begin(ellipse1);
            Wait2.Begin(ellipse2);
            Wait3.Begin(ellipse3);
            Wait4.Begin(ellipse4);
            Wait5.Begin(ellipse5);
            timer.Start();

            login();
           
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                lockImage.Margin = new Thickness(32, 5, 0, 5);
                LoginBorder.Visibility = Visibility.Hidden;
                TextBoxPId.Visibility = Visibility.Hidden;
                TextBoxPassword.Visibility = Visibility.Hidden;



                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 6);
                timer.Tick += new EventHandler(timer_tick);
                Wait1.Begin(ellipse1);
                Wait2.Begin(ellipse2);
                Wait3.Begin(ellipse3);
                Wait4.Begin(ellipse4);
                Wait5.Begin(ellipse5);
                timer.Start();

                login();
            }
        }

        private void timer_tick(object sender, EventArgs e)
        {

            lockImage.Margin = new Thickness(30, 5, 0, 5);
            timer.Stop();
            Wait1.SeekAlignedToLastTick(TimeSpan.FromSeconds(7));
            if (status == Status.errorId)
            {
                MessageBox.Show("ID输入有误，请重试。");
                LoginBorder.Visibility = Visibility.Visible;
                TextBoxPId.Visibility = Visibility.Visible;
                TextBoxPassword.Visibility = Visibility.Visible;
            }
            else if (status == Status.errorPassword)
            {
                MessageBox.Show("密码错误或未注册。");
            }
            else if (status == Status.success)
            {
                winindex.Show();
                this.Close();
            }

            LoginBorder.Visibility = Visibility.Visible;
            TextBoxPId.Visibility = Visibility.Visible;
            TextBoxPassword.Visibility = Visibility.Visible;
        }

        

        private void login()
        {
            int id;
            try { id = int.Parse(TextBoxPId.Text); }
            catch
            {
                status = Status.errorId;
                return;
            }

            Person p = DatabaseLinker.Login(id, TextBoxPassword.Password);
            Console.WriteLine(TextBoxPassword.Password + " " + id);

            if (null == p) status = Status.errorPassword;
            else if (p is Person)
            {
                //MessageBox.Show("Welcome, " + p.Name + "!");
                winindex = new WindowIndex(p);
                winindex.Hide();
                status = Status.success;
            }
            else status = Status.errorPassword;
        }

    }
}
