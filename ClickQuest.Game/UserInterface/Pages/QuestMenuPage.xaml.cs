using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.UserInterface.Pages
{
    public partial class QuestMenuPage : Page
    {
        private readonly List<QuestButton> _questButtons;

        private int _currentQuestPosition;

        public QuestMenuPage()
        {
            InitializeComponent();

            _questButtons = new List<QuestButton>();
            _currentQuestPosition = 0;
        }

        public void LoadPage()
        {
            if (User.Instance.CurrentHero.Quests.Count >= 3)
            {
                // User already has 3 quests - only refresh the quest page.
                RefreshQuestButtons();
            }
            else
            {
                // No quests assigned - reroll them.
                RerollQuests();
            }
        }

        // Generate 3 random quests based on class.
        // When: User finishes quest / There is a new hero without quests / User clicks reroll button.
        public void RerollQuests()
        {
            User.Instance.CurrentHero.Quests.Clear();
            _questButtons.Clear();

            var questsForCurrentHeroClass = GameAssets.Quests.Where(x => x.HeroClass == User.Instance.CurrentHero.HeroClass || x.HeroClass == HeroClass.All).ToList();

            for (var i = 0; i < 3; i++)
            {
                int randomizedIndex = Rng.Next(0, questsForCurrentHeroClass.Count);

                bool isQuestRare = questsForCurrentHeroClass.ElementAt(randomizedIndex).Rare;
                if (isQuestRare)
                {
                    // Randomize once again.
                    randomizedIndex = Rng.Next(0, questsForCurrentHeroClass.Count);
                }

                Quest randomizedQuest = questsForCurrentHeroClass.ElementAt(randomizedIndex).CopyQuest();
                User.Instance.CurrentHero.Quests.Add(randomizedQuest);

                questsForCurrentHeroClass.RemoveAt(randomizedIndex);
            }

            RefreshQuestButtons();
        }

        // Create buttons using Quests in current Hero
        // When: User switches to a different hero.
        public void RefreshQuestButtons()
        {
            QuestPanel.Children.Clear();
            _questButtons.Clear();

            for (var i = 0; i < 3; i++)
            {
                QuestButton button = new QuestButton(User.Instance.CurrentHero.Quests[i]);

                _questButtons.Add(button);
            }

            QuestPanel.Children.Add(_questButtons[0]);
        }

        private void RerollButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if any quest is currently assigned - if so, user can't reroll quests.
            if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
            {
                if (User.Instance.Gold >= Quest.RerollGoldCost)
                {
                    MessageBoxResult result = AlertBox.Show("Are you sure you want to reroll your current quests for 100 gold?");

                    if (result == MessageBoxResult.Yes)
                    {
                        (Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{Quest.RerollGoldCost}", (SolidColorBrush)FindResource("BrushGold"), FloatingTextController.GoldPositionPoint);

                        User.Instance.Gold -= Quest.RerollGoldCost;

                        RerollQuests();

                        // Increase achievement amount.
                        User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestRerollsAmount]++;
                    }
                }
                else
                {
                    AlertBox.Show("You don't have enough gold to reroll", MessageBoxButton.OK);
                }
            }
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back to Town.
            InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");
        }

        private void LeftNavigationButton_OnClick(object sender, RoutedEventArgs e)
        {
            QuestPanel.Children.Clear();
            QuestPanel.Children.Add(_questButtons[--_currentQuestPosition]);

            RightNavigationButton.IsEnabled = true;
            RightNavigationButton.Style = (Style)FindResource("ButtonStyleGeneral");

            NavigationTextBlock.Text = $"{_currentQuestPosition + 1} / {_questButtons.Count}";

            if (_currentQuestPosition == 0)
            {
                LeftNavigationButton.IsEnabled = false;
                LeftNavigationButton.Style = (Style)FindResource("ButtonStyleDisabled");
            }
        }

        private void RightNavigationButton_OnClick(object sender, RoutedEventArgs e)
        {
            QuestPanel.Children.Clear();
            QuestPanel.Children.Add(_questButtons[++_currentQuestPosition]);

            LeftNavigationButton.IsEnabled = true;
            LeftNavigationButton.Style = (Style)FindResource("ButtonStyleGeneral");

            NavigationTextBlock.Text = $"{_currentQuestPosition + 1} / {_questButtons.Count}";

            if (_currentQuestPosition == 2)
            {
                RightNavigationButton.IsEnabled = false;
                RightNavigationButton.Style = (Style)FindResource("ButtonStyleDisabled");
            }
        }
    }
}