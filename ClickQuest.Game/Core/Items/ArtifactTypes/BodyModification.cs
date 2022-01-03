﻿using System.Linq;
using System.Windows;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.Core.Items.ArtifactTypes
{
	public class BodyModification : ArtifactTypeFunctionality
	{
		public override bool CanBeEquipped()
		{
			bool isNoOtherBodyModificationEquipped = User.Instance.CurrentHero.EquippedArtifacts
				.Count(x=>x.ArtifactType==ArtifactType.BodyModification) == 0;

			if (isNoOtherBodyModificationEquipped)
			{
				return true;
			}

			AlertBox.Show("This artifact cannot be equipped right now - only one Body Modification can be equipped at a time.", MessageBoxButton.OK);
			return false;
		}
		
		public BodyModification()
        {
        	ArtifactType = ArtifactType.BodyModification;
        	Description = "You can only equip one Artifact of this type.";
        }
	}
}