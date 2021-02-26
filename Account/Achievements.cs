using ClickQuest.Heroes;
using ClickQuest.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Account
{
	[Owned]
	public partial class Achievements : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		#region PrivateFields

		// General
		private TimeSpan _totalTimePlayed;
		private long _experienceGained;

		// Currencies
		private int _goldEarned;
		private int _goldSpent;
		private int _generalIngotsEarned;
		private int _fineIngotsEarned;
		private int _superiorIngotsEarned;
		private int _exceptionalIngotsEarned;
		private int _mythicIngotsEarned;
		private int _masterworkIngotsEarned;
		private int _generalDungeonKeysEarned;
		private int _fineDungeonKeysEarned;
		private int _superiorDungeonKeysEarned;
		private int _exceptionalDungeonKeysEarned;
		private int _mythicDungeonKeysEarned;
		private int _masterworkDungeonKeysEarned;

		// Combat
		private long _totalDamageDealt;
		private int _critsAmount;
		private int _poisonTicksAmount;
		private int _monstersDefeated;

		// Dungeons
		private int _bossesDefeated;

		// Quests
		private int _questsCompleted;
		private int _questRerollsAmount;
		
		// Blessings
		private int _blessingsUsed;

		// Equipment
		private int _materialsGained;
		private int _recipesGained;
		private int _generalArtifactsGained;
		private int _fineArtifactsGained;
		private int _superiorArtifactsGained;
		private int _exceptionalArtifactsGained;
		private int _mythicArtifactsGained;
		private int _masterworkArtifactsGained;

		#endregion

		#region Properties
		public TimeSpan TotalTimePlayed
		{
			get
			{
				return _totalTimePlayed;
			}
			set
			{
				_totalTimePlayed = value;
				OnPropertyChanged();
			}
		}
		public long ExperienceGained
		{ 
			get 
			{
				return _experienceGained;
			}  
			set 
			{
				_experienceGained = value;
				OnPropertyChanged();
			}
		}
		public int GoldEarned
		{
			get
			{
				return _goldEarned;
			}
			set
			{
				_goldEarned = value;
				OnPropertyChanged();
			}
		}
		public int GoldSpent
		{
			get
			{
				return _goldSpent;
			}
			set
			{
				_goldSpent = value;
				OnPropertyChanged();
			}
		}
		public int GeneralIngotsEarned 
		{ 
			get 
			{
				return _generalIngotsEarned;
			}  
			set 
			{
				_generalIngotsEarned = value;
				OnPropertyChanged();
			}
		}
		public int FineIngotsEarned 
		{ 
			get 
			{
				return _fineIngotsEarned;
			}  
			set 
			{
				_fineIngotsEarned = value;
				OnPropertyChanged();
			}
		}
		public int SuperiorIngotsEarned 
		{ 
			get 
			{
				return _superiorIngotsEarned;
			}  
			set 
			{
				_superiorIngotsEarned = value;
				OnPropertyChanged();
			}
		}
		public int ExceptionalIngotsEarned 
		{ 
			get 
			{
				return _exceptionalIngotsEarned;
			}  
			set 
			{
				_exceptionalIngotsEarned = value;
				OnPropertyChanged();
			}
		}
		public int MythicIngotsEarned 
		{ 
			get 
			{
				return _mythicIngotsEarned;
			}  
			set 
			{
				_mythicIngotsEarned = value;
				OnPropertyChanged();
			}
		}
		public int MasterworkIngotsEarned 
		{ 
			get 
			{
				return _masterworkIngotsEarned;
			}  
			set 
			{
				_masterworkIngotsEarned = value;
				OnPropertyChanged();
			}
		}
		public int GeneralDungeonKeysEarned 
		{ 
			get 
			{
				return _generalDungeonKeysEarned;
			}  
			set 
			{
				_generalDungeonKeysEarned = value;
				OnPropertyChanged();
			}
		}
		public int FineDungeonKeysEarned 
		{ 
			get 
			{
				return _fineDungeonKeysEarned;
			}  
			set 
			{
				_fineDungeonKeysEarned = value;
				OnPropertyChanged();
			}
		}
		public int SuperiorDungeonKeysEarned 
		{ 
			get 
			{
				return _superiorDungeonKeysEarned;
			}  
			set 
			{
				_superiorDungeonKeysEarned = value;
				OnPropertyChanged();
			}
		}
		public int ExceptionalDungeonKeysEarned 
		{ 
			get 
			{
				return _exceptionalDungeonKeysEarned;
			}  
			set 
			{
				_exceptionalDungeonKeysEarned = value;
				OnPropertyChanged();
			}
		}
		public int MythicDungeonKeysEarned 
		{ 
			get 
			{
				return _mythicDungeonKeysEarned;
			}  
			set 
			{
				_mythicDungeonKeysEarned = value;
				OnPropertyChanged();
			}
		}
		public int MasterworkDungeonKeysEarned 
		{ 
			get 
			{
				return _masterworkDungeonKeysEarned;
			}  
			set 
			{
				_masterworkDungeonKeysEarned = value;
				OnPropertyChanged();
			}
		}
		public long TotalDamageDealt
		{ 
			get 
			{
				return _totalDamageDealt;
			}  
			set 
			{
				_totalDamageDealt = value;
				OnPropertyChanged();
			}
		}
		public int CritsAmount
		{ 
			get 
			{
				return _critsAmount;
			}  
			set 
			{
				_critsAmount = value;
				OnPropertyChanged();
			}
		}
		public int PoisonTicksAmount
		{ 
			get 
			{
				return _poisonTicksAmount;
			}  
			set 
			{
				_poisonTicksAmount = value;
				OnPropertyChanged();
			}
		}
		public int MonstersDefeated 
		{ 
			get 
			{
				return _monstersDefeated;
			}  
			set 
			{
				_monstersDefeated = value;
				OnPropertyChanged();
			}
		}
		public int BossesDefeated 
		{ 
			get 
			{
				return _bossesDefeated;
			}  
			set 
			{
				_bossesDefeated = value;
				OnPropertyChanged();
			}
		}
		public int QuestsCompleted 
		{ 
			get 
			{
				return _questsCompleted;
			}  
			set 
			{
				_questsCompleted = value;
				OnPropertyChanged();
			}
		}
		public int QuestRerollsAmount
		{ 
			get 
			{
				return _questRerollsAmount;
			}  
			set 
			{
				_questRerollsAmount = value;
				OnPropertyChanged();
			}
		}
		public int BlessingsUsed
		{ 
			get 
			{
				return _blessingsUsed;
			}  
			set 
			{
				_blessingsUsed = value;
				OnPropertyChanged();
			}
		}
		public int MaterialsGained
		{ 
			get 
			{
				return _materialsGained;
			}  
			set 
			{
				_materialsGained = value;
				OnPropertyChanged();
			}
		}
		public int RecipesGained
		{ 
			get 
			{
				return _recipesGained;
			}  
			set 
			{
				_recipesGained = value;
				OnPropertyChanged();
			}
		}
		public int GeneralArtifactsGained
		{ 
			get 
			{
				return _generalArtifactsGained;
			}  
			set 
			{
				_generalArtifactsGained = value;
				OnPropertyChanged();
			}
		}
		public int FineArtifactsGained
		{ 
			get 
			{
				return _fineArtifactsGained;
			}  
			set 
			{
				_fineArtifactsGained = value;
				OnPropertyChanged();
			}
		}
		public int SuperiorArtifactsGained
		{ 
			get 
			{
				return _superiorArtifactsGained;
			}  
			set 
			{
				_superiorArtifactsGained = value;
				OnPropertyChanged();
			}
		}
		public int ExceptionalArtifactsGained
		{ 
			get 
			{
				return _exceptionalArtifactsGained;
			}  
			set 
			{
				_exceptionalArtifactsGained = value;
				OnPropertyChanged();
			}
		}
		public int MythicArtifactsGained
		{ 
			get 
			{
				return _mythicArtifactsGained;
			}  
			set 
			{
				_mythicArtifactsGained = value;
				OnPropertyChanged();
			}
		}
		public int MasterworkArtifactsGained
		{ 
			get 
			{
				return _masterworkArtifactsGained;
			}  
			set 
			{
				_masterworkArtifactsGained = value;
				OnPropertyChanged();
			}
		}
		#endregion
	}
}
