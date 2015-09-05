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
    /// WindowIndex.xaml 的交互逻辑
    /// </summary>
    public partial class WindowIndex : Window
    {        
        public WindowIndex(Person p)
        {
            InitializeComponent();

            person = p;
        }

         //开学第一天

        const int cntCol = 7, cntRow = 14;
        Person person;
        RentTable schedule;
        DateTime firstDate = RentTime.FirstDate;
        DateTime currDate = new DateTime(2015, 9, 14);
        int weeks = 1, weekDay = 0, currClass = 1;

        List<TextBlock> RectangleRent = new List<TextBlock>();

        private void SetDateClass(DateTime date, int cc)
        {
            currDate = date;
            currClass = cc;
            TimeSpan ts = currDate - firstDate;
            int days = ts.Days;

            weeks = days / 7 + 1;
            weekDay = days % 7;
            if (weekDay < 0)
            {
                weekDay += 7;
                MessageBox.Show("您选择的日期不在本学期内。");
            }
            
            LabelWeek.Content = "第" + weeks + "周";
            DateChosen.SelectedDate = date;
            TextBlockClassTime.Text = RentTime.ClassTime[currClass - 1];

            RectangleChosonClass.SetValue(Grid.ColumnProperty, weekDay);
            RectangleChosonClass.SetValue(Grid.RowProperty, currClass - 1);

        }
        private void SetDateClass(int wkDay, int cc)
        {
            weekDay = wkDay;
            currDate = firstDate + new TimeSpan(7 * (weeks - 1) + wkDay, 0, 0, 0);

            SetDateClass(currDate, cc);
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            Building.Initialize();
            schedule = DatabaseLinker.GetPersonRentTable(person.pId);

            foreach (Rent r in schedule.Rents)
            {
                TextBlock rect = new TextBlock();
                GridSchdeuleSmall.Children.Add(rect);
                rect.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
                rect.Text = r.Info;
                rect.FontSize = 30;
                rect.FontWeight = FontWeights.Bold;
                rect.Foreground = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
                rect.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
                rect.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
                rect.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);
            }

            Console.WriteLine(schedule.Display());

            SetDateClass(currDate, currClass);
        }        

        private void GridSchedule_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            
            Point pos = e.GetPosition(GridSchdeuleSmall);
            double aCol = GridSchdeuleSmall.ActualWidth, aRow = GridSchdeuleSmall.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);

            SetDateClass(col, row + 1);
        }
        private void DateChosen_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DateTime date = (DateTime)DateChosen.SelectedDate;
            SetDateClass(date, currClass);
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (currClass > 1) --currClass;
                    break;
                case Key.Down:
                    if (currClass < cntRow) ++currClass;
                    break;
                case Key.Left:
                    if (currDate > firstDate) currDate -= new TimeSpan(1, 0, 0, 0);
                    break;
                case Key.Right:
                    currDate += new TimeSpan(1, 0, 0, 0);
                    break;
            }
            SetDateClass(currDate, currClass);
        }
    }
}
