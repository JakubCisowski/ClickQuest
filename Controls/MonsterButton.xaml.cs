using System.Windows.Controls;
using System.Windows;
using ClickQuest.Enemies;
using ClickQuest.Places;
using System;
using ClickQuest.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
		#endregion

        private Monster _monster;
        private Region _region;
        private Random _rng = new Random();

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

        public MonsterButton(Region region)
        {            
            InitializeComponent();
            _region = region;

            SpawnMonster();
            this.DataContext = Monster;
        }

        private void MonsterButton_Click(object sender, RoutedEventArgs e)
		{
            int damage = Account.User.CurrentHero.ClickDamage;

            double num = _rng.Next(1, 101) / 100d;

            if (num <= Account.User.CurrentHero.CritChance)
            {
                damage *= 2;
            }

            Monster.CurrentHealth -= damage;

            if (Monster.CurrentHealth<=0)
            {
               SpawnMonster();
            }
        }

        public void SpawnMonster()
        {
            double num = _rng.Next(1, 101) / 100d;
            int i = 0;

            while(num > _region.Monsters[i].Frequency)
            {
                num -= _region.Monsters[i].Frequency;
                i++;
            }

            Monster = new Monster(_region.Monsters[i].Monster);
            this.DataContext = Monster;
        }
    }
}