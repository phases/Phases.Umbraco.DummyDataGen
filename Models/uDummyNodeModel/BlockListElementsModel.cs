using DummyDataGen.Models.BlockGridMdels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataGen.Models.uDummyNodeModel
{
    public class AreaBlock
    {
        public string key { get; set; }
        public string alias { get; set; }
        public string createLabel { get; set; }
        public int? columnSpan { get; set; }
        public int? rowSpan { get; set; }
        public int? minAllowed { get; set; }
        public int? maxAllowed { get; set; }
        public List<object> specifiedAllowance { get; set; }
    }

    public class Block
    {
        public List<object> columnSpanOptions { get; set; }
        public int? rowMinSpan { get; set; }
        public int? rowMaxSpan { get; set; }
        public bool allowAtRoot { get; set; }
        public bool allowInAreas { get; set; }
        public int? areaGridColumns { get; set; }
        public List<AreaGrid> areas { get; set; }
        public object backgroundColor { get; set; }
        public object iconColor { get; set; }
        public object thumbnail { get; set; }
        public string contentElementTypeKey { get; set; }
        public object settingsElementTypeKey { get; set; }
        public string view { get; set; }
        public object stylesheet { get; set; }
        public string label { get; set; }
        public string editorSize { get; set; }
        public bool inlineEditing { get; set; }
        public bool forceHideContentEditorInOverlay { get; set; }
        public object groupKey { get; set; }
    }

    public class BlockGroup
    {
        public string key { get; set; }
        public string name { get; set; }
    }

    public class Root
    {
        public List<Block> Blocks { get; set; }
        public List<BlockGroup> BlockGroups { get; set; }
        public ValidationLimit ValidationLimit { get; set; }
        public bool UseLiveEditing { get; set; }
        public object MaxPropertyWidth { get; set; }
        public int? GridColumns { get; set; }
        public object LayoutStylesheet { get; set; }
        public object CreateLabel { get; set; }
    }

    public class ValidationLimit
    {
        public object min { get; set; }
        public object max { get; set; }
    }
}
