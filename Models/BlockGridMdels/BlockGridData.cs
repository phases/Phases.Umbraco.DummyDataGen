using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;

namespace DummyDataGen.Models.BlockGridMdels
{
    public class BlockGridData
    {
        public BlockGridData(BlockGridLayout layout, BlockGridElementData[] contentData, BlockGridElementData[] settingsData)
        {
            Layout = layout;
            ContentData = contentData;
            SettingsData = settingsData;
        }
        [JsonProperty("layout")]
        public BlockGridLayout Layout { get; }

        [JsonProperty("contentData")]
        public BlockGridElementData[] ContentData { get; }

        [JsonProperty("settingsData")]
        public BlockGridElementData[] SettingsData { get; }
    }
    public class BlockGridLayout
    {
        public BlockGridLayout(BlockGridLayoutItem[] layoutItems) => LayoutItems = layoutItems;

        [JsonProperty("Umbraco.BlockGrid")]
        public BlockGridLayoutItem[] LayoutItems { get; }
    }
    public class BlockGridLayoutItem
    {
        public BlockGridLayoutItem(Udi contentUdi, /*List<AreaGrid> areas,*/ Udi settingsUdi, int columnSpan, int rowSpan)
        {
            ContentUdi = contentUdi;
            SettingsUdi = settingsUdi;
            ColumnSpan = columnSpan;
            RowSpan = rowSpan;
            //Areas = areas;
        }

        [JsonProperty("contentUdi")]
        public Udi ContentUdi { get; }

        [JsonProperty("settingsUdi")]
        public Udi SettingsUdi { get; }

        [JsonProperty("areas")]
        // areas are omitted from this sample for abbreviation
        public List<AreaGrid> Areas { get; set; }

        [JsonProperty("columnSpan")]
        public int ColumnSpan { get; }

        [JsonProperty("rowSpan")]
        public int RowSpan { get; }

    }
    public class AreaGrid
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("columnSpan")]
        public int ColumnSpan { get; }

        [JsonProperty("rowSpan")]
        public int RowSpan { get; }
        [JsonProperty("minAllowed")]
        public int MinAllowed { get; }
        [JsonProperty("maxAllowed")]
        public int MaxAllowed { get; }
        [JsonProperty("createLabel")]
        public string CreateLabel { get; set; }
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("items")]
        public List<BlockGridLayoutItem> Items { get; set; }
    }
    public class BlockGridElementData
    {
        public BlockGridElementData(Guid contentTypeKey, Udi udi, Dictionary<string, object> data)
        {
            ContentTypeKey = contentTypeKey;
            Udi = udi;
            Data = data;
        }

        [JsonProperty("contentTypeKey")]
        public Guid ContentTypeKey { get; }

        [JsonProperty("udi")]
        public Udi Udi { get; }

        [JsonExtensionData]
        public Dictionary<string, object> Data { get; }
    }
}
