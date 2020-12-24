using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ClickQuest.Heroes;

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
        #endregion

        public Quest(int id, bool rare, HeroClass heroClass, string name, int duration, string description)
        {
            Id=id;
            Rare=rare;
            HeroClass=heroClass;
            Name=name;
            Duration=duration;
            Description=description;
        }
    }
}