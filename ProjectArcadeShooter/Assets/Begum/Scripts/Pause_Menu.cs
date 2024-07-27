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

}
