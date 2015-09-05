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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int id;
            try { id = int.Parse(TextBoxPId.Text); }
            catch { MessageBox.Show("ID输入有误，请重试。"); return; }

            Person p = DatabaseLinker.Login(id, TextBoxPassword.Password);
            Console.WriteLine(TextBoxPassword.Password+" "+id);

            if (null == p) MessageBox.Show("密码错误或未注册。");
            else if (p is Person)
            {
                MessageBox.Show("Welcome, " + p.Name + "!");
                new WindowIndex(p).Show();
                this.Close();
            }
            else MessageBox.Show("密码错误或未注册。");
        }
    }
}
