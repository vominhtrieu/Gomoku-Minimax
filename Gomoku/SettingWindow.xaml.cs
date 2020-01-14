using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        int index = 1;
        public SettingWindow()
        {
            InitializeComponent();
            if(File.Exists("setting.cfg"))
            {
                FileStream file = new FileStream("setting.cfg", FileMode.Open, FileAccess.Read);
                index = file.ReadByte();
            }
            else
            {
                index = 1;
            }

            for(int i = 1; i <= 5; i++)
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Checked += ChangeLV;
                radioButton.Content = i.ToString();
                if (i == index)
                    radioButton.IsChecked = true;
                ComLVPanel.Children.Add(radioButton);
            }
        }


        private void SaveSetting(object sender, RoutedEventArgs e)
        {
            FileStream file = new FileStream("setting.cfg", FileMode.OpenOrCreate, FileAccess.Write);
            file.WriteByte((byte)index);
            file.Close();
            ((MainWindow)this.Owner).ChangeLV(index);
            this.Close();
        }

        private void ChangeLV(object sender, RoutedEventArgs e)
        {
            index = Convert.ToInt32(((RadioButton)sender).Content.ToString());
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
