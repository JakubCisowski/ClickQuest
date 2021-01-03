using ClickQuest.Heroes;
using ClickQuest.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace ClickQuest.Items
{
    public partial class Quest
    {
        #region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

        #region Private Fields
        private int _id;
        private bool _rare;
        private HeroClass _heroClass;
        private string _name;
        private int _duration;
        private string _description;
        private List<int> _rewardRecipeIds;
        private List<int> _rewardMaterialIds;
        private List<int> _rewardBlessingIds;
        private List<Rarity> _rewardIngots;
        private DispatcherTimer _timer;
        private DateTime _endDate;
        #endregion

        #region Properties
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DbKey { get; set; }
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id=value;
                OnPropertyChanged();
            }
        }
        [NotMapped]
        public bool Rare
        {
            get
            {
                return _rare;
            }
            set
            {
                _rare=value;
                OnPropertyChanged();
            }
        }
        [NotMapped]
        public HeroClass HeroClass
        {
            get
            {
                return _heroClass;
            }
            set
            {
                _heroClass=value;
                OnPropertyChanged();
            }
        }
        [NotMapped]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name=value;
                OnPropertyChanged();
            }
        }
        [NotMapped]
        public int Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration=value;
                OnPropertyChanged();
            }
        }
        [NotMapped]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description=value;
                OnPropertyChanged();
            }
        }
        [NotMapped]
        public List<int> RewardRecipeIds
        {
            get
            {
                return _rewardRecipeIds;
            }
            set
            {
                _rewardRecipeIds=value;
            }
        }
        [NotMapped]
        public List<int> RewardMaterialIds
        {
            get
            {
                return _rewardMaterialIds;
            }
            set
            {
                _rewardMaterialIds=value;
            }
        }
        [NotMapped]
        public List<int> RewardBlessingIds
        {
            get
            {
                return _rewardBlessingIds;
            }
            set
            {
                _rewardBlessingIds=value;
            }
        }
        [NotMapped]
        public List<Rarity> RewardIngots
        {
            get
            {
                return _rewardIngots;
            }
            set
            {
                _rewardIngots=value;
            }
        }
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate=value;
                OnPropertyChanged();
            }
        }
        #endregion

        public void CopyQuest(Quest quest)
        {
            Rare=quest.Rare;
            HeroClass=quest.HeroClass;
            Name=quest.Name;
            Duration=quest.Duration;
            Description=quest.Description;

            RewardRecipeIds=quest.RewardRecipeIds;
            RewardMaterialIds=quest.RewardMaterialIds;
            RewardBlessingIds=quest.RewardBlessingIds;
            RewardIngots=quest.RewardIngots;
        }

        public void StartQuest()
        {
            // Set quest end date (if not yet set).
            if (EndDate!=default(DateTime))
            {
                this.EndDate = DateTime.Now.AddSeconds(Duration);
            }
            
            // Start timer (checks if quest is finished).
            _timer.Start();
        }

        public void StopQuest()
        {   
            // Stop timer.
            _timer.Stop();

            // todo: Assign rewards.

            // Reroll new set of 3 quests.
            (Data.Database.Pages["QuestMenu"] as QuestMenuPage).RerollQuests();
        }

        private void Timer_Tick(object source, EventArgs e)
        {
            // Check if quest is finished.
            if (DateTime.Now>=EndDate)
            {
                StopQuest();
            }
        }

        public Quest()
        {
            RewardRecipeIds = new List<int>();
            RewardMaterialIds = new List<int>();
            RewardBlessingIds = new List<int>();
            RewardIngots = new List<Rarity>();
            _timer = new DispatcherTimer();
            _timer.Interval=new TimeSpan(0,0,0,1);
            _timer.Tick+=Timer_Tick;
        }

        public Quest(int id, bool rare, HeroClass heroClass, string name, int duration, string description)
        {
            Id=id;
            Rare=rare;
            HeroClass=heroClass;
            Name=name;
            Duration=duration;
            Description=description;

            RewardRecipeIds = new List<int>();
            RewardMaterialIds = new List<int>();
            RewardBlessingIds = new List<int>();
            RewardIngots = new List<Rarity>();
            _timer = new DispatcherTimer();
            _timer.Interval=new TimeSpan(0,0,0,1);
            _timer.Tick+=Timer_Tick;
        }
    }
}