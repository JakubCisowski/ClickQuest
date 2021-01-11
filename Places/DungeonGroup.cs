using ClickQuest.Items;
using System.Collections.Generic;
using System.Drawing;

namespace ClickQuest.Places
{
    public partial class DungeonGroup
    {
        private int _id;
        private string _name;
        private string _description;
        private List<int> _keyRequirements;
        private Color _color;

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description=value;
            }
        }

        public List<int> KeyRequirements
        {
            get
            {
                return _keyRequirements;
            }
            set
            {
                _keyRequirements=value;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public DungeonGroup(int id, string name, string description, List<int> keyRequirements, Color color)
        {
            Id=id;
            Name=name;
            Description=description;
            KeyRequirements=keyRequirements;
            Color=color;
        }
    }
}