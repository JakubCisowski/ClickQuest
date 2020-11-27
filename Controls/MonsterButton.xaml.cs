using ClickQuest.Account;
using ClickQuest.Enemies;
using ClickQuest.Heroes;
using ClickQuest.Pages;
using ClickQuest.Places;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClickQuest.Controls
{
    public partial class MonsterButton : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private Monster _monster;
        private Region _region;
        private Random _rng = new Random();
        private RegionPage _regionPage;

        public Monster Monster
        {
            get
            {
                return _monster;
            }
            set
            {
                _monster = value;
                OnPropertyChanged();
            }
        }

        public MonsterButton(Region region, RegionPage regionPage)
        {
            InitializeComponent();
            _region = region;
            _regionPage = regionPage;

            SpawnMonster();
            this.DataContext = Monster;
        }

        private void MonsterButton_Click(object sender, RoutedEventArgs e)
        {
            int damage = Account.User.Instance.CurrentHero.ClickDamage;

            double num = _rng.Next(1, 101) / 100d;

            if (num <= Account.User.Instance.CurrentHero.CritChance)
            {
                damage *= 2;
            }

            Monster.CurrentHealth -= damage;

            if (Monster.CurrentHealth <= 0)
            {
                GrantVictoryBonuses();
                SpawnMonster();
            }
        }

        public void SpawnMonster()
        {
            double num = _rng.Next(1, 10001) / 10000d;
            int i = 0;

            while (num > _region.Monsters[i].Frequency)
            {
                num -= _region.Monsters[i].Frequency;
                i++;
            }

            Monster = new Monster(_region.Monsters[i].Monster);
            this.DataContext = Monster;
        }

        public void GrantVictoryBonuses()
        {
            // Granting EXP (based on moster hp)
            User.Instance.CurrentHero.Experience += Experience.CalculateMonsterXpReward(_monster.Health);

            // Granting Loot
            double num = _rng.Next(1, 10001) / 10000d;
            int i = 0;

            while (num > _monster.Loot[i].Frequency)
            {
                num -= _monster.Loot[i].Frequency;
                i++;
            }
            _regionPage.TestRewardsBlock.Text = "Loot: " + _monster.Loot[i].Item.Name + ", Exp: " + Experience.CalculateMonsterXpReward(_monster.Health);

            // Not adding "empty loot" to inventory
            if (_monster.Loot[i].Item.Id != 0)
            {
                _monster.Loot[i].Item.Quantity++;
                if (!Account.User.Instance.Items.Contains(_monster.Loot[i].Item))
                {
                    Account.User.Instance.Items.Add(_monster.Loot[i].Item);
                }
                EquipmentWindow.Instance.UpdateEquipment();
            }
        }
    }
}