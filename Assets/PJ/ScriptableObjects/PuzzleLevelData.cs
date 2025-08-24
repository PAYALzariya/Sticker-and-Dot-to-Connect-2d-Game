using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleLevel", menuName = "Puzzle/Level")]
public class PuzzleLevelData : ScriptableObject
{
    public Sprite backgroundImage;
    public List<PuzzlePieceData> Empty_List,Fill_List;
}

[System.Serializable]
public class PuzzlePieceData
{
    public Sprite pieceImage;
    public Vector2 correctPosition;
    public string id;
}
