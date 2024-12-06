using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

[CustomEditor(typeof(TIFU_Importer))]
public class TIFU_ImporterEditor : ScriptedImporterEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tsxDir"), new GUIContent("Tileset Directory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("imageDir"), new GUIContent("Image Directory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("createTiles"), new GUIContent("Create Tiles"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("includeSprites"), new GUIContent("\tInclude Sprites"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("includeColliders"), new GUIContent("\tInclude Collisions"));

        base.ApplyRevertGUI();
    }
}