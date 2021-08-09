using System.Linq;
using ClickQuest.Data.GameData;
using ClickQuest.Enemies;

namespace ClickQuest.Places
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