using ClickQuest.Data;
using ClickQuest.Items;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClickQuest.Controls
{
    public partial class QuestButton : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private Quest _quest;

        public QuestButton(Quest quest)
        {
            InitializeComponent();

            _quest = quest;
            this.DataContext = _quest;

            GenerateRewards();
        }

        public void GenerateRewards()
        {
            #region Blessings

            var counter = 1;
            var previousId = 0;

            for (int i = 0; i < _quest.RewardBlessingIds.Count; i++)
            {
                // If reward id stays the same (or it's first id in the list) - increment the counter.
                if ((i == 0 || previousId == _quest.RewardBlessingIds[i]) && (i != _quest.RewardBlessingIds.Count - 1))
                {
                    counter++;
                }
                // New reward id / last id in the list - display reward info on the button.
                else
                {
                    var block = new TextBlock()
                    {
                        Name = "RewardBlessing" + i.ToString()
                    };

                    block.Text = $"{counter}x {Database.Blessings.FirstOrDefault(x => x.Id == _quest.RewardBlessingIds[i]).Name}";
                    QuestRewardsPanel.Children.Add(block);

                    counter = 1;
                }

                previousId = _quest.RewardBlessingIds[i];
            }

            #endregion Blessings

            #region Materials

            counter = 1;
            previousId = 0;

            for (int i = 0; i < _quest.RewardMaterialIds.Count; i++)
            {
                // If reward id stays the same (or it's first id in the list) - increment the counter.
                if ((i == 0 || previousId == _quest.RewardMaterialIds[i]) && (i != _quest.RewardMaterialIds.Count - 1))
                {
                    counter++;
                }
                // New reward id / last id in the list - display reward info on the button.
                else
                {
                    var block = new TextBlock()
                    {
                        Name = "RewardNaterial" + i.ToString()
                    };

                    block.Text = $"{counter}x {Database.Materials.FirstOrDefault(x => x.Id == _quest.RewardMaterialIds[i]).Name}";
                    QuestRewardsPanel.Children.Add(block);

                    counter = 1;
                }

                previousId = _quest.RewardMaterialIds[i];
            }

            #endregion Materials

            #region Recipes

            counter = 1;
            previousId = 0;

            for (int i = 0; i < _quest.RewardRecipeIds.Count; i++)
            {
                // If reward id stays the same (or it's first id in the list) - increment the counter.
                if ((i == 0 || previousId == _quest.RewardRecipeIds[i]) && (i != _quest.RewardRecipeIds.Count - 1))
                {
                    counter++;
                }
                // New reward id / last id in the list - display reward info on the button.
                else
                {
                    var block = new TextBlock()
                    {
                        Name = "RewardRecipe" + i.ToString()
                    };

                    block.Text = $"{counter}x {Database.Recipes.FirstOrDefault(x => x.Id == _quest.RewardRecipeIds[i]).Name}";
                    QuestRewardsPanel.Children.Add(block);

                    counter = 1;
                }

                previousId = _quest.RewardRecipeIds[i];
            }

            #endregion Recipes

            #region Ingots

            counter = 1;
            previousId = 0;

            for (int i = 0; i < _quest.RewardIngots.Count; i++)
            {
                // If reward id stays the same (or it's first id in the list) - increment the counter.
                if ((i == 0 || previousId == (int)_quest.RewardIngots[i]) && (i != _quest.RewardIngots.Count - 1))
                {
                    counter++;
                }
                // New reward id / last id in the list - display reward info on the button.
                else
                {
                    var block = new TextBlock()
                    {
                        Name = "RewardIngot" + i.ToString()
                    };

                    block.Text = counter > 1 ? $"{counter}x {_quest.RewardIngots[i].ToString()} Ingots" : $"{counter}x {_quest.RewardIngots[i].ToString()} Ingot";
                    QuestRewardsPanel.Children.Add(block);

                    counter = 1;
                }

                previousId = (int)_quest.RewardIngots[i];
            }

            #endregion Ingots
        }

        private void QuestButton_Click(object sender, RoutedEventArgs e)
        {
            // Start this quest (if another one isnt currently assigned).
            if (Account.User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
            {
                _quest.StartQuest();
            }
        }
    }
}