using System.Collections;
using System;

[Serializable]
public class UserData {

	public int id;
	
	public string nombre,
	email,
	password,
	fecha_nacimiento,
	reg_id,
	plataforma,
	ciudad,
	fbid,
	sexo,
	foto;

	public string date_month;
	public string date_day;
	public string date_year;

	public byte[] ImgBytes;
	public string temp_img;
	
	//public Dictionary<string, int> ExercisesMetricas;
	
	public UserData(){
		id = 0;
		password = "";
		fecha_nacimiento = "";
		reg_id = "";
		plataforma = "";
		ciudad = "";
		fbid = "";
		sexo = "";
		foto = temp_img = "";
		//ExercisesMetricas = new Dictionary<string, int> ();
	}

	public void save(){

	}

	public void populateUser(string[] row_){
		id = int.Parse( row_ [0] );
		email = row_ [1];
		nombre = row_ [2];
		fbid = row_ [3];
		fecha_nacimiento = row_ [4];
		sexo = row_ [5];
	}

	public void format_month(string month_){
		int monthInt_ = 0;
		switch (month_) {
		case "ENERO": monthInt_ = 01; break;
		case "FEBRERO": monthInt_ = 02; break;
		case "MARZO": monthInt_ = 03; break;
		case "ABRIL": monthInt_ = 04; break;
		case "MAYO": monthInt_ = 05; break;
		case "JUNIO": monthInt_ = 06; break;
		case "JULIO": monthInt_ = 07; break;
		case "AGOSTO": monthInt_ = 08; break;
		case "SEPTIEMBRE": monthInt_ = 09; break;
		case "OCTUBRE": monthInt_ = 10; break;
		case "NOVIEMBRE": monthInt_ = 11; break;
		case "DICIEMBRE": monthInt_ = 12; break;
		}
		date_month = monthInt_.ToString();
	}

}
