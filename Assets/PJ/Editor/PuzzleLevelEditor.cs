using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuzzleLevelData))]
public class PuzzleLevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PuzzleLevelData level = (PuzzleLevelData)target;
        if (GUILayout.Button("Print Puzzle Part Positions"))
        {
            foreach (var part in level.Empty_List)
            {
                Debug.Log($"Part {part.id} -> {part.correctPosition}");

            }
        }
    }
}
