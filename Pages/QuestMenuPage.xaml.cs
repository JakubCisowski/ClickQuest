using ClickQuest.Account;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Heroes;
using ClickQuest.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
    public partial class QuestMenuPage : Page
    {
        private Random _rng;

        public QuestMenuPage()
        {
            InitializeComponent();

            _rng = new Random();
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
        }

        private void RerollButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if any quest is currently assigned - if so, user can't reroll quests.
            if(Account.User.Instance.CurrentHero.Quests.All(x=>x.EndDate==default(DateTime)))
            {
                // Later: add price.
                RerollQuests();
            }
        }

        // Generate 3 random quests based on class.
        // When: User finishes quest / There is a new hero without quests / User clicks reroll button.
        public void RerollQuests()
        {
            // Remove current hero quests - both from hero and Entity database.
            Account.User.Instance.CurrentHero.Quests.Clear();
            Entity.EntityOperations.RemoveQuests();

            // Generate 3 quest ids.
            var questsForCurrentHeroClass = Data.Database.Quests.Where(x => x.HeroClass == Account.User.Instance.CurrentHero.HeroClass || x.HeroClass == HeroClass.All).ToList();

            for (int i = 0; i < 3; i++)
            {
                int index = _rng.Next(0, questsForCurrentHeroClass.Count());
                if (questsForCurrentHeroClass.ElementAt(index).Rare == true)
                {
                    index = _rng.Next(0, questsForCurrentHeroClass.Count());
                }

                // Add selected quest id to the list.
                Account.User.Instance.CurrentHero.Quests.Add(questsForCurrentHeroClass.ElementAt(index));
                questsForCurrentHeroClass.RemoveAt(index);
            }

            // Refresh quest buttons.
            RefreshQuests();
        }

        // Create buttons using Quests in current Hero
        // When: User switches to a different hero.
        public void RefreshQuests()
        {
            QuestPanel.Children.Clear();

            for (int i = 0; i < Account.User.Instance.CurrentHero.Quests.Count; i++)
            {
                var button = new QuestButton(Account.User.Instance.CurrentHero.Quests[i]);

                QuestPanel.Children.Add(button);
            }
        }

        public void LoadPage()
        {
            // Copy 3 quests from json to hero (because Entity Framework is ..).
            for (int i=0;i<Account.User.Instance.CurrentHero.Quests.Count;i++)
            {
                var questFromDb = Data.Database.Quests.FirstOrDefault(x=>x.Id==Account.User.Instance.CurrentHero.Quests[i].Id);
                Account.User.Instance.CurrentHero.Quests[i].CopyQuest(questFromDb);
            }

            // Either refresh or reroll quests.
            if (Account.User.Instance.CurrentHero.Quests.Count >= 3)
            {
                // User already has 3 quests - only refresh the quest page.
                RefreshQuests();
            }
            else
            {
                // No quests assigned - reroll them.
                RerollQuests();
            }
        }
    }
}