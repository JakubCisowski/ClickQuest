using System.Linq;
using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Enemies;

namespace ClickQuest.Game.Places
{
	public class MonsterSpawnPattern
	{
		public int MonsterId { get; set; }
		public double Frequency { get; set; }

		public Monster Monster
		{
			get
			{
				return GameData.Monsters.FirstOrDefault(x => x.Id == MonsterId);
			}
		}
	}
}