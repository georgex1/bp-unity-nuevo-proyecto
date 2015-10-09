using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

public class ImagePickerPerfil : MonoBehaviour {
	public Texture2D image;
	public GameObject imageTexture;
	
	private MainController GMS;
	
	internal void Start() {
		
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();
	}
	
	public void getImage(){
		#if UNITY_EDITOR
		test_guardar();
		#else
		NPBinding.MediaLibrary.PickImage(eImageSource.BOTH, 1.0f, PickImageFinished);
		#endif
	}
	
	private void test_guardar(){
		
		byte[] fileData = File.ReadAllBytes("Assets/Resources/fluence GT2.jpg");
		Texture2D  tex = new Texture2D(2, 2);
		tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		imageTexture.renderer.material.mainTexture = tex;

		GMS.userData.temp_img = GMS.generateId ().ToString () + ".png";
		
		StartCoroutine (GMS.saveTextureToFile (imageTexture.renderer.material.mainTexture as Texture2D, GMS.userData.temp_img, 'u'));
		StartCoroutine (loadImage ());
		
		/*
		Texture2D savedTexture = imageTexture.renderer.material.mainTexture as Texture2D;
		Texture2D newTexture = new Texture2D(savedTexture.width, savedTexture.height, TextureFormat.ARGB32, false);
		
		newTexture.SetPixels(0,0, savedTexture.width, savedTexture.height, savedTexture.GetPixels());
		newTexture.Apply();
		
		byte[] fileDataTS = newTexture.EncodeToPNG ();
		
		File.WriteAllBytes(Application.persistentDataPath + "/SavedScreen2.png", fileDataTS);
		Debug.Log (Application.persistentDataPath + "/SavedScreen2.png");
		
		Debug.Log (savedTexture.width + "x" + savedTexture.height);
		Sprite sprite = Sprite.Create (tex, new Rect(0,0, savedTexture.width, savedTexture.height), new Vector2(0f,0f));
		
		
		GameObject.Find ("backImage").GetComponent<Image>().sprite = sprite;*/
		
	}

	private void PickImageFinished (ePickImageFinishReason _reason, Texture2D _image)
	{
		string reasonString = _reason + "";
		if (reasonString == "SELECTED") {
			GMS.showLoading (true);
			image = _image;
			imageTexture.renderer.material.mainTexture = image;
		
			GMS.userData.temp_img = GMS.generateId ().ToString () + ".png";
		
			StartCoroutine (GMS.saveTextureToFile (imageTexture.renderer.material.mainTexture as Texture2D, GMS.userData.temp_img, 'u'));
			StartCoroutine (loadImage ());
		}
	}
	
	IEnumerator loadImage(){
		yield return new WaitForSeconds(1);
		Sprite sprite = GMS.spriteFromFile (GMS.userData.temp_img);
		GameObject.Find ("backImage").GetComponent<Image>().sprite = sprite;
		GMS.showLoading (false);
	}
	
}