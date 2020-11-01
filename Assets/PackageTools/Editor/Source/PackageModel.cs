using System;
using UnityEngine;

namespace RiskyBusiness.Packages.Tooling
{
    [Serializable]
    public class PackageModel
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unity;
        public string unityRelease;
        public string author;
        public string license;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}