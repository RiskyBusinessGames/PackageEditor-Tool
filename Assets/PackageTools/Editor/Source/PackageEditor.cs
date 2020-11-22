using System;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RiskyBusiness.Packages.Tooling
{
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
        
        [MenuItem("RiskyBusinessGames/Tools/Package Management/Package Editor", false, 2)]
        private static void OpenWindow()
        {
            GetWindow<PackageEditor>().Show();
        }
        
        #region Package JSON
        
        [PropertyOrder(1)]
        [HorizontalGroup("Group -1"), LabelWidth(85)]
        [Sirenix.OdinInspector.FilePath(Extensions = "json")]
        [SerializeField] private string _packagePath;

        [PropertyOrder(1)]
        [HorizontalGroup("Group -1"), LabelWidth(85)]
        [DisableIf("@this._packagePath == string.Empty")]
        [Button]
        public void LoadPackageJson()
        {
            string fileContents = File.ReadAllText(_packagePath);
            _packageModel = JsonUtility.FromJson<PackageModel>(fileContents);
            
            _packageName = _packageModel.Name;
            _packageVersion = _packageModel.Version;
            _description = _packageModel.Description;
            _author = _packageModel.Author.Name;
            _licence = _packageModel.License;

            _version = _packageVersion;

            SetVersionContainerValues();
        }

        private void SetVersionContainerValues()
        {
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

        [PropertyOrder(2)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _packageName;

        [PropertyOrder(2)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _packageVersion;
        
        [PropertyOrder(2)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _description;
        
        [PropertyOrder(2)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _author;
        
        [PropertyOrder(2)]
        [BoxGroup("Package JSON Contents", centerLabel: true)] 
        [ReadOnly]
        [SerializeField] private string _licence;
        
        #endregion

        #region Package Version
        
        [PropertyOrder(3)]
        [PropertySpace(8)]
        [SerializeField] private string _version = "1.0.0";
        
        [PropertyOrder(3)]
        [HorizontalGroup("Group 1"), LabelWidth(15)]
        [Button]
        public void MajorVersion()
        {
            _versionType = VersionType.Major;
            UpdateVersionString(0);
        }
        
        [PropertyOrder(3)]
        [HorizontalGroup("Group 1"), LabelWidth(15)]
        [Button]
        public void MinorVersion()
        {
            _versionType = VersionType.Minor;
            UpdateVersionString(1);
        }
        
        [PropertyOrder(3)]
        [HorizontalGroup("Group 1"), LabelWidth(15)]
        [Button]
        public void PatchVersion()
        {
            _versionType = VersionType.Patch;
            UpdateVersionString(2);
        }
        
        #endregion

        #region Package ChangeLog
        
        [PropertyOrder(4)]
        [PropertySpace(16)]
        [HorizontalGroup("Group 2"), LabelWidth(100)]
        [Sirenix.OdinInspector.FilePath(Extensions = "md")]
        [SerializeField] private string _changeLogPath;

        [PropertyOrder(4)]
        [PropertySpace(16)]
        [HorizontalGroup("Group 2")]
        [DisableIf("@this._changeLogPath == string.Empty")]
        [Button]
        public void LoadLog()
        {
            try
            {
                string fileContents = File.ReadAllText(_changeLogPath);
                // Load the change log contexts into the text area for editing
                _changeLog = fileContents;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Couldn't load file, caught exception: {exception}");
            }
        }
        
        [PropertyOrder(4)]
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
                try
                {
                    File.WriteAllText(_changeLogPath, _changeLog);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Couldn't write to file, caught exception: {exception}");
                }

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
                var command = $@"npm version {_versionType.ToString().ToLower()}";
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
            
            // Reset the lower parts of the version string if major or minor
            switch (index)
            {
                case 0:
                    _versionContainer[1] = 0;
                    _versionContainer[2] = 0;
                    break;
                case 1:
                    _versionContainer[2] = 0;
                    break;
            }

            BuildVersionString();
        }

        private void BuildVersionString()
        {
            for (var i = 0; i < _versionContainer.Length; i++)
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
            string executingDirectory = GetExecutingDirectory();
            if (executingDirectory == string.Empty)
            {
                Debug.LogError($"No executing directory could be created, aborting: {command}");
                return;
            }
            
            // 

            ProcessStartInfo processStartInfo;
#if UNITY_EDITOR_OSX
            processStartInfo = TerminalCommand(command, executingDirectory);
#elif UNITY_EDITOR_64
            processStartInfo = WindowsCommand(command, executingDirectory);
#endif
            Process process = Process.Start(processStartInfo);

            if (process != null)
            {
                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd();
                Debug.Log(output);
                process.WaitForExit();
            }
            
            LoadPackageJson();
        }

        private ProcessStartInfo TerminalCommand(string command, string executingDirectory)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", //"/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal",
                Arguments = command,
                WorkingDirectory = executingDirectory,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                UseShellExecute = false
            };
            
            return processInfo;
        }

        private ProcessStartInfo WindowsCommand(string command, string executingDirectory)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/k {command}",
                WorkingDirectory = executingDirectory,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            
            return processInfo;
        }

        private string GetExecutingDirectory()
        {
            string fileName = Path.GetFileName(_packagePath);

            if (_packagePath != null)
            {
                if (fileName != string.Empty)
                {
                    string directoryPath = _packagePath.Replace(fileName, "");
                    directoryPath = directoryPath.Replace("Assets/", "");
                    var executingDirectory = $"{Application.dataPath}/{directoryPath}";
                    return executingDirectory;
                }
            }

            return string.Empty;
        }
        
        #endregion
    }
}