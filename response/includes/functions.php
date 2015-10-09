<?php 
include('PHPMailer_v5.1/class.phpmailer.php');
include('config.php');
define("PAG_ITEMS", 50);

date_default_timezone_set('America/Argentina/Buenos_Aires');
setlocale(LC_ALL, 'es_ES');

function sql_connect() {
    global $con;
    $ret = false;
    $con = mysql_connect(DB_HOST, DB_USER, DB_PASS);
    mysql_set_charset('utf8', $con);

    if ($con != false) {
        mysql_select_db(DB_NAME, $con);
        $ret = true;
    }
    return $ret;
}

function sql_disconnect() {
    global $con;
    mysql_close($con);
}

if (!empty($_GET)) {
    foreach ($_GET as $nombre_var => $valor_var) {
        if (!empty($valor_var)) {
            $_GET[$nombre_var] = strip_tags($valor_var);
        }
    }
}

if (!empty($_POST)) {
    foreach ($_POST as $nombre_var => $valor_var) {
        if (!empty($valor_var)) {
            if (!is_array($_POST[$nombre_var])) {
                $_POST[$nombre_var] = strip_tags($valor_var);
            }
        }
    }
}

function utf8_encode_posts(){
    if (!empty($_POST)) {
        foreach ($_POST as $nombre_var => $valor_var) {
            if (!empty($valor_var)) {
                if (!is_array($_POST[$nombre_var])) {
                    $_POST[$nombre_var] = utf8_encode($valor_var);
                }
            }
        }
    }
}

function cleanQuery($string) {
    if (get_magic_quotes_gpc()) {  // prevents duplicate backslashes
        $string = stripslashes($string);
    }
    $badWords = array('/delete/', '/update/', '/union/', '/insert/', '/drop/');
    $string = preg_replace($badWords, '', $string);

    $string = filter_var($string, FILTER_SANITIZE_SPECIAL_CHARS);
    $string = @mysql_real_escape_string($string);

    return utf8_decode($string);
}

function upload_image($post_name, $image_name = '', $ismasterAdmin = false) {
    if (!preg_match('/.jpg/', $_FILES[$post_name] ['name']) && !preg_match('/.JPG/', $_FILES[$post_name] ['name']) && !preg_match('/.jpeg/', $_FILES[$post_name] ['name']) && !preg_match('/.gif/', $_FILES[$post_name] ['name']) && !preg_match('/.png/', $_FILES[$post_name] ['name'])) {
        return false;
    } else {
        $bfolder = './';
        if($ismasterAdmin){
            $bfolder = '../../';
        }
        $imgType = explode('.', $_FILES[$post_name] ['name']);
        $image_name = ($image_name == '') ? $_FILES [$post_name]['name'] : $image_name .'.'. $imgType[ count($imgType) -1 ];
        $ruta = UPLOAD_PATH . $image_name;
        move_uploaded_file($_FILES [$post_name]['tmp_name'], $bfolder . $ruta);
        return $image_name;
    }
}

sql_connect();

/* check emails */

function check_email($checkEmails) {
    $error = '';
    $regexp = '/^[a-z0-9]+([_\\.-][a-z0-9]+)*@([a-z0-9]+([\.-][a-z0-9]+)*)+\\.[a-z]{2,}$/';
    foreach ($checkEmails as $checkEmail) {
        if (!empty($checkEmail)) {
            $userEmails = true;
            /* Check the email format */
            if (!preg_match($regexp, $checkEmail)) {
                $error = ' email, ';
            }
            /* Prevets extra headers injection */
            if (preg_match("/\r/", $checkEmail) or preg_match("/\n/", $checkEmail)) {
                $error = ' El email ingresado no es correcto., ';
            }
        } else {
            $error = ' El email ingresado no es correcto.<br />';
        }
    }
    return $error;
}

function check_empty($fields, $pref = '', $labels = '') {
    $pref = ($pref != '') ? $pref : 'Check ';
    $error = '';
    $i = 0;
    foreach ($fields as $checkField) {
        if (empty($_POST[$checkField]) or ( is_int($_POST[$checkField]) and ( $_POST[$checkField] == 0))) {
            $ck = str_replace('id_', '', $checkField);
            $ck = str_replace('_', ' ', $checkField);
            $check = (@$labels[$i] != '') ? $labels[$i] : $ck;
            $error.=$pref . $check . ', ';
            $error . ', ';
        }
        $i++;
    }
    return $error;
}

function check_pass($pass, $confirm_pass, $size = 5) {
    $error = '';
    if (empty($_POST[$pass]) || $_POST[$confirm_pass] != $_POST[$pass] || strlen($_POST[$pass]) < $size) {
        $error = 'Check the passwords <br />';
    }
    return $error;
}

function db_update($table, $fields, $pk = ''/* valores para un update */, $show_query = 0) {
    // ej: $err=db_update('users',array('pass','user_name'),('id','id2'),1);
    // include("conexion.php");
    if ($pk != '') {
        $query = 'update ' . $table . ' set ';
        $total_fields_pk = count($pk);
        $cant_fields_pk = 1;
    } else
        $query = 'insert ignore into ' . $table . ' set ';
    $total_fields = count($fields);
    $cant_fields = 1;
    foreach ($fields as $field => $field_value) {
        if ($field == 'pass' || $field == 'password') {
            $query.= $field . " = '" . md5(cleanQuery($field_value)) . "' ";
        } else {
            $query.=$field . " = '" . cleanQuery($field_value) . "' ";
        }
        if ($total_fields != $cant_fields)
            $query.=' , ';
        $cant_fields++;
    }
    if ($pk != '') {
        $query.=' where ';
        foreach ($pk as $field => $field_value) {
            $query.=$field . " = '" . cleanQuery($field_value) . "' ";
            if ($total_fields_pk != $cant_fields_pk)
                $query.=' and ';
            $cant_fields_pk++;
        }
    }
    if ($show_query != 0)
        echo $query . '<br />';
    
    //file_put_contents('request.txt', $query, FILE_APPEND);
    sql_connect();
    if (mysql_query($query)) {
        return mysql_insert_id();
        sql_disconnect();
    } else {
        return mysql_insert_id();
        sql_disconnect();
    }
}

function get($table = '', $select = '*', $where = array(), $order = array(), $count = false) {
    $where_vals = ' WHERE 1';
    $order_vals = '';
    if (!empty($where)) {
        foreach ($where as $where_key => $where_val) {
            $compS = '';
            if (!strpbrk($where_key, '=<>')) {
                $compS = ' = ';
            }
            $where_vals .= ' AND ' . $where_key . " " . $compS . " '" . cleanQuery($where_val) . "'";
        }
    }

    if (!empty($order)) {
        $order_vals .= ' order by ';
        foreach ($order as $order_key => $order_val) {
            $order_vals .= $order_key . " " . cleanQuery($order_val) . ', ';
        }
        $order_vals = substr($order_vals, 0, -2);
    }

    $query = "Select " . $select . " FROM " . $table . " " . $where_vals;
    file_put_contents('sqldebug.txt', $query, FILE_APPEND);

    $sql = mysql_query($query);
    if ($count) {
        return mysql_num_rows($sql);
    } else {
        $data_ = array();
        while ($row = mysql_fetch_array($sql, MYSQL_ASSOC)) {
            $data_[] = $row;
        }
        return $data_;
    }
}

function encode_tojson($string = ''){
    return mb_convert_encoding(utf8_decode($string), "UTF-8", "HTML-ENTITIES");
}

function delete($id_ = '', $table = '', $showQuery = false) {
    $query = "";
    if (is_numeric($id_)) {
        $query = "delete from " . $table . " where id = '" . cleanQuery($id_) . "'";
        if (!mysql_query($query)) {
            return false;
        }
    } elseif (is_array($id_) && !empty($id_)) {
        $extw = " where ";
        foreach ($id_ as $id_key => $id_val) {
            $extw .= cleanQuery($id_key) . ' = ' . cleanQuery($id_val) . " and ";
        }
        $extw = substr($extw, 0, -4);
        $query = "delete from " . $table . $extw;
        if (!mysql_query($query)) {
            return false;
        }
    }
    if ($showQuery) {
        echo $query;
    }

    return true;
}

function getClientIP() {

    if (isset($_SERVER)) {

        if (isset($_SERVER["HTTP_X_FORWARDED_FOR"]))
            return $_SERVER["HTTP_X_FORWARDED_FOR"];

        if (isset($_SERVER["HTTP_CLIENT_IP"]))
            return $_SERVER["HTTP_CLIENT_IP"];

        return $_SERVER["REMOTE_ADDR"];
    }

    if (getenv('HTTP_X_FORWARDED_FOR'))
        return getenv('HTTP_X_FORWARDED_FOR');

    if (getenv('HTTP_CLIENT_IP'))
        return getenv('HTTP_CLIENT_IP');

    return getenv('REMOTE_ADDR');
}

function send_emails($data = array(), $subject = '', $addresses = array(), $template = 'email_template') {
    $email_content = file_get_contents(SITE_URL . 'includes/' . $template . '.html');

    $mail = new PHPMailer();
    $mail->IsHTML(true); // El correo se env�a como HTML

    $mail->CharSet = 'utf-8';

    $mail->From = MAIL_FROM;
    $mail->FromName = MAIL_FROMNAME;
    $mail->Subject = utf8_encode($subject);

    $emcontent = '';
    if (is_array($data)) {
        foreach ($data as $data_key => $data_value) {
            $emcontent .= '<p>' . $data_key . ': ' . $data_value . '</p>';
        }
    } else {
        $emcontent .= $data;
    }

    $email_content = str_replace('{{subject}}', $subject, $email_content);
    $email_content = str_replace('{{emailcontent}}', $emcontent, $email_content);

    foreach ($addresses as $address) {
        $mail->AddAddress($address);
    }

    $mail->Body = utf8_encode($email_content);

    $mail->Send();
}

function randomText($length) {
    $pattern = "1234567890abcdefghijklmnopqrstuvwxyz";
    $key = '';
    for ($i = 0; $i < $length; $i++) {
        $key .= $pattern{rand(0, 35)};
    }
    return $key;
}

function p($data_ = array()) {
    echo '<pre>';
    print_r($data_);
    echo '</pre>';
}

function getActualDate() {
    return date('Y-m-d H:i:s', $_SERVER['REQUEST_TIME']);
}

function getforpush() {
    $query = "select u.regid, u.plataforma, u.id as usuario_id, n.titulo, n.descripcion, n.id
        from usuarios u, notificaciones n
        where u.id NOT IN ( select usuarios_id from notificaciones_usuarios where notificaciones_id = n.id )
        and ( n.fecha_envio is null or n.fecha_envio <= NOW() ) and u.regid is not null
        limit 50
        ";

    $sql = mysql_query($query);
    $data_ = array();
    while ($row = mysql_fetch_array($sql, MYSQL_ASSOC)) {
        $data_[] = $row;
    }

    return $data_;
}

function user_agent(){
    $iPod = strpos($_SERVER['HTTP_USER_AGENT'],"iPod");
    $iPhone = strpos($_SERVER['HTTP_USER_AGENT'],"iPhone");
    $iPad = strpos($_SERVER['HTTP_USER_AGENT'],"iPad");
    $android = strpos($_SERVER['HTTP_USER_AGENT'],"Android");
    //file_put_contents('./public/upload/install_log/agent',$_SERVER['HTTP_USER_AGENT']);
    if($iPad||$iPhone||$iPod){
        return 'ios';
    }else if($android){
        return 'android';
    }else{
        return 'pc';
    }
}

?>