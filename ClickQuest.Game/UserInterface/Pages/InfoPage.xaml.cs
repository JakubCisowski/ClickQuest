using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.UserInterface.Pages
{
	public partial class InfoPage : Page
	{
		private Page _previousPage;
		private string _previousLocationInfo;

		public InfoPage(Page previousPage, string previousLocationInfo)
		{
			InitializeComponent();

			_previousPage = previousPage;
			_previousLocationInfo = previousLocationInfo;

			RegionsListView.ItemsSource = GameAssets.Regions.Select(x => x.Name);
			DungeonsListView.ItemsSource = GameAssets.Dungeons.Select(x => x.Name);
			GameMechanicsListView.ItemsSource = GameAssets.GameMechanicsTabs.Select(x => x.Name);
		}

		private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(_previousPage, _previousLocationInfo);
		}

		private void GameMechanicsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InfoPanel.Children.Clear();

			var selectedName = e.AddedItems[0].ToString();

			var description = GameAssets.GameMechanicsTabs.FirstOrDefault(x => x.Name == selectedName).Description;

			var nameTextBlock = new TextBlock()
			{
				Text=selectedName,
				FontSize = 24,
				FontFamily = (FontFamily)this.FindResource("FontFancy"),
				TextAlignment=TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			var separator = new Separator()
			{
				Height = 2,
				Width = 400,
				Margin = new Thickness(10)
			};

			var descriptionTextBlock = new TextBlock()
			{
				FontSize = 18,
				TextAlignment=TextAlignment.Justify,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5),
				TextWrapping = TextWrapping.Wrap
			};

			descriptionTextBlock.Inlines.AddRange(DescriptionsController.GenerateDescriptionRuns(description));

			InfoPanel.Children.Add(nameTextBlock);
			InfoPanel.Children.Add(separator);
			InfoPanel.Children.Add(descriptionTextBlock);
		}
	}
}