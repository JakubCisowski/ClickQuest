using System.Linq;
using System.Windows;
using System.Windows.Input;
using ClickQuest.Game.Models;

namespace ClickQuest.Game.UserInterface.Controls;

public partial class RenameBox : Window
{
	public static string ArtifactSetName;
	private static RenameBox _messageBox;

	public RenameBox(string previousName)
	{
		InitializeComponent();
		Owner = Application.Current.MainWindow;

		RenameTextBox.Text = previousName;
	}

	public static string Show(string previousName)
	{
		ArtifactSetName = previousName;

		_messageBox = new RenameBox(previousName);

		_messageBox.RenameTextBox.Focus();
		_messageBox.RenameTextBox.CaretIndex = int.MaxValue;

		_messageBox.ShowDialog();

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
		var newName = RenameTextBox.Text;
		if (User.Instance.CurrentHero.ArtifactSets.Any(x => x.Name == newName) && newName != ArtifactSetName)
		{
			AlertBox.Show("An artifact set with this name already exists.", MessageBoxButton.OK);
			return;
		}

		ArtifactSetName = RenameTextBox.Text;
		_messageBox.Close();
		_messageBox = null;
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		_messageBox.Close();
		_messageBox = null;
	}
}