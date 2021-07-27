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

using Microsoft.EntityFrameworkCore;

using TransTool.Db;

namespace TransTool.ControlPages
{
    /// <summary>
    /// ModTranslationComparePage.xaml 的交互逻辑
    /// </summary>
    public partial class ModTranslationComparePage : Page
    {
        public readonly ModDataContext _context = new ModDataContext();

        public ModTranslationComparePage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = false;
            CurrentModButton.IsEnabled = false;
            ToggleInfoNode();
            await Task.Run(async () =>
            {
                await _context.Database.EnsureCreatedAsync();
                await _context.ModInfos.LoadAsync();
                await _context.TranslationHistories.LoadAsync();
                await _context.LanguageKeyValuePairs.LoadAsync();
                await _context.SaveChangesAsync();
            });
            ModList.ItemsSource = _context.ModInfos.Local.ToObservableCollection();
            ToggleHistoryNode();
            ToggleInfoNode();
            ModList.DisplayMemberPath = "ModNickName";
            HistoryView.DisplayMemberPath = "TimeOffset";
            AddButton.IsEnabled = true;
            CurrentModButton.IsEnabled = true;
        }

        private async void Page_UnLoaded(object sender, RoutedEventArgs e)
        {
            await _context.DisposeAsync();
        }

        private async void Add_OnClick(object sender, RoutedEventArgs e)
        {
            await _context.ModInfos.AddAsync(new ModInfo()
            {
                Guid = Guid.NewGuid(),
                CurseForgeId = 114514,
                ModName = "test",
                ModNickName = "野兽先辈",
                ModSlug = "t-e-s-t",
                TranslationHistories = new List<TranslationHistory>()
                {
                    new TranslationHistory()
                    {
                        Guid = Guid.NewGuid(),
                        TimeOffset = DateTimeOffset.Now,
                        LanguageKeyValuePairs = new List<LanguageKeyValuePair>()
                        {
                            new LanguageKeyValuePair()
                            {
                                Guid = Guid.NewGuid(),
                                Key = "a.b.c",
                                Localized = "ABC",
                                Origin = "abc"
                            }
                        },
                        Memo = "123"
                    }
                }
            });
            await _context.SaveChangesAsync();
        }

        private void ToggleHistoryNode()
        {
            if (ModList.SelectedValue is ModInfo mi)
            {
                var his = _context.ModInfos.FirstOrDefault(_ => _.Guid == mi.Guid)?.TranslationHistories;
                HistoryView.ItemsSource = his;
                HistoryView.SelectedIndex = 0;
            }
        }

        private void CurrentModButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ModList.SelectedValue is ModInfo mi)
            {
                InCfId.Text = mi.CurseForgeId.ToString();
                InCfSlug.Text = mi.ModSlug;
                InModName.Text = mi.ModName;
                InNickName.Text = mi.ModNickName;
            }
        }

        private void ModList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleHistoryNode();
        }

        private void HistoryView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleInfoNode();
        }

        private void ToggleInfoNode()
        {
            if (HistoryView.SelectedValue is TranslationHistory t)
            {
                if (string.IsNullOrEmpty(t.Memo))
                {
                    MemoBlock.Text = "无备注";
                }
                else
                {
                    MemoBlock.Text = "备注：" + t.Memo;
                }

                Mode1.IsEnabled = true;
                Mode2.IsEnabled = true;
                Mode3.IsEnabled = true;
                Mode4.IsEnabled = true;
                Mode5.IsEnabled = true;
                ExportButton.IsEnabled = true;
                ExportO.IsEnabled = true;
                ExportC.IsEnabled = true;
            }
            else
            {
                Mode1.IsEnabled = false;
                Mode2.IsEnabled = false;
                Mode3.IsEnabled = false;
                Mode4.IsEnabled = false;
                Mode5.IsEnabled = false;
                ExportButton.IsEnabled = false;
                ExportO.IsEnabled = false;
                ExportC.IsEnabled = false;
            }
        }
    }
}
