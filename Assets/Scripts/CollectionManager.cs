using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    public GameObject pages;
    public GameObject pagePrefab;
    public Button prevPageButton, nextPageButton;

    public GameObject popupWindow;
    public Image popupImage;

    private int cardsInOnePage = 6; // !!!!!! pageprefab deki buton sayısı ile bu sayı aynı olmalı
    private int pageCount; // number of pages
    private Sprite[] allImages;
    private List<Sprite> unlockedImages;

    private int currentPageIndex;
    private int popupImageIndex;

    private int numOfTiles;
    
    private void Awake()
    {
        allImages = Resources.LoadAll<Sprite>("LevelImages");
        unlockedImages = new List<Sprite>();
        if (allImages.Length % cardsInOnePage > 0)
            pageCount = allImages.Length / cardsInOnePage + 1;
        else
            pageCount = allImages.Length / cardsInOnePage;
    }
    
    private void Start()
    {
        CreatePages();
        currentPageIndex = 0;
        prevPageButton.onClick.AddListener(delegate { PreviousPage(); });
        nextPageButton.onClick.AddListener(delegate { NextPage(); });
    }

    void CreatePages()
    {
        int levelIndex = 1;

        for (int i = 0; i < pageCount; i++)
        {
            // instantiate page prefab, find the image buttons
            var newPage = Instantiate(pagePrefab, new Vector3(0, 0, 0), Quaternion.identity, pages.transform);
            Button[] buttons = newPage.GetComponentsInChildren<Button>(true);

            for (int j = 0; j < cardsInOnePage; j++)
            {
                if ((i * cardsInOnePage + j) < allImages.Length)
                {
                    // set the image of each button
                    buttons[j].GetComponent<Image>().sprite = allImages[(i * cardsInOnePage) + j];
                    // check if image is unlocked, if unlocked add click listener
                    string imageName = buttons[j].image.sprite.name;
                    if (PlayerPrefs.GetInt("Level" + (levelIndex + 1).ToString()) == 1)
                    {
                        buttons[j].gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        buttons[j].gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        buttons[j].onClick.AddListener(delegate { ShowPopup(imageName); });
                        unlockedImages.Add(buttons[j].image.sprite);
                    }

                }
                else
                    Destroy(buttons[j].gameObject);

                levelIndex++;
            }

            // show the first page on start up and hide others
            if (i == 0)
                newPage.SetActive(true);
            else
                newPage.SetActive(false);

            // Set the page number text
            Text pageNumber = newPage.GetComponentInChildren<Text>(true);
            pageNumber.text = "PAGE - " + (i + 1);
        }
    }

    // find the index of clicked card and set the popup image
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
    

    void PreviousPage()
    {
        FindActivePage();
        if (currentPageIndex == 0)
            currentPageIndex = pageCount - 1;
        else
            currentPageIndex--;
        ActivateNewPage();
    }

    void NextPage()
    {
        FindActivePage();
        if (currentPageIndex == pageCount - 1)
            currentPageIndex = 0;
        else
            currentPageIndex++;
        ActivateNewPage();
    }

    void FindActivePage()
    {
        for (int i = 0; i < pages.transform.childCount; i++)
        {
            if (pages.transform.GetChild(i).gameObject.activeSelf == true)
            {
                currentPageIndex = i;
                break;
            }
        }
    }

    void ActivateNewPage()
    {
        for (int i = 0; i < pages.transform.childCount; i++)
        {
            if (i == currentPageIndex)
                pages.transform.GetChild(i).gameObject.SetActive(true);
            else
                pages.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    /*
    // Check for swipe action
    void Update()
    {
        if (popupWindow.activeSelf)
            if (popupImage.sprite != null)
                popupImageIndex = unlockedImages.FindIndex(card => card.name == popupImage.sprite.name);
        
        Swipe();
    }


    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public void Swipe()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //save began touch 2d point

        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            //normalize the 2d vector
            currentSwipe.Normalize();

            ChangePopupImage();
        }
#endif

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                firstPressPos = new Vector2(t.position.x, t.position.y); //save began touch 2d point

            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);
                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                //normalize the 2d vector
                currentSwipe.Normalize();

                ChangePopupImage();
            }
        }
    }

    void ChangePopupImage()
    {
        //swipe left (go forward)
        if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
        {
            if (popupImageIndex < unlockedImages.Count - 1)
                popupImage.sprite = unlockedImages[popupImageIndex + 1];
            else
                popupImage.sprite = unlockedImages[0];
        }
        //swipe right (go back)
        if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
        {
            if (popupImageIndex > 0)
                popupImage.sprite = unlockedImages[popupImageIndex - 1];
            else
                popupImage.sprite = unlockedImages[unlockedImages.Count - 1];
        }
    }
    */
}
