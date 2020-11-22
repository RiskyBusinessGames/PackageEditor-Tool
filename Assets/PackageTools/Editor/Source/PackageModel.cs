using System;
using Newtonsoft.Json;
using UnityEngine;

namespace RiskyBusiness.Packages.Tooling
{
    [Serializable]
    public class PackageModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("Version")]
        public string Version { get; set; }
        
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("unity")]
        public string Unity { get; set; }
        
        [JsonProperty("unityRelease")]
        public string UnityRelease { get; set; }
        
        [JsonProperty("keywords")]
        public string[] Keywords { get; set; }
        
        [JsonProperty("author")]
        public Author Author { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("license")]
        public string License { get; set; }
        
        [JsonProperty("publishConfig")]
        public PublishConfig PublishConfig { get; set; }
        
        [JsonProperty("repository")]
        public Repository Repository { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("url")]
        public string URL { get; set; }
    }

    public class PublishConfig
    {
        [JsonProperty("registry")]
        public string Registry { get; set; }
    }
        
    public class Repository
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("url")]
        public string URL { get; set; }
        
        [JsonProperty("directory")]
        public string Directory { get; set; }
    }
}