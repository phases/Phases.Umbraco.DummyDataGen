using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataGen.Models.uDummyNodeModel
{
    public class Area
    {
        public int grid { get; set; }
        public bool allowAll { get; set; }
        public List<string> allowed { get; set; }
    }

    public class Config
    {
        public string label { get; set; }
        public string description { get; set; }
        public string key { get; set; }
        public string view { get; set; }
    }

    public class Items
    {
        public List<Style> styles { get; set; }
        public List<Config> config { get; set; }
        public int columns { get; set; }
        public List<Template> templates { get; set; }
        public List<Layout> layouts { get; set; }
    }

    public class Layout
    {
        public string name { get; set; }
        public List<Area> areas { get; set; }
    }

    public class Rte
    {
        public List<string> toolbar { get; set; }
        public List<string> stylesheets { get; set; }
        public int maxImageSize { get; set; }
        public string mode { get; set; }
    }

    public class Section
    {
        public int grid { get; set; }
    }

    public class Style
    {
        public string label { get; set; }
        public string description { get; set; }
        public string key { get; set; }
        public string view { get; set; }
        public string modifier { get; set; }
    }

    public class Template
    {
        public string name { get; set; }
        public List<Section> sections { get; set; }
    }

    public class GridDataTypeModel
    {
        public Items Items { get; set; }
        public Rte Rte { get; set; }
        public bool IgnoreUserStartNodes { get; set; }
        public object MediaParentId { get; set; }
    }
}
