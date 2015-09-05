using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SilverlightApplicationGridBorderSample
{
    /// <summary>
    /// 为Grid添加的一个特殊功能
    /// 作者：陈希章
    /// 反馈：ares@xizhang.com
    /// </summary>
    public class GridHelper
    {   
        //请注意：可以通过propa这个快捷方式生成下面三段代码
        
        public static bool GetShowBorder(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowBorderProperty);
        }

        public static void SetShowBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowBorderProperty, value);
        }

        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.RegisterAttached("ShowBorder", typeof(bool), typeof(GridHelper), new PropertyMetadata(OnShowBorderChanged));


        //这是一个事件处理程序，需要手工编写，必须是静态方法
        private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            if((bool)e.OldValue)
            {
                grid.Loaded -= (s, arg) => { };
            }
            if((bool)e.NewValue)
            {
                grid.Loaded += (s, arg) =>
                {
                    //确定行和列数
                    var rows = grid.RowDefinitions.Count;
                    var columns = grid.ColumnDefinitions.Count;

                    //每个格子添加一个Border进去
                    for(int i = 0; i < rows; i++)
                    {
                        for(int j = 0; j < columns; j++)
                        {
                            var border = new Border() { BorderBrush = new SolidColorBrush(Colors.LightGray), BorderThickness = new Thickness(0.3) };
                            Grid.SetRow(border, i);
                            Grid.SetColumn(border, j);

                            grid.Children.Add(border);
                        }
                    }

                };
            }

        }

    }

}
