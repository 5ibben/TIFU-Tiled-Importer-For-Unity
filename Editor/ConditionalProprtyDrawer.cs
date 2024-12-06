//code courtesy of: https://gamedev.stackexchange.com/questions/157127/how-to-make-a-property-visible-in-the-inspector-window-if-another-property-is-tr
using UnityEngine;
using UnityEditor;

public class ConditionalPropertyAttribute : PropertyAttribute
{
    public string condition;

    public ConditionalPropertyAttribute(string condition)
    {
        this.condition = condition;
    }
}

[CustomPropertyDrawer(typeof(ConditionalPropertyAttribute))]
public class ConditionalPropertyDrawer : PropertyDrawer
{

    // Determine whether this field should be visible.
    // (We could probably do some caching here...)
    bool ShouldShow(SerializedProperty property)
    {
        var conditionAttribute = (ConditionalPropertyAttribute)attribute;
        string conditionPath = conditionAttribute.condition;

        // If this property is defined inside a nested type 
        // (like a struct inside a MonoBehaviour), look for
        // our condition field inside the same nested instance.
        string thisPropertyPath = property.propertyPath;
        int last = thisPropertyPath.LastIndexOf('.');
        if (last > 0)
        {
            string containerPath = thisPropertyPath.Substring(0, last + 1);
            conditionPath = containerPath + conditionPath;
        }

        // Get the SerializedProperty representing the field that is our criterion.
        var conditionProperty = property.serializedObject.FindProperty(conditionPath);

        // For now, we'll only support bool criteria, and default to visible if there's a problem.
        if (conditionProperty == null || conditionProperty.type != "bool")
            return true;

        // Use the condition property's boolean value to drive visibility.
        return conditionProperty.boolValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
            EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            // Provision the normal vertical spacing for this control.
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            // Collapse the unseen derived property.
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}