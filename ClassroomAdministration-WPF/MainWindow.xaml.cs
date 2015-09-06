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
            lockImage.Margin = new Thickness(42, 5, 0, 5);

            //LoginBorder.Visibility = Visibility.Hidden;
            //TextBoxPId.Visibility = Visibility.Hidden;
            //TextBoxPassword.Visibility = Visibility.Hidden;

            int id;
            try { id = int.Parse(TextBoxPId.Text); }
            catch 
            {
                MessageBox.Show("ID输入有误，请重试。");
                LoginBorder.Visibility = Visibility.Visible;
                TextBoxPId.Visibility = Visibility.Visible;
                TextBoxPassword.Visibility = Visibility.Visible;
                return; 
            }

            Person p = DatabaseLinker.Login(id, TextBoxPassword.Password);
            Console.WriteLine(TextBoxPassword.Password + " " + id);

            if (null == p) MessageBox.Show("密码错误或未注册。");
            else if (p is Person)
            {
                //MessageBox.Show("Welcome, " + p.Name + "!");
                new WindowIndex(p).Show();
                this.Close();
            }
            else MessageBox.Show("密码错误或未注册。");


            LoginBorder.Visibility = Visibility.Visible;
            TextBoxPId.Visibility = Visibility.Visible;
            TextBoxPassword.Visibility = Visibility.Visible;
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int id;
                try { id = int.Parse(TextBoxPId.Text); }
                catch
                {
                    MessageBox.Show("ID输入有误，请重试。");
                    LoginBorder.Visibility = Visibility.Visible;
                    TextBoxPId.Visibility = Visibility.Visible;
                    TextBoxPassword.Visibility = Visibility.Visible;
                    return;
                }

                Person p = DatabaseLinker.Login(id, TextBoxPassword.Password);
                Console.WriteLine(TextBoxPassword.Password + " " + id);

                if (null == p) MessageBox.Show("密码错误或未注册。");
                else if (p is Person)
                {
                    //MessageBox.Show("Welcome, " + p.Name + "!");
                    new WindowIndex(p).Show();
                    this.Close();
                }
                else MessageBox.Show("密码错误或未注册。");    
            }
        }







    }
}
