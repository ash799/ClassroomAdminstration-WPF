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
    /// WindowApplyRent.xaml 的交互逻辑
    /// </summary>
    public partial class WindowApplyRent : Window
    {
        Person person;
        Classroom classroom;
        DateTime date;
        int classChosen, classStart, classEnd;
        WindowIndex father;

        public WindowApplyRent(Person p, Classroom clsr, DateTime dt, int cc, WindowIndex f)
        {
            InitializeComponent();

            person = p;
            classroom = clsr;
            date = dt;
            classChosen = cc;
            father = f;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            TBinfo.Text = person.Name+"申请的活动";
            TBinfo.Focus();

            TBhost.Text = "申请人: " + DatabaseLinker.GetName(person.pId);

            TBclassroom.Text = "教室: " + classroom.Name;

            for (int i = 0; i < 6; ++i)
                if (classChosen >= RentTime.typicalClassRent[i, 0] && classChosen <= RentTime.typicalClassRent[i, 1])
                {
                    classStart = RentTime.typicalClassRent[i, 0];
                    classEnd = RentTime.typicalClassRent[i, 1];
                }
            TBrentTime.Text = "时间: " + date.Month + "月" + date.Day + "日, 第" + classStart + "节至第" + classEnd + "节";
        }

        private void TBclassroom_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Background = new SolidColorBrush(MyColor.NameColor(TBinfo.Text));
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
            if (TBinfo.Text.Trim()=="") 
            {
                MessageBox.Show("活动名称不能为空。");
                return;
            }
            if (TBinfo.Text.Length>140)
            {
                MessageBox.Show("活动名称过长。");
                return;
            }

            RentTime rt = new RentTime(date, date, 0, classStart, classEnd);
            Rent r = new Rent(0, TBinfo.Text, classroom.cId, person.pId, false, rt);

            if (DatabaseLinker.SetRent(r))
            {
                MessageBox.Show("活动申请已发送。");
                father.RefreshSchedule();
                this.Close();
            }
            else
                MessageBox.Show("活动申请发送失败。");
            
        }




    }
}
