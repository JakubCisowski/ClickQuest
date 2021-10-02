using System.Linq;
using System.Windows.Controls;
using ClickQuest.ContentManager.Models;

namespace ClickQuest.ContentManager.UserInterface
{
	// zrobić jakiś osobny control typu StackPanel i tam, w zależności od datacontext (typu obiektu i jego pól) generować textboxy itp
	public partial class InputPanel : UserControl
	{
		private ContentType _currentContentType;
		private object _dataContext;

		public InputPanel(ContentType contentType)
		{
			InitializeComponent();

			_currentContentType = contentType;

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			switch (_currentContentType)
			{
				case ContentType.Artifacts:
					ContentSelectionBox.ItemsSource = GameContent.Artifacts.Select(x => x.Name);
					break;

				case ContentType.Materials:
					ContentSelectionBox.ItemsSource = GameContent.Materials.Select(x => x.Name);
					break;

				case ContentType.Recipes:
					ContentSelectionBox.ItemsSource = GameContent.Recipes.Select(x => x.Name);
					break;

				case ContentType.Blessings:
					ContentSelectionBox.ItemsSource = GameContent.Blessings.Select(x => x.Name);
					break;

				case ContentType.Monsters:
					ContentSelectionBox.ItemsSource = GameContent.Monsters.Select(x => x.Name);
					break;

				case ContentType.Bosses:
					ContentSelectionBox.ItemsSource = GameContent.Bosses.Select(x => x.Name);
					break;

				case ContentType.Regions:
					ContentSelectionBox.ItemsSource = GameContent.Regions.Select(x => x.Name);
					break;

				case ContentType.Quests:
					ContentSelectionBox.ItemsSource = GameContent.Quests.Select(x => x.Name);
					break;

				case ContentType.Shop:
					// special
					break;

				case ContentType.Priest:
					// special
					break;
			}
		}

		public void RefreshControls<T>()
		{
			// https://stackoverflow.com/questions/63834841/how-to-add-a-materialdesignhint-to-a-textbox-in-code
			// ?
			// var idBox = new TextBox();
			// HintAssist.SetHint(idBox, "ID");
			
			// clear grid's first column to avoid duplicating the controls added below
			// how?

			var panel = new StackPanel();

			if (typeof(T) == typeof(Artifact))
			{
				var selectedArtifact = _dataContext as Artifact;

				var idBox = new TextBox() {Text = selectedArtifact.Id.ToString()};
				var nameBox = new TextBox() {Text = selectedArtifact.Name.ToString()};
				var valueBox = new TextBox() {Text = selectedArtifact.Value.ToString()};

				var makeChangesButton = new Button()
				{
					Content = "Make changes"
				};
				makeChangesButton.Click += MakeChangesButton_Click;

				panel.Children.Add(idBox);
				panel.Children.Add(nameBox);
				panel.Children.Add(valueBox);
				panel.Children.Add(makeChangesButton);
			}
			
			Grid.SetColumn(panel, 1);

			MainGrid.Children.Add(panel);
		}
		
		private void MakeChangesButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{

		}

		private void AddNewButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{

		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (e.Source as ComboBox)?.SelectedValue.ToString();

			// nie wiem czy nie trzeba jednak oddzielnych paneli od poszczególnych kontrolek bo sie robi zadyma tutaj
			
			switch (_currentContentType)
			{
				case ContentType.Artifacts:
					_dataContext = GameContent.Artifacts.FirstOrDefault(x => x.Name == selectedName);
					RefreshControls<Artifact>();
					break;
			}
		}
	}
}