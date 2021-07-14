using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public partial class Ingot : Item
	{
		public Ingot() : base()
		{

		}

		public override Ingot CopyItem()
		{
			Ingot copy = new Ingot();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = 1;

			return copy;
		}
		public Ingot(Rarity rarity, int quantity)
		{
			Rarity = rarity;
			Quantity = quantity;
		}

		public override void AddAchievementProgress(int amount)
		{
				NumericAchievementType achievementType = 0;
				
				// Increase achievement amount.
				switch(Rarity)
				{
					case Rarity.General:
						achievementType=NumericAchievementType.GeneralIngotsEarned;
						break;
					case Rarity.Fine:
						achievementType=NumericAchievementType.FineIngotsEarned;
						break;
					case Rarity.Superior:
						achievementType=NumericAchievementType.SuperiorIngotsEarned;
						break;
					case Rarity.Exceptional:
						achievementType=NumericAchievementType.ExceptionalIngotsEarned;
						break;
					case Rarity.Mythic:
						achievementType=NumericAchievementType.MythicIngotsEarned;
						break;
					case Rarity.Masterwork:
						achievementType=NumericAchievementType.MasterworkIngotsEarned;
						break;
				}
				User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
		}
	}
}