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
using System.Collections.ObjectModel;

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

            EnsureSkins();
            ApplySkin(StarrySkin);

            if (person.pId == 0) SetStatus(status.Message);
            else SetStatus(status.Table);
        }

        enum status { Info, Table, Message }
        status currStatus = status.Table;

        #region 换肤

        public enum skin { Starry, ColorBox }
        static public skin currSkin = skin.Starry;
        static public Color textColor = Colors.White;

        static ResourceDictionary ColorBoxSkin = null, StarrySkin = null;

        //初始化ResourceDictionary
        void EnsureSkins()
        {
            if (ColorBoxSkin == null)
            {
                ColorBoxSkin = new ResourceDictionary();
                ColorBoxSkin.Source = new Uri("StyleColorBox.xaml", UriKind.Relative);

                StarrySkin = new ResourceDictionary();
                StarrySkin.Source = new Uri("StyleStarry.xaml", UriKind.Relative);
            }
        }
        //应用ResourceDictionary
        void ApplySkin(ResourceDictionary newSkin)
        {
            Collection<ResourceDictionary> appMergedDictionaries = Application.Current.Resources.MergedDictionaries;

            if (appMergedDictionaries.Count != 0)
                appMergedDictionaries.Remove(appMergedDictionaries[0]);

            appMergedDictionaries.Add(newSkin);

        }

        //设置皮肤
        void SetSkin(skin newSkin)
        {
            if (currSkin == newSkin) return;

            currSkin = newSkin;

            switch (currSkin)
            {
                case skin.Starry:
                    BorderMain.Background = new ImageBrush(ChangeBitmapToImageSource(Properties.Resources.tableback2));
                    ApplySkin(StarrySkin);
                    textColor = Colors.White;
                    break;
                case skin.ColorBox:
                    BorderMain.Background = new ImageBrush(ChangeBitmapToImageSource(Properties.Resources.Color3));
                    ApplySkin(ColorBoxSkin);
                    textColor = Colors.Black;
                    break;
            }

            TextBlockChosenDate.Foreground = new SolidColorBrush(textColor);
            TextBoxCId.Foreground = new SolidColorBrush(textColor);

            RefreshSchedule();

        }

        //从资源中抓取图片
        public static ImageSource ChangeBitmapToImageSource(System.Drawing.Bitmap bitmap)
        {

            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return wpfBitmap;

        }

        #endregion

        #region 个人信息逻辑
        private void GridInfo_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 系统消息逻辑

        List<SysMsg> listSysMsg;

        private void GridMessage_Loaded(object sender, RoutedEventArgs e)
        {
            listSysMsg = DatabaseLinker.GetPersonSysMsgList(person.pId);

            for (int i = listSysMsg.Count - 1; i >= 0; --i)
            {
                SysMsg msg = listSysMsg[i];
                TextBlock tb = new TextBlock();
                TextBlockMessageInitialize(tb, msg);
                stackPanelMessage.Children.Add(tb);
            }

            if (person.pId == 0)
            {
                List<Rent> listRent = DatabaseLinker.GetUnapprovedRentTable();

                foreach (Rent r in listRent)
                {
                    TextBlock tb = new TextBlock();
                    TextBlockUnapprovedRentInitialize(tb, r);
                    stackPanelMessage.Children.Add(tb);
                }
            }

        }
        
       // 系统消息初始化
        private void TextBlockMessageInitialize(TextBlock tb, SysMsg msg)
        {
            string sendName = DatabaseLinker.GetName(msg.SendId);

            tb.FontSize = 24;
            tb.Padding = new Thickness(16);
            tb.Inlines.Add(new Bold(new Run(sendName + ": ("+msg.Time.ToString("yyyy/MM/dd")+")\r\n")));
            tb.Inlines.Add(new Run("  " + msg.Info));
            tb.TextWrapping = TextWrapping.Wrap;

          //  tb.MouseEnter += tbRent_MouseEnter;
          //  tb.MouseLeave += tbRent_MouseLeave;

            tb.Tag = msg;
        }

        //(管理员功能)查看未审核课程
        private void TextBlockUnapprovedRentInitialize(TextBlock tb, Rent r)
        {
            string applicantName = DatabaseLinker.GetName(r.pId);
           string s = r.Info;
            Classroom c = Building.GetClassroom(r.cId); if (c != null) s += ("@" + c.Name);

            tb.Inlines.Add(new Bold(new Run(applicantName + ":\r\n")));
            tb.Inlines.Add(new Run("  " + s));

            tb.FontSize = 24;
            tb.Padding = new Thickness(16);

            tb.MouseDown += tbRent_MouseDown;
            tb.MouseEnter += tbRent_MouseEnter;
            tb.MouseLeave += tbRent_MouseLeave;

            tb.Tag = r;
        }

        void tbRent_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            tb.Background = new SolidColorBrush(MyColor.NameColor(tb.Text, 0.2));
        }
        void tbRent_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            tb.Background = null;
        }
        void tbRent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rent r = (sender as TextBlock).Tag as Rent;

            if (MaxBorder.IsEnabled == true)
                new WindowRent(r, this).ShowDialog();
            else new WindowRent(r, this, "big").ShowDialog();
        }
        

        #endregion

        #region 课程表逻辑

        //课表尺寸
        const int cntCol = 7, cntRow = 14;

        //初始日期
        DateTime firstDate = RentTime.FirstDate;
        DateTime currDate = RentTime.FirstDate;
        int currWeek = 0, currWeekDay = 0, currClass = 1;
        //星期表头
        Label[] head1, head2;
        string[] weekDayName = RentTime.weekDayName;

        //左侧: 个人课程表
        Person person;
        RentTable schedule1;
        List<TextBlock> TextBlockRents1 = new List<TextBlock>();
        //右侧: 教室课程表
        Classroom classroom = null;
        RentTable schedule2;
        List<TextBlock> TextBlockRents2 = new List<TextBlock>();

        //选中的格子
        Rent chosenRent1 = null, chosenRent2 = null;
        TextBlock TBHighlight1 = null, TBHighlight2 = null;

        //课程表页面加载
        private void GridTable_Loaded(object sender, RoutedEventArgs e)
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
        }
        //初始化单个课程
        private void TextBlockInitialize(TextBlock tb, Rent r, bool MouseShow = true, bool InGrid = true)
        {
            tb.Tag = r;

            tb.Background = MyColor.NameBrush(r.Info); //new SolidColorBrush(MyColor.NameColor(r.Info));
            tb.Text = r.Info; if (!r.Approved) tb.Text += "(未审核)";
            Classroom c = Building.GetClassroom(r.cId); if (c != null) tb.Text += ("@" + c.Name);
            tb.FontSize = 16;

            tb.Foreground = new SolidColorBrush(textColor);
            tb.TextWrapping = TextWrapping.Wrap;

            if (InGrid)
            {
                tb.SetValue(Grid.ColumnProperty, r.Time.WeekDay);
                tb.SetValue(Grid.RowProperty, r.Time.StartClass - 1);
                tb.SetValue(Grid.RowSpanProperty, r.Time.KeepClass);
            }

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
            tb.Background = MyColor.NameBrush(r.Info);//new SolidColorBrush(MyColor.NameColor(r.Info));
        }
        void tb_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            Rent r = (Rent)(tb.Tag);
            tb.Background = MyColor.NameBrush(r.Info, 0.8);//new SolidColorBrush(MyColor.NameColor(r.Info, 0.8));
        }

        //选择日期时间
        private void SetDateClass(DateTime date, int cc)
        {
            Console.WriteLine("date: " + date + ", class: " + cc);

            if (cc < 1 || cc > 14) return;

            if (date < firstDate)
            {
                MessageBox.Show("您选择的日期不在本学期内。");
                return;
            }

            currDate = date;
            currClass = cc;

            int days = (currDate - firstDate).Days;
            SetWeeks(days / 7 + 1);
            currWeekDay = days % 7;

            DateChosen.SelectedDate = date;
            TextBlockChosenDate.Text = date.ToString("yyyy/MM/dd");

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

            Console.WriteLine("HightLight Rent: " + r.Info);

            tbh = new TextBlock();
            grid.Children.Add(tbh);
            TextBlockInitialize(tbh, r, false);

            tbh.Background = MyColor.NameBrush(r.Info, 1);//new SolidColorBrush(MyColor.NameColor(r.Info, 1));
           // tbh.Foreground = new SolidColorBrush(Colors.White);

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

        //设置教室。在教室发生变化的时候切换UI
        private void SetCId(int cId)
        {
            Classroom C = Building.GetClassroom(cId);
            if (null == C) return;
            if (classroom != null && classroom.cId == C.cId) return;

            TextBoxCId.Text = cId.ToString();

            classroom = C;
            schedule2 = DatabaseLinker.GetClassroomRentTable(cId);
            LabelClassroom.Content = classroom.Name + "的第" + currWeek + "周";

            ScheduleInitialize(GridSchedule2, schedule2, TextBlockRents2, RectangleChosonClass2);

            checkoutWeek();
        }

        #endregion      
        #region 课程表交互
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

        //单击选定框
        private void RectangleChosonRent1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (chosenRent1 != null && chosenRent2 == null) SetCId(chosenRent1.cId);

            if (schedule2 != null &&
                schedule1.QuiteFreeTime(currDate, currClass) && schedule2.QuiteFreeTime(currDate, currClass)
                && schedule2 != null)
            {
                if (MaxBorder.IsEnabled == true)
                    new WindowApplyRent(person, classroom, currDate, currClass, this).ShowDialog();
                else
                    new WindowApplyRent(person, classroom, currDate, currClass, this, "big").ShowDialog();
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
            LabelClassroom.Content = "进入教室列表";
            LabelClassroom.Background = new SolidColorBrush(Color.FromArgb(51, 255, 255, 255));
            LabelClassroom.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 86, 157, 229));
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
        #endregion

        #region 键盘托管, 鼠标滚轮
        //全体键盘托管
        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            //换肤
            switch (e.Key)
            {
                case Key.F1: SetSkin(skin.Starry); break;
                case Key.F2: SetSkin(skin.ColorBox); break;
            }

            switch (currStatus)
            {
                case status.Table:
                    //课程表控制
                    if (!TextBoxCId.IsKeyboardFocused)
                    {
                        switch (e.Key)
                        {
                            case Key.Up:        if (currClass > 1) --currClass; break;
                            case Key.Down:      if (currClass < cntRow) ++currClass; break;
                            case Key.Left:      if (currDate > firstDate) currDate -= new TimeSpan(1, 0, 0, 0); break;
                            case Key.Right:     currDate += new TimeSpan(1, 0, 0, 0); break;
                            case Key.Home:      currClass = 1; break;
                            case Key.End:       currClass = cntRow; break;
                            case Key.PageUp:    if (currWeek > 1) currDate -= new TimeSpan(7, 0, 0, 0); break;
                            case Key.PageDown:  currDate += new TimeSpan(7, 0, 0, 0); break;
                            case Key.Enter:     RectangleChosonRent1_MouseDown(null, null);  break;
                        }
                        SetDateClass(currDate, currClass);
                    }
                    else
                    //教室控制
                    {
                        int b, c;
                        if (classroom == null) { b = 0; c = 0; }
                        else { b = classroom.Building.bId; c = classroom.cId; }

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
                    break;
            }
        }
        //鼠标滚轮托管
        private void Window_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            switch (currStatus)
            {
                case status.Table:
                    int d = e.Delta / 120;
                    if (d > 0)
                        if (currWeek > 1) currDate -= new TimeSpan(7, 0, 0, 0);
                    if (d < 0)
                        currDate += new TimeSpan(7, 0, 0, 0);
                    SetDateClass(currDate, currClass);
                    break;
            }
        }
        #endregion

        #region 各个public函数
        public void SetClassroom(int cId)
        {
           // TextBoxCId.Text = cId.ToString();
            SetStatus(status.Table);
            SetCId(cId);
            checkoutWeek();
        }
        public void RefreshSchedule()
        {
          //  SetStatus(status.Table);

            schedule1 = DatabaseLinker.GetPersonRentTable(person.pId);
            ScheduleInitialize(GridSchedule1, schedule1, TextBlockRents1, RectangleChosonClass1);

            if (classroom != null)
            {
                schedule2 = DatabaseLinker.GetClassroomRentTable(classroom.cId);
                ScheduleInitialize(GridSchedule2, schedule2, TextBlockRents2, RectangleChosonClass2);
            }

            checkoutWeek();
        }
        public void GotoDateClass(DateTime date, int cc)
        {
            SetStatus(status.Table);

            SetDateClass(date, cc);
            checkoutWeek();
        }
        public Person Peron { get { return person; } }
        public RentTable Schedule { get { return schedule1; } }
        #endregion

        #region 窗口必备功能

        //窗口拖动
        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
           this.DragMove();
        }
        
        //最大化
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
        //还原
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
        //最小化
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
        //关闭
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

        #endregion

        #region 上侧导航按钮
        private void BorderButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            b.BorderThickness = new Thickness(3);
        }
        private void BorderButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            b.BorderThickness = new Thickness(0);
        }

        private void SetStatus(status S)
        {
            currStatus = S;
            GridInfo.Visibility = currStatus == status.Info ? Visibility.Visible : Visibility.Collapsed;
            GridTable.Visibility = currStatus == status.Table ? Visibility.Visible : Visibility.Collapsed;
            GridMessage.Visibility = currStatus == status.Message ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BorderInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetStatus(status.Info);
        }

        private void BorderTable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetStatus(status.Table);
        }

        private void BorderMessage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetStatus(status.Message);
        }


        private void SkinBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            SkinBorder.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        }
        private void SkinBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SkinBorder.Background = null;
            Canvas.SetLeft(imageSkin, 5);
        }
        private void SkinBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(imageSkin, 3);
        }
        private void SkinBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(imageSkin, 5);
            
            switch (currSkin)
            {
                case skin.Starry:      SetSkin(skin.ColorBox); break;
                case skin.ColorBox:    SetSkin(skin.Starry); break;
            }
        }

        #endregion

    }
}
