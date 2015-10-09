using UnityEngine;
using System.Collections;

public class login : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		MainController GMS = GM.GetComponent<MainController>();

		//auto login usuario
		GMS.db.OpenDB(GMS.dbName);
		ArrayList result = GMS.db.BasicQueryArray ("select id, email, nombre, fbid, fecha_nacimiento, sexo from usuarios limit 1");
		if (result.Count > 0) {
			GMS.userData.populateUser(  ((string[])result [0]) );

			Application.LoadLevel ("home");
		}
		GMS.db.CloseDB();

	}

}
