using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Heroes
{
    public enum HeroClass
    {
        Slayer, Venom
    }
    // Base Hero class
    public class Hero : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Private Fields

        private string _name;
        private int _experience;
        private int _level;
        private int _gold;
        private int _clickDamage;
        private double _critChance;
        private int _poisonDamage;
        private HeroClass _heroClass;

        // Specialisations/Professions
        // Artifacts
        // Materials

        #endregion

        #region Properties

        public int ClickDamagePerLevel { get; }
        public double CritChancePerLevel { get; }
        public int PoisonDamagePerLevel { get; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public int Experience
        {
            get
            {
                return _experience;
            }
            set
            {
                _experience = value;
                Heroes.Experience.CheckIfLeveledUp(this);
                OnPropertyChanged();
            }
        }

        public int Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                _gold = value;
                OnPropertyChanged();
            }
        }

        public int ClickDamage
        {
            get
            {
                return _clickDamage;
            }
            set
            {
                _clickDamage = value;
                OnPropertyChanged();
            }
        }

        public double CritChance
        {
            get
            {
                return _critChance;

            }
            set
            {
                _critChance = value;
                OnPropertyChanged();
            }
        }

        public int PoisonDamage
        {
            get
            {
                return _poisonDamage;
            }
            set
            {
                _poisonDamage = value;
                OnPropertyChanged();
            }
        }

        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }

        public string ThisHeroClass
        {
            get
            {
                return _heroClass.ToString();
            }
        }

        #endregion

        public Hero(HeroClass heroClass, string heroName)
        {
            _heroClass = heroClass;
            Experience = 0;
            Gold = 0;
            Level = 0;
            Name = heroName;
            ClickDamagePerLevel = 1;

            switch (heroClass)
            {
                case HeroClass.Slayer:
                    ClickDamage = 2;
                    CritChance = 0.25;
                    CritChancePerLevel = 0.004;
                    PoisonDamage = 0;
                    PoisonDamagePerLevel = 0;
                    break;
                case HeroClass.Venom:
                    ClickDamage = 2;
                    CritChance = 0;
                    CritChancePerLevel = 0;
                    PoisonDamage = 1;
                    PoisonDamagePerLevel = 2;
                    break;
            }
        }

        public void GrantLevelUpBonuses()
        {
            if (Level >= 100)
            {
                return;
            }

            switch (_heroClass)
            {
                case HeroClass.Slayer:
                    ClickDamage += ClickDamagePerLevel;
                    CritChance += CritChancePerLevel;
                    break;
                case HeroClass.Venom:
                    ClickDamage += ClickDamagePerLevel;
                    PoisonDamage += PoisonDamagePerLevel;
                    break;
            }
        }
    }
}
