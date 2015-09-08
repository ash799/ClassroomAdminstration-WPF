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
    /// WindowClassroomList.xaml 的交互逻辑
    /// </summary>
    public partial class WindowClassroomList : Window
    {
        RentTable rentTable;
        WindowIndex father;

        public WindowClassroomList(RentTable r, WindowIndex ff)
        {
            InitializeComponent();

            rentTable = r;
            father = ff;
        }

        private void stackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Building building in Building.AllBuildings)
            {
                TextBlock tbTitle = new TextBlock();
                tbTitle.Padding = new Thickness(24, 16, 26, 10);
                tbTitle.TextWrapping = TextWrapping.Wrap;
                tbTitle.Text = building.Name;
                tbTitle.FontSize = 24;
             //   tbTitle.Background = new SolidColorBrush(MyColor.NameColor(building.Name, 0.05));

                WrapPanel subPanel = new WrapPanel();
                subPanel.Orientation = Orientation.Horizontal;
                subPanel.Margin = new Thickness(24, 0, 24, 0);

                foreach (Classroom classroom in building.Classrooms)
                {
                    TextBlock tb = new TextBlock();
                    tb.Height = 100;
                    tb.Width = 100;
                    tb.FontSize = 16;
                    tb.Margin = new Thickness(5, 5, 5, 5);
                    tb.TextWrapping = TextWrapping.Wrap;

                    Rent rent = rentTable.GetClassroom(classroom.cId);
                    if (rent == null)
                    {
                        tb.Height = 50;
                        tb.Width = 50;
                        tb.FontSize = 12;
                        tb.Text = classroom.Name;
                        tb.Background = new SolidColorBrush(MyColor.NameColor(tb.Text, 0.05));
                    }
                    else
                    {
                        tb.Text = classroom.Name + ":" + rent.Info;
                        tb.Background = new SolidColorBrush(MyColor.NameColor(tb.Text)); 
                    }

                    tb.Tag = classroom;
                    tb.MouseDown += tb_MouseDown;
                    tb.MouseEnter += tb_MouseEnter;
                    tb.MouseLeave += tb_MouseLeave;

                    subPanel.Children.Add(tb);
                }

                stackPanel.Children.Add(tbTitle);
                stackPanel.Children.Add(subPanel);

            }
        }

        void tb_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (!tb.Text.Contains(":"))
            {
                tb.Background = new SolidColorBrush(MyColor.NameColor(tb.Text, 0.05));
            }
            else
            {
                tb.Background = new SolidColorBrush(MyColor.NameColor(tb.Text));
            }
        }

        void tb_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (!tb.Text.Contains(":"))
            {
                tb.Background = new SolidColorBrush(MyColor.NameColor(tb.Text, 0.2)); 
            }
            else
            {
                tb.Background = new SolidColorBrush(MyColor.NameColor(tb.Text, 0.5)); 
            }
        }

        void tb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Classroom c = (Classroom)((TextBlock)sender).Tag;

            father.SetClassroom(c.cId);
            this.Close();
        }


    }
}
