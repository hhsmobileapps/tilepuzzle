using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPopulate : MonoBehaviour
{
    public GameObject prefab; // This is our prefab object that will be exposed in the inspector

    public int numberToCreate; // number of objects to create. Exposed in inspector

    void Start()
    {
        //unlock level 1
        PlayerPrefs.SetInt("Level1",1);

        
        PlayerPrefs.SetInt("Level4", 1);
        PlayerPrefs.SetInt("Level8", 1);
        PlayerPrefs.SetInt("Level13", 1);
        PlayerPrefs.SetInt("Level20", 1);
        PlayerPrefs.SetInt("Level27", 1);
        PlayerPrefs.SetInt("Level33", 1);
        PlayerPrefs.SetInt("Level38", 1);
        PlayerPrefs.SetInt("Level43", 1);
        PlayerPrefs.SetInt("Level50", 1);
        


        Populate();
    }

    void Update()
    {

    }

    void Populate()
    {
        GameObject newObj; // Create GameObject instance

        for (int i = 0; i < numberToCreate; i++)
        {
            // Create new instances of our prefab until we've created as many as we specified
            newObj = (GameObject)Instantiate(prefab, transform);

            newObj.name = "Level" + (i+1).ToString();
            newObj.GetComponent<LevelItem>().SetLevelText((i + 1));
            newObj.GetComponent<LevelItem>().SetBgImage((i + 1));
            newObj.GetComponent<LevelItem>().levelIndex = i + 1;


        }

    }
}