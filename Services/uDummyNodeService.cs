using DummyDataGen.Models.BlockGridMdels;
using DummyDataGen.Models.BlockListModels;
using DummyDataGen.Models.uDummyNodeModel;
using DummyDataGen.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;
using Umbraco.Cms.Core;

namespace DummyDataGen.Services
{
    public class uDummyNodeService : IuDummyNodeService
    {
        private IContentTypeService _contentTypeService;
        private IContentService _contentService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IPublishedContentCache _cache;
        private readonly IDataTypeService _dataTypeService;
        private readonly IMediaService _mediaService;
        private readonly static Random rnd = new Random();
        private readonly IUmbracoHelperAccessor _helperAccessor;
        private readonly UmbracoHelper _helper;
        private readonly IWebHostEnvironment _enviornment;
        public uDummyNodeService(
            IContentTypeService contentTypeService,
            IContentService contentService,
            IDataTypeService dataTypeService,
            IMediaService mediaService,
            IUmbracoHelperAccessor umbracoHelperAccessor,
            IWebHostEnvironment enviornment,
            IUmbracoContextFactory umbracoContextFactory)
        {
            _contentTypeService = contentTypeService;
            _contentService = contentService;
            _umbracoContextFactory = umbracoContextFactory;
            _dataTypeService = dataTypeService;
            _mediaService = mediaService;
            _helperAccessor = umbracoHelperAccessor;
            _cache = _umbracoContextFactory.EnsureUmbracoContext().UmbracoContext.Content;
            _helperAccessor.TryGetUmbracoHelper(out _helper);
            _enviornment = enviornment;
        }
        public string CreateDummyNode(UDummyNodeCreationModel uDummyNodeCreationModel)
        {
            string createdNodeUrl = string.Empty;
            try
            {

                if (uDummyNodeCreationModel != null && !string.IsNullOrWhiteSpace(uDummyNodeCreationModel.DocTypeAlias) && !string.IsNullOrWhiteSpace(uDummyNodeCreationModel.ParentNodeId))
                {
                    var contentType = _contentTypeService.Get(uDummyNodeCreationModel.DocTypeAlias);

                    if (contentType != null && contentType.Id > 0)
                    {
                        for (int i = 0; i < uDummyNodeCreationModel.Count; i++)
                        {
                            var propertyTypes = contentType.CompositionPropertyTypes;

                            //Default Naming 
                            var sequenceDigits = 0;
                            string format = "";
                            string nodeNameUnique = "";
                            if (string.IsNullOrWhiteSpace(uDummyNodeCreationModel.SequenceDigits))
                            {
                                sequenceDigits = int.Parse("00");
                                sequenceDigits = sequenceDigits + i + 1;
                                format = "D" + uDummyNodeCreationModel.SequenceDigits.Length;
                                nodeNameUnique = uDummyNodeCreationModel?.PrefixSequence + sequenceDigits.ToString(format) + uDummyNodeCreationModel?.PostfixSequence;
                            }
                            else
                            {
                                sequenceDigits = int.Parse(uDummyNodeCreationModel.SequenceDigits);
                                sequenceDigits = sequenceDigits + i + 1;
                                format = "D" + uDummyNodeCreationModel.SequenceDigits.Length;
                                nodeNameUnique = uDummyNodeCreationModel.PrefixSequence + sequenceDigits.ToString(format) + uDummyNodeCreationModel.PostfixSequence;
                            }

                            int parentNodeId = Convert.ToInt32(uDummyNodeCreationModel.ParentNodeId);
                            IContent nodeRequest = null;
                            nodeRequest = _contentService.Create(nodeNameUnique, parentNodeId, uDummyNodeCreationModel.DocTypeAlias);

                            if (propertyTypes?.Any() == true)
                            {
                                var jsonValue = JsonConvert.SerializeObject(propertyTypes);
                                if (nodeRequest != null)
                                {
                                    SetValuesInProperties(propertyTypes, nodeRequest);
                                    var result = _contentService.SaveAndPublish(nodeRequest);
                                    if (result != null && result.Content != null)
                                    {
                                        var idOfCreatedPage = result.Content.Id;
                                        if (idOfCreatedPage > 0)
                                        {
                                            //var node = _helper.Content(idOfCreatedPage);

                                            createdNodeUrl = $"/umbraco#/content/content/edit/{result.Content?.Id.ToString()}";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var result = _contentService.SaveAndPublish(nodeRequest);
                                if (result != null && result.Content != null)
                                {
                                    var idOfCreatedPage = result.Content.Id;
                                    if (idOfCreatedPage > 0)
                                    {
                                        //var node = _helper.Content(idOfCreatedPage);

                                        createdNodeUrl = $"/umbraco#/content/content/edit/{result.Content?.Id.ToString()}";
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                var msg = ex;
            }

            return createdNodeUrl;
        }
        private void SetValuesInProperties(IEnumerable<IPropertyType> propertyTypes, IContent nodeRequest)
        {
            int checkBoxSetCount = 1;
            var checkBoxCount = propertyTypes.Where(p => p.PropertyEditorAlias == "Umbraco.TrueFalse")?.Count();
            using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
            foreach (var curPropertyType in propertyTypes)
            {
                try
                {
                    if (curPropertyType != null)
                    {
                        string description = curPropertyType.Description;
                        bool needToAddData = !string.IsNullOrWhiteSpace(description) && description.Contains("*exclude-udummy") ? false : true;

                        if (needToAddData)
                        {
                            if (curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.textbox" || curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.textboxmultiple" || curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.textarea")
                            {
                                nodeRequest.SetValue(curPropertyType.Alias, $"This is test for {curPropertyType.Alias}");
                            }

                            else if (curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.datetime")
                            {
                                nodeRequest.SetValue(curPropertyType.Alias, DateTime.Now);
                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.Tags")
                            {
                                var testTags = "Tag 1,Tag 2";
                                var tagsList = testTags.Split(',').ToList();

                                nodeRequest.SetValue(curPropertyType.Alias, tagsList);

                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.MultiUrlPicker")
                            {
                                List<Link> linksToAdd = new List<Link>();
                                Link link = new Link
                                {
                                    Target = "_blank",
                                    Name = "Test URL",
                                    Url = "https://www.google.com/",
                                    Type = LinkType.External

                                };

                                linksToAdd.Add(link);
                                var linkJson = JsonConvert.SerializeObject(linksToAdd);
                                nodeRequest.SetValue(curPropertyType.Alias, linkJson);

                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.TrueFalse")
                            {
                                bool isEven = true;
                                if (checkBoxCount > 1)
                                {
                                    isEven = checkBoxSetCount % 2 == 0 ? true : false;
                                }

                                nodeRequest.SetValue(curPropertyType.Alias, isEven);

                                checkBoxSetCount++;
                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.Integer")
                            {
                                var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                                if (dataType != null)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                                    if (!string.IsNullOrWhiteSpace(jsonModel))
                                    {
                                        var prevalues = GetConfigurationFromDataType(jsonModel);

                                        int index = 0;
                                        index = rnd.Next(50);

                                        if (prevalues.min > 0 && prevalues.max > 0)
                                        {
                                            index = rnd.Next(prevalues.min, prevalues.max);
                                        }
                                        else if (prevalues.max > 0)
                                        {
                                            index = rnd.Next(prevalues.max);
                                        }
                                        nodeRequest.SetValue(curPropertyType.Alias, index);
                                    }
                                }
                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.DropDown.Flexible" || curPropertyType.PropertyEditorAlias == "Umbraco.CheckBoxList")
                            {
                                var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                                if (dataType != null)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                                    if (!string.IsNullOrWhiteSpace(jsonModel))
                                    {
                                        var prevalues = GetConfigurationFromDataType(jsonModel);

                                        if (prevalues != null && prevalues.Items != null && prevalues.Items.Any())
                                        {
                                            List<string> values = new List<string>();

                                            int index = rnd.Next(prevalues.Items.Count);
                                            var selected = prevalues.Items[index];
                                            values.Add(selected.value);
                                            if (prevalues.Multiple || curPropertyType.PropertyEditorAlias == "Umbraco.CheckBoxList")
                                            {
                                                if (prevalues.Items.Count() > 1)
                                                {
                                                    int index2 = rnd.Next(prevalues.Items.Count);
                                                    var selected2 = prevalues.Items[index2];
                                                    values.Add(selected2.value);
                                                }
                                            }

                                            if (values != null && values.Any())
                                            {
                                                string[] valueArray = values.Distinct()?.ToArray();

                                                nodeRequest.SetValue(curPropertyType.Alias, JsonConvert.SerializeObject(valueArray));
                                            }
                                        }
                                    }

                                }


                            }

                            else if (curPropertyType.PropertyEditorAlias.Contains("Umbraco.MediaPicker"))
                            {
                                var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);
                                if (dataType != null)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                                    if (!string.IsNullOrWhiteSpace(jsonModel))
                                    {
                                        var prevalues = GetConfigurationFromDataType(jsonModel);

                                        var existingRootFoler = _mediaService.GetRootMedia()?.Where(r => r.Name?.ToLower() == "udummy media")?.FirstOrDefault();

                                        if (existingRootFoler != null && existingRootFoler.Id > 0)
                                        {
                                            var folder = umbracoContextReference.UmbracoContext.Media.GetById(existingRootFoler.Id);
                                            if (folder != null)
                                            {
                                                var mediaItems = folder.Descendants()?.Where(m => m.ContentType.Alias == "Image")?.ToList();

                                                if (mediaItems?.Any() == true)
                                                {
                                                    List<IPublishedContent> randomMediaItems = new List<IPublishedContent>();

                                                    int index = rnd.Next(mediaItems.Count);
                                                    //if (index > 0)
                                                    //{
                                                    var randomMediaItem = mediaItems[index];

                                                    if (randomMediaItem != null && randomMediaItem.Id > 0)
                                                    {
                                                        randomMediaItems.Add(randomMediaItem);

                                                    }
                                                    //}

                                                    if (prevalues.Multiple)
                                                    {
                                                        int index2 = rnd.Next(mediaItems.Count);
                                                        if (index2 > 0)
                                                        {
                                                            randomMediaItem = mediaItems[index2];

                                                            if (randomMediaItem != null && randomMediaItem.Id > 0)
                                                            {
                                                                randomMediaItems.Add(randomMediaItem);
                                                            }
                                                        }
                                                    }

                                                    if (randomMediaItems?.Any() == true)
                                                    {
                                                        SetMediaItem(randomMediaItems.Distinct().ToList(), nodeRequest, curPropertyType.Alias);
                                                    }
                                                }
                                            }

                                        }

                                    }
                                }

                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.NestedContent")
                            {
                                var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                                if (dataType != null)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                                    if (!string.IsNullOrWhiteSpace(jsonModel))
                                    {
                                        var ncDataTypeModel = JsonConvert.DeserializeObject<NCDataTypeModel>(jsonModel);

                                        if (ncDataTypeModel != null)
                                        {
                                            var ncData = HandleNestedCotentModel(ncDataTypeModel, curPropertyType.Alias);

                                            if (!string.IsNullOrWhiteSpace(ncData))
                                            {
                                                nodeRequest.SetValue(curPropertyType.Alias, ncData);
                                            }
                                        }
                                    }
                                }
                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.Grid")
                            {
                                var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                                if (dataType != null)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                                    if (!string.IsNullOrWhiteSpace(jsonModel))
                                    {
                                        var gridDataTypeModel = JsonConvert.DeserializeObject<GridDataTypeModel>(jsonModel);

                                        if (gridDataTypeModel != null)
                                        {
                                            var gridData = GetDataForGridByGridDataTypeModel(gridDataTypeModel);

                                            if (gridData != null)
                                            {
                                                nodeRequest.SetValue(curPropertyType.Alias, gridData.ToString());
                                            }
                                        }
                                    }
                                }
                            }

                            else if (curPropertyType.PropertyEditorAlias == "Umbraco.TinyMCE")
                            {
                                var existingRootFoler = _mediaService.GetRootMedia()?.Where(r => r.Name?.ToLower() == "udummy media")?.FirstOrDefault();
                                if (existingRootFoler != null && existingRootFoler.Id > 0)
                                {
                                    //var folder = umbracoContextReference.UmbracoContext.Media.GetById(existingRootFoler.Id);
                                    var folder = umbracoContextReference.UmbracoContext.Media.GetById(existingRootFoler.Id);
                                    var mediaItems = folder.Descendants()?.Where(m => m.ContentType.Alias == "File" && m.Name == "TestRte")?.ToList();
                                    var data = System.IO.File.ReadAllText(_enviornment.ContentRootPath + "/wwwroot/" + mediaItems.First().Url());
                                    //var mediaItems = folder.Descendants()?.Where(m => m.ContentType.Alias == "File")?.ToList();
                                    //string text = System.IO.File.ReadAllText(_enviornment.ContentRootPath + "/wwwroot/" + mediaItems.First().Url());
                                    nodeRequest.SetValue(curPropertyType.Alias, new HtmlString(data).ToString());
                                }
                            }
                            else if (curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.blockgrid")
                            {
                                var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);
                                if (dataType != null)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                                    var ncDataTypeModel = JsonConvert.DeserializeObject<Root>(jsonModel);
                                    //var blockValueContents = new Dictionary<string, object>();
                                    var contentDataList = new List<BlockGridElementData>();
                                    var settingsDataList = new List<BlockGridElementData>();
                                    var layoutDataList = new List<Models.BlockGridMdels.BlockGridLayoutItem>();
                                    if (ncDataTypeModel != null)
                                    {
                                        foreach (var item in ncDataTypeModel?.Blocks)
                                        {
                                            var contentTypeGuid = new Guid(item.contentElementTypeKey);
                                            var contentType = _contentTypeService.Get(contentTypeGuid);

                                            var settingsTypeGuid = new Guid(item.contentElementTypeKey);
                                            var settingsType = _contentTypeService.Get(settingsTypeGuid);

                                            var propertyType = contentType.CompositionPropertyTypes;

                                            var contentData = new List<BlockGridElementData>();
                                            var settingsData = new List<BlockGridElementData>();

                                            var contentUdi = Umbraco.Cms.Core.Udi.Create(Constants.UdiEntityType.Element, Guid.NewGuid());
                                            var settingsUdi = Umbraco.Cms.Core.Udi.Create(Constants.UdiEntityType.Element, Guid.NewGuid());
                                            if (propertyType != null)
                                            {
                                                var values = new Dictionary<string, object>();
                                                foreach (var type in propertyType)
                                                {
                                                    string descriptionBlockGrid = type.Description;
                                                    bool needToAddDataInBlockGrid = !string.IsNullOrWhiteSpace(descriptionBlockGrid) && descriptionBlockGrid.Contains("*exclude-udummy") ? false : true;
                                                    if (needToAddDataInBlockGrid)
                                                    {
                                                        var data = GetDataForNCProperty(type);
                                                        if (type.PropertyEditorAlias == "Umbraco.TinyMCE")
                                                            values.Add(type.Alias, new HtmlString(data).ToString());
                                                        else
                                                            values.Add(type.Alias, data);
                                                    }

                                                }
                                                contentDataList.Add(new BlockGridElementData(contentType.Key, contentUdi, values));
                                                settingsDataList.Add(new BlockGridElementData(settingsType.Key, settingsUdi, new Dictionary<string, object>
                                                {
                                                    { "featured", "0" },
                                                }));
                                                layoutDataList.Add(new Models.BlockGridMdels.BlockGridLayoutItem(contentUdi, /*item.areas,*/ settingsUdi, 1, 1));
                                            }
                                        }
                                        var blockGridData = new BlockGridData(
                                        new BlockGridLayout(layoutDataList.ToArray()),
                                                                                    contentDataList.ToArray(),
                                                                                    settingsDataList.ToArray());
                                        var propertyValue = JsonConvert.SerializeObject(blockGridData);
                                        nodeRequest.SetValue(curPropertyType.Alias, propertyValue);
                                    }

                                }
                            }
                            else if (curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.blocklist")
                            {
                                var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);
                                if (dataType != null)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                                    var ncDataTypeModel = JsonConvert.DeserializeObject<Root>(jsonModel);
                                    var contentDataList = new List<BlockListElementData>();
                                    var settingsDataList = new List<BlockListElementData>();
                                    var layoutDataList = new List<Models.BlockListModels.BlockListLayoutItem>();
                                    if (ncDataTypeModel != null)
                                    {
                                        foreach (var item in ncDataTypeModel?.Blocks)
                                        {
                                            var contentTypeGuid = new Guid(item.contentElementTypeKey);
                                            var contentType = _contentTypeService.Get(contentTypeGuid);

                                            var settingsTypeGuid = new Guid(item.contentElementTypeKey);
                                            var settingsType = _contentTypeService.Get(settingsTypeGuid);

                                            var layoutItems = new List<Models.BlockListModels.BlockListLayoutItem>();

                                            var propertyType = contentType.CompositionPropertyTypes;

                                            var contentData = new List<BlockListElementData>();
                                            var settingsData = new List<BlockListElementData>();

                                            var contentUdi = Umbraco.Cms.Core.Udi.Create(Constants.UdiEntityType.Element, Guid.NewGuid());
                                            var settingsUdi = Umbraco.Cms.Core.Udi.Create(Constants.UdiEntityType.Element, Guid.NewGuid());
                                            if (propertyType != null)
                                            {
                                                var values = new Dictionary<string, object>();
                                                foreach (var type in propertyType)
                                                {
                                                    string descriptionBlockGrid = type.Description;
                                                    bool needToAddDataInBlockGrid = !string.IsNullOrWhiteSpace(descriptionBlockGrid) && descriptionBlockGrid.Contains("*exclude-udummy") ? false : true;
                                                    if (needToAddDataInBlockGrid)
                                                    {
                                                        var data = GetDataForNCProperty(type);
                                                        if (type.PropertyEditorAlias == "Umbraco.TinyMCE")
                                                            values.Add(type.Alias, new HtmlString(data));
                                                        else
                                                            values.Add(type.Alias, data);
                                                    }

                                                }
                                                contentDataList.Add(new BlockListElementData(contentType.Key, contentUdi, values));
                                                settingsDataList.Add(new BlockListElementData(settingsType.Key, settingsUdi, new Dictionary<string, object>
                                                {
                                                    { "featured", "0" },
                                                }));
                                                layoutDataList.Add(new Models.BlockListModels.BlockListLayoutItem(contentUdi, settingsUdi));
                                            }
                                        }


                                        var blockGridData = new BlockListData(
                                        new BlockListLayout(layoutDataList.ToArray()),
                                                                                   contentDataList.ToArray(),
                                                                                   settingsDataList.ToArray());
                                        var propertyValue = JsonConvert.SerializeObject(blockGridData);
                                        nodeRequest.SetValue(curPropertyType.Alias, propertyValue);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"Error on {curPropertyType.Alias} - {ex}";
                }
            }
        }
        private StringBuilder GetDataForGridByGridDataTypeModel(GridDataTypeModel gridDataTypeModel)
        {
            var JSONString = new StringBuilder();

            if (gridDataTypeModel.Items != null && gridDataTypeModel.Items.templates?.Any() == true && gridDataTypeModel.Items.layouts?.Any() == true)
            {
                var name = gridDataTypeModel.Items.templates?.FirstOrDefault()?.name;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var sectionName = gridDataTypeModel.Items.templates?.FirstOrDefault()?.sections?.FirstOrDefault()?.grid;
                    JSONString.Append("{");
                    JSONString.Append("\" name\":" + "\"" + name + "\",");
                    JSONString.Append("\"sections\": [ { \"grid\":" + "\"" + sectionName + "\",");
                    JSONString.Append("\"rows\": [");
                    foreach (var row in gridDataTypeModel.Items.layouts)
                    {
                        if (row != null)
                        {
                            JSONString.Append("{");
                            JSONString.Append("\"name\": \"" + row.name + "\",");
                            //JSONString.Append("\"areas\": [ { \"grid \": \"" + row.areas.FirstOrDefault().grid + "\",");
                            JSONString.Append("\"areas\": [ ");
                            foreach (var area in row?.areas)
                            {
                                JSONString.Append("{ \"grid \": \"" + area?.grid + "\",");
                                JSONString.Append("\"controls\": [");
                                foreach (var control in area?.allowed)
                                {
                                    if (!string.IsNullOrWhiteSpace(control))
                                    {

                                        if (control == "rte")
                                        {
                                            var rteValueTest = "<p>This is Test RTE Content</p>";
                                            var escapedValue = HttpUtility.JavaScriptStringEncode(rteValueTest);
                                            JSONString.Append("{");
                                            JSONString.Append("\"value\": \"" + escapedValue + "\",");
                                            JSONString.Append("\"editor\": { \"name\" : \"Rich text editor\" ,");
                                            JSONString.Append("\"alias\" : \"" + control + "\" ,");
                                            JSONString.Append("\"view\" : \"rte\" ,");
                                            JSONString.Append("\"icon\" : \"icon-article\" ,");
                                            JSONString.Append("\"active\": false,");
                                            JSONString.Append("},");
                                            JSONString.Append("\"styles\": null,");
                                            JSONString.Append("\"config\": null");
                                            JSONString.Append("},");
                                        }


                                        //if (control.valueMedia != null)
                                        //{
                                        //    JSONString.Append("{");
                                        //    JSONString.Append("\"value\": { ");
                                        //    JSONString.Append("\"focalPoint\": { ");
                                        //    JSONString.Append("\"left\" : \"" + control.valueMedia.focalPoint.left + "\" ,");
                                        //    JSONString.Append("\"top\" : \"" + control.valueMedia.focalPoint.top + "\" ,");
                                        //    JSONString.Append("},");
                                        //    JSONString.Append("\"id\" : \"" + control.valueMedia.id + "\" ,");
                                        //    JSONString.Append("\"udi\" : \"" + control.valueMedia.udi + "\" ,");
                                        //    JSONString.Append("\"image\" : \"" + control.valueMedia.image + "\" ,");
                                        //    JSONString.Append("\"caption\" : \"" + control.valueMedia.caption + "\" ,");
                                        //    JSONString.Append("},");
                                        //    JSONString.Append("\"editor\": { \"name\" : \"Image\" ,");
                                        //    JSONString.Append("\"alias\" : \"" + control.editor.alias + "\" ,");
                                        //    JSONString.Append("\"view\" : \"media\" ,");
                                        //    JSONString.Append("\"render\" : \"\" ,");
                                        //    JSONString.Append("\"icon\" : \"icon-picture\" ,");
                                        //    JSONString.Append("\"config\": \"{}\",");
                                        //    JSONString.Append("},");
                                        //    JSONString.Append("\"styles\": null,");
                                        //    JSONString.Append("\"config\": null");
                                        //    JSONString.Append("},");
                                        //}

                                    }
                                }
                                JSONString.Append("],");
                                JSONString.Append("\"styles\": null,");
                                JSONString.Append("\"config\": null");
                                JSONString.Append("},");
                            }
                            JSONString.Append("],");
                            JSONString.Append("\"styles\": null,");
                            JSONString.Append("\"config\": null");

                            JSONString.Append("},");
                        }
                    }

                    JSONString.Append("]");
                    JSONString.Append("}");
                    JSONString.Append("]");
                    JSONString.Append("}");

                }
            }

            return JSONString;
        }
        private string HandleNestedCotentModel(NCDataTypeModel nCDataTypeModel, string NCalias)
        {
            string jsonValues = string.Empty;
            if (nCDataTypeModel.ContentTypes?.Any() == true)
            {
                var listOfValues = new List<Dictionary<string, string>>();
                int ncCount = 3;

                if (nCDataTypeModel.MinItems != null && Convert.ToInt32(nCDataTypeModel.MinItems) > 0 && Convert.ToInt32(nCDataTypeModel.MinItems) > ncCount)
                {
                    ncCount = Convert.ToInt32(nCDataTypeModel.MinItems);
                }

                if (nCDataTypeModel.MaxItems != null && Convert.ToInt32(nCDataTypeModel.MaxItems) > 0 && Convert.ToInt32(nCDataTypeModel.MaxItems) < ncCount)
                {
                    ncCount = Convert.ToInt32(nCDataTypeModel.MaxItems);
                }

                for (int i = 0; i < ncCount; i++)
                {
                    var values = new Dictionary<string, string>();
                    Guid guid = Guid.NewGuid();

                    values.Add("key", $"{guid}");
                    values.Add("name", $"ncTest");

                    foreach (var type in nCDataTypeModel.ContentTypes)
                    {
                        if (type != null && !string.IsNullOrWhiteSpace(type.ncAlias))
                        {
                            values.Add("ncContentTypeAlias", $"{type.ncAlias}");
                            var docTypeInsideNC = _contentTypeService.Get(type.ncAlias);

                            if (docTypeInsideNC != null)
                            {
                                var propertyTypes = docTypeInsideNC.CompositionPropertyTypes;

                                if (propertyTypes?.Any() == true)
                                {
                                    foreach (var curPropertyType in propertyTypes)
                                    {
                                        if (curPropertyType != null)
                                        {
                                            var data = GetDataForNCProperty(curPropertyType);

                                            values.Add($"{curPropertyType.Alias}", $"{data}");
                                        }
                                    }
                                }

                            }
                        }
                    }

                    listOfValues.Add(values);
                }

                if (listOfValues?.Any() == true)
                {
                    jsonValues = JsonConvert.SerializeObject(listOfValues);
                }
            }
            return jsonValues;
        }
        private string GetDataForNCProperty(IPropertyType curPropertyType)
        {
            string data = string.Empty;
            using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
            if (curPropertyType != null)
            {

                if (curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.textbox" || curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.textboxmultiple" || curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.textarea")
                {
                    data = $"This is test for {curPropertyType.Alias}";
                }

                else if (curPropertyType.PropertyEditorAlias.ToLower() == "umbraco.datetime")
                {
                    data = DateTime.Now.ToString();
                }

                else if (curPropertyType.PropertyEditorAlias == "Umbraco.MultiUrlPicker")
                {
                    List<Link> linksToAdd = new List<Link>();
                    Link link = new Link
                    {
                        Target = "_blank",
                        Name = "Test URL",
                        Url = "https://www.google.com/",
                        Type = LinkType.External

                    };

                    linksToAdd.Add(link);
                    var linkJson = JsonConvert.SerializeObject(linksToAdd);
                    data = linkJson;

                }

                else if (curPropertyType.PropertyEditorAlias == "Umbraco.TrueFalse")
                {
                    data = "1";
                }

                else if (curPropertyType.PropertyEditorAlias == "Umbraco.Integer")
                {
                    var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                    if (dataType != null)
                    {
                        var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                        if (!string.IsNullOrWhiteSpace(jsonModel))
                        {
                            var prevalues = GetConfigurationFromDataType(jsonModel);

                            int index = 0;
                            index = rnd.Next(50);

                            if (prevalues.min > 0 && prevalues.max > 0)
                            {
                                index = rnd.Next(prevalues.min, prevalues.max);
                            }
                            else if (prevalues.max > 0)
                            {
                                index = rnd.Next(prevalues.max);
                            }
                            data = index.ToString();
                        }
                    }
                }

                else if (curPropertyType.PropertyEditorAlias == "Umbraco.DropDown.Flexible" || curPropertyType.PropertyEditorAlias == "Umbraco.CheckBoxList")
                {
                    var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                    if (dataType != null)
                    {
                        var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                        if (!string.IsNullOrWhiteSpace(jsonModel))
                        {
                            var prevalues = GetConfigurationFromDataType(jsonModel);

                            if (prevalues != null && prevalues.Items != null && prevalues.Items.Any())
                            {
                                List<string> values = new List<string>();

                                int index = rnd.Next(prevalues.Items.Count);
                                var selected = prevalues.Items[index];
                                values.Add(selected.value);
                                if (prevalues.Multiple || curPropertyType.PropertyEditorAlias == "Umbraco.CheckBoxList")
                                {
                                    if (prevalues.Items.Count() > 1)
                                    {
                                        int index2 = rnd.Next(prevalues.Items.Count);
                                        var selected2 = prevalues.Items[index2];
                                        values.Add(selected2.value);
                                    }
                                }

                                if (values != null && values.Any())
                                {
                                    string[] valueArray = values.Distinct()?.ToArray();

                                    data = JsonConvert.SerializeObject(valueArray);
                                }
                            }
                        }

                    }


                }

                else if (curPropertyType.PropertyEditorAlias.Contains("Umbraco.MediaPicker"))
                {
                    var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                    if (dataType != null)
                    {
                        var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                        if (!string.IsNullOrWhiteSpace(jsonModel))
                        {
                            var prevalues = GetConfigurationFromDataType(jsonModel);

                            var existingRootFoler = _mediaService.GetRootMedia()?.Where(r => r.Name?.ToLower() == "udummy media")?.FirstOrDefault();

                            if (existingRootFoler != null && existingRootFoler.Id > 0)
                            {
                                var folder = umbracoContextReference.UmbracoContext.Media.GetById(existingRootFoler.Id);
                                if (folder != null)
                                {
                                    var mediaItems = folder.Descendants()?.Where(m => m.ContentType.Alias == "Image")?.ToList();

                                    if (mediaItems?.Any() == true)
                                    {
                                        List<IPublishedContent> randomMediaItems = new List<IPublishedContent>();

                                        int index = rnd.Next(mediaItems.Count);
                                        //if (index > 0)
                                        //{
                                        var randomMediaItem = mediaItems[index];

                                        if (randomMediaItem != null && randomMediaItem.Id > 0)
                                        {
                                            randomMediaItems.Add(randomMediaItem);

                                        }
                                        //}

                                        if (prevalues.Multiple)
                                        {
                                            int index2 = rnd.Next(mediaItems.Count);
                                            if (index2 > 0)
                                            {
                                                randomMediaItem = mediaItems[index2];

                                                if (randomMediaItem != null && randomMediaItem.Id > 0)
                                                {
                                                    randomMediaItems.Add(randomMediaItem);
                                                }
                                            }
                                        }

                                        if (randomMediaItems?.Any() == true)
                                        {
                                            if (randomMediaItems != null && randomMediaItems.Any())
                                            {
                                                data = GetMediaUDIs(randomMediaItems);
                                            }

                                        }
                                    }
                                }

                            }

                        }
                    }

                }

                else if (curPropertyType.PropertyEditorAlias == "Umbraco.NestedContent")
                {
                    var dataType = _dataTypeService.GetDataType(curPropertyType.DataTypeId);

                    if (dataType != null)
                    {
                        var jsonModel = JsonConvert.SerializeObject(dataType.Configuration);
                        if (!string.IsNullOrWhiteSpace(jsonModel))
                        {
                            var ncDataTypeModel = JsonConvert.DeserializeObject<NCDataTypeModel>(jsonModel);

                            if (ncDataTypeModel != null)
                            {
                                data = HandleNestedCotentModel(ncDataTypeModel, curPropertyType.Alias);
                            }
                        }
                    }
                }
                else if (curPropertyType.PropertyEditorAlias == "Umbraco.TinyMCE")
                {
                    var existingRootFoler = _mediaService.GetRootMedia()?.Where(r => r.Name?.ToLower() == "udummy media")?.FirstOrDefault();
                    if (existingRootFoler != null && existingRootFoler.Id > 0)
                    {
                        var folder = umbracoContextReference.UmbracoContext.Media.GetById(existingRootFoler.Id);
                        var mediaItems = folder.Descendants()?.Where(m => m.ContentType.Alias == "File" && m.Name == "TestRte")?.ToList();
                        data = System.IO.File.ReadAllText(_enviornment.ContentRootPath + "/wwwroot/" + mediaItems.First().Url());
                    }
                }


            }

            return data;
        }
        private string GetMediaFromUmbraco(string mediaID)
        {
            if (!string.IsNullOrWhiteSpace(mediaID))
            {
                using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
                var node = umbracoContextReference.UmbracoContext.Media.GetById(Convert.ToInt32(mediaID));
                if (node != null)
                {
                    var udiString = Umbraco.Cms.Core.Udi.Create("media", node.Key).ToString();
                    return udiString;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        private DataTypeModel GetConfigurationFromDataType(string jsonModel)
        {
            try
            {
                DataTypeModel dataTypeModel = JsonConvert.DeserializeObject<DataTypeModel>(jsonModel);

                if (dataTypeModel != null)
                {
                    return dataTypeModel;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        private void SetMediaItem(List<IPublishedContent> randomMediaItems, IContent nodeRequest, string alias)
        {
            if (randomMediaItems != null && randomMediaItems.Any())
            {
                var mediaUDIs = GetMediaUDIs(randomMediaItems);

                if (!string.IsNullOrWhiteSpace(mediaUDIs))
                {
                    nodeRequest.SetValue(alias, mediaUDIs);
                }
            }
        }

        private string GetMediaUDIs(List<IPublishedContent> randomMediaItems)
        {
            string listOfMediaIdsAreCSV = string.Empty;
            if (randomMediaItems != null && randomMediaItems.Any())
            {
                List<string> mediaUDIs = new List<string>();
                foreach (var randomMediaItem in randomMediaItems)
                {
                    if (randomMediaItem != null && randomMediaItem.Id > 0)
                    {
                        string mediaUDI = GetMediaFromUmbraco(randomMediaItem.Id.ToString());

                        if (!string.IsNullOrWhiteSpace(mediaUDI))
                        {
                            mediaUDIs.Add(mediaUDI);
                        }
                    }
                }


                if (mediaUDIs?.Any() == true)
                {
                    listOfMediaIdsAreCSV = string.Join(",", mediaUDIs);

                }
            }

            return listOfMediaIdsAreCSV;
        }
    }
}
