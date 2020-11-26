using System.Windows.Controls;
using ClickQuest.Enemies;

namespace ClickQuest.Controls
{
    public partial class MonsterButton : UserControl
    {
        private Monster _monster;

        public MonsterButton(Monster monster)
        {
            InitializeComponent();

            _monster = monster;
            this.DataContext=_monster;

            this.MonsterHealth.Text = _monster.CurrentHealth.ToString() + "/" + _monster.Health.ToString();
        }
    }
}