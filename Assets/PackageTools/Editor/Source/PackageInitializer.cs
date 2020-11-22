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
        [MenuItem("RiskyBusinessGames/Tools/Package Management/Package Initializer", false, 1)]
        private static void OpenWindow()
        {
            GetWindow<PackageInitializer>().Show();
        }
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _packageName = "com.company.product";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _version = "0.0.0";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _displayName = "product name";
        
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
        [FolderPath(RequireExistingPath = true, AbsolutePath = true)]
        [SerializeField] private string _packageDirectory;

        [PropertyOrder(2)]
        [PropertySpace(8)]
        [Button]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreatePackageJSON()
        {
            if (_packageDirectory != string.Empty)
            {
                var packageModel = new PackageModel
                {
                    Name = _packageName,
                    Version = _version,
                    DisplayName = _displayName,
                    Description = _description,
                    Unity = _unity,
                    UnityRelease = _unityRelease,
                    Author = new Author { Name = _author },
                    PublishConfig = new PublishConfig(),
                    Repository = new Repository()
                };

                string packageJSON = packageModel.ToString();

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
        [PropertySpace(8)]
        [Button(ButtonSizes.Medium)]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreateChangeLog()
        {
            CreateFile("CHANGELOG.md", "# CHANGELOG");
        }
        
        [PropertyOrder(4)]
        [PropertySpace(8)]
        [Button(ButtonSizes.Medium)]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreateReadMe()
        {
            CreateFile("README.md", "# README");
        }
        
        [PropertyOrder(4)]
        [PropertySpace(8)]
        [Button(ButtonSizes.Medium)]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreateLicense()
        {
            CreateFile("LICENSE.md", "# LICENSE");
        }

        [PropertyOrder(5)]
        [PropertySpace(8)]
        [Button(ButtonSizes.Large)]
        [DisableIf("@this._packageDirectory == string.Empty")]
        public void CreateAll()
        {
            CreatePackageJSON();
            CreateChangeLog();
            CreateReadMe();
            CreateLicense();
        }
        
        private void CreateFile(string filename, string textToWrite)
        {
            if (_packageDirectory != string.Empty)
            {
                string packagePath = Path.Combine(_packageDirectory, filename);

                bool state = EditorUtility.DisplayDialog($"Create {filename}?",
                    $"Are you sure you want to create a new {filename} file, it will override the existing {filename} file.?", "Yes","No");

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

