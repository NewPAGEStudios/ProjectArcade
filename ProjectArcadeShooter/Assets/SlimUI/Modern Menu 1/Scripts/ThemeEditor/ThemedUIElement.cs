using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlimUI.ModernMenu{
	[System.Serializable]
	public class ThemedUIElement : ThemedUI {
		[Header("Parameters")]
		Color outline;
		Image image;
		GameObject message;
		public enum OutlineStyle {solidThin, solidThick, dottedThin, dottedThick};
		public bool hasImage = false;
		public bool isText = false;

		protected override void OnSkinUI(){
			base.OnSkinUI();

			if(hasImage){
				image = GetComponent<Image>();
				image.color = themeController.currentColor;
			}

			message = gameObject;

			if(isText){
				if(message.TryGetComponent<TextMeshPro>(out TextMeshPro tmpBoom))
				{
					tmpBoom.color = themeController.textColor;

                }
				else if(message.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI tmpUGUI))
				{
					tmpUGUI.color = themeController.textColor;
				}
			}
		}
	}
}