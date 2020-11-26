using System.Windows.Controls;
using System.Windows;
using ClickQuest.Places;
using System;
using ClickQuest.Controls;

namespace ClickQuest.Pages
{
    public partial class RegionPage : Page
    {
        private Region _region;
        Random rng = new Random();

        public RegionPage(Region currentRegion)
        {
            InitializeComponent();

            _region = currentRegion;
            this.DataContext = _region;

            SpawnMonster();
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(new TownPage());
		}

        private void SpawnMonster()
        {
            double num = rng.Next(1,101)/100d;
            int i = 0;

            while(num > _region.Monsters[i].Frequency)
            {
                num -= _region.Monsters[i].Frequency;
                i++;
            }

            var button = new MonsterButton(_region.Monsters[i].Monster);

            this.RegionPanel.Children.Insert(1, button);
        }
    }
}