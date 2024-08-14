using DummyDataGen.Models.uDummyNodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataGen.Services.Interface
{
    public interface IuDummyNodeService
    {
        string CreateDummyNode(UDummyNodeCreationModel uDummyNodeCreationModel);
    }
}
