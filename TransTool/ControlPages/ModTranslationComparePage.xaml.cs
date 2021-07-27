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
            await Task.Run(async () =>
            {
                await _context.Database.EnsureCreatedAsync();
                await _context.ModInfos.LoadAsync();
                await _context.SaveChangesAsync();
            });
            ModList.ItemsSource = _context.ModInfos.Local.ToObservableCollection();
            ToggleHistoryNode();
            ModList.DisplayMemberPath = "ModNickName";
            HistoryView.DisplayMemberPath = "TimeOffset";
        }

        private async void Page_UnLoaded(object sender, RoutedEventArgs e)
        {
            await _context.DisposeAsync();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
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
                var his = _context.ModInfos.First(_ => _.Guid == mi.Guid).TranslationHistories;
                HistoryView.ItemsSource = null;
            }
        }
    }
}
