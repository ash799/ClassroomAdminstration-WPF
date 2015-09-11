﻿using System;
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

        //课表尺寸
        const int cntCol = 7, cntRow = 14;

        //初始日期
        DateTime firstDate = RentTime.FirstDate;
        DateTime currDate = RentTime.FirstDate;
        int currWeek = 0, currWeekDay = 0, currClass = 1;
        //星期表头
        Label[] head1, head2;
        string[] weekDayName = RentTime.weekDayName; 
        
        //选中的格子
        Rent chosenRent1 = null, chosenRent2 = null;
        TextBlock TBHighlight1 = null, TBHighlight2 = null;

        //页面加载
        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            Building.Initialize();

            headInitialize();

            schedule1 = DatabaseLinker.GetPersonRentTable(person.pId);
            ScheduleInitialize(GridSchedule1, schedule1, TextBlockRents1, RectangleChosonClass1);

            if (chosenRent1 != null) SetCId(chosenRent1.cId);
        }
        //初始化星期表头
        private void headInitialize()
        {
            head1 = new Label[7];
            for (int i = 0; i < 7; ++i)
            {
                head1[i] = new Label();
                head1[i].Foreground = new SolidColorBrush(Color.FromArgb(230, 255, 255, 255));
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
                head2[i].Foreground = new SolidColorBrush(Color.FromArgb(230, 255, 255, 255));
                GridScheduleHead2.Children.Add(head2[i]);
                head2[i].Content = weekDayName[i];
                head2[i].VerticalContentAlignment = VerticalAlignment.Center;
                head2[i].HorizontalContentAlignment = HorizontalAlignment.Center;
                head2[i].SetValue(Grid.ColumnProperty, i);
            }

        }
        //初始化课表
        private void ScheduleInitialize(Grid grid, RentTable rentTable, List<TextBlock> textBlockList, Label chosen)
        {
            foreach (TextBlock tb in textBlockList)
                if (grid.Children.Contains(tb)) grid.Children.Remove(tb);
            textBlockList.Clear();

            foreach (Rent r in rentTable.Rents)
            {
                TextBlock tb = new TextBlock();
                grid.Children.Add(tb);
                textBlockList.Add(tb);

                TextBlockInitialize(tb, r);
            }

            chosen.Visibility = Visibility.Visible;

            if (grid.Children.Contains(chosen)) grid.Children.Remove(chosen);
            grid.Children.Add(chosen);

            SetDateClass(currDate, currClass);
            checkoutWeek();
        }
        //初始化单个课程
        private void TextBlockInitialize(TextBlock tb, Rent r, bool MouseShow = true)
        {
            tb.Tag = r;

            tb.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
            tb.Text = r.Info;
            Classroom c = Building.GetClassroom(r.cId); if (c != null) tb.Text += ("@" + c.Name);
            tb.FontSize = 16;

            tb.Foreground = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
            tb.TextWrapping = TextWrapping.Wrap;
            tb.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
            tb.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
            tb.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);

            if (MouseShow)
            {
                tb.MouseEnter += tb_MouseEnter;
                tb.MouseLeave += tb_MouseLeave;
            }
        }
        void tb_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            Rent r = (Rent)(tb.Tag);
            tb.Background = new SolidColorBrush(MyColor.NameColor(r.Info));
        }
        void tb_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            Rent r = (Rent)(tb.Tag);
            tb.Background = new SolidColorBrush(MyColor.NameColor(r.Info, 0.8));
        }

        //左侧: 个人课程表
        Person person;
        RentTable schedule1;
        List<TextBlock> TextBlockRents1 = new List<TextBlock>();
        //右侧: 教室课程表
        Classroom classroom = null;
        RentTable schedule2;
        List<TextBlock> TextBlockRents2 = new List<TextBlock>();

        //设置周数
        private void checkoutWeek()
        {
            LabelWeek.Content = person.Name + "的第" + currWeek + "周";
            if (classroom != null) LabelClassroom.Content = classroom.Name + "的第" + currWeek + "周";

            ScheduleCheckoutWeek(schedule1, TextBlockRents1);
            ScheduleCheckoutWeek(schedule2, TextBlockRents2);

            ScheduleHeadCheckoutWeek(head1);
            ScheduleHeadCheckoutWeek(head2);
        }
        private void SetWeeks(int w)
        {
            if (w != currWeek)
            {
                currWeek = w;
                checkoutWeek();
            }

            currWeek = w;
        }
        //按照周数更新课程表
        private void ScheduleCheckoutWeek(RentTable rentTable, List<TextBlock> TBList)
        {
            DateTime date = firstDate + new TimeSpan(7 * (currWeek - 1), 0, 0, 0);

            if (rentTable == null) return;

            List<Rent> list = rentTable.GetFromWeek(date);

            foreach (TextBlock tb in TBList)
                if (list.Contains((Rent)tb.Tag)) tb.Visibility = Visibility.Visible;
                else tb.Visibility = Visibility.Collapsed;
        }
        private void ScheduleHeadCheckoutWeek(Label[] head)
        {
            for (int i = 0; i < 7; ++i)
            {
                DateTime theDate = firstDate + new TimeSpan(7 * (currWeek - 1) + i, 0, 0, 0);
                head[i].Content = theDate.Month + "." + theDate.Day + weekDayName[i];
            }
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
                        if (currDate > firstDate) currDate -= new TimeSpan(1, 0, 0, 0);
                        break;
                    case Key.Right:
                        currDate += new TimeSpan(1, 0, 0, 0);
                        break;
                    case Key.Home:
                        currClass = 1;
                        break;
                    case Key.End:
                        currClass = cntRow;
                        break;
                    case Key.PageUp:
                        if (currWeek > 1) currDate -= new TimeSpan(7, 0, 0, 0);
                        break;
                    case Key.PageDown:
                        currDate += new TimeSpan(7, 0, 0, 0);
                        break;
                    case Key.Enter:
                        RectangleChosonRent1_MouseDown(null, null);
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

        
        //选择日期时间
        private void SetDateClass(DateTime date, int cc)
        {
            Console.WriteLine("date: "+date+", class: "+cc);

            if (cc < 1 || cc > 14) return;

            if (date < firstDate)
            {
                MessageBox.Show("您选择的日期不在本学期内。");
                return;
            }

            currDate = date;
            currClass = cc;

            DateLabel.Content = currDate.ToString("yyyy/MM/dd");

            int days = (currDate - firstDate).Days;
            SetWeeks(days / 7 + 1);
            currWeekDay = days % 7; 
            
            DateChosen.SelectedDate = date;  

            ChosenRentControl();

            RectangleChosonClass1.SetValue(Grid.ColumnProperty, currWeekDay);
            RectangleChosonClass1.SetValue(Grid.RowProperty, currClass - 1);
            RectangleChosonClass2.SetValue(Grid.ColumnProperty, currWeekDay);
            RectangleChosonClass2.SetValue(Grid.RowProperty, currClass - 1);
        }
        private void SetDateClass(int weekDay, int cc)
        {
            currWeekDay = weekDay;
            currDate = firstDate + new TimeSpan(7 * (currWeek - 1) + weekDay, 0, 0, 0);

            SetDateClass(currDate, cc);
        }

        //检查选中的时间点的课程
        private void ChosenRentControl()
        {
            if (schedule1 != null && schedule2 != null)
            {
                chosenRent1 = schedule1.GetRentFromDateClass(currDate, currClass);
                TBHighlight1 = Hightlight(TBHighlight1, chosenRent1, GridSchedule1);
                chosenRent2 = schedule2.GetRentFromDateClass(currDate, currClass);
                TBHighlight2 = Hightlight(TBHighlight2, chosenRent2, GridSchedule2);

                if (schedule1.QuiteFreeTime(currDate, currClass) && schedule2.QuiteFreeTime(currDate, currClass))
                {
                    RectangleChosonClass1.Content = "+申请活动";
                    RectangleChosonClass1.Foreground = new SolidColorBrush(Color.FromArgb(230, 255, 255, 255));
                    RectangleChosonClass2.Content = "+申请活动";
                    RectangleChosonClass2.Foreground = new SolidColorBrush(Color.FromArgb(230, 255, 255, 255));
                }
                else
                {
                    RectangleChosonClass1.Content = "";//chosenRent1 == null ? "" : "查看信息";
                    RectangleChosonClass2.Content = "";//chosenRent2 == null ? "" : "查看信息";
                }
            }
            else if (schedule1 != null)
            {
                chosenRent1 = schedule1.GetRentFromDateClass(currDate, currClass);
                TBHighlight1 = Hightlight(TBHighlight1, chosenRent1, GridSchedule1);
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
            TextBlockInitialize(tbh, r, false);

            tbh.Background = new SolidColorBrush(MyColor.NameColor(r.Info, 1));
            tbh.Foreground = new SolidColorBrush(Colors.White);

            if (GridSchedule1 == grid) tbh.MouseDown += RectangleChosonRent1_MouseDown;
            else if (GridSchedule2 == grid) tbh.MouseDown += RectangleChosonRent2_MouseDown;

            return tbh;
        }
        private TextBlock Rent2Block(Rent r)
        {
            foreach (TextBlock tb in TextBlockRents1)
                if ((Rent)tb.Tag == r) return tb;
            foreach (TextBlock tb in TextBlockRents2)
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
        private void GridSchedule1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxCId_Copy.Focus();

            Point pos = e.GetPosition(GridSchedule1);
            double aCol = GridSchedule1.ActualWidth, aRow = GridSchedule1.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);

            SetDateClass(col, row + 1);

            if (chosenRent1 != null) RectangleChosonRent1_MouseDown(null, null);
        }
        private void GridSchedule2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxCId_Copy.Focus();

            if (classroom == null) return;

            Point pos = e.GetPosition(GridSchedule2);
            double aCol = GridSchedule2.ActualWidth, aRow = GridSchedule2.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);

            SetDateClass(col, row + 1);

            if (chosenRent2 != null) RectangleChosonRent2_MouseDown(null, null);

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
            Classroom C = Building.GetClassroom(cId);
            if (null == C) return;            
            if (classroom != null && classroom.cId == C.cId) return;

            classroom = C;
            schedule2 = DatabaseLinker.GetClassroomRentTable(cId);
            LabelClassroom.Content = classroom.Name + "的第" + currWeek + "周";

            ScheduleInitialize(GridSchedule2, schedule2, TextBlockRents2, RectangleChosonClass2);

            ChosenRentControl();
        }

        //单击选定框
        private void RectangleChosonRent1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (chosenRent1 != null && chosenRent2 == null) SetCId(chosenRent1.cId);

            if (schedule2 != null &&
                schedule1.QuiteFreeTime(currDate, currClass) && schedule2.QuiteFreeTime(currDate, currClass)
                && schedule2 != null)
            {
                if(MaxBorder.IsEnabled==true)
                    new WindowApplyRent(person, classroom, currDate, currClass, this).ShowDialog();
                else
                    new WindowApplyRent(person, classroom, currDate, currClass, this,"big").ShowDialog();
            }
            else
                if (chosenRent1 != null)
                {
                    if (MaxBorder.IsEnabled == true)
                        new WindowRent(chosenRent1, this).ShowDialog();
                    else new WindowRent(chosenRent1, this, "big").ShowDialog();
                }
                else if (chosenRent2 != null)
                    if (MaxBorder.IsEnabled == true)
                        new WindowRent(chosenRent2, this).ShowDialog();
                    else new WindowRent(chosenRent2, this, "big").ShowDialog();

            if (e != null) e.Handled = true;
        }
        private void RectangleChosonRent2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (schedule1.QuiteFreeTime(currDate, currClass) && schedule2.QuiteFreeTime(currDate, currClass) && schedule2 != null)
            {
                if (MaxBorder.IsEnabled == true)
                    new WindowApplyRent(person, classroom, currDate, currClass, this).ShowDialog();
                else
                    new WindowApplyRent(person, classroom, currDate, currClass, this, "big").ShowDialog();
            }
            else
                if (chosenRent2 != null) //MessageBox.Show(chosenClassroomRent.Display());
                {
                    if (MaxBorder.IsEnabled == true)
                        new WindowRent(chosenRent2, this).ShowDialog();
                    else new WindowRent(chosenRent2, this, "big").ShowDialog();
                }
            if (e != null) e.Handled = true;
        }

        //进入教室列表
        private void LabelClassroom_MouseEnter(object sender, MouseEventArgs e)
        {
            LabelClassroom.Content = "选择教室";
            LabelClassroom.BorderBrush = new SolidColorBrush(Color.FromArgb(128, 23, 0, 255));
        }
        private void LabelClassroom_MouseLeave(object sender, MouseEventArgs e)
        {
            if (classroom != null) LabelClassroom.Content = classroom.Name + "的第" + currWeek + "周";
            LabelClassroom.Background = null;
            LabelClassroom.BorderBrush = null;
        }
        private void LabelClassroom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RentTable rt = new RentTable(DatabaseLinker.GetDateRentTable(currDate).GetFromDateClass(currDate, currClass));
            new WindowClassroomList(rt, this).ShowDialog();
        }


        //子窗口调用函数
        public void SetClassroom(int cId)
        {
            TextBoxCId.Text = cId.ToString();
            SetCId(cId);
        }
        public void RefreshSchedule()
        {
            schedule1 = DatabaseLinker.GetPersonRentTable(person.pId);
            ScheduleInitialize(GridSchedule1, schedule1, TextBlockRents1, RectangleChosonClass1);

            if (classroom != null)
            {
                schedule2 = DatabaseLinker.GetClassroomRentTable(classroom.cId);
                ScheduleInitialize(GridSchedule2, schedule2, TextBlockRents2, RectangleChosonClass2);
            }
        }
        public void GotoDateClass(DateTime date, int cc)
        {
            SetDateClass(date, cc);
        }
        public Person Peron { get { return person; } }
        public RentTable Schedule { get { return schedule1; } }

        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
           this.DragMove();
        }

        

        private void MaxBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            MaxBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        }
        private void MaxBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            MaxLabel.Margin = new Thickness(0, 0, 5, 0);
            MaxBorder.Background = null;
        }
        private void MaxBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MaxLabel.Margin = new Thickness(0, 0, 7, 0);
        }
        private void MaxBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            
            MaxLabel.Margin = new Thickness(0, 0, 5, 0);
            this.WindowState = WindowState.Maximized;
            MaxBorder.Visibility = Visibility.Hidden;
            MaxBorder.IsEnabled = false;
            NormalBorder.Visibility = Visibility.Visible;
            NormalBorder.IsEnabled = true;

        }

        private void NormalBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            NormalBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        }

        private void NormalBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            canvas_normalborder.Margin = new Thickness(0);
            NormalBorder.Background = null;
        }

        private void NormalBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas_normalborder.Margin = new Thickness(-2, 0, 0, 0);
        }

        private void NormalBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Normal;

            MaxLabel.Margin = new Thickness(0,0,5,0);
            MaxBorder.Visibility = Visibility.Visible;
            MaxBorder.IsEnabled = true;
            NormalBorder.Visibility = Visibility.Hidden;
            NormalBorder.IsEnabled = false;
        }

        private void MinBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            MinBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        }

        private void MinBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            MinLabel.Margin = new Thickness(0, 0, 3, 0);
            MinBorder.Background = null;

        }

        private void MinBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MinLabel.Margin = new Thickness(0, 0, 5, 0);
        }

        private void MinBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseBorder_MouseEnter_1(object sender, MouseEventArgs e)
        {
            CloseBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        }

        private void CloseBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseLabel.Margin = new Thickness(0, 0, -3, 0);
            CloseBorder.Background = null;
        }

        private void CloseBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseLabel.Margin = new Thickness(0, 0, -1, 0);
            
        }
        private void CloseBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void BorderInfo_MouseEnter_1(object sender, MouseEventArgs e)
        {
            BorderInfo.BorderThickness = new Thickness(2);
        }
        private void BorderInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderInfo.BorderThickness = new Thickness(0);
        }

        private void BorderMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderMessage.BorderThickness = new Thickness(2);
        }
        private void BorderMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderMessage.BorderThickness = new Thickness(0);
        }
        private void BorderTable_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderTable.BorderThickness = new Thickness(2);
        }
        private void BorderTable_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderTable.BorderThickness = new Thickness(0);
        }

        private void BorderClassroom_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderClassroom.BorderThickness = new Thickness(2);
        }

        private void BorderClassroom_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderClassroom.BorderThickness = new Thickness(0);
        }

        private void SkinBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            SkinBorder.Background = new SolidColorBrush(Color.FromArgb(100,255,255,255));
        }

        private void SkinBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SkinBorder.Background = null;
        }  

        private void SkinBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(imageSkin, 3);
        }

        private void SkinBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(imageSkin, 5);

            //////////////////////////////////////change skin mode
        }

        

        

        

    }
}
