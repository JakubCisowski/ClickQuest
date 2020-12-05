using ClickQuest.Account;
using System.Windows;

namespace ClickQuest
{
	public partial class EquipmentWindow : Window
	{
		private static EquipmentWindow _instance;
		public static EquipmentWindow Instance
		{
			get
			{
				if (_instance is null)
				{
					_instance = new EquipmentWindow();
				}
				return _instance;
			}
		}
		private EquipmentWindow()
		{
			InitializeComponent();

			UpdateEquipment();
		}

		public void UpdateEquipment()
		{
			ItemsListViewMaterials.ItemsSource = User.Instance.Materials;
			ItemsListViewRecipes.ItemsSource = User.Instance.Recipes;
			ItemsListViewArtifacts.ItemsSource = User.Instance.Artifacts;

			ItemsListViewMaterials.Items.Refresh();
			ItemsListViewRecipes.Items.Refresh();
			ItemsListViewArtifacts.Items.Refresh();
		}
	}
}
