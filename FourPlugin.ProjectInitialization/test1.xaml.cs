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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FourPlugin.ProjectInitialization
{
    /// <summary>
    /// test1.xaml 的交互逻辑
    /// </summary>
    public partial class test1 : Window
    {
        public test1()
        {
            InitializeComponent();
        }



        public string Key
        {
            get
            {
                string keystr = "D1";
                if (KeyCombo.SelectedIndex == 1)
                {
                    keystr = "D2";
                }
                return keystr;
            }
        }

        private void Click_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Click_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
