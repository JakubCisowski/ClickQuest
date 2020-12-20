using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace ClickQuest.Account
{
    public partial class Specialization : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

        #region Singleton

        private static Specialization _instance;

        public static Specialization Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new Specialization();
                }
                return _instance;
            }
        }

        #endregion Singleton

        #region Private Fields

        private int specBuyingAmount;
        private int specBuyingThreshold;
        private int specBuyingBuff;

        private int specMeltingAmount;
        private int specMeltingThreshold;
        private int specMeltingBuff;

        private int specCraftingAmount;
        private int specCraftingThreshold;
        private int specCraftingBuff;

        private int specQuestingAmount;
        private int specQuestingThreshold;
        private int specQuestingBuff;

        private int specBlessingAmount;
        private int specBlessingThreshold;
        private int specBlessingBuff;

        private int specKillingAmount;
        private int specKillingThreshold;
        private int specKillingBuff;

        #endregion Private Fields

        #region Properties

        public int SpecBuyingAmount
        {
            get
            {
                return specBuyingAmount;
            }
            set
            {
                specBuyingAmount = value;
                UpdateBuffs();
                OnPropertyChanged();
            }
        }

        public int SpecBuyingThreshold
        {
            get
            {
                return specBuyingThreshold;
            }
            set
            {
                specBuyingThreshold = value;
                OnPropertyChanged();
            }
        }

        public int SpecBuyingBuff
        {
            get
            {
                return specBuyingBuff;
            }
            set
            {
                specBuyingBuff = value;
                OnPropertyChanged();
            }
        }

        public int SpecMeltingAmount
        {
            get
            {
                return specMeltingAmount;
            }
            set
            {
                specMeltingAmount = value;
                UpdateBuffs();
                OnPropertyChanged();
            }
        }

        public int SpecMeltingThreshold
        {
            get
            {
                return specMeltingThreshold;
            }
            set
            {
                specMeltingThreshold = value;
                OnPropertyChanged();
            }
        }

        public int SpecMeltingBuff
        {
            get
            {
                return specMeltingBuff;
            }
            set
            {
                specMeltingBuff = value;
                OnPropertyChanged();
            }
        }

        public int SpecCraftingAmount
        {
            get
            {
                return specCraftingAmount;
            }
            set
            {
                specCraftingAmount = value;
                UpdateBuffs();
                OnPropertyChanged();
            }
        }

        public int SpecCraftingThreshold
        {
            get
            {
                return specCraftingThreshold;
            }
            set
            {
                specCraftingThreshold = value;
                OnPropertyChanged();
            }
        }

        public int SpecCraftingBuff
        {
            get
            {
                return specCraftingBuff;
            }
            set
            {
                specCraftingBuff = value;
                OnPropertyChanged();
            }
        }

        public int SpecQuestingAmount
        {
            get
            {
                return specQuestingAmount;
            }
            set
            {
                specQuestingAmount = value;
                UpdateBuffs();
                OnPropertyChanged();
            }
        }

        public int SpecQuestingThreshold
        {
            get
            {
                return specQuestingThreshold;
            }
            set
            {
                specQuestingThreshold = value;
                OnPropertyChanged();
            }
        }

        public int SpecQuestingBuff
        {
            get
            {
                return specQuestingBuff;
            }
            set
            {
                specQuestingBuff = value;
                OnPropertyChanged();
            }
        }

        public int SpecBlessingAmount
        {
            get
            {
                return specBlessingAmount;
            }
            set
            {
                specBlessingAmount = value;
                UpdateBuffs();
                OnPropertyChanged();
            }
        }

        public int SpecBlessingThreshold
        {
            get
            {
                return specBlessingThreshold;
            }
            set
            {
                specBlessingThreshold = value;
                OnPropertyChanged();
            }
        }

        public int SpecBlessingBuff
        {
            get
            {
                return specBlessingBuff;
            }
            set
            {
                specBlessingBuff = value;
                OnPropertyChanged();
            }
        }

        public int SpecKillingAmount
        {
            get
            {
                return specKillingAmount;
            }
            set
            {
                specKillingAmount = value;
                UpdateBuffs();
                OnPropertyChanged();
            }
        }

        public int SpecKillingThreshold
        {
            get
            {
                return specKillingThreshold;
            }
            set
            {
                specKillingThreshold = value;
                OnPropertyChanged();
            }
        }

        public int SpecKillingBuff
        {
            get
            {
                return specKillingBuff;
            }
            set
            {
                specKillingBuff = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public Specialization()
        {
            UpdateBuffs();
        }

        public void UpdateBuffs()
        {
            // Const buff value for reaching every threshold.
            var SpecBlessingBuffConst = 15; // Increases blessings duration in seconds.
            var SpecKillingBuffConst = 1; // Increases click damage (after effects like crit, poison are applied - const value).
            var SpecCraftingBuffConst = 1; 
            var SpecBuyingBuffConst = 15; 
            var SpecMeltingBuffConst = 1;
            var SpecQuestingBuffConst = 1;

            // Buff gains thresholds.
            SpecBlessingThreshold = 10; // Amount increases every time a Blessing is bought.
            SpecKillingThreshold = 3;   // Amount increases every time a monster is killed.
            SpecCraftingThreshold = 10;
            SpecBuyingThreshold = 10;
            SpecMeltingThreshold = 10;
            SpecQuestingThreshold = 10;

            // Updating current buff value based on constants and amount (which is not constant).
            SpecBlessingBuff = SpecBlessingAmount / SpecBlessingThreshold * SpecBlessingBuffConst;
            SpecKillingBuff = SpecKillingAmount / SpecKillingThreshold * SpecKillingBuffConst;
            SpecCraftingBuff = SpecCraftingAmount / SpecCraftingThreshold * SpecCraftingBuffConst;
            SpecBuyingBuff = SpecBuyingAmount / SpecBuyingThreshold * SpecBuyingBuffConst;
            SpecMeltingBuff = SpecMeltingAmount / SpecMeltingThreshold * SpecMeltingBuffConst;
            SpecQuestingBuff = SpecQuestingAmount / SpecQuestingThreshold * SpecQuestingBuffConst;
        }
    }
}