using DanielLochner.Assets.SimpleZoom;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PuzzleManager : MonoBehaviour
{
    
    public static PuzzleManager instance;
    private GameObject currentLevel;
    public GameObject dragObjectPrefab;
    public Transform levelParentTra,dragObjectParent;
    public int CurrentLevelCount, fillCount;
    private int fillCountTemp=1;
    public RectTransform contentREct;
    public List<GameObject> levelFillImage, levelemptyImage;
    public TextMeshProUGUI placedTextCount;
    Vector2 originalPos = Vector2.zero;
    [SerializeField] private RectTransform compassUI;   // whole compass container
    [SerializeField] private RectTransform compassArrow; // arrow image inside
    [SerializeField] private Text countdownText;        // UI text for timer
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            CurrentLevelCount = PlayerPrefs.GetInt("CurrentLevel");
        }
        else
        {
            CurrentLevelCount = 1;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevelCount);
        }
        originalPos = contentREct.anchoredPosition;
        LoadLevel();
    }
    /// <summary>
    /// Loads a prefab level from Resources/Prefabs/ by name.
    /// </summary>
    public void LoadLevel()
    {
        contentREct.anchoredPosition = originalPos;
        // Load prefab from Resources/Prefabs/{levelName}
        if (CurrentLevelCount == 4)
        {
            CurrentLevelCount = 1;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevelCount);
        }
        GameObject prefab = Resources.Load<GameObject>($"Level {CurrentLevelCount}");
        if (prefab != null)
        {
            // Destroy old level if it exists
            if (currentLevel != null)
            {
                Destroy(currentLevel);
            }

            // Instantiate new level under parent, keeping prefab's original size
            currentLevel = Instantiate(prefab, levelParentTra, false); 
            // Clear list first
            levelemptyImage.Clear();
            // Loop through children
            foreach (Transform child in currentLevel.transform.GetChild(2))
            {
                levelemptyImage.Add(child.gameObject);
            }
            // Clear list first
            levelFillImage.Clear();
            // Loop through children
            foreach (Transform child in currentLevel.transform.GetChild(1))
            {
                levelFillImage.Add(child.gameObject);//Add(child.GetComponent<Image>().sprite);
            }
            foreach (Transform child in dragObjectParent)
            {
                Destroy(child);
            }
            ReorderListempty();
            ReorderListFill();
            fillCountTemp =1;
            fillCount = 0;
            for (int i = 0; i < 3; i++)
            {
                GameObject dragobject = Instantiate(dragObjectPrefab, dragObjectParent,false);
                dragobject.GetComponent<Image>().sprite = levelemptyImage[i].GetComponent<Image>().sprite;
                dragobject.transform.GetChild(0).GetComponent<Image>().sprite = levelFillImage[i].GetComponent<Image>().sprite;
              //  dragobject.transform.GetChild(0).GetComponent<Image>()
                dragobject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (fillCountTemp.ToString());
                fillCountTemp++;
            }
            placedTextCount.text = $"PLACED {fillCount} / {levelFillImage.Count}";


            Debug.Log($"Loaded level prefab: " + currentLevel.name);
        }
        else
        {
            Debug.LogError($"Level {CurrentLevelCount} not found in Resources/Prefabs/");
        }
    }
    public void ReorderListempty()
    {
        levelemptyImage = levelemptyImage
             .OrderBy(go =>
             {
                 // Extract number inside parentheses from name "Image (X)"
                 var match = Regex.Match(go.name, @"\((\d+)\)");
                 if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                     return num;

                 // If no match, keep at end
                 return int.MaxValue;
             })
             .ToList();
    }
    public void ReorderListFill()
    {
        levelFillImage = levelFillImage
            .OrderBy(go =>
            {
                // Extract number inside parentheses from name "Image (X)"
                var match = Regex.Match(go.name, @"\((\d+)\)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                    return num;

                // If no match, keep at end
                return int.MaxValue;
            })
            .ToList();
    }
    public void LoadNewDragObject(int counterFill)
    {
        if (fillCountTemp - 1 < levelFillImage.Count)
        {
            Debug.Log("callnew");
            GameObject dragobject = Instantiate(dragObjectPrefab, dragObjectParent, false);
            dragobject.GetComponent<Image>().sprite = levelemptyImage[fillCountTemp - 1].GetComponent<Image>().sprite;
            dragobject.transform.GetChild(0).GetComponent<Image>().sprite = levelFillImage[fillCountTemp - 1].GetComponent<Image>().sprite;
            //  dragobject.transform.GetChild(0).GetComponent<Image>()
            dragobject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (fillCountTemp.ToString());
            fillCountTemp++;
        }
        if (fillCount <= levelFillImage.Count)
        {
            DestroyMatchingParents((counterFill).ToString());
         //   levelemptyImage[counterFill - 1].gameObject.SetActive(true);
            levelemptyImage[counterFill - 1].GetComponent<Image>().sprite= levelFillImage[counterFill - 1].GetComponent<Image>().sprite;
            fillCount++;
            placedTextCount.text = $"PLACED {fillCount} / {levelFillImage.Count}";

            if (fillCount == levelFillImage.Count || fillCount >= levelFillImage.Count)
            {
                Debug.Log("levelFillImage finish");
                StartCoroutine(loadnewLEvel());
            }
        }
    }
    IEnumerator loadnewLEvel()
    {
        currentLevel.transform.GetChild(0).gameObject.GetComponent<Image>().enabled=(true);
        currentLevel.transform.GetChild(2).gameObject.GetComponent<Mask>().showMaskGraphic = (false);
        yield return new WaitForSeconds(1.5f);
        CurrentLevelCount += 1;
        PlayerPrefs.SetInt("CurrentLevel", CurrentLevelCount);
        LoadLevel();
    }
    public void DestroyMatchingParents(string targetValue)
    {
        // Loop through all direct children of dragObjectParent
        foreach (Transform child in dragObjectParent)
        {
            TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null && tmp.text == targetValue)
            {
                Debug.Log($"Destroying {child.name} under {dragObjectParent.name} (TMP text == {targetValue})");
                Destroy(child.gameObject);
                return; // remove this return if you want to destroy ALL matches
            }
        }
    }
    public void LaodHomeSCene()
    {
       // PlayerPrefs.SetInt("CurrentLevel", currentlevel);
        SceneManager.LoadScene(0);
   }
    /// <summary>
    /// Unloads the current level prefab instance.
    /// </summary>
    public void UnloadLevel()
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
            currentLevel = null;
            Debug.Log("Unloaded current level.");
        }
    }
    public Image myImage;
    public float blinkDuration = 0.5f;
    public bool IsMagnify = false;
    [ContextMenu("XYZ")]

    public void DoMagnifyEffect()
    {

        GameObject dragobject = dragObjectParent.GetChild(0).gameObject;
        Debug.Log(dragobject.name);
        for (int i = 0; i < levelemptyImage.Count; i++)
        {
            if (dragobject.GetComponent<Image>().sprite.name == levelemptyImage[i].GetComponent<Image>().sprite.name)
            {
                SimpleZoom.simpleZoom.customPosition = levelemptyImage[i].transform.position;
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                 SimpleZoom.simpleZoom.GetComponentInParent<Canvas>().worldCamera,
                 levelemptyImage[i].transform.position
             );
                SimpleZoom.simpleZoom.CenterOnObject( levelemptyImage[i].GetComponent<RectTransform>(),  0.3f);


                blinkingimage = levelemptyImage[i].GetComponent<Image>();
                MagnifyEffect(levelemptyImage[i].GetComponent<Image>());
            }
        }

    }
    public void OnClickCompssEffect()
    {

        GameObject dragobject = dragObjectParent.GetChild(0).gameObject;
        Debug.Log(dragobject.name);
        for (int i = 0; i < levelemptyImage.Count; i++)
        {
            if (dragobject.GetComponent<Image>().sprite.name == levelemptyImage[i].GetComponent<Image>().sprite.name)
            {
                SimpleZoom.simpleZoom.customPosition = levelemptyImage[i].transform.position;
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                 SimpleZoom.simpleZoom.GetComponentInParent<Canvas>().worldCamera,
                 levelemptyImage[i].transform.position
             );
             //   SimpleZoom.simpleZoom.CenterOnObject(levelemptyImage[i].GetComponent<RectTransform>(), 0.3f);
                SimpleZoom.simpleZoom.ShowCompass(levelemptyImage[i].GetComponent<RectTransform>(),compassUI,compassArrow,countdownText);

                // blinkingimage = levelemptyImage[i].GetComponent<Image>();
                //  MagnifyEffect(levelemptyImage[i].GetComponent<Image>());
            }
        }

    }
    public RenderMode rm;
    public Image blinkingimage;
    public void MagnifyEffect(Image myImage)
    {
        //foreach (var tagged in StickerGameManager.instance.refSprites)
        {
            //  if (tagged.sprite == myImage.sprite && !tagged.isMatched)
            {
                BlinkImage(myImage);
            }
        }
    }


    public void BlinkImage(Image targetImage)
    {
        // Start the color tween loop: white ↔ green
      targetImage.DOColor(Color.green, blinkDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    public void StopBlinking(Image targetImage)
    {
        if (targetImage == blinkingimage)
        {
            DOTween.Kill(targetImage);
            targetImage.DOColor(Color.white, 0);
            IsMagnify = false;
        }
    }

}

/*  /* old  public PuzzleLevelData levelData;
    public GameObject piecePrefab;
    public RectTransform backgroundHolder;
    public RectTransform pieceHolder;
    public Image backgroundImage;
    public CanvasGroup canvasGroup;
    void Start()
{
    /* // Set background
     backgroundImage.sprite = levelData.backgroundImage;

     // Instantiate pieces
     foreach (var pieceData in levelData.pieces)
     {
         GameObject pieceObj = Instantiate(piecePrefab, pieceHolder);
         pieceObj.GetComponent<Image>().sprite = pieceData.pieceImage;
         pieceObj.GetComponent<Image>().SetNativeSize();
         PuzzlePiece pp = pieceObj.GetComponent<PuzzlePiece>();
         //  pp.canvas = GetComponentInParent<Canvas>();
         Debug.Log("Current position " + pieceData.correctPosition);
         pp.Init(pieceData.pieceImage, pieceData.correctPosition);

         // Randomize start position
         *//*pieceObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(
             Random.Range(-300, 300), Random.Range(-500, 500)
         );*//*
     }*/
