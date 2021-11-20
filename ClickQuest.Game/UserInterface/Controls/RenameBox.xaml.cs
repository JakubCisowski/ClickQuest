using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.UserInterface.Controls
{
	public partial class RenameBox : Window
	{
		public static string ArtifactSetName;
		private static RenameBox MessageBox;

		public RenameBox(string previousName)
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;

			RenameTextBox.Text = previousName;
		}

		public static string Show(string previousName)
		{
			ArtifactSetName = previousName;

			MessageBox = new RenameBox(previousName);

			MessageBox.ShowDialog();

			return ArtifactSetName;
		}

		private void RenameBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (User.Instance.CurrentHero.ArtifactSets.Any(x=>x.Name == RenameTextBox.Text))
		 	{
				AlertBox.Show("An artifact set with this name already exists.", MessageBoxButton.OK);
				return;
			}

			ArtifactSetName = RenameTextBox.Text;
			MessageBox.Close();
			MessageBox = null;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Close();
			MessageBox = null;
		}
	}
}