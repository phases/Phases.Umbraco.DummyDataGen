using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataGen.Models.uDummyNodeModel
{
    public class UDummyNodeCreationModel
    {
        public string DocTypeAlias { get; set; }
        public string ParentNodeId { get; set; }
        public int Count { get; set; }
        public string PrefixSequence { get; set; }
        public string PostfixSequence { get; set; }
        public string SequenceDigits { get; set; }
    }
}
