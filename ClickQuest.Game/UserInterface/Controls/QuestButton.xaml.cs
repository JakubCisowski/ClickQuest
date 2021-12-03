using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.Extensions.UserInterface.ToolTips;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Game.UserInterface.Controls
{
	public partial class QuestButton : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly Quest _quest;

		public QuestButton(Quest quest)
		{
			InitializeComponent();

			_quest = quest;
			DataContext = _quest;

			GenerateRewardsInterface();

			if (_quest.Rare)
			{
				PackIcon star1 = new PackIcon
				{
					Width = 25,
					Height = 25,
					Kind = PackIconKind.Star,
					Foreground = (SolidColorBrush) FindResource("BrushQuestRare"),
					VerticalAlignment = VerticalAlignment.Center
				};

				PackIcon star2 = new PackIcon
				{
					Width = 25,
					Height = 25,
					Kind = PackIconKind.Star,
					Foreground = (SolidColorBrush) FindResource("BrushQuestRare"),
					VerticalAlignment = VerticalAlignment.Center
				};

				QuestNamePanel.Children.Insert(0, star1);
				QuestNamePanel.Children.Add(star2);
			}
		}

		public void GenerateRewardsInterface()
		{
			foreach (QuestRewardPattern rewardPattern in _quest.QuestRewardPatterns)
			{
				StackPanel panel = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(0, 0, 0, 5)
				};

				PackIcon rewardIcon = new PackIcon
				{
					Width = 30,
					Height = 30,
					VerticalAlignment = VerticalAlignment.Center
				};

				TextBlock rewardText = new TextBlock
				{
					FontSize = 22,
					VerticalAlignment = VerticalAlignment.Center
				};

				ToolTip toolTip = null;

				SolidColorBrush rewardColor = null;

				switch (rewardPattern.QuestRewardType)
				{
					case RewardType.Material:
						var material = GameAssets.Materials.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

						toolTip = ItemToolTipController.GenerateItemToolTip(material);

						rewardIcon.Kind = PackIconKind.Cog;

						rewardText.Text = $"{rewardPattern.Quantity}x {material.Name}";

						rewardColor = ColorsController.GetRarityColor(material.Rarity);

						break;

					case RewardType.Recipe:
						var recipe = GameAssets.Recipes.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

						toolTip = ItemToolTipController.GenerateItemToolTip(recipe);

						rewardIcon.Kind = PackIconKind.ScriptText;

						rewardText.Text = $"{rewardPattern.Quantity}x {recipe.Name}";

						rewardColor = ColorsController.GetRarityColor(recipe.Rarity);

						break;

					case RewardType.Artifact:
						var artifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

						toolTip = ItemToolTipController.GenerateItemToolTip(artifact);

						rewardIcon.Kind = PackIconKind.DiamondStone;

						rewardText.Text = $"{rewardPattern.Quantity}x {artifact.Name}";

						rewardColor = ColorsController.GetRarityColor(artifact.Rarity);

						break;

					case RewardType.Blessing:
						var blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

						toolTip = ItemToolTipController.GenerateBlessingToolTip(blessing);

						rewardIcon.Kind = PackIconKind.BookCross;

						rewardText.Text = $"{blessing.Name}";

						rewardColor = ColorsController.GetRarityColor(blessing.Rarity);

						break;

					case RewardType.Ingot:
						var ingot = GameAssets.Ingots.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

						toolTip = ItemToolTipController.GenerateCurrencyToolTip<Ingot>((int) ingot.Rarity);

						rewardIcon.Kind = PackIconKind.Gold;

						rewardText.Text = $"{rewardPattern.Quantity}x {ingot.Name}";

						rewardColor = ColorsController.GetRarityColor(ingot.Rarity);

						break;
				}

				rewardIcon.Foreground = rewardColor;
				rewardText.Foreground = rewardColor;

				panel.Children.Add(rewardIcon);
				panel.Children.Add(rewardText);

				GeneralToolTipController.SetToolTipDelayAndDuration(panel);

				panel.ToolTip = toolTip;

				RewardsPanel.Children.Add(panel);
			}
		}

		private void QuestButton_Click(object sender, RoutedEventArgs e)
		{
			// Start this quest (if another one isn't currently assigned).
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
			{
				_quest.StartQuest();
			}
		}
	}
}