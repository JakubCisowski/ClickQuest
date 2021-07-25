using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Adventures;
using ClickQuest.Player;

namespace ClickQuest.Controls
{
	public partial class QuestButton : UserControl, INotifyPropertyChanged
	{
		#region Private Fields

		private readonly Quest _quest;

		#endregion

		public QuestButton(Quest quest)
		{
			InitializeComponent();

			_quest = quest;
			DataContext = _quest;
		}

		private void QuestButton_Click(object sender, RoutedEventArgs e)
		{
			// Start this quest (if another one isnt currently assigned).
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
			{
				_quest.StartQuest();
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged
	}
}