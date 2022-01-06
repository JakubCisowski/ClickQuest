using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls.Styles.Themes;

namespace ClickQuest.Game.UserInterface.Windows;

public partial class SettingsWindow
{
	private static SettingsWindow _instance;

	public static SettingsWindow Instance
	{
		get
		{
			if (_instance is null)
			{
				_instance = new SettingsWindow();
			}

			return _instance;
		}
	}

	public SettingsWindow()
	{
		InitializeComponent();
	}

	public new void Show()
	{
		_instance.Visibility = Visibility.Visible;

		switch (User.Instance.Theme)
		{
			case ColorTheme.Blue:
				BlueThemeBorder.BorderBrush = (SolidColorBrush)FindResource("BrushBlack");
				break;

			case ColorTheme.Orange:
				OrangeThemeBorder.BorderBrush = (SolidColorBrush)FindResource("BrushBlack");
				break;

			case ColorTheme.Pink:
				PinkThemeBorder.BorderBrush = (SolidColorBrush)FindResource("BrushBlack");
				break;
		}
	}

	private void SettingsWindow_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
		{
			DragMove();
		}
	}

	private void OkButton_OnClick(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void SettingsWindow_Closing(object sender, CancelEventArgs e)
	{
		// If the window is closed, keep it open but hide it instead.
		e.Cancel = true;
		Visibility = Visibility.Hidden;
	}

	private void ThemeBorder_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		var borderName = (sender as Border).Name;

		var selectedTheme = ColorTheme.Blue;

		if (borderName.StartsWith("Blue"))
		{
			selectedTheme = ColorTheme.Blue;
			BlueThemeBorder.BorderBrush = (SolidColorBrush)FindResource("BrushBlack");
		}
		else if (borderName.StartsWith("Orange"))
		{
			selectedTheme = ColorTheme.Orange;
			OrangeThemeBorder.BorderBrush = (SolidColorBrush)FindResource("BrushBlack");
		}
		else if (borderName.StartsWith("Pink"))
		{
			selectedTheme = ColorTheme.Pink;
			PinkThemeBorder.BorderBrush = (SolidColorBrush)FindResource("BrushBlack");
		}

		ColorsController.ChangeApplicationColorTheme(selectedTheme);
		User.Instance.Theme = selectedTheme;
	}
}