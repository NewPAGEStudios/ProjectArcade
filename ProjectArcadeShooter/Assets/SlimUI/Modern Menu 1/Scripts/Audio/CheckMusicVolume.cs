using UnityEngine;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
namespace SlimUI.ModernMenu{
	public class CheckMusicVolume : MonoBehaviour {

		public AudioMixer auMix;

		void Awake ()
		{
            auMix.SetFloat("MasterVolume", Mathf.Lerp(-40,0,PlayerPrefs.GetFloat("MasterVolume")));
            if (PlayerPrefs.GetFloat("MasterVolume") == 0)
            {
                auMix.SetFloat("MasterVolume", -80);
            }

            auMix.SetFloat("MusicVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("MusicVolume")));
            if (PlayerPrefs.GetFloat("MusicVolume") == 0)
            {
                auMix.SetFloat("MusicVolume", -80);
            }

            auMix.SetFloat("SFXVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("SFXVolume")));
            if (PlayerPrefs.GetFloat("SFXVolume") == 0)
            {
                auMix.SetFloat("SFXVolume", -80);
            }

            auMix.SetFloat("SoundVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("SoundVolume")));
            if (PlayerPrefs.GetFloat("SoundVolume") == 0)
            {
                auMix.SetFloat("SoundVolume", -80);
            }
        }

        public void UpdateVolume ()
        {
            auMix.SetFloat("MasterVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("MasterVolume")));
            if (PlayerPrefs.GetFloat("MasterVolume") == 0)
            {
                auMix.SetFloat("MasterVolume", -80);
            }

            auMix.SetFloat("MusicVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("MusicVolume")));
            if (PlayerPrefs.GetFloat("MusicVolume") == 0)
            {
                auMix.SetFloat("MusicVolume", -80);
            }

            auMix.SetFloat("SFXVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("SFXVolume")));
            if (PlayerPrefs.GetFloat("SFXVolume") == 0)
            {
                auMix.SetFloat("SFXVolume", -80);
            }

            auMix.SetFloat("SoundVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("SoundVolume")));
            if (PlayerPrefs.GetFloat("SoundVolume") == 0)
            {
                auMix.SetFloat("SoundVolume", -80);
            }
        }
    }
}