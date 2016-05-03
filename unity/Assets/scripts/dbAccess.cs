using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Text;

public class dbAccess : MonoBehaviour {
	private string connection;
	private SQLiteDB db = null;
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void OpenDB(string p)
	{
		db = new SQLiteDB();
		string filename = Application.persistentDataPath + "/" + p;
		//Debug.Log("Stablishing connection to: " + filename);
		db.Open(filename);
	}
	
	public void CloseDB(){
		db.Close();
	}
	
	public ArrayList BasicQueryArray(string query){
		
		SQLiteQuery qr;

		ArrayList readArray = new ArrayList();

		try{

			qr = new SQLiteQuery(db, query);

			while (qr.Step()) {

				string[] row = new string[qr.columnNames.Length];
				int j = 0;
				
				//Debug.Log( "tipo: " + qr.GetType() );
				foreach(string colName in qr.columnNames){

					if(qr.columnTypes[j] == 1){
						row [j] = qr.GetInteger(colName).ToString();
					}else if(qr.columnTypes[j] == 3 || qr.columnTypes[j] == 2){
						row [j] = qr.GetOther(colName);
					}else if(qr.columnTypes[j] == 5){
						row [j] = qr.GetOther(colName);
					}else{
						Debug.Log("no type 1 or 3, type: " + qr.columnTypes[j]);
					}

					/*if(colName == "id"){
						row [j] = qr.GetInteger(colName).ToString();
					}else{
						row [j] = qr.GetString(colName);
					}*/

					j++;
				}
				
				readArray.Add (row);
				//qr.GetType();
				
			}
			
			qr.Release();

		}
		catch(Exception e){
			Debug.Log(e);
		}

		return readArray; // return matches
		
		/*dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader();
		//string[,] readArray = new string[reader, reader.FieldCount];
		

		
		//if (reader.FieldCount > 0) {
		
		while (reader.Read()) {
			//Debug.Log(reader.GetString (1));
			string[] row = new string[reader.FieldCount];
			int j = 0;
			while (j < reader.FieldCount) {
				Debug.Log(reader.GetDataTypeName(j));
				if(reader.GetDataTypeName(j) ==  "int"){
					row [j] = reader.GetInt32(j).ToString();
				}else if(reader.GetDataTypeName(j) == "text"){
					try{
						row [j] = reader.GetString (j);
					}catch(Exception e){
						Debug.Log(e);
					}
				}
				
				j++;
			}
			readArray.Add (row);
		}
		//}
		reader.Close ();
		return readArray; // return matches*/
	}
	
	public int BasicQueryInsert(string query){
		try
		{
			SQLiteQuery qr;
			qr = new SQLiteQuery(db, query);
			qr.Step();
			qr.Release();
			
		}
		catch(Exception e){
			
			Debug.Log(e);
			return 0;
		}
		return 1;
	}
	
	
	public bool CreateTable(string name,string[] col, string[] colType){ // Create a table, name, column array, column type array
		string query;
		query  = "CREATE TABLE IF NOT EXISTS " + name + "(" + col[0] + " " + colType[0] + " PRIMARY KEY ";
		for(var i=1; i< col.Length; i++){
			query += ", " + col[i] + " " + colType[i];
		}
		query += ")";
		try{
			SQLiteQuery qr;
			qr = new SQLiteQuery(db, query);
			qr.Step();
			qr.Release();
		}
		catch(Exception e){
			
			Debug.Log(e);
			return false;
		}
		return true;
	}
	
	
	public int UpdateSingle(string tableName, string colName , string value, string whereName , string whereValue){ // single insert
		string query;
		query = "UPDATE " + tableName + " set "+colName+" = " + "'" + value + "' where "+whereName+" =  '"+whereValue+"' ";
		try
		{
			SQLiteQuery qr;
			qr = new SQLiteQuery(db, query);
			qr.Step();
			qr.Release();
		}
		catch(Exception e){
			
			Debug.Log(e);
			return 0;
		}
		return 1;
	}
	
	public int InsertIntoSingle(string tableName, string colName , string value ){ // single insert
		string query;
		query = "INSERT INTO " + tableName + "(" + colName + ") " + "VALUES (" + value + ")";
		try
		{
			SQLiteQuery qr;
			qr = new SQLiteQuery(db, query);
			qr.Step();
			qr.Release();
		}
		catch(Exception e){
			
			Debug.Log(e);
			return 0;
		}
		return 1;
	}
	
	public int InsertIgnoreInto(string tableName, string[] col, string[] values, string id_){ // Specific insert with col and values
		string query;
		
		ArrayList result = BasicQueryArray ("select id from " + tableName + " where id = '"+id_+"' ");
		if (result.Count == 0) {
			query = "INSERT INTO " + tableName + "(" + col[0];
			for(int i=1; i< col.Length; i++){
				query += ", " + col[i];
			}
			query += ") VALUES (" + "'"+values[0]+"'";
			for(int i=1; i< col.Length; i++){
				query += ", " + "'"+values[i] + "'";
			}
			query += ")";
		} else {
			query = "UPDATE " + tableName + " set " + col[0] + " = '"+values[0] + "'";
			for(int i=1; i< col.Length; i++){
				query += " , " + col[i] + " = '"+values[i] + "'";
			}
			query += " WHERE id = '"+ id_ + "' ";
		}
		
		
		//Debug.Log(query);
		try
		{
			SQLiteQuery qr;
			qr = new SQLiteQuery(db, query);
			qr.Step();
			qr.Release();
		}
		catch(Exception e){
			
			Debug.Log(e);
			return 0;
		}
		return 1;
	}
	
	public int InsertIntoSpecific(string tableName, string[] col, string[] values){ // Specific insert with col and values
		string query;
		query = "INSERT INTO " + tableName + "(" + col[0];
		for(int i=1; i< col.Length; i++){
			query += ", " + col[i];
		}
		query += ") VALUES (" + "'"+values[0]+"'";
		for(int i=1; i< col.Length; i++){
			query += ", " + "'"+values[i] + "'";
		}
		query += ")";
		//Debug.Log(query);
		try
		{
			SQLiteQuery qr;
			qr = new SQLiteQuery(db, query);
			qr.Step();
			qr.Release();
		}
		catch(Exception e){
			
			Debug.Log(e);
			return 0;
		}
		return 1;
	}
	
	public int InsertInto(string tableName , string[] values ){ // basic Insert with just values
		string query;
		query = "INSERT INTO " + tableName + " VALUES (" + values[0];
		for(int i=1; i< values.Length; i++){
			query += ", " + values[i];
		}
		query += ")";
		try
		{
			SQLiteQuery qr;
			qr = new SQLiteQuery(db, query);
			qr.Step();
			qr.Release();
		}
		catch(Exception e){
			
			Debug.Log(e);
			return 0;
		}
		return 1;
	}
	
	public ArrayList SingleSelectWhere(string tableName , string itemToSelect,string wCol,string wPar, string wValue){ // Selects a single Item
		string query;
		query = "SELECT " + itemToSelect + " FROM " + tableName + " WHERE " + wCol + wPar + wValue;	
		
		SQLiteQuery qr;
		qr = new SQLiteQuery(db, query);
		ArrayList readArray = new ArrayList();
		
		while (qr.Step()) {
			
			string[] row = new string[qr.columnNames.Length];
			int j = 0;
			
			//Debug.Log( "tipo: " + qr.GetType() );
			foreach(string colName in qr.columnNames){
				if(colName == "id"){
					row [j] = qr.GetInteger(colName).ToString();
				}else if(colName == "text"){
					row [j] = qr.GetString(colName);
				}
				j++;
			}
			
			readArray.Add (row);
			//qr.GetType();
			
		}
		
		qr.Release();
		return readArray; // return matches
	}
	
	public ArrayList getLastId(){ // Selects a single Item
		string query;
		query = " SELECT last_insert_rowid() as last_id " ;	
		
		SQLiteQuery qr;
		qr = new SQLiteQuery(db, query);
		ArrayList readArray = new ArrayList();
		
		while (qr.Step()) {
			
			string[] row = new string[qr.columnNames.Length];
			int j = 0;
			
			//Debug.Log( "tipo: " + qr.GetType() );
			foreach(string colName in qr.columnNames){
				if(colName == "id"){
					row [j] = qr.GetInteger(colName).ToString();
				}else if(colName == "text"){
					row [j] = qr.GetString(colName);
				}
				j++;
			}
			
			readArray.Add (row);
			//qr.GetType();
			
		}
		
		qr.Release();
		return readArray; // return matches
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}