using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClickQuest.Items
{
    public partial class Ingot
    {
        private Rarity _rarity;
        private int _quantity;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{ get; set; }

        public Rarity Rarity
        {
            get
            {
                return _rarity;
            }
            set
            {
                _rarity = value;
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
            }
        }

        public Ingot(Rarity rarity, int quantity)
        {
            Rarity = rarity;
            Quantity = quantity;
        }
    }
}