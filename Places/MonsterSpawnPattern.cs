using System.Linq;
using ClickQuest.Data;
using ClickQuest.Enemies;

namespace ClickQuest.Places
{
	public class MonsterSpawnPattern
	{
		public int MonsterId { get; set; }
		public double Frequency { get; set; }

		public Monster GetMonster()
		{
			return GameData.Monsters.FirstOrDefault(x => x.Id == MonsterId);
		}
	}
}