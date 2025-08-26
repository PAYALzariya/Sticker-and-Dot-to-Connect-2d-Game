using DanielLochner.Assets.SimpleZoom;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
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
        LoadLevel();
    }
    /// <summary>
    /// Loads a prefab level from Resources/Prefabs/ by name.
    /// </summary>
    public void LoadLevel()
    {

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
            fillCountTemp=1;
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


            Debug.Log($"Loaded level prefab: " + currentLevel.name);
        }
        else
        {
            Debug.LogError($"Level {CurrentLevelCount} not found in Resources/Prefabs/");
        }
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
            levelFillImage[counterFill - 1].gameObject.SetActive(true);
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
        
        GameObject dragobject =dragObjectParent.GetChild(0).gameObject;
        Debug.Log(dragobject.name);
        for (int i = 0; i < levelemptyImage.Count; i++)
        {
            if(dragobject.GetComponent<Image>().sprite.name == levelemptyImage[i].GetComponent<Image>().sprite.name)
            {
                Debug.Log(dragobject.GetComponent<Image>().sprite.name + "   " + levelemptyImage[i].GetComponent<Image>().sprite.name);
               // SimpleZoom.simpleZoom.ZoomTarget = ZoomTarget.Custom;
                SimpleZoom.simpleZoom.customPosition = levelemptyImage[i].transform.position;
                //SimpleZoom.simpleZoom.GoToPosition(levelemptyImage[i].transform.position,2f,0.2f);
                // Convert world position -> screen position
                // Convert the rect’s world position to screen position
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                    SimpleZoom.simpleZoom.GetComponentInParent<Canvas>().worldCamera,
                    levelemptyImage[i].transform.position
                );
                SimpleZoom.simpleZoom.Magi(screenPos);
                blinkingimage= levelemptyImage[i].GetComponent<Image>();
                MagnifyEffect(levelemptyImage[i].GetComponent<Image>());
            }
        }

    }
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
