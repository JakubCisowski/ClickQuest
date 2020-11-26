using System.Windows.Controls;
using System.Windows;
using ClickQuest.Places;
using ClickQuest.Heroes;

namespace ClickQuest.Pages
{
    public partial class HeroStatsPage : Page
    {
        private Hero _hero;

        public HeroStatsPage()
        {
            InitializeComponent();

            _hero = Account.User.CurrentHero;
            this.DataContext=_hero;
        }
    }
}