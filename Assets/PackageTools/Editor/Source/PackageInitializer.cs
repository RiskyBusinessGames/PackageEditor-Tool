using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace RiskyBusiness.Packages.Tooling
{
    public class PackageInitializer : OdinEditorWindow
    {
        [MenuItem("RiskyBusinessGames/Tools/Package Initializer")]
        private static void OpenWindow()
        {
            GetWindow<PackageInitializer>().Show();
        }
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _packageName = "";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _version = "0.0.0";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _displayName = "";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _description = "";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _unity = "2020.1";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _unityRelease = "1f1";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _author = "";
        
        [PropertyOrder(2)]
        [FolderPath(RequireExistingPath = true)]
        [SerializeField] private string _packageDirectory;

        [PropertyOrder(2)]
        [Button]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreatePackageJSON()
        {
            if (_packageDirectory != string.Empty)
            {
                var packageModel = new PackageModel
                {
                    name = _packageName,
                    version = _version,
                    displayName = _displayName,
                    description = _description,
                    unity = _unity,
                    unityRelease = _unityRelease,
                    author = _author
                };

                string packageJSON = JsonUtility.ToJson(packageModel);

                string packagePath = Path.Combine(_packageDirectory, "package.json");

                bool state = EditorUtility.DisplayDialog("Create Package JSON?",
                    "Are you sure you want to create a new package file, it will override the existing package.json file.?", "Yes","No");

                if (state)
                {
                    try
                    {
                        File.WriteAllText(packagePath, packageJSON);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception exception)
                    {
                        Debug.Log($"Exception Caught when writing to file: {exception}");
                        throw;
                    }
                }
            }
        }
        
        [PropertyOrder(3)]
        [Button]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreateChangeLog()
        {
            CreateFile("CHANGELOG.md", "# CHANGELOG");
        }
        
        [PropertyOrder(4)]
        [Button]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreateReadMe()
        {
            CreateFile("README.md", "# README");
        }
        
        [PropertyOrder(4)]
        [Button]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreateLicense()
        {
            CreateFile("LICENSE.md", "# LICENSE");
        }

        private void CreateFile(string filename, string textToWrite)
        {
            if (_packageDirectory != string.Empty)
            {
                string packagePath = Path.Combine(_packageDirectory, filename);

                bool state = EditorUtility.DisplayDialog($"Create {filename}?",
                    "Are you sure you want to create a new {filename} file, it will override the existing {filename} file.?", "Yes","No");

                if (state)
                {
                    try
                    {
                        File.WriteAllText(packagePath, textToWrite);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception exception)
                    {
                        Debug.Log($"Exception Caught when writing to file: {exception}");
                        throw;
                    }
                }
            }
        }
    }
}

