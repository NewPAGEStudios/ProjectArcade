using SlimUI.ModernMenu;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject continueBut;
    string path;

    public GameObject LoadingScreen;


    private void Awake()
    {
        path = Application.persistentDataPath + "/player.newp";

        if (File.Exists(path))
        {
            continueBut.gameObject.SetActive(true);
        }
        else
        {
            continueBut.gameObject.SetActive(false);
        }
    }


    public void Play()
    {
        bool val = true;
        PlayerPrefs.SetInt("newGame",val ? 1 : 0);
        PlayerPrefs.Save();
        GetComponent<UIMenuManager>().LoadScene("MainScene_harbiMain");

    }
    public void Continue()
    {
        bool val = false;
        PlayerPrefs.SetInt("newGame", val ? 1 : 0);
        PlayerPrefs.Save();
        GetComponent<UIMenuManager>().LoadScene("MainScene_harbiMain");
    }
    public void deleteSave()
    {
        File.Delete(path);
        continueBut.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
}

