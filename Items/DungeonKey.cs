using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public partial class DungeonKey : Item
	{
		public DungeonKey() : base()
		{
		}

		public override void AddAchievementProgress(int amount)
		{
			NumericAchievementType achievementType = 0;
			// Increase achievement amount.
			switch(Rarity)
			{
				case Rarity.General:
					achievementType = NumericAchievementType.GeneralDungeonKeysEarned;
					break;
				case Rarity.Fine:
					achievementType = NumericAchievementType.FineDungeonKeysEarned;
					break;
				case Rarity.Superior:
					achievementType = NumericAchievementType.SuperiorDungeonKeysEarned;
					break;
				case Rarity.Exceptional:
					achievementType = NumericAchievementType.ExceptionalDungeonKeysEarned;
					break;
				case Rarity.Mythic:
					achievementType = NumericAchievementType.MythicDungeonKeysEarned;
					break;
				case Rarity.Masterwork:
					achievementType = NumericAchievementType.MasterworkDungeonKeysEarned;
					break;
			}
			User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
		}

		public override DungeonKey CopyItem(int quantity)
		{
			var copy = new DungeonKey();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = quantity;

			return copy;
		}
	}
}