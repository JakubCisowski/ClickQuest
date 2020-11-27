using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using ClickQuest.Account;
using ClickQuest.Items;

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
			ItemsListViewMaterials.ItemsSource = User.Instance.Items.Where(x=>x is Material).ToList();
			ItemsListViewRecipes.ItemsSource = User.Instance.Items.Where(x=>x is Recipe).ToList();
			ItemsListViewArtifacts.ItemsSource = User.Instance.Items.Where(x=>x is Artifact).ToList();

            ItemsListViewMaterials.Items.Refresh();
            ItemsListViewRecipes.Items.Refresh();
            ItemsListViewArtifacts.Items.Refresh();
		}
	}
}
