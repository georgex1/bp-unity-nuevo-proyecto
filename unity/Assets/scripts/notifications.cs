using UnityEngine;
using System.Collections;

//Add these Namespaces
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility.Demo;

public class notifications : MonoBehaviour {

	private MainController GMS;

	[SerializeField, EnumMaskField(typeof(NotificationType))]
	private NotificationType	m_notificationType;
	
	
	void Start()
	{
		Debug.Log ("start notif controller");
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		string config_alerts = PlayerPrefs.GetString ("config_alerts");
		Debug.Log ("notificaciones activadas: " + config_alerts);
		NPBinding.NotificationService.RegisterNotificationTypes (m_notificationType);
		//if (config_alerts == "true") {
			NPBinding.NotificationService.RegisterForRemoteNotifications ();
		//}
	}

	public void disableNotifs(){
		NPBinding.NotificationService.UnregisterForRemoteNotifications ();
	}

	public void enableNotifs(){
		NPBinding.NotificationService.RegisterForRemoteNotifications ();
	}

	void OnEnable ()
	{
		Debug.Log ("enable notif");
		// Register RemoteNotificated related callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	+= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			+= DidReceiveRemoteNotificationEvent;
		
		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			+= DidReceiveLocalNotificationEvent;
		
	}
	
	void OnDisable ()
	{
		// Un-Register from callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	-= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			-= DidReceiveRemoteNotificationEvent;
		
		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			-= DidReceiveLocalNotificationEvent;
		
	}
	
	
	#region API Callbacks
	
	private void DidReceiveLocalNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Received DidReceiveLocalNotificationEvent : " + _notification.ToString());
	}
	
	private void DidReceiveRemoteNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Received DidReceiveRemoteNotificationEvent : " + _notification.ToString());
		string[] 	m_buttons				= new string[] { "Cancelar", "Ver" };
		NPBinding.UI.ShowAlertDialogWithMultipleButtons("Alerta!", _notification.AlertBody, m_buttons, MultipleButtonsAlertClosed); 
	}

	private void MultipleButtonsAlertClosed (string _buttonPressed)
	{
		if (_buttonPressed == "Ver") {
			GMS.call_updates ("notificaciones");
			Application.LoadLevel ("notificaciones");
		}
	}

	private void DidLaunchWithRemoteNotificationEvent (CrossPlatformNotification _notification)
	{
		GMS.call_updates ("notificaciones");
		Application.LoadLevel ("notificaciones");
	}
	
	private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
	{
		if(string.IsNullOrEmpty(_error))
		{
			Debug.Log("Device Token : " + _deviceToken);
			GMS.userData.reg_id = _deviceToken;


#if !UNITY_EDITOR
#if UNITY_ANDROID
			GMS.userData.plataforma = "Android";
#else
			GMS.userData.plataforma = "IOS";
#endif
#endif
		}
		else
		{
			Debug.Log("Error in registering for remote notifications : " + _deviceToken);
		}
	}
	
	#endregion
}
