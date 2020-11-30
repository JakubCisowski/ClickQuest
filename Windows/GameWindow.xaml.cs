using System.ComponentModel;
using System.Windows;
using ClickQuest.Data;
using ClickQuest.Entity;

namespace ClickQuest
{
	public partial class GameWindow : Window
	{
		public GameWindow()
		{
			InitializeComponent();
        }

		protected override void OnClosing(CancelEventArgs e)
		{
			Entity.EntityOperations.SaveGame();

			base.OnClosing(e);
		}
	}
}
