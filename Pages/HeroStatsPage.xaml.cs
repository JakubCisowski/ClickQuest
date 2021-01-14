using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClickQuest.Pages
{
    public partial class HeroStatsPage : Page
    {
        private Hero _hero;

        public HeroStatsPage()
        {
            InitializeComponent();

            _hero = Account.User.Instance.CurrentHero;
            this.DataContext = _hero;

            GenerateIngots();
            GenerateGold();
            GenerateSpecializations();
        }

        private void ShowEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            EquipmentWindow.Instance.Show();
        }

        private void GenerateGold()
        {
            Binding binding = new Binding("Gold");
            binding.Source = Account.User.Instance;
            binding.StringFormat = "Gold: {0}";

            GoldBlock.SetBinding(TextBlock.TextProperty, binding);
        }

        private void GenerateIngots()
        {
            // Make sure hero isn't null (while loading databse constructor calls this function).
            if (_hero != null)
            {
                IngotsPanel.Children.Clear();

                for (int i = 0; i < Account.User.Instance.Ingots.Count; i++)
                {
                    var block = new TextBlock()
                    {
                        Name = "Ingot" + i.ToString()
                    };

                    Binding binding = new Binding("Quantity");
                    binding.Source = Account.User.Instance.Ingots[i];
                    Binding binding2 = new Binding("Rarity");
                    binding2.Source = Account.User.Instance.Ingots[i];

                    MultiBinding multiBinding = new MultiBinding();
                    multiBinding.StringFormat = "{1} ingots: {0}";
                    multiBinding.Bindings.Add(binding);
                    multiBinding.Bindings.Add(binding2);

                    block.SetBinding(TextBlock.TextProperty, multiBinding);

                    IngotsPanel.Children.Add(block);
                }
            }
        }

        private void GenerateSpecializations()
        {
            SpecializationsPanel.Children.Clear();

            var sectionBlock = new TextBlock()
            {
                Name = "Specializations",
                Text = "Specializations:"
            };
            SpecializationsPanel.Children.Add(sectionBlock);


            {
                var block = new TextBlock()
                {
                    Name = "SpecBuying"
                };

                Binding binding = new Binding("SpecBuyingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecBuyingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecBuyingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Buying - Amount: {0}; Buff: {2}; Threshold: {1}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }

            {
                var block = new TextBlock()
                {
                    Name = "SpecMelting"
                };

                Binding binding = new Binding("SpecMeltingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecMeltingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecMeltingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Melting - Amount: {0}; Buff: {2}; Threshold: {1}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }

            {
                var block = new TextBlock()
                {
                    Name = "SpecCrafting"
                };

                Binding binding = new Binding("SpecCraftingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecCraftingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecCraftingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Crafting - Amount: {0}; Buff: {2}; Threshold: {1}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }

            {
                var block = new TextBlock()
                {
                    Name = "SpecQuesting"
                };

                Binding binding = new Binding("SpecQuestingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecQuestingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecQuestingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Questing - Amount: {0}; Buff: {2}; Threshold: {1}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }

            {
                var block = new TextBlock()
                {
                    Name = "SpecKilling"
                };

                Binding binding = new Binding("SpecKillingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecKillingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecKillingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Killing - Amount: {0}; Buff: {2}; Threshold: {1}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }

            {
                var block = new TextBlock()
                {
                    Name = "SpecBlessing"
                };

                Binding binding = new Binding("SpecBlessingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecBlessingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecBlessingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Blessing - Amount: {0}; Buff: {2}; Threshold: {1}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }

            {
                var block = new TextBlock()
                {
                    Name = "SpecDungeon"
                };

                Binding binding = new Binding("SpecDungeonAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecDungeonThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecDungeonBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Dungeon - Amount: {0}; Buff: {2}; Threshold: {1}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
        }
    }
}