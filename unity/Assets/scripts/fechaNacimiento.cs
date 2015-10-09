using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fechaNacimiento : MonoBehaviour {
	public GameObject btnDay;
	public GameObject btnMonth;
	public GameObject btnYear;

	public GameObject DDDay;
	public GameObject DDMonth;
	public GameObject DDYear;

	private MainController GMS;

	// Use this for initialization
	void Start () {
		generarDias ();
		generarMeses ();
		generarAnios ();

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void selectDay(GameObject opcion){
		btnDay.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		Debug.Log (opcion.GetComponentInChildren<Text> ().text);
		GMS.userData.date_day = opcion.GetComponentInChildren<Text> ().text;
		DDDay.SetActive (false);
	}
	
	public void selectMonth(GameObject opcion){
		btnMonth.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		Debug.Log (opcion.GetComponentInChildren<Text> ().text);

		GMS.userData.format_month (opcion.GetComponentInChildren<Text> ().text);

		DDMonth.SetActive (false);
	}
	
	public void selectYear(GameObject opcion){
		btnYear.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		Debug.Log (opcion.GetComponentInChildren<Text> ().text);
		GMS.userData.date_year = opcion.GetComponentInChildren<Text> ().text;
		DDYear.SetActive (false);
	}

	public void generarDias(){
		string[] ArrayDias = new string[31];
		for(int i = 0; i < ArrayDias.Length; i++){
			int val = i + 1;
			ArrayDias[i] = val.ToString();
		}
		
		GameObject OptionDefault = GameObject.Find("DDDay/PanelMask/PanelScroll/Option");
		
		foreach (string aux in ArrayDias) {
			GameObject clone = Instantiate(OptionDefault, OptionDefault.transform.position, OptionDefault.transform.rotation) as GameObject;
			clone.transform.SetParent(OptionDefault.transform.parent);
			clone.transform.localScale = new Vector3(1, 1, 1);
			clone.GetComponentInChildren<Text> ().text = aux;
		}
		
		Destroy (OptionDefault);
		
		DDDay.SetActive(false);
	}
	
	public void generarMeses(){
		string[] ArrayMeses = new string[12]{
			"ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE", 
		};
		
		GameObject OptionDefault = GameObject.Find("DDMonth/PanelMask/PanelScroll/Option");
		
		foreach (string aux in ArrayMeses) {
			GameObject clone = Instantiate(OptionDefault, OptionDefault.transform.position, OptionDefault.transform.rotation) as GameObject;
			clone.transform.SetParent(OptionDefault.transform.parent);
			clone.transform.localScale = new Vector3(1, 1, 1);
			clone.GetComponentInChildren<Text> ().text = aux;
		}
		
		Destroy (OptionDefault);
		
		DDMonth.SetActive(false);
	}
	
	public void generarAnios(){
		int anioDesde = 1935;
		string[] ArrayAnios = new string[80];
		for(int i = 0; i < ArrayAnios.Length; i++){
			int val = i + anioDesde;
			ArrayAnios[i] = val.ToString();
		}
		
		GameObject OptionDefault = GameObject.Find("DDYear/PanelMask/PanelScroll/Option");
		
		foreach (string aux in ArrayAnios) {
			GameObject clone = Instantiate(OptionDefault, OptionDefault.transform.position, OptionDefault.transform.rotation) as GameObject;
			clone.transform.SetParent(OptionDefault.transform.parent);
			clone.transform.localScale = new Vector3(1, 1, 1);
			clone.GetComponentInChildren<Text> ().text = aux;
		}
		
		Destroy (OptionDefault);
		
		DDYear.SetActive(false);
	}
}
