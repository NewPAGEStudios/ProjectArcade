using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainGrid;
    string path;
    private void Awake()
    {
        path = Application.persistentDataPath + "/player.newp";

        if (File.Exists(path))
        {
            mainGrid.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            mainGrid.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    public void Play()
    {
        Debug.Log("OK");
        bool val = true;
        PlayerPrefs.SetInt("newGame",val ? 1 : 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Continue()
    {
        Debug.Log("OK");
        bool val = false;
        PlayerPrefs.SetInt("newGame", val ? 1 : 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void deleteSave()
    {
        File.Delete(path);
        mainGrid.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void Quit()
    {
        Debug.Log("Oyuncu Oyundan Çýktý");
        Application.Quit();
    }
}

