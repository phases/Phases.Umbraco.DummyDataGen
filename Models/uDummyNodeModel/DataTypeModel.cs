using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataGen.Models.uDummyNodeModel
{
    public class Item
    {
        public int id { get; set; }
        public string value { get; set; }
    }

    public class DataTypeModel
    {
        public bool Multiple { get; set; }
        public int min { get; set; }
        public int step { get; set; }
        public int max { get; set; }
        public List<Item> Items { get; set; }
    }
}
