using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;

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
        }

        private void ShowEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            EquipmentWindow.Instance.Show();
        }

        private void GenerateIngots()
        {
            // Make sure hero isn't null (while loading databse constructor calls this function).
            if(_hero != null)
			{
				IngotsPanel.Children.Clear();

				for (int i = 0; i < _hero.Ingots.Count; i++)
				{
					var block = new TextBlock()
					{
						Name = "Ingot" + i.ToString(),
						Text = _hero.Ingots[i].Rarity + " ingots: " + _hero.Ingots[i].Quantity.ToString()
					};

					IngotsPanel.Children.Add(block);
				}
			}
        }
    }
}