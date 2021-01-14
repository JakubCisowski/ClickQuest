using ClickQuest.Account;
using System.Windows;

namespace ClickQuest
{
    public partial class EquipmentWindow : Window
    {
        #region Singleton
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
        #endregion
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

        public new void Show()
        {
            _instance.Visibility = Visibility.Visible;
        }

        private void EquipmentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If the window is closed, keep it open but hide it instead.
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
