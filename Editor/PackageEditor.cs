using System;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RiskyBusiness.Tools
{
    [Serializable]
    public class PackageModel
    {
        public string name;
        public string version;
        public string description;
        public string author;
        public string license;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public enum VersionType
    {
        Major,
        Minor,
        Patch
    }

    public class PackageEditor : OdinEditorWindow
    {
        private VersionType _versionType;
        private PackageModel _packageModel;
        
        private readonly int[] _versionContainer = {1, 0, 0};
        private readonly StringBuilder _stringBuilder = new StringBuilder(12);
        
        [MenuItem("Tools/Package Editor")]
        private static void OpenWindow()
        {
            GetWindow<PackageEditor>().Show();
        }
        
        #region Package JSON
        
        [PropertyOrder(-10)]
        [HorizontalGroup("Group -1"), LabelWidth(85)]
        [Sirenix.OdinInspector.FilePath(Extensions = "json")]
        [SerializeField] private string _packagePath;

        [PropertyOrder(-10)]
        [HorizontalGroup("Group -1"), LabelWidth(85)]
        [DisableIf("@this._packagePath == string.Empty")]
        [Button]
        public void LoadPackageJson()
        {
            string fileContents = File.ReadAllText(_packagePath);
            _packageModel = JsonUtility.FromJson<PackageModel>(fileContents);
            
            _packageName = _packageModel.name;
            _packageVersion = _packageModel.version;
            _description = _packageModel.description;
            _author = _packageModel.author;
            _licence = _packageModel.license;

            _version = _packageVersion;

            // Convert the string version and update the int array for versions
            string[] versionParts = _version.Split('.');
            for (var i = 0; i < versionParts.Length; i++)
            {
                string part = versionParts[i];
                if (int.TryParse(part, out int versionValue))
                {
                    _versionContainer[i] = versionValue;
                }
            }
        }

        [PropertyOrder(-9)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _packageName;

        [PropertyOrder(-9)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _packageVersion;
        
        [PropertyOrder(-9)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _description;
        
        [PropertyOrder(-9)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _author;
        
        [PropertyOrder(-9)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _licence;
        
        #endregion

        #region Package Version
        
        [PropertyOrder(0)]
        [PropertySpace(8)]
        [SerializeField] private string _version = "1.0.0";
        
        [PropertyOrder(1)]
        [HorizontalGroup("Group 1"), LabelWidth(15)]
        [Button]
        public void MajorVersion()
        {
            _versionType = VersionType.Major;
            UpdateVersionString(0);
        }
        
        [PropertyOrder(1)]
        [HorizontalGroup("Group 1"), LabelWidth(15)]
        [Button]
        public void MinorVersion()
        {
            _versionType = VersionType.Minor;
            UpdateVersionString(1);
        }
        
        [PropertyOrder(1)]
        [HorizontalGroup("Group 1"), LabelWidth(15)]
        [Button]
        public void PatchVersion()
        {
            _versionType = VersionType.Patch;
            UpdateVersionString(2);
        }
        
        #endregion

        #region Package ChangeLog
        
        [PropertyOrder(2)]
        [PropertySpace(16)]
        [HorizontalGroup("Group 2"), LabelWidth(100)]
        [Sirenix.OdinInspector.FilePath(Extensions = "md")]
        [SerializeField] private string _changeLogPath;

        [PropertyOrder(2)]
        [PropertySpace(16)]
        [HorizontalGroup("Group 2")]
        [DisableIf("@this._changeLogPath == string.Empty")]
        [Button]
        public void LoadLog()
        {
            string fileContents = File.ReadAllText(_changeLogPath);
            _changeLog = fileContents;
        }
        
        [PropertyOrder(3)]
        [PropertySpace(16)]
        [TextArea(10, 15)]
        [HideLabel]
        [SerializeField] private string _changeLog;

        [PropertyOrder(4)]
        [DisableIf("@this._changeLog == string.Empty")]
        [Button]
        public void SaveLog()
        {
            bool state = EditorUtility.DisplayDialog("Save Log?",
                "Are you sure you wish to overwrite the current log?", "Yes","No");

            if (state)
            {
                File.WriteAllText(_changeLogPath, _changeLog);
            }
        }
        #endregion

        #region NPM Commands
        
        /*[PropertyOrder(5)]
        [PropertySpace(16)]
        [TextArea(2, 3)]
        [SerializeField] private string _versionCommitLog;*/
        
        [PropertyOrder(5)]
        [PropertySpace(8)]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void UpdatePackageVersion()
        {
            bool state = EditorUtility.DisplayDialog("Update Package Version?",
                "Are you sure you want to update this packages version?", "Yes","No");

            if (state)
            {
                string command = @"npm version " + _versionType.ToString().ToLower();
                ExecuteCommand(command);
            }
        }
        
        [PropertyOrder(6)]
        [PropertySpace(8)]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void PublishPackage()
        {
            bool state = EditorUtility.DisplayDialog("Publish Package?",
                "Are you sure you want to publish this package?", "Yes","No");

            if (state)
            {
                ExecuteCommand("npm publish");
            }
        }
        
        private void UpdateVersionString(int index)
        {
            _versionContainer[index]++;
            if (index == 0)
            {
                _versionContainer[1] = 0;
                _versionContainer[2] = 0;
            }
            
            if (index == 1)
            {
                _versionContainer[2] = 0;
            }
            
            for (int i = 0; i < _versionContainer.Length; i++)
            {
                _stringBuilder.Append(_versionContainer[i]);

                if (i < _versionContainer.Length - 1)
                {
                    _stringBuilder.Append(".");
                }
            }

            _version = _stringBuilder.ToString();
            _stringBuilder.Clear();
        }
        
        private void ExecuteCommand(string command)
        {
            string fileName = Path.GetFileName(_packagePath);
            
            if (_packagePath != null)
            {
                if (fileName != string.Empty)
                {
                    string directoryPath = _packagePath.Replace(fileName, "");
                    directoryPath = directoryPath.Replace("Assets/", "");
                    var executingDirectory = $"{Application.dataPath}/{directoryPath}";

                    var processInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/k {command}",
                        WorkingDirectory = executingDirectory,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    
                    };

                    Process process = Process.Start(processInfo);
                    process?.WaitForExit();
                
                    LoadPackageJson();
                }
            }
        }
        
        #endregion
    }
}