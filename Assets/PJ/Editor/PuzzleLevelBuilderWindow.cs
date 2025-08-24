#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI; // for UI Image sprite fallback

public class PuzzleLevelBuilderWindow : EditorWindow
{
    [Header("Source & Target")]
    [SerializeField] private GameObject prefabOrInstance;           // Prefab asset or a scene instance
    [SerializeField] private PuzzleLevelData targetLevel;           // ScriptableObject to populate

    [Header("Hierarchy Roots (by name)")]
    [SerializeField] private string emptyRootName = "Empty";        // Parent that holds empty placeholders
    [SerializeField] private string fillRootName = "Fill";         // Parent that holds real pieces with sprites

    [Header("Options")]
    [SerializeField] private bool useLocalPositions = true;         // localPosition vs world position
    [SerializeField] private bool roundPositions = false;           // whether to round positions
    [SerializeField] private float roundTo = 0.01f;                 // grid/precision size when rounding
    [SerializeField] private bool deepSearch = false;               // scan only direct children or all descendants
    [SerializeField] private bool clearBeforePopulate = true;       // clear existing lists in the level asset

    [MenuItem("Window/Puzzle Level Builder")]
    public static void Open()
    {
        var win = GetWindow<PuzzleLevelBuilderWindow>(true, "Puzzle Level Builder", true);
        win.minSize = new Vector2(420, 360);
        win.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Source & Target", EditorStyles.boldLabel);
        prefabOrInstance = (GameObject)EditorGUILayout.ObjectField("Prefab / Instance", prefabOrInstance, typeof(GameObject), true);
        targetLevel = (PuzzleLevelData)EditorGUILayout.ObjectField("Target Level", targetLevel, typeof(PuzzleLevelData), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Hierarchy Roots (by name)", EditorStyles.boldLabel);
        emptyRootName = EditorGUILayout.TextField("Empty Root Name", emptyRootName);
        fillRootName = EditorGUILayout.TextField("Fill Root Name", fillRootName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
        useLocalPositions = EditorGUILayout.ToggleLeft("Use Local Positions (recommended)", useLocalPositions);
        roundPositions = EditorGUILayout.ToggleLeft("Round Positions", roundPositions);
        using (new EditorGUI.DisabledScope(!roundPositions))
        {
            roundTo = EditorGUILayout.Slider("Round To", Mathf.Max(0.0001f, roundTo), 0.0001f, 1f);
        }
        deepSearch = EditorGUILayout.ToggleLeft("Include All Descendants (deep scan)", deepSearch);
        clearBeforePopulate = EditorGUILayout.ToggleLeft("Clear Existing Lists", clearBeforePopulate);

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(prefabOrInstance == null || targetLevel == null))
        {
            if (GUILayout.Button("Read Prefab/Instance → Populate Level"))
            {
                PopulateFromSource(prefabOrInstance, targetLevel);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Select a prefab asset (Project) or an instance in the Scene.\n" +
                                "Builder will find children under 'Empty' and 'Fill' parents by name,\n" +
                                "capture their positions, and assign sprites (from SpriteRenderer or UI Image).\n" +
                                "For 'Empty' entries, if there is a matching name under 'Fill', its sprite is used.", MessageType.Info);
    }

    private void PopulateFromSource(GameObject source, PuzzleLevelData level)
    {
        if (source == null || level == null)
        {
            ShowNotification(new GUIContent("Assign both a Source and a Target Level"));
            return;
        }

        GameObject working = null;
        bool spawnedTemp = false;

        // If user provided a prefab asset, instantiate a temporary preview to read from
        var prefabType = PrefabUtility.GetPrefabAssetType(source);
        if (prefabType != PrefabAssetType.NotAPrefab && !source.scene.IsValid())
        {
            working = (GameObject)PrefabUtility.InstantiatePrefab(source);
            spawnedTemp = true;
        }
        else
        {
            // It's a scene object or a GameObject outside prefab system
            working = source;
        }

        try
        {
            if (working == null)
            {
                ShowNotification(new GUIContent("Could not access source object"));
                return;
            }

            var emptyRoot = FindChildRecursive(working.transform, emptyRootName);
            var fillRoot = FindChildRecursive(working.transform, fillRootName);

            if (emptyRoot == null && fillRoot == null)
            {
                ShowNotification(new GUIContent($"Neither '{emptyRootName}' nor '{fillRootName}' was found"));
                return;
            }

            if (clearBeforePopulate)
            {
                level.Empty_List = new List<PuzzlePieceData>();
                level.Fill_List = new List<PuzzlePieceData>();
            }
            else
            {
                if (level.Empty_List == null) level.Empty_List = new List<PuzzlePieceData>();
                if (level.Fill_List == null) level.Fill_List = new List<PuzzlePieceData>();
            }

            // Build a lookup for Fill sprites (by name) so Empty entries can borrow the sprite
            var fillSpriteLookup = new Dictionary<string, Sprite>();
            if (fillRoot != null)
            {
                foreach (var t in EnumChildren(fillRoot, deepSearch))
                {
                    var s = GetSpriteOn(t);
                    if (s != null) fillSpriteLookup[t.name] = s;
                }
            }

            // 1) Scan Empty children => Empty_List (positions, sprite from matching Fill if available)
            if (emptyRoot != null)
            {
                foreach (var t in EnumChildren(emptyRoot, deepSearch))
                {
                    var data = new PuzzlePieceData();
                    data.id = t.name;
                    data.correctPosition = ReadPos(t, useLocalPositions, roundPositions, roundTo);
                    Sprite spriteHere = GetSpriteOn(t);
                    if (spriteHere == null && fillSpriteLookup.TryGetValue(t.name, out var borrowed))
                        spriteHere = borrowed;
                    data.pieceImage = spriteHere;

                    level.Empty_List.Add(data);
                }
            }

            // 2) Scan Fill children => Fill_List (positions + their own sprite)
            if (fillRoot != null)
            {
                foreach (var t in EnumChildren(fillRoot, deepSearch))
                {
                    var data = new PuzzlePieceData();
                    data.id = t.name;
                    data.correctPosition = ReadPos(t, useLocalPositions, roundPositions, roundTo);
                    data.pieceImage = GetSpriteOn(t);
                    level.Fill_List.Add(data);
                }
            }

            // Persist changes
            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();

            ShowNotification(new GUIContent($"Level updated: {level.name}\nEmpty: {level.Empty_List.Count} | Fill: {level.Fill_List.Count}"));
            Debug.Log($"[PuzzleLevelBuilder] Updated '{level.name}'. Empty={level.Empty_List.Count}, Fill={level.Fill_List.Count}");
        }
        finally
        {
            if (spawnedTemp && working != null)
            {
                DestroyImmediate(working);
            }
        }
    }

    private static Transform FindChildRecursive(Transform root, string childName)
    {
        if (root == null || string.IsNullOrEmpty(childName)) return null;
        if (root.name == childName) return root;
        foreach (Transform c in root)
        {
            var r = FindChildRecursive(c, childName);
            if (r != null) return r;
        }
        return null;
    }

    private static IEnumerable<Transform> EnumChildren(Transform parent, bool deep)
    {
        if (parent == null) yield break;
        foreach (Transform child in parent)
        {
            yield return child;
            if (deep)
            {
                foreach (var d in EnumChildren(child, true))
                    yield return d;
            }
        }
    }

    private static Vector2 ReadPos(Transform t, bool local, bool round, float step)
    {
        Vector3 p = local ? t.localPosition : t.position;
        var v = new Vector2(p.x, p.y);
        if (round)
        {
            v.x = Mathf.Round(v.x / step) * step;
            v.y = Mathf.Round(v.y / step) * step;
        }
        return v;
    }

    private static Sprite GetSpriteOn(Transform t)
    {
        if (t == null) return null;
        var sr = t.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null) return sr.sprite;

        var img = t.GetComponent<Image>();
        if (img != null && img.sprite != null) return img.sprite;

        return null;
    }
}

// Optional Enhance your existing custom inspector to jump into the builder window
[CustomEditor(typeof(PuzzleLevelData))]
public class PuzzleLevelDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        if (GUILayout.Button("Open Puzzle Level Builder"))
        {
            PuzzleLevelBuilderWindow.Open();
        }
    }
}
#endif
