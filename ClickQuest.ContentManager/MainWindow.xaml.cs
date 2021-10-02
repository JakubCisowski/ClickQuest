using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.UserInterface;

namespace ClickQuest.ContentManager
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl tabControl)
			{
				var currentTabName = (tabControl.SelectedItem as TabItem).Header.ToString();
				var currentTabNameAsContentType = (ContentType) Enum.Parse(typeof(ContentType), currentTabName);

				var inputPanel = new InputPanel(currentTabNameAsContentType);
				(tabControl.SelectedContent as Grid)?.Children.Clear();
				(tabControl.SelectedContent as Grid)?.Children.Add(inputPanel);
			}
		}
	}
}