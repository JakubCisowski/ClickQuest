using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Helpers.ToolTips;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Game.UserInterface.Controls;

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
			var star1 = new PackIcon
			{
				Width = 25,
				Height = 25,
				Kind = PackIconKind.Star,
				Foreground = (SolidColorBrush)FindResource("BrushQuestRare"),
				VerticalAlignment = VerticalAlignment.Center
			};

			var star2 = new PackIcon
			{
				Width = 25,
				Height = 25,
				Kind = PackIconKind.Star,
				Foreground = (SolidColorBrush)FindResource("BrushQuestRare"),
				VerticalAlignment = VerticalAlignment.Center
			};

			QuestNamePanel.Children.Insert(0, star1);
			QuestNamePanel.Children.Add(star2);
		}
	}

	public void GenerateRewardsInterface()
	{
		foreach (var rewardPattern in _quest.QuestRewardPatterns)
		{
			var panel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 0, 0, 5)
			};

			var rewardIcon = new PackIcon
			{
				Width = 30,
				Height = 30,
				VerticalAlignment = VerticalAlignment.Center
			};

			var rewardText = new TextBlock
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

					toolTip = ItemToolTipHelper.GenerateItemToolTip(material);

					rewardIcon.Kind = PackIconKind.Cog;

					rewardText.Text = $"{rewardPattern.Quantity}x {material.Name}";

					rewardColor = ColorsHelper.GetRarityColor(material.Rarity);

					break;

				case RewardType.Recipe:
					var recipe = GameAssets.Recipes.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

					toolTip = ItemToolTipHelper.GenerateItemToolTip(recipe);

					rewardIcon.Kind = PackIconKind.ScriptText;

					rewardText.Text = $"{rewardPattern.Quantity}x {recipe.Name}";

					rewardColor = ColorsHelper.GetRarityColor(recipe.Rarity);

					break;

				case RewardType.Artifact:
					var artifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

					toolTip = ItemToolTipHelper.GenerateItemToolTip(artifact);

					rewardIcon.Kind = PackIconKind.DiamondStone;

					rewardText.Text = $"{rewardPattern.Quantity}x {artifact.Name}";

					rewardColor = ColorsHelper.GetRarityColor(artifact.Rarity);

					break;

				case RewardType.Blessing:
					var blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

					toolTip = ItemToolTipHelper.GenerateBlessingToolTip(blessing);

					rewardIcon.Kind = PackIconKind.BookCross;

					rewardText.Text = $"{blessing.Name}";

					rewardColor = ColorsHelper.GetRarityColor(blessing.Rarity);

					break;

				case RewardType.Ingot:
					var ingot = GameAssets.Ingots.FirstOrDefault(x => x.Id == rewardPattern.QuestRewardId);

					toolTip = ItemToolTipHelper.GenerateCurrencyToolTip<Ingot>((int)ingot.Rarity);

					rewardIcon.Kind = PackIconKind.Gold;

					rewardText.Text = $"{rewardPattern.Quantity}x {ingot.Name}";

					rewardColor = ColorsHelper.GetRarityColor(ingot.Rarity);

					break;
			}

			rewardIcon.Foreground = rewardColor;
			rewardText.Foreground = rewardColor;

			panel.Children.Add(rewardIcon);
			panel.Children.Add(rewardText);

			GeneralToolTipHelper.SetToolTipDelayAndDuration(panel);

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
		else
		{
			AlertBox.Show("Your hero is already completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
		}
	}
}