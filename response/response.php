<?php

$isDebug = true;
if ($isDebug) {
    $dataEnter = "////////////////////////////////////////////////////
        " . date('d-m-Y h:i:s');
    file_put_contents('request.txt', $dataEnter, FILE_APPEND);
    file_put_contents('request.txt', print_r($_POST,true), FILE_APPEND);
}
$appHash = "R3dN1t!";
//db changes
//alter table premios add column tags varchar(95);
//alter table contenidos add column tags varchar(95);


header('Content-type: application/json; charset=utf-8');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

error_reporting(E_ALL ^ E_NOTICE);

$response = array();
$response['status'] = 'ok';
$response['content'] = array('mgs' => '', 'toclose' => (string)0, 'hasArray' => 0 );
$response['arrayData'] = array('data' => '');
$response['arrayContent'] = array('data' => '');

if ($appHash != @$_POST['appHash']) {
    $response['content'] = 'hash incorrecto';
    $response['status'] = 'error';
} else {
    include("includes/functions.php");

    if (@$_POST['action'] == 'check_connection') {
        $response['content']['mgs'] = 'check_connection';
    }
    
    if (@$_POST['action'] == 'login_facebook') {
        
        if($_POST['fbid'] != '' && $_POST['fbid'] != '0' && $_POST['fbid'] != 'null'){

            $regDataC = get('usuarios', '*', array('fbid' => $_POST['fbid']), '', true);

            $userData = array('fbid' => $_POST['fbid'], 'nombre' => $_POST['nombre'], 'email' => $_POST['email'], 
                'sexo' => @$_POST['sexo'], 'ciudad' => @$_POST['ciudad'], 'plataforma' => $_POST['plataforma'], 'regid' => $_POST['regid'] );
            
            if(@!empty($_POST['fecha_nacimiento'])){
                $formatDate = explode('/', $_POST['fecha_nacimiento']);
                $formatDate_ = $formatDate[2].'-'.$formatDate[0].'-'.$formatDate[1];
                $userData['fecha_nacimiento'] = $formatDate_;
            }
            $where_ = '';
            
            if($regDataC > 0){
                $where_ = array('fbid' => $_POST['fbid']);
            }else{
                
                //busco usuarios creados temporalmente con el email de facebook
                $userDataC_ = get('usuarios', '*', array('email' => $_POST['email'], 'password' => 'null', 'fbid' => 'null'), '', true);
                if($userDataC_ > 0){
                    $userData_ = get('usuarios', '*', array('email' => $_POST['email'], 'password' => 'null', 'fbid' => 'null'));
                    
                    $where_ = array('id' => $userData_[0]['id']);
                }
                
                $userData['fecha_entrada'] = getActualDate();
            }

            $insert_id = db_update('usuarios', $userData, $where_);
            if(@!empty($where_)){
                $uData = get("usuarios", 'id', array( 'fbid' => $_POST['fbid'] ) );
                $insert_id = $uData[0]['id'];
            }
            
            $response['content']['hasArray'] = 1;
            $response['arrayData']['id'] = (string)$insert_id;
        }else{
            $response['status'] = 'error';
            $response['content']['mgs'] = encode_tojson("Ocurrio un problema, por favor intentalo mas tarde.");
            $response['content']['toclose'] = (string)0;
        }
    }
    
    if (@$_POST['action'] == 'get_personas') {
        $_POST['usuarios_id'];
        
        $query = "select p.*, 
            pu.parentescos_id, pu.puntos, pu.kilometros, pu.invitado_por_usuarios_id, pu.puntos_totales, pu.aceptado,
            uu.nombre as usuario_nombre
            from perros p 
            inner join perros_usuarios pu on pu.perros_id = p.id
            left join usuarios uu on uu.id = pu.invitado_por_usuarios_id
            where pu.usuarios_id = '".cleanQuery($_POST['usuarios_id'])."'
            ";
        
        $sql = mysql_query($query);
        
        $empresasData2 = array();
        $i = 0;
        while ($dada = mysql_fetch_array($sql, MYSQL_ASSOC)) {
            $i ++;
            $empresasData2[$i] = array();
            //p($dada);
            foreach($dada as $dada2_key => $dada2_val){
                //$empresasData2[$i][$dada2_key] = mb_convert_encoding($dada2_val, "UTF-8", "HTML-ENTITIES");
                //p($dada2);
                $response['arrayContent'][$i][$dada2_key] =  encode_tojson($dada2_val) ;
            }
            $response['arrayContent'][$i] = json_encode($response['arrayContent'][$i]);

        }
        
        
        $response['content']['mgs'] = encode_tojson($type.'_updated');
        $response['content']['hasArray'] = encode_tojson($i);
    }
    
    if (@$_POST['action'] == 'upload_perfil') {
        
        $_POST['usuarios_id'];
        //subir imagen del perro
        $image_name = upload_image('fileUpload', str_replace('.png', '', $_POST['usuario_foto']) );
        db_update('usuarios', array('foto' => $image_name), array('id' => $_POST['usuarios_id']));
        
        $response['content']['mgs'] = 'imagen cargada correctamente';
    }
    
    if (@$_POST['action'] == 'get_updates') {
        $_POST['serverupdate'];
        $_POST['table'];
        
        $type = cleanQuery($_POST['table']);
        
        $where = array( 'serverupdate > ' => $_POST['serverupdate'] );
        /*if($type == 'respuestas_usuarios' || $type == 'notificaciones' || $type == 'videos_usuarios_empresas'){
            $where['usuarios_id'] = $_POST['usuarios_id'];
        }*/
        
        $empresasData2 = array();
        $empresasData =  get($type, '*', $where );
        $i = 0;
        if(!empty($empresasData)){
            foreach($empresasData as $dada){
                $i ++;
                $empresasData2[$i] = array();
                //p($dada);
                foreach($dada as $dada2_key => $dada2_val){
                    //$empresasData2[$i][$dada2_key] = mb_convert_encoding($dada2_val, "UTF-8", "HTML-ENTITIES");
                    //p($dada2);
                    $response['arrayContent'][$i][$dada2_key] =  encode_tojson($dada2_val) ;
                }
                $response['arrayContent'][$i] = json_encode($response['arrayContent'][$i]);
                
            }
        }
        
        $response['content']['mgs'] = encode_tojson($type.'_updated');
        $response['content']['hasArray'] = encode_tojson($i);
    }
    
    if (@$_POST['action'] == 'sync') {
        if($_POST['func'] == "perros_puntos"){
            //ej de response sync
            /*$fields_array = json_decode( $_POST['fields'] );
            $values_array = json_decode( $_POST['values'] );
            
            db_update("perros_usuarios", array('puntos' => $values_array[0], 'kilometros' => $values_array[1]), array('perros_id' => $values_array[2], 'usuarios_id' => $values_array[3]) );
            
            //insertar/actualizar paseo
            $c = get('paseos', '*', array('id' => $values_array[4]), '', true);
            $paseoData = array('puntos' => $values_array[6], 'kilometros' => $values_array[5], 'fecha_entrada' => getActualDate(), 'perros_id' => $values_array[2], 'usuarios_id' => $values_array[3]);
            
            if($c > 0){
                db_update("paseos", $paseoData, array('id' => $values_array[4]) );
            }else{
                db_update("paseos", $paseoData );
            }
            
            $response['content']['hasArray'] = 1;*/
        }
        if($_POST['func'] == "perros_invitacion_respuesta"){
            
        }
        
        $response['arrayData']['id'] = @(string)$_POST['id'];
    }
    
    
    
    
    //send_emails(utf8_decode($emailContent), $emailSubject, unserialize(EMAIL_ADMIN_UR));
}

$responseJson = json_encode($response);
if (@$_POST['callback'])
    echo $_POST['callback'] . "(" . $responseJson . ")";
else
    echo $responseJson;
?>