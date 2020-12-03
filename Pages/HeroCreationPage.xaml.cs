using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;
using ClickQuest.Account;
using ClickQuest.Items;
using ClickQuest.Data;
using ClickQuest.Heroes;

namespace ClickQuest.Pages
{
	public partial class HeroCreationPage : Page
	{
        public HeroCreationPage()
        {
            InitializeComponent();

            // Populate ComboBox with hero classes and pre-select the first element.
            HeroClassBox.ItemsSource = Enum.GetValues(typeof(HeroClass)).Cast<HeroClass>();
            HeroClassBox.SelectedIndex = 0;
        }

        public void CreateHeroButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if hero name is chosen.
            if (!string.IsNullOrEmpty(HeroNameBox.Text))
            {
                // Create hero based on user inputs and select it.
                var hero = new Hero((HeroClass)Enum.Parse(typeof(HeroClass),HeroClassBox.SelectedValue.ToString()), HeroNameBox.Text);
                User.Instance.Heroes.Add(hero);
                User.Instance.CurrentHero = hero;

                // Refresh bindings.
                Data.Database.RefreshPages();

                (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
            }
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["MainMenu"]);
        }
	}
}