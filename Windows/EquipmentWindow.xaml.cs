using ClickQuest.Account;
using ClickQuest.Items;
using System.Linq;
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
			ItemsListViewMaterials.ItemsSource = User.Instance.Items.Where(x => x is Material).ToList();
			ItemsListViewRecipes.ItemsSource = User.Instance.Items.Where(x => x is Recipe).ToList();
			ItemsListViewArtifacts.ItemsSource = User.Instance.Items.Where(x => x is Artifact).ToList();

			ItemsListViewMaterials.Items.Refresh();
			ItemsListViewRecipes.Items.Refresh();
			ItemsListViewArtifacts.Items.Refresh();
		}
	}
}
