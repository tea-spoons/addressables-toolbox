
namespace TeaSpoons.AddressablesToolbox.Editor
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEditor;
    using TeaSpoons.PackageCore.Editor;

    [CustomPropertyDrawer(typeof(SmartReferenceBase<,>), true)]
    public class SmartReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var expectsComponent = false;
            var supportsComponents = false;

            if (property.TryGetTargetObject(out SmartReferenceBase smartReference))
            {
                expectsComponent = typeof(Component).IsAssignableFrom(smartReference.targetType);
                supportsComponents = smartReference.supportsComponents;
            }

            if (expectsComponent == supportsComponents)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative(SmartReferenceBase<Object, AssetReference>.PropertyNames.reference), label);
            }
            else
            {
                var controlId = GUIUtility.GetControlID(FocusType.Passive, position);
                position = EditorGUI.PrefixLabel(position, controlId, label);

                var message = expectsComponent ?
                    "Component references not supported." :
                    "Component reference required.";
                EditorGUI.HelpBox(position, message, MessageType.Error);
            }

            EditorGUI.EndProperty();
        }
    }
}
