﻿using Newtonsoft.Json;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Extraction
{
    public class ArchiveSettings
    {
        [JsonProperty(PropertyName = "archiveId")]
        public string ArchiveId { get; set; }
        [JsonProperty(PropertyName = "autoExtract")]
        public bool? AutoExtract { get; set; }
        [JsonProperty(PropertyName = "extractPath")]
        public string ExtractPath { get; set; }
        [JsonProperty(PropertyName = "finalPassword")]
        public string FinalPassword { get; set; }
        [JsonProperty(PropertyName = "removeDownloadLinksAfterExtraction")]
        public bool? RemoveDownloadLinksAfterExtraction { get; set; }
        [JsonProperty(PropertyName = "removeFilesAfterExtraction")]
        public bool? RemoveFilesAfterExtraction { get; set; }
    }
}
