using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
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

		// Generate 3 random quests based on class.
		// When: User finishes quest / There is a new hero without quests / User clicks reroll button.
		public void RerollQuests()
		{
			// Remove current hero quests - both from hero and Entity database.
			User.Instance.CurrentHero.Quests.Clear();
			EntityOperations.RemoveQuests();

			// Generate 3 quest ids.
			var questsForCurrentHeroClass = GameData.Quests.Where(x => x.HeroClass == User.Instance.CurrentHero.HeroClass || x.HeroClass == HeroClass.All).ToList();

			for (int i = 0; i < 3; i++)
			{
				int index = _rng.Next(0, questsForCurrentHeroClass.Count());
				if (questsForCurrentHeroClass.ElementAt(index).Rare == true)
				{
					index = _rng.Next(0, questsForCurrentHeroClass.Count());
				}

				// Add selected quest id to the list (create copy to keep database clean).
				var quest = new Quest();
				quest.CopyQuest(questsForCurrentHeroClass.ElementAt(index));
				User.Instance.CurrentHero.Quests.Add(quest);
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

			for (int i = 0; i < User.Instance.CurrentHero.Quests.Count; i++)
			{
				var button = new QuestButton(User.Instance.CurrentHero.Quests[i]);

				QuestPanel.Children.Add(button);
			}
		}

		public void LoadPage()
		{
			// Either refresh or reroll quests.
			if (User.Instance.CurrentHero.Quests.Count >= 3)
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

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Town.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(GameData.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			(GameData.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(GameData.Pages["Town"] as TownPage).StatsFrame.Refresh();
		}

		private void RerollButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if any quest is currently assigned - if so, user can't reroll quests.
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
			{
				// Later: add price.
				RerollQuests();

				// Increase achievement amount.
				User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestRerollsAmount]++;
			}
		}

		#endregion
	}
}