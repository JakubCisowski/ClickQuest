using System.Linq;
using ClickQuest.ContentManager.Logic.Models;

namespace ClickQuest.ContentManager.Logic.DataTypes.Structs;

public class MonsterSpawnPattern
{
	public int MonsterId { get; set; }
	public double Frequency { get; set; }

	public Monster Monster
	{
		get
		{
			return GameAssets.Monsters.FirstOrDefault(x => x.Id == MonsterId);
		}
	}
}