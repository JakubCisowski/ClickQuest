using System;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Items
{
	public class Artifact : Item
	{
		[JsonIgnore]
		public ArtifactFunctionality ArtifactFunctionality { get; set; }

		public string Lore { get; set; }
		public string ExtraInfo { get; set; }
		public ArtifactType ArtifactType { get; set; }
		public string MythicTag { get; set; }
		public const double MeltingIngredientsRatio = 0.6;
		public const double CraftingRatio = 20;

		[JsonIgnore]
		public int BaseIngotBonus
		{
			get
			{
				return 100;
			}
		}

		public override Artifact CopyItem(int quantity)
		{
			var copy = new Artifact();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Quantity = quantity;
			copy.ArtifactType = ArtifactType;
			copy.ArtifactFunctionality = ArtifactFunctionality;
			copy.Description = Description;
			copy.Lore = Lore;
			copy.ExtraInfo = ExtraInfo;
			copy.MythicTag = MythicTag;

			return copy;
		}

		public override void AddAchievementProgress(int amount = 1)
		{
			NumericAchievementType achievementType = 0;
			// Increase achievement amount.
			switch (Rarity)
			{
				case Rarity.General:
					achievementType = NumericAchievementType.GeneralArtifactsGained;
					break;
				case Rarity.Fine:
					achievementType = NumericAchievementType.FineArtifactsGained;
					break;
				case Rarity.Superior:
					achievementType = NumericAchievementType.SuperiorArtifactsGained;
					break;
				case Rarity.Exceptional:
					achievementType = NumericAchievementType.ExceptionalArtifactsGained;
					break;
				case Rarity.Mythic:
					achievementType = NumericAchievementType.MythicArtifactsGained;
					break;
				case Rarity.Masterwork:
					achievementType = NumericAchievementType.MasterworkArtifactsGained;
					break;
			}

			User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
		}

		public override void AddItem(int amount = 1)
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Artifacts);

			AddAchievementProgress();
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		public override void RemoveItem(int amount = 1)
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Materials);
		}

		public void CreateMythicTag(string enemyName = "")
		{
			if (Rarity == Rarity.Mythic)
			{
				if (enemyName == "")
				{
					MythicTag = "Crafted by " + User.Instance.CurrentHero.Name + " on " + DateTime.Now.ToString("dd/MM/yyyy");
				}
				else
				{
					MythicTag = "Dropped from " + enemyName + " by " + User.Instance.CurrentHero.Name + " on " + DateTime.Now.ToString("dd/MM/yyyy");
				}
			}
		}
	}
}