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
using System.Windows.Shapes;

namespace ClassroomAdministration_WPF
{
    /// <summary>
    /// WindowRent.xaml 的交互逻辑
    /// </summary>
    public partial class WindowRent : Window
    {
        Rent rent;
        WindowIndex father;

        public WindowRent(Rent r, WindowIndex fatherWindow)
        {
            InitializeComponent();
            rent = r;
            father = fatherWindow;
        }
        public WindowRent(Rent r, WindowIndex fatherWindow,string str)
        {
            InitializeComponent();
            if (str == "big")
            {
                this.Width = 450;
                this.Height = 450;
                BorderBackground.Width = 450;
                BorderBackground.Height = 450;
                TBinfo.FontSize = 40;
                TBhost.FontSize = 22;
                TBrentTime.FontSize = 22;
                TBclassroom.FontSize = 22;
                TBChoose.FontSize = 22;
                TBexit.FontSize = 22;
            }
            rent = r;
            father = fatherWindow;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            TBinfo.Text = rent.Info;
            TBinfo.Background = new SolidColorBrush(MyColor.NameColor(rent.Info));

            TBhost.Text = "申请人: " + DatabaseLinker.GetName(rent.pId);

            Classroom c = Building.GetClassroom(rent.cId);
            if (c != null) TBclassroom.Text = "教室: " + c.Name; else TBclassroom.Visibility = Visibility.Collapsed;

            TBrentTime.Text = "时间: "+rent.Time.Display();

            if (father.Schedule.Contains(rent.rId))
                TBChoose.Text = "从我的课程表中删除";
            else
                TBChoose.Text = "加入我的课程表";

        }

        private void TBclassroom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            father.SetClassroom(rent.cId);
            this.Close();
        }
        private void TBclassroom_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Background = new SolidColorBrush(MyColor.NameColor(rent.Info));
        }
        private void TBclassroom_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Background = null;
        }

        private void TBexit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void TBChoose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (father.Schedule.Contains(rent.rId))
            {
                if (MessageBox.Show("确定删除 "+rent.Info+"？", "删除课程", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DatabaseLinker.DeleteTakepartin(father.Peron.pId, rent.rId);
                    father.Schedule.Remove(rent);
                    father.RefreshSchedule();
                    this.Close();
                }
            }
            else
            {
                Rent rr = father.Schedule.Add(rent.rId);
                if (rr == null)
                    DatabaseLinker.AddTakepartin(father.Peron.pId, rent.rId);
                else
                {
                    MessageBox.Show("添加失败。此课程同您的课程 " + rr.Info + " 存在冲突。");
                    if (!rent.Time.OnceActivity) father.GotoDateClass(rr.Time.StartDate, rr.Time.StartClass);
                }

                father.RefreshSchedule();

                this.Close();
            }
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.Close();
                    break;
                case Key.Q:
                    TBChoose_MouseDown(null, null);
                    break;
            }
        }

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //Close Button
        private void CloseBorder_MouseEnter_1(object sender, MouseEventArgs e)
        {
            CloseBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
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

        ////Min Button
        //private void MinBorder_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    MinBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        //}
        //private void MinBorder_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    MinLabel.Margin = new Thickness(0, 0, 1, 0);
        //}
        //private void MinBorder_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    MinLabel.Margin = new Thickness(0, 0, 3, 0);
        //    MinBorder.Background = null;
        //}
        //private void MinBorder_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    MinLabel.Margin = new Thickness(0, 0, 3, 0);
        //    this.WindowState = WindowState.Minimized;
        //    this.Owner.WindowState = WindowState.Minimized;
        //}

    }
}
