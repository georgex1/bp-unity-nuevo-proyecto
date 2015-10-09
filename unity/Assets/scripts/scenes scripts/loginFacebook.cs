using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;

public class loginFacebook : MonoBehaviour {
	private bool enabled = false;

	void Start () {
		FB.Init(SetInit, OnHideUnity);
	}

	public void btnLogin(){
		FB.Login("email,user_birthday,user_friends", AuthCallback);
		//Application.LoadLevel("loader");
	}

	void AuthCallback(FBResult result) {
		if(FB.IsLoggedIn) {
			FB.API("/me?fields=id,name,first_name,last_name,email,birthday,gender", Facebook.HttpMethod.GET, APICallback);


			/*GameObject GM = GameObject.FindGameObjectWithTag ("mainController");
			mainController GMS = GM.GetComponent<mainController>();
			GMS.FbUserId = FB.UserId;
			print (GMS.FbUserId);*/

			Debug.Log(FB.UserId);
		} else {
			Debug.Log("User cancelled login");
		}
	}
	
	private void SetInit() {
		enabled = true; 
		print ("fb init");
		// "enabled" is a magic global; this lets us wait for FB before we start rendering
	}
	
	private void OnHideUnity(bool isGameShown) {
		/*if (!isGameShown) {
			// pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// start the game back up - we're getting focus again
			Time.timeScale = 1;
		}*/
	}

	void APICallback(FBResult result){
		if (result.Error != null){
			// Let's just try again
			FB.API("/me?fields=id,name,first_name,last_name,email,birthday,gender,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);
			return;
		}
		GameObject GM = GameObject.Find ("MainController");
		MainController GMS = GM.GetComponent<MainController>();

		IDictionary search = (IDictionary) Json.Deserialize (result.Text);

		GMS.userData.fbid = (string)search ["id"];
		GMS.userData.nombre = (string)search ["first_name"] + ' '+ (string)search ["last_name"];
		GMS.userData.email = (string)search ["email"];
		GMS.userData.sexo = (string)search ["gender"];
		GMS.userData.fecha_nacimiento = (string)search ["birthday"];

		GMS.SendMessage ("loginFacebook");
	}
}

