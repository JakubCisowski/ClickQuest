using ClickQuest.Account;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClickQuest.Pages
{
    public partial class EquipmentPage : Page
    {
        public EquipmentPage()
        {
            InitializeComponent(); 
            UpdateEquipment();
        }

        public void UpdateEquipment()
        {
            MaterialsPanel.Children.Clear();
            RecipesPanel.Children.Clear();
            ArtifactsPanel.Children.Clear();

            foreach (var material in User.Instance.Materials)
            {
                var grid = new Grid();

                var nameBlock = new TextBlock()
                {
                    FontSize=16,
                    HorizontalAlignment=HorizontalAlignment.Left
                };

                var quantityBlock = new TextBlock()
                {
                    FontSize=16,
                    HorizontalAlignment=HorizontalAlignment.Right
                };

                var binding = new Binding("Name");
                binding.Source=material;
                binding.StringFormat="{0}";
                
                var binding2 = new Binding("Quantity");
                binding2.Source=material;
                binding2.StringFormat="x{0}";

                nameBlock.SetBinding(TextBlock.TextProperty, binding);
                quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

                grid.Children.Add(nameBlock);
                grid.Children.Add(quantityBlock);

                MaterialsPanel.Children.Add(grid);
            }

            foreach (var material in User.Instance.Recipes)
            {
                var grid = new Grid();
                
                var nameBlock = new TextBlock()
                {
                    FontSize=16,
                    HorizontalAlignment=HorizontalAlignment.Left
                };

                var quantityBlock = new TextBlock()
                {
                    FontSize=16,
                    HorizontalAlignment=HorizontalAlignment.Right
                };

                var binding = new Binding("Name");
                binding.Source=material;
                binding.StringFormat="{0}";
                
                var binding2 = new Binding("Quantity");
                binding2.Source=material;
                binding2.StringFormat="x{0}";

                nameBlock.SetBinding(TextBlock.TextProperty, binding);
                quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

                grid.Children.Add(nameBlock);
                grid.Children.Add(quantityBlock);

                RecipesPanel.Children.Add(grid);
            }

            foreach (var material in User.Instance.Artifacts)
            {
                var grid = new Grid();
                
                var nameBlock = new TextBlock()
                {
                    FontSize=16,
                    HorizontalAlignment=HorizontalAlignment.Left
                };

                var quantityBlock = new TextBlock()
                {
                    FontSize=16,
                    HorizontalAlignment=HorizontalAlignment.Right
                };

                var binding = new Binding("Name");
                binding.Source=material;
                binding.StringFormat="{0}";
                
                var binding2 = new Binding("Quantity");
                binding2.Source=material;
                binding2.StringFormat="x{0}";

                nameBlock.SetBinding(TextBlock.TextProperty, binding);
                quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

                grid.Children.Add(nameBlock);
                grid.Children.Add(quantityBlock);

                ArtifactsPanel.Children.Add(grid);
            }
        }
    }
}
