using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;

namespace DummyDataGen.Models.BlockListModels
{
    public class BlockListData
    {
        public BlockListData(BlockListLayout layout, BlockListElementData[] contentData, BlockListElementData[] settingsData)
        {
            Layout = layout;
            ContentData = contentData;
            SettingsData = settingsData;
        }
        [JsonProperty("layout")]
        public BlockListLayout Layout { get; }

        [JsonProperty("contentData")]
        public BlockListElementData[] ContentData { get; }

        [JsonProperty("settingsData")]
        public BlockListElementData[] SettingsData { get; }
    }
    public class BlockListLayout
    {
        public BlockListLayout(BlockListLayoutItem[] layoutItems) => LayoutItems = layoutItems;

        [JsonProperty("Umbraco.BlockList")]
        public BlockListLayoutItem[] LayoutItems { get; }
    }
    public class BlockListLayoutItem
    {
        public BlockListLayoutItem(Udi contentUdi, Udi settingsUdi)
        {
            ContentUdi = contentUdi;
            SettingsUdi = settingsUdi;
        }

        [JsonProperty("contentUdi")]
        public Udi ContentUdi { get; }

        [JsonProperty("settingsUdi")]
        public Udi SettingsUdi { get; }

        [JsonProperty("areas")]
        // areas are omitted from this sample for abbreviation
        public object[] Areas { get; } = { };

        [JsonProperty("columnSpan")]
        public int ColumnSpan { get; }

        [JsonProperty("rowSpan")]
        public int RowSpan { get; }

    }
    public class BlockListElementData
    {
        public BlockListElementData(Guid contentTypeKey, Udi udi, Dictionary<string, object> data)
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
