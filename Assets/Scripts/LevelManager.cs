using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject puzzleGrid, tilePrefab, confettiRain, continueBtn, finishedPanel, hintPanel, pausePanel;
    public Text levelText, hintText;

    public AudioClip winSFX, dropSFX;

    Sprite imageSprite;
    Texture2D imageTexture;
    List<Vector3> rightPuzzlePos; //Start pos of puzzles used for checking finish

    int rows, columns, numOfTiles, numOfHints;

    float horCellSize, verCellSize;

    int lastLevelIndex;

    private void Awake()
    {
        // current level info
        lastLevelIndex = PlayerPrefs.GetInt("LastLevelIndex", 1);

        // determine number of tiles
        // IF PLAYING IN FREE MODE GET USER'S PREFERENCES (FROM IMAGES PANEL)
        if(PlayerPrefs.GetInt("IsFreeMode", 0) == 1)
        {
            rows = PlayerPrefs.GetInt("NumOfTiles", 4);
            columns = rows;
            imageSprite = Resources.Load<Sprite>("Puzzles/" + PlayerPrefs.GetString("SelectedImage"));
            // hide level info
            levelText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        // IF PLAYING IN LEVEL MODE GET THE PREDEFINED PARAMETERS
        else
        {
            int[] dimensions = GetNumOfCards(lastLevelIndex);
            rows = dimensions[0];
            columns = dimensions[1];
            imageSprite = Resources.Load<Sprite>("Puzzles/" + lastLevelIndex.ToString("00"));

            levelText.text = lastLevelIndex.ToString();
        }

        numOfTiles = rows * columns;
        numOfHints = Mathf.RoundToInt(numOfTiles * 0.2f);
        imageTexture = imageSprite.texture;
        /*
        // Set level text VERTICALLY GÜZEL OLMADI GİBİ SİLERSİN OLMADI
        levelText.text = "L\nE\nV\nE\nL\n";
        var intList = lastLevelIndex.ToString().Select(digit => digit.ToString()).ToList();
        foreach (var item in intList)
        {
            levelText.text += "\n" + item;
        }
        */
    }

    void Start()
    {
        AdjustGrid();
        DivideImage();
        ShuffleTiles();
    }

    void AdjustGrid()
    {
        // set the number fix column count
        puzzleGrid.GetComponent<GridLayoutGroup>().constraintCount = columns;
        // Get the height of panel, take it as reference and determine the width according to it
        float gridHeight = puzzleGrid.GetComponent<RectTransform>().rect.height;
        float ratio = (float)imageTexture.width / imageTexture.height;
        float gridWidth = gridHeight * ratio;
        // calculate the required horizontal/vertical space (paddings + space between adjacent buttons)
        float horSpaces = puzzleGrid.GetComponent<GridLayoutGroup>().padding.horizontal +
            puzzleGrid.GetComponent<GridLayoutGroup>().spacing.x * (columns - 1);
        float verSpaces = puzzleGrid.GetComponent<GridLayoutGroup>().padding.vertical +
            puzzleGrid.GetComponent<GridLayoutGroup>().spacing.y * (rows - 1);
        // calculate each button's available width / height
        horCellSize = (gridWidth - horSpaces) / columns;
        verCellSize = (gridHeight - verSpaces) / rows;
        // set the cell sizes
        puzzleGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(horCellSize, verCellSize);
    }

    // Divide the image into parts
    void DivideImage()
    {
        float height = imageTexture.height / rows;
        float width = imageTexture.width / columns;

        int index = 0;
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < columns; col++)
            {
                Sprite newSprite = Sprite.Create(imageTexture,
                    new Rect(col * width, row * height, width, height),
                    new Vector2(0.5f, 0.5f));
                
                GameObject n = Instantiate(tilePrefab);
                n.name = "" + index;
                n.GetComponent<Image>().sprite = newSprite;
                n.GetComponent <BoxCollider2D>().size = new Vector2(horCellSize, verCellSize);

                n.transform.SetParent(puzzleGrid.transform, false);

                index++;
            }
        }
    }

    void ShuffleTiles()
    {
        // Remove Gridlayout component (to make position arrangements)
        LayoutRebuilder.ForceRebuildLayoutImmediate(puzzleGrid.GetComponent<RectTransform>());
        Destroy(puzzleGrid.GetComponent<GridLayoutGroup>());

        // Save the right puzzle positions to a list before shuffling
        rightPuzzlePos = new List<Vector3>();
        for (int i = 0; i < puzzleGrid.transform.childCount; i++)
        {
            rightPuzzlePos.Add(puzzleGrid.transform.GetChild(i).transform.localPosition);
        }
        
        // Shuffle the tiles
        List<Vector3> randomP = new List<Vector3>();
        randomP = rightPuzzlePos.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < puzzleGrid.transform.childCount; i++)
        {
            puzzleGrid.transform.GetChild(i).transform.localPosition = randomP[i];
        }
    }

    public void CheckIsFinished()
    {
        // iterate over each child and check whether each one is in the correct place
        int partsDone = 0;
        for (int i = 0; i < puzzleGrid.transform.childCount; i++)
        {
            if (Vector2.Distance(puzzleGrid.transform.GetChild(i).transform.localPosition, rightPuzzlePos[i]) < 0.2f)
            {
                partsDone++;
            }
        }

        if (partsDone == puzzleGrid.transform.childCount)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        StartCoroutine(WinGameCoroutine());
    }
    
    IEnumerator WinGameCoroutine()
    {
        // Disable drag and drop property from all tiles
        DragAndDrop[] allScripts = FindObjectsOfType<DragAndDrop>();
        foreach (DragAndDrop dragDrop in allScripts)
        {
            dragDrop.enabled = false;
        }

        yield return new WaitForSeconds(0.25f);
        // show confetti, play sfx
        confettiRain.SetActive(true);
        SoundManager.Instance.PlaySFX(winSFX);

        yield return new WaitForSeconds(2f);
        // Show Interstitial Ad
        AdsManager.Instance.ShowInterstitialAd();

        yield return new WaitForSeconds(0.5f);

        // IF PLAYER IS PLAYING IN LEVEL MODE (NOT IN FREE MODE)
        if (PlayerPrefs.GetInt("IsFreeMode", 0) == 0)
        {
            // increment level index, set player pref's
            lastLevelIndex++;
            PlayerPrefs.SetInt("Level" + lastLevelIndex.ToString(), 1);

            if (lastLevelIndex <= 50)  // DİKKAT, ELLE GİRİLİYOR
            {
                PlayerPrefs.SetInt("LastLevelIndex", lastLevelIndex);
                continueBtn.SetActive(true);
            }
            else
            {
                finishedPanel.SetActive(true);
                PlayerPrefs.SetInt("LastLevelIndex", 1);
            }
            PlayerPrefs.Save();
        }
    }

    public void ShowHintPanel()
    {
        hintPanel.SetActive(true);
        hintText.text = numOfHints + " TILES WILL BE PUT TO THEIR CORRECT PLACE";
    }

    public void ShowHint()
    {
        StartCoroutine(HintCoroutine());
    }

    IEnumerator HintCoroutine()
    {
        yield return new WaitForSeconds(1f);

        int counter = 0;

        for (int i = 0; i < puzzleGrid.transform.childCount; i++)
        {
            // If the i-th tile is not in correct place
            if(Vector2.Distance(puzzleGrid.transform.GetChild(i).transform.localPosition, rightPuzzlePos[i]) > 0.2f)
            {
                // Find the tile that is in the place of i-th tile and interchange them
                for (int j = 0; j < puzzleGrid.transform.childCount; j++)
                {
                    if (j != i && Vector2.Distance(rightPuzzlePos[i], puzzleGrid.transform.GetChild(j).transform.localPosition) < 0.2f)
                    {
                        Vector3 tempPos = puzzleGrid.transform.GetChild(j).transform.localPosition;
                        puzzleGrid.transform.GetChild(j).transform.localPosition =
                            puzzleGrid.transform.GetChild(i).transform.localPosition;
                        puzzleGrid.transform.GetChild(i).transform.localPosition = tempPos;

                        SoundManager.Instance.PlaySFX(dropSFX);
                        counter++;
                        yield return new WaitForSeconds(.75f);
                        break;
                    }
                }
            }

            if (counter >= numOfHints)
                break;
        }

        CheckIsFinished();
    }

    public void ShowPausePanel()
    {
        pausePanel.SetActive(true);
        pausePanel.transform.GetChild(0).GetComponent<Image>().sprite = imageSprite;
    }

    // Set the divided images in a level wrt the level index FORMAT:[rows, columns]
    private int[] GetNumOfCards(int levelIndex)
    {
        if (levelIndex <= 2)
            return new int[] { 2, 3 };
        else if (levelIndex >= 3 && levelIndex <= 5)
            return new int[] { 2, 4 };
        else if (levelIndex >= 6 && levelIndex <= 8)
            return new int[] { 3, 4 };
        else if (levelIndex >= 9 && levelIndex <= 12)
            return new int[] { 3, 5 };
        else if (levelIndex >= 13 && levelIndex <= 17)
            return new int[] { 4, 5 };
        else if (levelIndex >= 18 && levelIndex <= 22)
            return new int[] { 4, 6 };
        else if (levelIndex >= 23 && levelIndex <= 27)
            return new int[] { 4, 7 };
        else if (levelIndex >= 28 && levelIndex <= 32)
            return new int[] { 5, 7 };
        else if (levelIndex >= 33 && levelIndex <= 35)
            return new int[] { 5, 8 };
        else if (levelIndex >= 36 && levelIndex <= 38)
            return new int[] { 5, 9 };
        else if (levelIndex >= 39 && levelIndex <= 41)
            return new int[] { 6, 8 };
        else if (levelIndex >= 42 && levelIndex <= 44)
            return new int[] { 6, 9 };
        else if (levelIndex >= 45 && levelIndex <= 46)
            return new int[] { 6, 10 };
        else if (levelIndex >= 47 && levelIndex <= 48)
            return new int[] { 7, 9 };
        else if (levelIndex >= 49 && levelIndex <= 50)
            return new int[] { 7, 10 };
        else
            return new int[] { 4, 5 };
    }

    public void ShowRewardedAd()
    {
        AdsManager.Instance.ShowRewardedAd();
    }

    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void LoadGame()
    {
        PlayerPrefs.SetInt("IsFreeMode", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Game");
    }
}
