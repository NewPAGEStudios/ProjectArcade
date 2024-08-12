using UnityEngine;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace SlimUI.ModernMenu{
	public class CheckMusicVolume : MonoBehaviour {

		public AudioSource music;

		public void  Start ()
		{
			// remember volume level from last time
			if(SceneManager.GetActiveScene().name == "MainScene")
			{
				music.volume = PlayerPrefs.GetFloat("MusicVolume");
			}
			else
			{
                GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
            }
        }

		public void UpdateVolume ()
        {
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                music.volume = PlayerPrefs.GetFloat("MusicVolume");
            }
            else
            {
                GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
            }
        }
    }
}