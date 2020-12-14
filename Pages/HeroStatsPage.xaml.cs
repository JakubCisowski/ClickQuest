using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClickQuest.Pages
{
	public partial class HeroStatsPage : Page
	{
		private Hero _hero;

		public HeroStatsPage()
		{
			InitializeComponent();

			_hero = Account.User.Instance.CurrentHero;
			this.DataContext = _hero;

			GenerateIngots();
			GenerateGold();
		}

		private void ShowEquipmentButton_Click(object sender, RoutedEventArgs e)
		{
			EquipmentWindow.Instance.Show();
		}

		private void GenerateGold()
		{
			Binding binding = new Binding("Gold");
			binding.Source = Account.User.Instance;
			binding.StringFormat = "Gold: {0}";

			GoldBlock.SetBinding(TextBlock.TextProperty, binding);
		}

		private void GenerateIngots()
		{
			// Make sure hero isn't null (while loading databse constructor calls this function).
			if (_hero != null)
			{
				IngotsPanel.Children.Clear();

				for (int i = 0; i < Account.User.Instance.Ingots.Count; i++)
				{
					var block = new TextBlock()
					{
						Name = "Ingot" + i.ToString()
					};

					Binding binding = new Binding("Quantity");
					binding.Source = Account.User.Instance.Ingots[i];
					Binding binding2 = new Binding("Rarity");
					binding2.Source = Account.User.Instance.Ingots[i];

					MultiBinding multiBinding = new MultiBinding();
					multiBinding.StringFormat = "{1} ingots: {0}";
					multiBinding.Bindings.Add(binding);
					multiBinding.Bindings.Add(binding2);

					block.SetBinding(TextBlock.TextProperty, multiBinding);

					IngotsPanel.Children.Add(block);
				}
			}
		}
	}
}