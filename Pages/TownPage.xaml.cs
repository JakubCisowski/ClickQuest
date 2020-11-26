using ClickQuest.Heroes;
using ClickQuest.Places;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace ClickQuest.Pages
{
	public partial class TownPage : Page
	{
		private Hero _hero;

		public TownPage()
		{
            InitializeComponent();

            _hero = Account.User.CurrentHero;
            this.DataContext = _hero;
			
			GenerateRegionButtons();
		}

		private void GenerateRegionButtons()
		{
			for (int i=0;i<Data.Database.Regions.Count;i++)
			{
				var button = new Button()
                {
                    Name = "Region" + Data.Database.Regions[i].Id.ToString(),
                    Content = Data.Database.Regions[i].Name,
                    Width = 50,
                    Height = 50
                };

                button.Click+=RegionButton_Click;

				RegionsPanel.Children.Add(button);
			}
		}

		private void RegionButton_Click(object sender, RoutedEventArgs e)
		{
            var regionId = int.Parse((sender as Button).Name.Substring(6));
			var region = Data.Database.Regions.FirstOrDefault(x=>x.Id==regionId);

			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(new RegionPage(region));
        }

	}
}
