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
        
        //时间相关的数据
        DateTime firstDate = RentTime.FirstDate;
        TimeSpan day = new TimeSpan(1, 0, 0, 0);
        DateTime currDate = new DateTime(2015, 9, 14);
        int weeks = 1, weekDay = 0, currClass = 1;
        Label[] head1, head2;

        //左侧个人课程表的后台数据
        Person person;
        RentTable schedule;
        List<TextBlock> TextBlockRents = new List<TextBlock>();

        //设置周数。在周数发生变化的时刻改变UI
        private void SetWeeks(int w)
        {
            if (w != weeks)
            {
                DateTime date = firstDate;

                date += new TimeSpan(7 * (w - 1), 0, 0, 0);

                CheckoutWeek(schedule, date, TextBlockRents);
                CheckoutWeek(scheduleClassroom, date, TextBlockRentsClassroom);

            }

            weeks = w;
        }
        //按照周数更新课程表
        private void CheckoutWeek(RentTable rentTable, DateTime date, List<TextBlock> TBList)
        {
            if (rentTable == null) return;

            List<Rent> list = rentTable.GetFromWeek(date);

            foreach (TextBlock tb in TBList)
                if (list.Contains((Rent)tb.Tag)) tb.Visibility = Visibility.Visible;
                else tb.Visibility = Visibility.Collapsed;
        }

        //用户选择了某个日期时间
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

            LabelWeek.Content = " 第" + weeks + "周@" + person.Name;
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

        //页面加载
        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            Building.Initialize();
            schedule = DatabaseLinker.GetPersonRentTable(person.pId);

            ScheduleInitialize(GridScheduleSmall, schedule, TextBlockRents);

            Console.WriteLine(schedule.Display());

            SetDateClass(currDate, currClass);
        }        

        //鼠标单击Grid
        private void GridSchedule_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            TextBoxCId_Copy.Focus();

            Point pos = e.GetPosition(GridScheduleSmall);
            double aCol = GridScheduleSmall.ActualWidth, aRow = GridScheduleSmall.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);

            SetDateClass(col, row + 1);
        }
        private void GridScheduleClass_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            TextBoxCId_Copy.Focus();

            Point pos = e.GetPosition(GridScheduleClass);
            double aCol = GridScheduleClass.ActualWidth, aRow = GridScheduleClass.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);

            SetDateClass(col, row + 1);
        }        
        
        //通过Calendar选择了date后
        private void DateChosen_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DateTime date = (DateTime)DateChosen.SelectedDate;
            SetDateClass(date, currClass);
        }
        
        //全体键盘托管
        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (!TextBoxCId.IsKeyboardFocused)
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
            else
            {
                int b, c;
                if (classroom == null)
                {
                    b = 0;
                    c = 0;
                }
                else
                {
                    b = classroom.Building.bId;
                    c = classroom.cId;
                }
               

                switch (e.Key)
                {
                    case Key.Up:
                        while (c < Classroom.MaxCId)
                        {
                            ++c;
                            if (Building.GetClassroom(c) != null) break;
                        }
                        break;
                    case Key.Down:
                        while (c > Classroom.MinCId)
                        {
                            --c;
                            if (Building.GetClassroom(c) != null) break;
                        }
                        break;
                    case Key.PageUp:
                        while (b < Building.MaxBId)
                        {
                            ++b;
                            if (Building.GetBuilding(b) != null)
                            {
                                c = Building.GetBuilding(b).Classrooms[0].cId;
                                break;
                            }
                        }
                        break;
                    case Key.PageDown:
                        while (b > Building.MinBId)
                        {
                            --b;
                            if (Building.GetBuilding(b) != null)
                            {
                                c = Building.GetBuilding(b).Classrooms[0].cId;
                                break;
                            }
                        }
                        break;

                }
                SetCId(c);
            }
        }

        //右侧教室课程表的后台信息
        Classroom classroom = null;
        RentTable scheduleClassroom;
        List<TextBlock> TextBlockRentsClassroom = new List<TextBlock>();

        //通过textBox选择教室
        private void TextBoxCId_TextChanged(object sender, TextChangedEventArgs e)
        {
            int cId;
            if (int.TryParse(TextBoxCId.Text, out cId))
            {
                SetCId(cId);
            }
        }
        
        //设置教室。在教师发生变化的时候切换UI
        private void SetCId(int cId)
        {
            TextBoxCId.Text = cId.ToString();

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

        //左右课程表的初始化设置
        private void ScheduleInitialize(Grid grid, RentTable rentTable, List<TextBlock> textBlockList)
        {
            foreach (Rent r in rentTable.Rents)
            {
                TextBlock tb = new TextBlock();
                tb.Tag = r;
                grid.Children.Add(tb);
                textBlockList.Add(tb);

                tb.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
                tb.Text = r.Info;
                tb.FontSize = 18;
                tb.FontWeight = FontWeights.Bold;
                tb.Foreground = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
                tb.TextWrapping = TextWrapping.Wrap;
                tb.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
                tb.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
                tb.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);
            }

        }

    }
}
