using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ModernWpf.Controls;

using TransTool.ControlPages;

using Page = System.Windows.Controls.Page;
using Path = System.IO.Path;

namespace TransTool.Navigation
{
    /// <summary>
    /// NavigationRootPage.xaml 的交互逻辑
    /// </summary>
    public partial class NavigationRootPage : Page
    {
        private readonly ControlPagesData _controlPagesData = new ControlPagesData();

        public NavigationRootPage()
        {
            InitializeComponent();
            InitData();
            NavigateToSelectedPage();
        }

        private void NavigateToSelectedPage()
        {
            if (PageList.SelectedValue is ControlInfoDataItem type)
            {
                RootFrame?.Navigate(type.PageType);
            }
        }


        private void ControlsSearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is ControlInfoDataItem dataItem)
            {
                var pageType = dataItem?.PageType;
                RootFrame.Navigate(pageType);
            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                var item = _controlPagesData.FirstOrDefault(i => i.Title.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    RootFrame.Navigate(item.PageType);
                }
            }
        }

        private void ControlsSearchBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var suggestions = new List<ControlInfoDataItem>();

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var querySplit = sender.Text.Split(' ');
                var matchingItems = _controlPagesData.Where(
                    item =>
                    {
                        // Idea: check for every word entered (separated by space) if it is in the name,  
                        // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button" 
                        // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words 
                        bool flag = true;
                        foreach (string queryToken in querySplit)
                        {
                            // Check if token is not in string 
                            if (item.Title.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                // Token is not in string, so we ignore this item. 
                                flag = false;
                            }
                        }
                        return flag;
                    });
                foreach (var item in matchingItems)
                {
                    suggestions.Add(item);
                }
                if (suggestions.Count > 0)
                {
                    ControlsSearchBox.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title);
                }
                else
                {
                    ControlsSearchBox.ItemsSource = new string[] { "无结果" };
                }
            }
        }

        private void PageList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigateToSelectedPage();
        }

        private void InitData()
        {
            var path = Environment.CurrentDirectory;
            if (!Directory.Exists(Path.Combine(path, "data")))
            {
                Directory.CreateDirectory(Path.Combine(path, "data"));
            }
        }
    }

    public class ControlPagesData : List<ControlInfoDataItem>
    {
        public ControlPagesData()
        {
            AddPage(typeof(FileConvert), "语言文件格式转换");
            AddPage(typeof(FillUntranslatedWordPage), "填充未翻译条目");
            AddPage(typeof(ModTranslationComparePage), "翻译文件历史比对");
        }

        private void AddPage(Type pageType, string displayName = null)
        {
            Add(new ControlInfoDataItem(displayName, pageType));
        }
    }

    public class ControlInfoDataItem
    {
        public string Title { get; }
        public Type PageType { get; }
        public override string ToString()
        {
            return Title;
        }

        public ControlInfoDataItem(string title, Type pageType)
        {
            Title = title;
            PageType = pageType;
        }
    }
}
