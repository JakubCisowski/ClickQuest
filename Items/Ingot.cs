using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ClickQuest.Items
{
    public partial class Ingot : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        #region Private Fields

        private Rarity _rarity;
        private int _quantity;

        #endregion Private Fields

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Rarity Rarity
        {
            get
            {
                return _rarity;
            }
            set
            {
                _rarity = value;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        public Ingot(Rarity rarity, int quantity)
        {
            Rarity = rarity;
            Quantity = quantity;
        }
    }
}