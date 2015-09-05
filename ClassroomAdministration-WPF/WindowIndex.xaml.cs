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
        Person person;

        public WindowIndex(Person p)
        {
            InitializeComponent();

            person = p;
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            Building.Initialize();

            DateTime d = new DateTime(2015, 10, 14);
            RentTable rt = DatabaseLinker.GetDateRentTable(d);
            Console.WriteLine(new RentTable(rt.GetTableFromDate(d)).Display());

            List<SysMsg> list = DatabaseLinker.GetPersonSysMsgList(person.pId);
            foreach (SysMsg m in list) Console.WriteLine(m.Info);
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            const int cntCol = 7, cntRow = 14;
            Point pos = e.GetPosition(GridSchdeuleSmall);
            double aCol = GridSchdeuleSmall.ActualWidth, aRow = GridSchdeuleSmall.ActualHeight;

            int col = (int)(pos.X / aCol * cntCol), row = (int)(pos.Y / aRow * cntRow);
            RectangleChosonClass.SetValue(Grid.ColumnProperty, col);
            RectangleChosonClass.SetValue(Grid.RowProperty, row);
        }


    }
}
