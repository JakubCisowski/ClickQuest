using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Heroes;
using ClickQuest.Items;
using ClickQuest.Adventures;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Extensions.InterfaceManager;

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
			User.Instance.CurrentHero.Quests.Clear();
			EntityOperations.RemoveQuests();

			var questsForCurrentHeroClass = GameData.Quests.Where(x => x.HeroClass == User.Instance.CurrentHero.HeroClass || x.HeroClass == HeroClass.All).ToList();

			for (int i = 0; i < 3; i++)
			{
				int randomizedIndex = _rng.Next(0, questsForCurrentHeroClass.Count());

				bool isQuestRare = questsForCurrentHeroClass.ElementAt(randomizedIndex).Rare == true;
				if (isQuestRare)
				{
					// Randomize once again.
					randomizedIndex = _rng.Next(0, questsForCurrentHeroClass.Count());
				}

				var randomizedQuest = questsForCurrentHeroClass.ElementAt(randomizedIndex).CopyQuest();
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

			for (int i = 0; i < User.Instance.CurrentHero.Quests.Count; i++)
			{
				var button = new QuestButton(User.Instance.CurrentHero.Quests[i]);

				QuestPanel.Children.Add(button);
			}
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

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Town.
			InterfaceController.ChangePage(Data.GameData.Pages["Town"], "Town");
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