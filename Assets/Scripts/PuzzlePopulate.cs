using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePopulate : MonoBehaviour
{
    public GameObject puzzleItem;
    public GameObject popupWindow;

    public int numOfLevels;

    Sprite[] allImages;

    void Start()
    {
        allImages = Resources.LoadAll<Sprite>("Puzzles");
        Populate();
    }

    void Populate()
    {
        GameObject newObj;

        for (int i = 0; i < allImages.Length; i++)
        {
            newObj = Instantiate(puzzleItem, transform);
            newObj.GetComponent<Image>().sprite = allImages[i];
            string imageName = newObj.GetComponent<Image>().sprite.name;

            // If i-th level is completed succesfully (for only first 50 puzzles) unlock it
            bool flag1 = (i <= numOfLevels && PlayerPrefs.GetInt("Level" + (i + 1).ToString()) == 1);
            // If all the levels completed succesfully then unlock all the remaining puzzles (for free game mode)
            bool flag2 = (i > numOfLevels && PlayerPrefs.GetInt("Level" + numOfLevels.ToString()) == 1);

            if (flag1 || flag2)
            {
                newObj.transform.GetChild(0).gameObject.SetActive(false);
                newObj.transform.GetChild(1).gameObject.SetActive(true);
                newObj.GetComponent<Button>().onClick.AddListener(delegate { ShowPopup(imageName); });
            }            
            
        }
    }

    void ShowPopup(string imgName)
    {
        popupWindow.SetActive(true);
        PlayerPrefs.SetString("SelectedImage", imgName);
        PlayerPrefs.Save();
    }

    // Get the user's preference (button click)
    public void GetTilePreference(int tiles)
    {
        PlayerPrefs.SetInt("IsFreeMode", 1);
        PlayerPrefs.SetInt("NumOfTiles", tiles);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
