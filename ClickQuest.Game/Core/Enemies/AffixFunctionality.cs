using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickQuest.Game.Core.Enemies
{
	public class AffixFunctionality
	{
		public Affix Affix {get;set;}

		// Use to increase poison damage dealt (eg. by a percentage).
		public virtual void OnDealingPoisonDamage(ref int poisonDamage)
		{
		}
	}
}
