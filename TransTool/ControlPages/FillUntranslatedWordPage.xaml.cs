using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using ModernWpf.Controls;

using Page = System.Windows.Controls.Page;

namespace TransTool.ControlPages
{
    /// <summary>
    /// FillUntranslatedWordPage.xaml 的交互逻辑
    /// </summary>
    public partial class FillUntranslatedWordPage : Page
    {
        public FillUntranslatedWordPage()
        {
            InitializeComponent();
        }

        private void Select_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var dig = new OpenFileDialog();
            dig.Multiselect = false;
            dig.DefaultExt = ".lang";
            dig.Filter = "Lang Files (*.lang)|*.lang|Json Files (*.json)|*.json";
            var showDialog = dig.ShowDialog();
            if (showDialog == true)
            {
                sender.Text = dig.FileName;
            }
        }
    }
}
