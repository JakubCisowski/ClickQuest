using ClickQuest.Heroes;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class HeroStatsPage : Page
	{
		private Hero _hero;

		public HeroStatsPage()
		{
			InitializeComponent();

			_hero = Account.User.CurrentHero;
			this.DataContext = _hero;
		}
	}
}