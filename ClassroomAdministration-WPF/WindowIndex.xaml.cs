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

        const int cntCol = 7, cntRow = 14;
        
        DateTime firstDate = RentTime.FirstDate;
        TimeSpan day = new TimeSpan(1, 0, 0, 0);
        DateTime currDate = new DateTime(2015, 9, 14);
        int weeks = 1, weekDay = 0, currClass = 1;

        Person person;
        RentTable schedule;
        List<TextBlock> TextBlockRents = new List<TextBlock>();

        private void SetWeeks(int w)
        {
            if (w != weeks)
            {
                DateTime date = firstDate;
                List<Rent> list;

                date += new TimeSpan(7 * (w - 1), 0, 0, 0);

                list = schedule.GetFromWeek(date);

                foreach (TextBlock tb in TextBlockRents)
                    if (list.Contains((Rent)tb.Tag)) tb.Visibility = Visibility.Visible;
                    else tb.Visibility = Visibility.Collapsed;
            }

            weeks = w;
        }
        private void SetDateClass(DateTime date, int cc)
        {
            if (date < firstDate)
            {
                MessageBox.Show("您选择的日期不在本学期内。");
                return;
            }

            currDate = date;
            currClass = cc;
            TimeSpan ts = currDate - firstDate;
            int days = ts.Days;

            SetWeeks(days / 7 + 1);
            weekDay = days % 7;
            if (weekDay < 0) { weekDay += 7; }
            
            LabelWeek.Content = person.Name+" 第" + weeks + "周";
            DateChosen.SelectedDate = date;
            TextBlockClassTime.Text = RentTime.StringClassTime[currClass - 1];

            RectangleChosonClass.SetValue(Grid.ColumnProperty, weekDay);
            RectangleChosonClass.SetValue(Grid.RowProperty, currClass - 1);
            RectangleChosonClassInClassroom.SetValue(Grid.ColumnProperty, weekDay);
            RectangleChosonClassInClassroom.SetValue(Grid.RowProperty, currClass - 1);

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

            ScheduleInitialize(GridScheduleSmall, schedule, TextBlockRents);

            //foreach (Rent r in schedule.Rents)
            //{
            //    TextBlock rect = new TextBlock();
            //    rect.Tag = r;
            //    GridSchdeuleSmall.Children.Add(rect);
            //    TextBlockRents.Add(rect);

            //    rect.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
            //    rect.Text = r.Info;
            //    rect.FontSize = 30;
            //    rect.FontWeight = FontWeights.Bold;
            //    rect.Foreground = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
            //    rect.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
            //    rect.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
            //    rect.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);
            //}

            Console.WriteLine(schedule.Display());

            SetDateClass(currDate, currClass);
        }        

        private void GridSchedule_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            
            Point pos = e.GetPosition(GridScheduleSmall);
            double aCol = GridScheduleSmall.ActualWidth, aRow = GridScheduleSmall.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);

            SetDateClass(col, row + 1);
        }
        private void GridScheduleClass_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(GridScheduleClass);
            double aCol = GridScheduleClass.ActualWidth, aRow = GridScheduleClass.ActualHeight;

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
                    if (currDate > firstDate) currDate -= day;
                    break;
                case Key.Right:
                    currDate += day;
                    break;
                case Key.Home:
                    currClass = 1;
                    break;
                case Key.End:
                    currClass = cntRow;
                    break;
                case Key.PageUp:
                    if (weeks > 1) currDate -= new TimeSpan(7, 0, 0, 0);
                    break;
                case Key.PageDown:
                    currDate += new TimeSpan(7, 0, 0, 0);
                    break;
            }
            SetDateClass(currDate, currClass);
        }


        Classroom classroom = null;
        RentTable scheduleClassroom;
        List<TextBlock> TextBlockRentsClassroom = new List<TextBlock>();

        private void TextBoxCId_TextChanged(object sender, TextChangedEventArgs e)
        {
            int cId;
            if (int.TryParse(TextBoxCId.Text, out cId))
            {
                SetCId(cId);
            }
        }
        private void SetCId(int cId)
        {
            Classroom C = Building.GetClassroom(cId);
            if (null == C) return;
            if (classroom != null && classroom.cId == C.cId) return;

            foreach (TextBlock tb in TextBlockRentsClassroom)
                if (GridScheduleClass.Children.Contains(tb)) GridScheduleClass.Children.Remove(tb);
            TextBlockRentsClassroom.Clear();

            classroom = C;
            scheduleClassroom = DatabaseLinker.GetClassroomRentTable(cId);
            LabelClassroom.Content = classroom.Display();

            ScheduleInitialize(GridScheduleClass, scheduleClassroom, TextBlockRentsClassroom);

        }

        private void ScheduleInitialize(Grid grid, RentTable rentTable, List<TextBlock> textBlockList)
        {
            foreach (Rent r in rentTable.Rents)
            {
                TextBlock rect = new TextBlock();
                rect.Tag = r;
                grid.Children.Add(rect);
                textBlockList.Add(rect);

                rect.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
                rect.Text = r.Info;
                rect.FontSize = 30;
                rect.FontWeight = FontWeights.Bold;
                rect.Foreground = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
                rect.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
                rect.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
                rect.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);
            }

        }
    }
}
