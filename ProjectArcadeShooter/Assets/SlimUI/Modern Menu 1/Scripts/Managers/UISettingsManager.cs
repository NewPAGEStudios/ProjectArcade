using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

namespace SlimUI.ModernMenu{
	public class UISettingsManager : MonoBehaviour {

		public enum Platform {Desktop, Mobile};
		public Platform platform;
		[Header("VIDEO SETTINGS")]
		public GameObject fullscreentext;
		public GameObject ambientocclusiontext;
		public GameObject texturelowtextLINE;
		public GameObject texturemedtextLINE;
		public GameObject texturehightextLINE;
		[SerializeField] private TMP_Dropdown resDropdown;


        [Header("GAME SETTINGS")]
		public GameObject swayBobtext;
        public GameObject damageVibrationtext;
		public GameObject minimapSmoothnestext;

        // sliders
        [Header("SLIDER SETTINGS")]
        public GameObject masterSlider;
        public GameObject musicSlider;
        public GameObject sFXSlider;
        public GameObject soundSlider;

        public GameObject miniMapSizeSlider;

        public GameObject sensitivityXSlider;
		public GameObject sensitivityYSlider;

		private float sliderValue = 0.0f;
		private float sliderValueMinimapSize = 0.0f;
        private float sliderValueXSensitivity = 0.0f;
		private float sliderValueYSensitivity = 0.0f;
		private float sliderValueSmoothing = 0.0f;

		private int currentResolutionIndex = 0;
		//Resolution
		private Resolution[] res;
		private List<Resolution> resList = new();
		private float scaleRatio;
        public void initValues()
        {
            PlayerPrefs.SetFloat("MinimapSize", PlayerPrefs.GetFloat("MinimapSize", 0));

            PlayerPrefs.SetFloat("YSensitivity", PlayerPrefs.GetFloat("YSensitivity", 10f));
            PlayerPrefs.SetFloat("XSensitivity", PlayerPrefs.GetFloat("XSensitivity", 10f));

            PlayerPrefs.SetInt("SwayNBobbing", PlayerPrefs.GetInt("SwayNBobbing", 1));
            PlayerPrefs.SetInt("DMGVibration", PlayerPrefs.GetInt("DMGVibration", 1));
            PlayerPrefs.SetInt("MinimapSmoothness", PlayerPrefs.GetInt("MinimapSmoothness", 1));

            PlayerPrefs.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume", 10f));
            PlayerPrefs.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 10f));
            PlayerPrefs.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", 10f));
            PlayerPrefs.SetFloat("SoundVolume", PlayerPrefs.GetFloat("SoundVolume", 10f));

			PlayerPrefs.SetInt("Textures", PlayerPrefs.GetInt("Texture", 1));
        }
        public void DefaultValues()
        {
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetFloat("MinimapSize", 0f);
			
			PlayerPrefs.SetFloat("YSensitivity", 10f);
            PlayerPrefs.SetFloat("XSensitivity", 10f);

            PlayerPrefs.SetInt("SwayNBobbing", 1);
            PlayerPrefs.SetInt("DMGVibration", 1);
            PlayerPrefs.SetInt("MinimapSmoothness",1);

            PlayerPrefs.SetFloat("MasterVolume",10f);
            PlayerPrefs.SetFloat("MusicVolume", 10f);
            PlayerPrefs.SetFloat("SFXVolume", 10f);
            PlayerPrefs.SetFloat("SoundVolume", 10f);

			PlayerPrefs.SetInt("Textures", 1);
			PlayerPrefs.SetInt("AmbientOcclusion", 1);

			miniMapSizeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MinimapSize", 0f);

            musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume", 10f);
            masterSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MasterVolume", 10f);
            sFXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXVolume", 10f);
            soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundVolume", 10f);

            sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity", 10);
            sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity", 10);

            // check minimapSmoth
            bool smMM = PlayerPrefs.GetInt("MinimapSmoothness") == 1f ? true : false;
            GameObject snMMParent = minimapSmoothnestext.transform.parent.gameObject;
            snMMParent.transform.GetChild(0).gameObject.SetActive(false);
            snMMParent.transform.GetChild(1).gameObject.SetActive(true);


            // check sway n Bobbing
            bool snb = PlayerPrefs.GetInt("SwayNBobbing", 1) == 1 ? true : false;
            GameObject snbParent = swayBobtext.transform.parent.gameObject;
            snbParent.transform.GetChild(0).gameObject.SetActive(false);
            snbParent.transform.GetChild(1).gameObject.SetActive(true);

            // check DamageVibration
            bool dv = PlayerPrefs.GetInt("DMGVibration", 1) == 1 ? true : false;
            GameObject dvParent = damageVibrationtext.transform.parent.gameObject;
            dvParent.transform.GetChild(0).gameObject.SetActive(false);
            dvParent.transform.GetChild(1).gameObject.SetActive(true);

            // check fullscreen
            GameObject fullscreen_parent = fullscreentext.transform.parent.gameObject;
			Screen.fullScreen = true;
            fullscreen_parent.transform.GetChild(0).gameObject.SetActive(false);
            fullscreen_parent.transform.GetChild(1).gameObject.SetActive(true);

            // check ambient occlusion
            GameObject ao_parent = ambientocclusiontext.transform.parent.gameObject;
            ao_parent.transform.GetChild(0).gameObject.SetActive(false);
            ao_parent.transform.GetChild(1).gameObject.SetActive(true);

            // check texture quality
            QualitySettings.SetQualityLevel(1);
            texturelowtextLINE.gameObject.SetActive(false);
            texturemedtextLINE.gameObject.SetActive(true);
            texturehightextLINE.gameObject.SetActive(false);
        }

        public void  Start (){
            initValues();

            //resolution init
            res = Screen.resolutions;


            ResDropdownUpdateValues();
			SetResolution(resList.IndexOf(Screen.currentResolution));

			// check slider values
			musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume",10f);
			miniMapSizeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MinimapSize", 0f);
			sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity", 10);
			sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity", 10);


            // check minimapSmoth
            bool smMM = PlayerPrefs.GetInt("MinimapSmoothness") == 1f ? true : false;
            GameObject snMMParent = minimapSmoothnestext.transform.parent.gameObject;
            snMMParent.transform.GetChild(0).gameObject.SetActive(false);
            snMMParent.transform.GetChild(1).gameObject.SetActive(false);
            if (smMM)
            {

                snMMParent.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                snMMParent.transform.GetChild(0).gameObject.SetActive(true);
            }


            // check sway n Bobbing
            bool snb = PlayerPrefs.GetInt("SwayNBobbing",1) == 1 ? true : false;
			GameObject snbParent = swayBobtext.transform.parent.gameObject;
			snbParent.transform.GetChild(0).gameObject.SetActive(false);
			snbParent.transform.GetChild(1).gameObject.SetActive(false);
			if (snb)
			{

				snbParent.transform.GetChild(1).gameObject.SetActive(true);
			}
			else
			{
				snbParent.transform.GetChild(0).gameObject.SetActive(true);
            }

			// check DamageVibration
			bool dv = PlayerPrefs.GetInt("DMGVibration",1) == 1 ? true : false;
			GameObject dvParent = damageVibrationtext.transform.parent.gameObject;
			dvParent.transform.GetChild(0).gameObject.SetActive(false);
			dvParent.transform.GetChild(1).gameObject.SetActive(false);
			if (dv)
			{
				dvParent.transform.GetChild(1).gameObject.SetActive(true);
                

            }
            else
			{
				dvParent.transform.GetChild(0).gameObject.SetActive(true);
			}


			// check fullscreen
			GameObject fullscreen_parent = fullscreentext.transform.parent.gameObject;
			fullscreen_parent.transform.GetChild(0).gameObject.SetActive(false);
			fullscreen_parent.transform.GetChild(1).gameObject.SetActive(false);
			if (Screen.fullScreen == true){

				fullscreen_parent.transform.GetChild(1).gameObject.SetActive(true);
				
			}
			else if(Screen.fullScreen == false){

				fullscreen_parent.transform.GetChild(0).gameObject.SetActive(true);
				
			}

			// check ambient occlusion
			GameObject ao_parent = ambientocclusiontext.transform.parent.gameObject;
			ao_parent.transform.GetChild(0).gameObject.SetActive(false);
			ao_parent.transform.GetChild(1).gameObject.SetActive(false);

			if (PlayerPrefs.GetInt("AmbientOcclusion", 1)==0){
				ao_parent.transform.GetChild(0).gameObject.SetActive(true);
				
			}
			else if(PlayerPrefs.GetInt("AmbientOcclusion",1)==1){

				ao_parent.transform.GetChild(1).gameObject.SetActive(true);
				
			}

			
            // check texture quality
            if (PlayerPrefs.GetInt("Textures") == 0){
				QualitySettings.SetQualityLevel(0);
				texturelowtextLINE.gameObject.SetActive(true);
				texturemedtextLINE.gameObject.SetActive(false);
				texturehightextLINE.gameObject.SetActive(false);
			}
			else if(PlayerPrefs.GetInt("Textures") == 1){
				QualitySettings.SetQualityLevel(1);
				texturelowtextLINE.gameObject.SetActive(false);
				texturemedtextLINE.gameObject.SetActive(true);
				texturehightextLINE.gameObject.SetActive(false);
			}
			else if(PlayerPrefs.GetInt("Textures") == 2){
				QualitySettings.SetQualityLevel(2);
				texturelowtextLINE.gameObject.SetActive(false);
				texturemedtextLINE.gameObject.SetActive(false);
				texturehightextLINE.gameObject.SetActive(true);
			}
		}

		public void Update (){
			sliderValueMinimapSize = miniMapSizeSlider.GetComponent<Slider>().value;

			sliderValueXSensitivity = sensitivityXSlider.GetComponent<Slider>().value;
			sliderValueYSensitivity = sensitivityYSlider.GetComponent<Slider>().value;
		}

        public void MiniMapSmooth()
        {
            bool smMM = PlayerPrefs.GetInt("MinimapSmoothness") == 1f ? true : false;
            GameObject snMMParent = minimapSmoothnestext.transform.parent.gameObject;
            snMMParent.transform.GetChild(0).gameObject.SetActive(false);
            snMMParent.transform.GetChild(1).gameObject.SetActive(false);
            if (smMM)
            {
                PlayerPrefs.SetInt("MinimapSmoothness", 0);
                snMMParent.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt("MinimapSmoothness", 1);
                snMMParent.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        public void SwayNBobbing()
		{
            bool snb = PlayerPrefs.GetInt("SwayNBobbing") == 1 ? true : false;
			GameObject snbParent = swayBobtext.transform.parent.gameObject;
			snbParent.transform.GetChild(0).gameObject.SetActive(false);
			snbParent.transform.GetChild(1).gameObject.SetActive(false);
			if (snb)
			{
				PlayerPrefs.SetInt("SwayNBobbing", 0);
				snbParent.transform.GetChild(0).gameObject.SetActive(true);
				
			}
			else
			{
				PlayerPrefs.SetInt("SwayNBobbing", 1);
				snbParent.transform.GetChild(1).gameObject.SetActive(true);
			}
		}

		public void dmgVibration()
		{
            bool dv = PlayerPrefs.GetInt("DMGVibration") == 1 ? true : false;
			GameObject dvParent = damageVibrationtext.transform.parent.gameObject;
			dvParent.transform.GetChild(0).gameObject.SetActive(false);
			dvParent.transform.GetChild(1).gameObject.SetActive(false);
			if (dv)
			{
				PlayerPrefs.SetInt("DMGVibration", 0);
				dvParent.transform.GetChild(0).gameObject.SetActive(true);


			}
			else
			{
				PlayerPrefs.SetInt("DMGVibration", 1);
				dvParent.transform.GetChild(1).gameObject.SetActive(true);
			}
		}


        public void FullScreen (){
			Screen.fullScreen = !Screen.fullScreen;

			GameObject fullscreen_parent = fullscreentext.transform.parent.gameObject;
			fullscreen_parent.transform.GetChild(0).gameObject.SetActive(false);
			fullscreen_parent.transform.GetChild(1).gameObject.SetActive(false);
			Debug.Log(Screen.fullScreen);
			if (Screen.fullScreen == true)
			{

				fullscreen_parent.transform.GetChild(0).gameObject.SetActive(true);

			}
			else if (Screen.fullScreen == false)
			{

				fullscreen_parent.transform.GetChild(1).gameObject.SetActive(true);

			}
		}
        public void MasterSlider()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterSlider.GetComponent<Slider>().value);
        }
        public void MusicSlider (){
			PlayerPrefs.SetFloat("MusicVolume", musicSlider.GetComponent<Slider>().value);
		}
        public void SFXSlider()
        {
            PlayerPrefs.SetFloat("SFXVolume", sFXSlider.GetComponent<Slider>().value);
        }
        public void SoundSlider()
        {
            PlayerPrefs.SetFloat("SoundVolume", soundSlider.GetComponent<Slider>().value);
        }

        public void sizeOfMinimapSlider()
        {
            sliderValueMinimapSize = miniMapSizeSlider.GetComponent<Slider>().value;
            PlayerPrefs.SetFloat("MinimapSize", sliderValueMinimapSize);
        }


        public void SensitivityXSlider ()
		{
			PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
		}

		public void SensitivityYSlider ()
		{
			PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
		}

		public void SensitivitySmoothing (){
			PlayerPrefs.SetFloat("MouseSmoothing", sliderValueSmoothing);
			Debug.Log(PlayerPrefs.GetFloat("MouseSmoothing"));
		}


		//public void ShadowsOff (){
		//	PlayerPrefs.SetInt("Shadows",0);
		//	QualitySettings.shadowCascades = 0;
		//	QualitySettings.shadowDistance = 0;
		//	shadowofftextLINE.gameObject.SetActive(true);
		//	shadowlowtextLINE.gameObject.SetActive(false);
		//	shadowhightextLINE.gameObject.SetActive(false);
		//}

		//public void ShadowsLow (){
		//	PlayerPrefs.SetInt("Shadows",1);
		//	QualitySettings.shadowCascades = 2;
		//	QualitySettings.shadowDistance = 75;
		//	shadowofftextLINE.gameObject.SetActive(false);
		//	shadowlowtextLINE.gameObject.SetActive(true);
		//	shadowhightextLINE.gameObject.SetActive(false);
		//}

		//public void ShadowsHigh (){
		//	PlayerPrefs.SetInt("Shadows",2);
		//	QualitySettings.shadowCascades = 4;
		//	QualitySettings.shadowDistance = 500;
		//	shadowofftextLINE.gameObject.SetActive(false);
		//	shadowlowtextLINE.gameObject.SetActive(false);
		//	shadowhightextLINE.gameObject.SetActive(true);
		//}

		//public void vsync (){
		//	if(QualitySettings.vSyncCount == 0){
		//		QualitySettings.vSyncCount = 1;
		//		vsynctext.GetComponent<TMP_Text>().text = "on";
		//	}
		//	else if(QualitySettings.vSyncCount == 1){
		//		QualitySettings.vSyncCount = 0;
		//		vsynctext.GetComponent<TMP_Text>().text = "off";
		//	}
		//}


		public void AmbientOcclusion (){
			GameObject ao_parent = ambientocclusiontext.transform.parent.gameObject;
			ao_parent.transform.GetChild(0).gameObject.SetActive(false);
			ao_parent.transform.GetChild(1).gameObject.SetActive(false);

			if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
			{
				PlayerPrefs.SetInt("AmbientOcclusion", 1);
				ao_parent.transform.GetChild(1).gameObject.SetActive(true);

			}
			else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
			{
				PlayerPrefs.SetInt("AmbientOcclusion", 0);
				ao_parent.transform.GetChild(0).gameObject.SetActive(true);

			}
		}


		public void TexturesLow (){
			PlayerPrefs.SetInt("Textures",0);
			QualitySettings.SetQualityLevel(0);
			texturelowtextLINE.gameObject.SetActive(true);
			texturemedtextLINE.gameObject.SetActive(false);
			texturehightextLINE.gameObject.SetActive(false);
		}

		public void TexturesMed (){
			PlayerPrefs.SetInt("Textures",1);
			QualitySettings.SetQualityLevel(1);
			texturelowtextLINE.gameObject.SetActive(false);
			texturemedtextLINE.gameObject.SetActive(true);
			texturehightextLINE.gameObject.SetActive(false);
		}

        public void TexturesHigh()
        {
			PlayerPrefs.SetInt("Textures",2);
			QualitySettings.SetQualityLevel(2);
			texturelowtextLINE.gameObject.SetActive(false);
			texturemedtextLINE.gameObject.SetActive(false);
			texturehightextLINE.gameObject.SetActive(true);
		}
		public void ResDropdownUpdateValues()
		{
			resDropdown.ClearOptions();
			resList.Clear();
			double currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i].refreshRateRatio.value == currentRefreshRate)
                {
                    resList.Add(res[i]);
                }
            }



            List<string> options_Res = new List<string>();
            for (int c = 0; c < resList.Count; c++)
            {
                string resolutionOption = resList[c].width + "x" + resList[c].height + " " + resList[c].refreshRateRatio + " Hz";
                options_Res.Add(resolutionOption);
                if (resList[c].width == Screen.width && resList[c].height == Screen.height)
                {
                    currentResolutionIndex = c;
                }
            }


            resDropdown.AddOptions(options_Res);
            resDropdown.value = currentResolutionIndex;
            resDropdown.RefreshShownValue();

        }
        public void SetResolution(int resolutionIndex)
		{
			Resolution resolution = resList[resolutionIndex];

			Camera.main.fieldOfView = 60;

			scaleRatio = ((float)resolution.width / (float)resolution.height) / 1.78f;

			Camera.main.fieldOfView /= scaleRatio;

			Camera.main.aspect = (float)resolution.width / (float)resolution.height;

            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);

        }
    }
}