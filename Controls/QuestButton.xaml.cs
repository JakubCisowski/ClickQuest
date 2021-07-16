using ClickQuest.Player;
using ClickQuest.Data;
using ClickQuest.Items;
using ClickQuest.Adventures;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Controls
{
	public partial class QuestButton : UserControl, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields
		private Quest _quest;

		#endregion

		public QuestButton(Quest quest)
		{
			InitializeComponent();

			_quest = quest;
			this.DataContext = _quest;
		}

		private void QuestButton_Click(object sender, RoutedEventArgs e)
		{
			// Start this quest (if another one isnt currently assigned).
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
			{
				_quest.StartQuest();
			}
		}
	}
}