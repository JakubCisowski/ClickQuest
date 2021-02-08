using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using System.Windows.Documents;

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
            GenerateDungeonKeys();
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
            // Make sure hero isn't null (constructor calls this function while loading database).
            if (_hero != null)
            {
                IngotKeyGrid.Children.Clear();

                for (int i = 0; i < Account.User.Instance.Ingots.Count; i++)
                {
                    var panel = new StackPanel()
                    {
                        Orientation=Orientation.Horizontal,
                        Margin= new Thickness(50,0,0,0)
                    };

                    var icon = new PackIcon()
                    {
                        Kind=PackIconKind.Gold,
                        Width=22,
                        Height=22,
                        VerticalAlignment=VerticalAlignment.Center
                    };

                    switch (i)
                    {
                        case 0:
                            icon.Foreground=new SolidColorBrush(Colors.Gray);
                            break;

                        case 1:
                            icon.Foreground=new SolidColorBrush(Colors.Brown);
                            break;

                        case 2:
                            icon.Foreground=new SolidColorBrush(Colors.Green);
                            break;

                        case 3:
                            icon.Foreground=new SolidColorBrush(Colors.Blue);
                            break;
                            
                        case 4:
                            icon.Foreground=new SolidColorBrush(Colors.Purple);
                            break;

                        case 5:
                            icon.Foreground=new SolidColorBrush(Colors.Gold);
                            break;
                    }
                    
                    panel.Children.Add(icon);

                    var block = new TextBlock()
                    {
                        Name = "Ingot" + i.ToString(),
                        FontSize = 18,
                        VerticalAlignment=VerticalAlignment.Center
                    };

                    Binding binding = new Binding("Quantity");
                    binding.Source = Account.User.Instance.Ingots[i];
                    // Binding binding2 = new Binding("Rarity");
                    // binding2.Source = Account.User.Instance.Ingots[i];

                    MultiBinding multiBinding = new MultiBinding();
                    multiBinding.StringFormat = "   {0}";
                    multiBinding.Bindings.Add(binding);
                    // multiBinding.Bindings.Add(binding2);

                    block.SetBinding(TextBlock.TextProperty, multiBinding);

                    panel.Children.Add(block);

                    IngotKeyGrid.Children.Add(panel);

                    Grid.SetColumn(panel, 0);
                    Grid.SetRow(panel, i);
                }
            }
        }

        private void GenerateDungeonKeys()
        {
            //Make sure hero isn't null (constructor calls this function while loading database).
            if (_hero != null)
            {
                for (int i = 0; i < Account.User.Instance.DungeonKeys.Count; i++)
                {
                    var panel = new StackPanel()
                    {
                        Orientation=Orientation.Horizontal,
                        Margin= new Thickness(50,0,0,0)
                    };

                    var icon = new PackIcon()
                    {
                        Kind=PackIconKind.Key,
                        Width=22,
                        Height=22,
                        VerticalAlignment=VerticalAlignment.Center
                    };

                    switch (i)
                    {
                        case 0:
                            icon.Foreground=new SolidColorBrush(Colors.Gray);
                            break;

                        case 1:
                            icon.Foreground=new SolidColorBrush(Colors.Brown);
                            break;

                        case 2:
                            icon.Foreground=new SolidColorBrush(Colors.Green);
                            break;

                        case 3:
                            icon.Foreground=new SolidColorBrush(Colors.Blue);
                            break;
                            
                        case 4:
                            icon.Foreground=new SolidColorBrush(Colors.Purple);
                            break;

                        case 5:
                            icon.Foreground=new SolidColorBrush(Colors.Gold);
                            break;
                    }

                    panel.Children.Add(icon);

                    var block = new TextBlock()
                    {
                        Name = "Key" + i.ToString(),
                        FontSize = 18,
                        VerticalAlignment=VerticalAlignment.Center
                    };

                    Binding binding = new Binding("Quantity");
                    binding.Source = Account.User.Instance.DungeonKeys[i];
                    // Binding binding2 = new Binding("Rarity");
                    // binding2.Source = Account.User.Instance.DungeonKeys[i];

                    MultiBinding multiBinding = new MultiBinding();
                    multiBinding.StringFormat = "   {0}";
                    multiBinding.Bindings.Add(binding);
                    // multiBinding.Bindings.Add(binding2);

                    block.SetBinding(TextBlock.TextProperty, multiBinding);

                    panel.Children.Add(block);

                    IngotKeyGrid.Children.Add(panel);

                    Grid.SetColumn(panel, 1);
                    Grid.SetRow(panel, i);
                }
            }
        }

        private void GenerateSpecializations()
        {
            SpecializationsPanel.Children.Clear();

            var sectionBlock = new TextBlock()
            {
                Name = "Specializations",
                Text = "Specializations:",
                FontSize=20,
                FontWeight = FontWeights.Medium,
                HorizontalAlignment=HorizontalAlignment.Center,
                Margin = new Thickness(0,3,0,10)
            };
            SpecializationsPanel.Children.Add(sectionBlock);

            #region SpecializationBuying
            {
                var block = new TextBlock()
                {
                    Name = "SpecBuying",
                    FontSize = 18
                };

                Binding binding = new Binding("SpecBuyingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecBuyingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecBuyingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Tradesman → Shop offer size + {2}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
            #endregion

            #region SpecializationMelting
            {
                var block = new TextBlock()
                {
                    Name = "SpecMelting",
                    FontSize = 18
                };

                Binding binding = new Binding("SpecMeltingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecMeltingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecMeltingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Melter → Extra ingot + {2}%";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
            #endregion

            #region SpecializationCrafting
            {
                var block = new TextBlock()
                {
                    Name = "SpecCrafting",
                    FontSize = 18
                };

                Binding binding = new Binding("SpecCraftingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecCraftingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecCraftingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Craftsman → Can craft + {2}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
            #endregion

            #region SpecializationQuesting
            {
                var block = new TextBlock()
                {
                    Name = "SpecQuesting",
                    FontSize = 18
                };

                Binding binding = new Binding("SpecQuestingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecQuestingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecQuestingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Adventurer → Quest time - {2}%";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
            #endregion

            #region SpecializationKilling
            {
                var block = new TextBlock()
                {
                    Name = "SpecKilling",
                    FontSize = 18
                };

                Binding binding = new Binding("SpecKillingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecKillingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecKillingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Clicker → Click damage + {2}";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
            #endregion

            #region SpecializationBlessing
            {
                var block = new TextBlock()
                {
                    Name = "SpecBlessing",
                    FontSize = 18
                };

                Binding binding = new Binding("SpecBlessingAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecBlessingThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecBlessingBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Prayer → Blessing duration + {2}s";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
            #endregion

            #region SpecializationDungeon
            {
                var block = new TextBlock()
                {
                    Name = "SpecDungeon",
                    FontSize = 18
                };

                Binding binding = new Binding("SpecDungeonAmount");
                binding.Source = Account.User.Instance.Specialization;
                Binding binding2 = new Binding("SpecDungeonThreshold");
                binding2.Source = Account.User.Instance.Specialization;
                Binding binding3 = new Binding("SpecDungeonBuff");
                binding3.Source = Account.User.Instance.Specialization;

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.StringFormat = "Daredevil → Bossfight timer + {2}s";
                multiBinding.Bindings.Add(binding);
                multiBinding.Bindings.Add(binding2);
                multiBinding.Bindings.Add(binding3);

                block.SetBinding(TextBlock.TextProperty, multiBinding);

                SpecializationsPanel.Children.Add(block);
            }
            #endregion
        }
    }
}