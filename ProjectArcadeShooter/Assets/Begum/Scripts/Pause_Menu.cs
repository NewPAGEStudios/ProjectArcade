using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause_Menu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    private GameController gc;


    [Header("SFX")]
    [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    public AudioSource hoverSound;
    [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
    public AudioSource swooshSound;


    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    public void Resume()
    {
        gc.ResumeGame();
    }
    public void OpenOption()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void LoadMenu(bool deleteSave)
    {
        if (deleteSave)
        {
            string path = Application.persistentDataPath + "/player.newp";
            File.Delete(path);
        }
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene(0);
    }

    //SFX 
    public void PlayHover()
    {
        hoverSound.Play();
    }

    public void PlaySwoosh()
    {
        swooshSound.Play();
    }


}
