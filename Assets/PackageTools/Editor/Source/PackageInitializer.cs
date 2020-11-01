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
        [SerializeField] private string _packageName;
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _version = "0.0.0";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _displayName;
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _description;
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _unity = "2020.1";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _unityRelease = "1f1";
        
        [PropertyOrder(1)]
        [BoxGroup("Package JSON Contents", centerLabel: true)]
        [SerializeField] private string _author;
        
        [PropertyOrder(1)]
        [HorizontalGroup("Group 1"), LabelWidth(85)]
        [Sirenix.OdinInspector.FilePath(Extensions = "json")]
        [SerializeField] private string _packagePath;
    }
}

