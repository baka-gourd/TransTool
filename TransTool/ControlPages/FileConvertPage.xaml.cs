using System;
using System.Collections.Generic;
using System.IO;
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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.UI.Xaml.Shapes;

using Page = System.Windows.Controls.Page;
using Path = System.IO.Path;
using System.Text.RegularExpressions;

namespace TransTool.ControlPages
{
    /// <summary>
    /// FileConvertPage.xaml 的交互逻辑
    /// </summary>
    public partial class FileConvert : Page
    {
        public FileConvert()
        {
            InitializeComponent();
        }

        private void LangBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var dig = new OpenFileDialog();
            dig.Multiselect = false;
            dig.DefaultExt = ".lang";
            dig.Filter = "Lang Files (*.lang)|*.lang";
            var showDialog = dig.ShowDialog();
            if (showDialog == true)
            {
                ToLang.IsEnabled = false;
                ToJson.IsEnabled = true;
                LangBox.Text = dig.FileName;
                SyncText(true);
            }
        }

        private void JsonBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var dig = new OpenFileDialog();
            dig.Multiselect = false;
            dig.DefaultExt = ".json";
            dig.Filter = "Json Files (*.json)|*.json";
            var showDialog = dig.ShowDialog();
            if (showDialog == true)
            {
                ToJson.IsEnabled = false;
                ToLang.IsEnabled = true;
                JsonBox.Text = dig.FileName;
                SyncText(false);
            }
        }

        private void SyncText(bool flag)
        {
            if (flag)
            {
                var a = Path.GetFileNameWithoutExtension(LangBox.Text);
                var b = Path.GetDirectoryName(LangBox.Text);
                if (!(a is null))
                {
                    JsonBox.PlaceholderText = Path.Combine(b ?? string.Empty, a + ".json");
                }
            }
            else
            {
                var a = Path.GetFileNameWithoutExtension(JsonBox.Text);
                var b = Path.GetDirectoryName(JsonBox.Text);
                if (!(a is null))
                {
                    LangBox.PlaceholderText = Path.Combine(b ?? string.Empty, a + ".lang");
                }
            }
        }

        private void Box_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
            {
                return;
            }
            if (sender.Name == "LangBox")
            {
                SyncText(true);
            }
            else
            {
                SyncText(false);
            }
        }

        private void ToLang_OnClick(object sender, RoutedEventArgs e)
        {
            var jFile = System.IO.File.OpenText(JsonBox.Text);
            var reader = new JsonTextReader(jFile);
            var jObj = (JObject)JToken.ReadFrom(reader);
            List<string> langList = new List<string>();
            foreach (var jValue in jObj)
            {
                langList.Add(jValue.Key + "=" + jValue.Value);
            }

            File.WriteAllLinesAsync(GetPath(true), langList);
        }

        private string GetPath(bool flag)
        {
            if (flag)
            {
                if (string.IsNullOrEmpty(LangBox.Text))
                {
                    return LangBox.PlaceholderText;
                }

                if (string.IsNullOrWhiteSpace(LangBox.Text))
                {
                    return LangBox.PlaceholderText;
                }

                return LangBox.Text;
            }
            else
            {
                if (string.IsNullOrEmpty(JsonBox.Text))
                {
                    return JsonBox.PlaceholderText;
                }

                if (string.IsNullOrWhiteSpace(JsonBox.Text))
                {
                    return JsonBox.PlaceholderText;
                }

                return JsonBox.Text;
            }
        }

        private void ToJson_OnClick(object sender, RoutedEventArgs e)
        {
            var keyReg = new Regex(".+(?==)");
            var nameReg = new Regex("(?<==).+");
            var findEqual = new Regex("=+");
            var findComment1 = new Regex("\n*\r");
            var findComment2 = new Regex("//(.*)");
            var findComment3 = new Regex("#(.*)");
            var findComment4 = new Regex("^( \\*)");
            var findComment5 = new Regex("^(/\\*)");
            var langJObject = new JObject();
            foreach (string str in System.IO.File.ReadAllLines(LangBox.Text, Encoding.UTF8))
            {
                if (!findEqual.IsMatch(str))
                    continue;
                if (findComment1.IsMatch(str))
                    continue;
                if (findComment2.IsMatch(str))
                    continue;
                if (findComment3.IsMatch(str))
                    continue;
                if (findComment4.IsMatch(str))
                    continue;
                if (findComment5.IsMatch(str))
                    continue;
                var key = keyReg.Match(str).ToString();
                var name = nameReg.Match(str).ToString();
                if (key == "" && name == "")
                    continue;
                if (!langJObject.TryGetValue(key, out _))
                {
                    langJObject.Add(key, name);
                }
            }

            File.WriteAllTextAsync(GetPath(false), langJObject.ToString());
        }
    }
}
