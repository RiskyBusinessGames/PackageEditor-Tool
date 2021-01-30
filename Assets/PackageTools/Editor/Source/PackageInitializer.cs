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
        
        [PropertyOrder(2)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _version = "0.0.0";
        
        [PropertyOrder(3)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _displayName = "product name";
        
        [PropertyOrder(4)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _description = "";
        
        [PropertyOrder(5)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _unity = "2020.1";
        
        [PropertyOrder(6)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _unityRelease = "1f1";
        
        [PropertyOrder(7)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _authorName = "";
        
        [PropertyOrder(8)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _authorEmail = "";
        
        [PropertyOrder(9)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _authorURL = "";
        
        [PropertyOrder(10)]
        [PropertySpace(8)]
        [FolderPath(RequireExistingPath = true, AbsolutePath = true)]
        [SerializeField] private string _packageDirectory;

        [PropertyOrder(11)]
        [PropertySpace(8)]
        [HorizontalGroup("Split", 0.5f, LabelWidth = 20)]
        [VerticalGroup("Split/Left")]
        [Button(ButtonSizes.Medium)]
        [DisableIf("@this._packageDirectory == string.Empty || this._packageDirectory == null")]
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
                    Author = new Author
                    {
                        Name = _authorName,
                        Email = _authorEmail,
                        URL = _authorURL
                    },
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
        
        [PropertyOrder(12)]
        [PropertySpace(8)]
        [VerticalGroup("Split/Left")]
        [Button(ButtonSizes.Medium)]
        [DisableIf("@this._packageDirectory == string.Empty || this._packageDirectory == null")]
        public void CreateChangeLog()
        {
            CreateFile("CHANGELOG.md", "# CHANGELOG");
        }
        
        [PropertyOrder(13)]
        [PropertySpace(8)]
        [VerticalGroup("Split/Right")]
        [Button(ButtonSizes.Medium)]
        [DisableIf("@this._packageDirectory == string.Empty || this._packageDirectory == null")]
        public void CreateReadMe()
        {
            CreateFile("README.md", "# README");
        }
        
        [PropertyOrder(14)]
        [PropertySpace(8)]
        [VerticalGroup("Split/Right")]
        [Button(ButtonSizes.Medium)]
        [DisableIf("@this._packageDirectory == string.Empty || this._packageDirectory == null")]
        public void CreateLicense()
        {
            CreateFile("LICENSE.md", "# LICENSE");
        }

        [PropertyOrder(15)]
        [PropertySpace(8)]
        [Button(ButtonSizes.Large)]
        [DisableIf("@this._packageDirectory == string.Empty || this._packageDirectory == null")]
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

