using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataGen.Models.uDummyNodeModel
{
    public class ContentType
    {
        public string ncAlias { get; set; }
        public string ncTabAlias { get; set; }
        public string nameTemplate { get; set; }
    }

    public class NCDataTypeModel
    {
        public List<ContentType> ContentTypes { get; set; }
        public object MinItems { get; set; }
        public object MaxItems { get; set; }
        public bool ConfirmDeletes { get; set; }
        public bool ShowIcons { get; set; }
        public bool HideLabel { get; set; }
    }
}
