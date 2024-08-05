using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause_Menu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField]private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionCanv;
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
        pauseMenuUI.GetComponent<Image>().color = new Color(pauseMenuUI.GetComponent<Image>().color.r, pauseMenuUI.GetComponent<Image>().color.g, pauseMenuUI.GetComponent<Image>().color.b, 1);
        pauseMenuUI.transform.GetChild(0).gameObject.SetActive(false);
        optionCanv.SetActive(true);
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
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
