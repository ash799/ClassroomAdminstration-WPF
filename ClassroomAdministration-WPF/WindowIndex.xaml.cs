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
        int weeks = 0, weekDay = 0, currClass = 1;
        
        Label[] head1, head2;
        string[] weekDayName = { "一", "二", "三", "四", "五", "六", "日" };

        Rent chosenRent = null, chosenClassroomRent = null;
        TextBlock TBHighlightLeft = null, TBHighlightRight = null;

        //页面加载
        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            Building.Initialize();

            headInitialize();

            //    SetDateClass(currDate, currClass);

            schedule = DatabaseLinker.GetPersonRentTable(person.pId);
            ScheduleInitialize(GridScheduleSmall, schedule, TextBlockRents, RectangleChosonClass);

            if (chosenRent != null) SetCId(chosenRent.cId);
        }
        private void headInitialize()
        {
            head1 = new Label[7];
            for (int i = 0; i < 7; ++i)
            {
                head1[i] = new Label();
                GridScheduleHead.Children.Add(head1[i]);
                head1[i].Content = weekDayName[i];
                head1[i].VerticalContentAlignment = VerticalAlignment.Center;
                head1[i].HorizontalContentAlignment = HorizontalAlignment.Center;
                head1[i].SetValue(Grid.ColumnProperty, i);
            }
            head2 = new Label[7];
            for (int i = 0; i < 7; ++i)
            {
                head2[i] = new Label();
                GridScheduleHead2.Children.Add(head2[i]);
                head2[i].Content = weekDayName[i];
                head2[i].VerticalContentAlignment = VerticalAlignment.Center;
                head2[i].HorizontalContentAlignment = HorizontalAlignment.Center;
                head2[i].SetValue(Grid.ColumnProperty, i);
            }

        }

        //左右课程表的初始化设置
        private void ScheduleInitialize(Grid grid, RentTable rentTable, List<TextBlock> textBlockList, Label chosen)
        {
            foreach (TextBlock tb in textBlockList)
                if (grid.Children.Contains(tb)) grid.Children.Remove(tb);
            textBlockList.Clear();

            foreach (Rent r in rentTable.Rents)
            {
                //TextBlock tb = new TextBlock();
                //tb.Tag = r;

                //tb.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
                //tb.Text = r.Info;
                //Classroom c = Building.GetClassroom(r.cId); if (c != null) tb.Text += ("@" + c.Name);
                //tb.FontSize = 16;
                ////  tb.FontWeight = FontWeights.Bold;
                //tb.Foreground = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
                //tb.TextWrapping = TextWrapping.Wrap;
                //tb.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
                //tb.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
                //tb.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);

                TextBlock tb = new TextBlock();
                grid.Children.Add(tb);
                textBlockList.Add(tb);

                TextBlockInitialize(tb, r);
            }

            chosen.Visibility = Visibility.Visible;

            if (grid.Children.Contains(chosen)) grid.Children.Remove(chosen);
            grid.Children.Add(chosen);

            SetDateClass(currDate, currClass);
            ResetWeeks();
        }
        private void TextBlockInitialize(TextBlock tb, Rent r)
        {
            tb.Tag = r;

            tb.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
            tb.Text = r.Info;
            Classroom c = Building.GetClassroom(r.cId); if (c != null) tb.Text += ("@" + c.Name);
            tb.FontSize = 16;

            tb.Foreground = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
            tb.TextWrapping = TextWrapping.Wrap;
            tb.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
            tb.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
            tb.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);
        }

        //左侧个人课程表的后台数据
        Person person;
        RentTable schedule;
        List<TextBlock> TextBlockRents = new List<TextBlock>();
        //右侧教室课程表的后台信息
        Classroom classroom = null;
        RentTable scheduleClassroom;
        List<TextBlock> TextBlockRentsClassroom = new List<TextBlock>();

        //设置周数。在周数发生变化的时刻改变UI
        private void ResetWeeks()
        {
            int w = weeks;
            DateTime date = firstDate;

            date += new TimeSpan(7 * (w - 1), 0, 0, 0);

            CheckoutWeek(schedule, date, TextBlockRents);
            CheckoutWeek(scheduleClassroom, date, TextBlockRentsClassroom);

            for (int i = 0; i < 7; ++i)
            {
                DateTime theDate = firstDate + new TimeSpan(7 * (w - 1) + i, 0, 0, 0);
                head1[i].Content = theDate.Month + "." + theDate.Day + weekDayName[i];
                head2[i].Content = theDate.Month + "." + theDate.Day + weekDayName[i];
            }
        }
        private void SetWeeks(int w)
        {
            if (w != weeks)
            {
                weeks = w;
                ResetWeeks();
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
                    case Key.Enter:
                        //if (chosenRent != null)
                        //    new WindowRent(chosenRent, this).ShowDialog();
                        RectangleChosonClass_MouseDown(null, null);
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
                        TextBoxCId.Text = c.ToString();
                        break;
                    case Key.Down:
                        while (c > Classroom.MinCId)
                        {
                            --c;
                            if (Building.GetClassroom(c) != null) break;
                        }
                        TextBoxCId.Text = c.ToString();
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
                        TextBoxCId.Text = c.ToString();
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
                        TextBoxCId.Text = c.ToString();
                        break;
                }
                SetCId(c);
            }
        }

        
        //用户选择了某个日期时间
        private void SetDateClass(DateTime date, int cc)
        {
            Console.WriteLine("date"+date+"  cc"+cc);

            if (cc < 0)
            {
                return;
            }

            if (date < firstDate)
            {
                MessageBox.Show("您选择的日期不在本学期内。");
                return;
            }

            currDate = date;  currClass = cc;
            TimeSpan ts = currDate - firstDate;
            int days = ts.Days;

            SetWeeks(days / 7 + 1);
            weekDay = days % 7; if (weekDay < 0) { weekDay += 7; }
            if (currClass < 1) currClass = 1; if (currClass > 14) currClass = 14;

            LabelWeek.Content = person.Name + "的第" + weeks + "周";
            if (classroom != null) LabelClassroom.Content = classroom.Name + "的第" + weeks + "周";
            DateChosen.SelectedDate = date;  //TextBlockClassTime.Text = RentTime.StringClassTime[currClass - 1];

            ChosenRentControl();

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

        //检查选中的时间点的课程
        private void ChosenRentControl()
        {
            if (schedule != null && scheduleClassroom != null)
            {
                chosenRent = schedule.GetRentFromDateClass(currDate, currClass);
                TBHighlightLeft = Hightlight(TBHighlightLeft, chosenRent, GridScheduleSmall);
                chosenClassroomRent = scheduleClassroom.GetRentFromDateClass(currDate, currClass);
                TBHighlightRight = Hightlight(TBHighlightRight, chosenClassroomRent, GridScheduleClass);

                if (schedule.QuiteFreeTime(currDate, currClass) && scheduleClassroom.QuiteFreeTime(currDate, currClass))
                {
                    RectangleChosonClass.Content = "+申请活动";
                    RectangleChosonClassInClassroom.Content = "+申请活动";
                }
                else
                {
                    RectangleChosonClass.Content = chosenRent == null ? "" : "查看信息";
                    RectangleChosonClassInClassroom.Content = chosenClassroomRent == null ? "" : "查看信息";
                }
            }
            else if (schedule != null)
            {
                chosenRent = schedule.GetRentFromDateClass(currDate, currClass);
                TBHighlightLeft = Hightlight(TBHighlightLeft, chosenRent, GridScheduleSmall);
                RectangleChosonClass.Content = chosenRent == null ? "" : "查看信息";
            }
        }
        private TextBlock Hightlight(TextBlock tbh, Rent r, Grid grid)
        {
            if (grid.Children.Contains(tbh))
            {
                tbh.Visibility = Visibility.Collapsed;
                grid.Children.Remove(tbh);
            }

            if (r == null) return null;

            Console.WriteLine("Rent: " + r.Info);

            tbh = new TextBlock();
            grid.Children.Add(tbh);
            TextBlockInitialize(tbh,r);
            tbh.Background = new SolidColorBrush(MyColor.NameColor(r.Info, 1, 150));
            tbh.Foreground = new SolidColorBrush(Colors.White);

            if (GridScheduleSmall == grid) tbh.MouseDown += RectangleChosonClass_MouseDown;
            else if (GridScheduleClass == grid) tbh.MouseDown += RectangleChosonClassInClassroom_MouseDown;

            return tbh;
        }
        private TextBlock Rent2Block(Rent r)
        {
            foreach (TextBlock tb in TextBlockRents)
                if ((Rent)tb.Tag == r) return tb;
            foreach (TextBlock tb in TextBlockRentsClassroom)
                if ((Rent)tb.Tag == r) return tb;
            return null;
        }

        //通过Calendar选择了date后
        private void DateChosen_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DateTime date = (DateTime)DateChosen.SelectedDate;
            SetDateClass(date, currClass);
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

            if (classroom == null) return;

            Point pos = e.GetPosition(GridScheduleClass);
            double aCol = GridScheduleClass.ActualWidth, aRow = GridScheduleClass.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);

            SetDateClass(col, row + 1);
        }        
        
        //通过textBox选择教室
        private void TextBoxCId_TextChanged(object sender, TextChangedEventArgs e)
        {
            int cId;
            if (int.TryParse(TextBoxCId.Text, out cId))
            {
                SetCId(cId);
            }
        }
        
        //设置教室。在教室发生变化的时候切换UI
        private void SetCId(int cId)
        {
           // TextBoxCId.Text = cId.ToString();

            Classroom C = Building.GetClassroom(cId);
            if (null == C) return;
            if (classroom != null && classroom.cId == C.cId) return;

            //foreach (TextBlock tb in TextBlockRentsClassroom)
            //    if (GridScheduleClass.Children.Contains(tb)) GridScheduleClass.Children.Remove(tb);
            //TextBlockRentsClassroom.Clear();

            classroom = C;
            scheduleClassroom = DatabaseLinker.GetClassroomRentTable(cId);
            LabelClassroom.Content = classroom.Name + "的第" + weeks + "周";

            ScheduleInitialize(GridScheduleClass, scheduleClassroom, TextBlockRentsClassroom, RectangleChosonClassInClassroom);

            ChosenRentControl();
        }

        //单击选定框
        private void RectangleChosonClass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (chosenRent != null && chosenClassroomRent == null) SetCId(chosenRent.cId);

            if (scheduleClassroom != null &&
                schedule.QuiteFreeTime(currDate, currClass) && scheduleClassroom.QuiteFreeTime(currDate, currClass)
                && scheduleClassroom != null)
                new WindowApplyRent(person, classroom, currDate, currClass, this).ShowDialog();
            else
                if (chosenRent != null)
                    new WindowRent(chosenRent, this).ShowDialog();
                else if (chosenClassroomRent != null)
                    new WindowRent(chosenClassroomRent, this).ShowDialog();

            if (e != null) e.Handled = true;
        }
        private void RectangleChosonClassInClassroom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (schedule.QuiteFreeTime(currDate, currClass) && scheduleClassroom.QuiteFreeTime(currDate, currClass) && scheduleClassroom != null)
                new WindowApplyRent(person, classroom, currDate, currClass, this).ShowDialog();
            else
                if (chosenClassroomRent != null) //MessageBox.Show(chosenClassroomRent.Display());
                    new WindowRent(chosenClassroomRent, this).ShowDialog();
            e.Handled = true;
        }

        //子窗口调用函数
        public void SetClassroom(int cId)
        {
            TextBoxCId.Text = cId.ToString();
            SetCId(cId);
        }
        public void RefreshSchedule()
        {
            schedule = DatabaseLinker.GetPersonRentTable(person.pId);
            ScheduleInitialize(GridScheduleSmall, schedule, TextBlockRents, RectangleChosonClass);

            if (classroom != null)
            {
                scheduleClassroom = DatabaseLinker.GetClassroomRentTable(classroom.cId);
                ScheduleInitialize(GridScheduleClass, scheduleClassroom, TextBlockRentsClassroom, RectangleChosonClassInClassroom);
            }
        }
        public void GotoDateClass(DateTime date, int cc)
        {
            SetDateClass(date, cc);
        }
        public Person Peron { get { return person; } }
        public RentTable Schedule { get { return schedule; } }

        //Choose Classroom
        private void LabelClassroom_MouseEnter(object sender, MouseEventArgs e)
        {
            LabelClassroom.Content = "选择教室";
            LabelClassroom.Background = new SolidColorBrush(MyColor.NameColor(LabelClassroom.Content.ToString(), 0.1));
        }
        private void LabelClassroom_MouseLeave(object sender, MouseEventArgs e)
        {
            if (classroom != null) LabelClassroom.Content = classroom.Name + "的第" + weeks + "周";
            LabelClassroom.Background = null;
        }
        private void LabelClassroom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RentTable rt = new RentTable( DatabaseLinker.GetDateRentTable(currDate).GetFromDateClass(currDate,currClass));
            new WindowClassroomList(rt, this).ShowDialog();
        }
    }
}
