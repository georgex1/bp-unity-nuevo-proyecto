<?php 
if($_SERVER['HTTP_HOST'] == "localhost") {
    define("DB_NAME", 'bp_millasperrunas');
    define("DB_USER", 'root');
    define("DB_PASS", 'root');
}else{
    define("DB_NAME", 'thepasto_rednit');
    define("DB_USER", 'thepasto_georgex');
    define("DB_PASS", '630R63x!');
}

define("MAIL_FROM", 'noreply@millasperrunas.com');
define("MAIL_FROMNAME", 'millasperrunas.com');
define("UPLOAD_PATH", 'assets/images/perros/');

define("DB_HOST", 'localhost');
define("GOOGLE_API_KEY", 'AIzaSyCp5-RlXT50gLIz7zyWTnFCGq-NXvvYrQk');
define("SITE_URL", 'http://thepastoapps.com/proyectos/millasperrunas/response/');
?>