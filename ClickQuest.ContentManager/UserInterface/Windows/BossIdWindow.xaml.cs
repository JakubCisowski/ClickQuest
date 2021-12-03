using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
    public partial class BossIdWindow : Window
    {
        private readonly Dungeon _dungeon;
        private int _bossId;
        private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

        public BossIdWindow(Dungeon dungeon, int bossId)
        {
            InitializeComponent();

            _dungeon = dungeon;
            _bossId = bossId;

            RefreshWindowControls();
        }

        public void RefreshWindowControls()
        {
            // Add controls to Dictionary for easier navigation.
            _controls.Clear();

            StackPanel panel = new StackPanel
            {
                Name = "MainInfoPanel"
            };

            TextBox idBox = new TextBox
            {
                Name = "IdBox",
                Text = _bossId.ToString(),
                Margin = new Thickness(10),
                IsEnabled = false
            };

            ComboBox nameBox = new ComboBox
            {
                Name = "NameBox",
                ItemsSource = GameContent.Bosses.Select(x => x.Name),
                Margin = new Thickness(10)
            };
            nameBox.SelectedValue = GameContent.Bosses.FirstOrDefault(x => x.Id == _bossId)?.Name;
            nameBox.SelectionChanged += NameBox_SelectionChanged;

            // Set TextBox and ComboBox hints.
            HintAssist.SetHint(idBox, "ID");
            HintAssist.SetHint(nameBox, "Name");

            _controls.Add(idBox.Name, idBox);
            _controls.Add(nameBox.Name, nameBox);

            foreach (var elem in _controls)
            {
                // Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
                if (elem.Value is TextBox textBox)
                {
                    textBox.Style = (Style)FindResource("MaterialDesignOutlinedTextBox");
                    HintAssist.SetFloatingScale(elem.Value, 1.0);
                    textBox.GotFocus += TextBox_GotFocus;
                }
                else if (elem.Value is ComboBox comboBox)
                {
                    comboBox.Style = (Style)FindResource("MaterialDesignOutlinedComboBox");
                    HintAssist.SetFloatingScale(elem.Value, 1.0);
                }

                panel.Children.Add(elem.Value);
            }

            MainGrid.Children.Add(panel);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).CaretIndex = int.MaxValue;
        }

        private void NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (_controls["IdBox"] as TextBox).Text = GameContent.Bosses.FirstOrDefault(x => x.Name == (sender as ComboBox).SelectedItem.ToString()).Id.ToString();
        }

        private void UpdateBossId()
        {
            int oldBossIdIndex = _dungeon.BossIds.IndexOf(_bossId);

            _bossId = int.Parse((_controls["IdBox"] as TextBox).Text);

            if (oldBossIdIndex == -1)
            {
                _dungeon.BossIds.Add(_bossId);
            }
            else
            {
                _dungeon.BossIds[oldBossIdIndex] = _bossId;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UpdateBossId();
        }
    }
}