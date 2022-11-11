using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Timer))]
public class TimerDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        var rect = new Rect(position.x, position.y, position.width, position.height);

        // Get time property
        SerializedProperty timeProperty = property.FindPropertyRelative("time");

        // Draw fields
        EditorGUI.PropertyField(rect, timeProperty, label);

        EditorGUI.EndProperty();
    }
}