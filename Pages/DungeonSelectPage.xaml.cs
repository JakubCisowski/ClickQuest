using System.Windows;
using System.Windows.Controls;
using ClickQuest.Data;
using ClickQuest.Enemies;
using ClickQuest.Places;

namespace ClickQuest.Pages
{
	public partial class DungeonSelectPage : Page
	{
        private DungeonGroup _dungeonGroupSelected;
        private Dungeon _dungeonSelected;
        private Monster _bossSelected;
        public DungeonSelectPage()
        {
            InitializeComponent();
            
            // Initially, display dungeon groups.
            LoadDungeonGroupSelection();
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
		{
            // Come back to town.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);

            // Reset selection.
            LoadDungeonGroupSelection();
		}

        public void LoadDungeonGroupSelection()
        {
            DungeonSelectPanel.Children.Clear();
        }

        public void LoadDungeonSelection()
        {
            DungeonSelectPanel.Children.Clear();
        }

        public void LoadBossSelection()
        {
            DungeonSelectPanel.Children.Clear();
        }
    }
}