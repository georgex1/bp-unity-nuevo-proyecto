<?php
class NOTIF_IOS {
    function __construct() {
    }
    /*--- Enviando notificaciones push ----*/
    public function send_notification($registrationIdsArray, $message, $extraData) {
        //include_once 'includes/config.php';
        $passphrase = 'pushchat';
        $deviceToken = $registrationIdsArray[0];
        $ctx = stream_context_create();
        stream_context_set_option($ctx, 'ssl', 'local_cert', 'pushcert-ios-prod.pem');
        stream_context_set_option($ctx, 'ssl', 'passphrase', $passphrase);

        // Open a connection to the APNS server
        $fp = stream_socket_client(
                'ssl://gateway.push.apple.com:2195', $err,
                $errstr, 60, STREAM_CLIENT_CONNECT|STREAM_CLIENT_PERSISTENT, $ctx);

        if (!$fp)
            exit("Failed to connect: $err $errstr" . PHP_EOL);

        //echo 'Connected to APNS' . PHP_EOL;

        // Create the payload body
        $body['aps'] = array(
                'alert' => $message,
                'sound' => 'default'
                );

        // Encode the payload as JSON
        $payload = json_encode($body);

        // Build the binary notification
        $msg = chr(0) . pack('n', 32) . pack('H*', $deviceToken) . pack('n', strlen($payload)) . $payload;

        // Send it to the server
        $result = fwrite($fp, $msg, strlen($msg));

        /*if (!$result)
                echo 'Message not delivered' . PHP_EOL;
        else
                echo 'Message successfully delivered' . PHP_EOL;
*/
        // Close the connection to the server
        fclose($fp);
        
        
    }
    
    
    
    /*public function send_notification($registrationIdsArray, $message, $extraData) {
        //include_once 'includes/config.php';
        $message = '';
        
        $badge = 1;
        $sound = 'default';
        $development = true;

        $payload = array();
        $payload['aps'] = array('alert' => $message, 'badge' => intval($badge), 'sound' => $sound);
        $payload['extraData'] = $extraData;
        $payload = json_encode($payload);

        $apns_url = NULL;
        $apns_cert = NULL;
        $apns_port = 2195;

        if($development){
            $apns_url = 'gateway.sandbox.push.apple.com';
            $apns_cert = '/mnt/stor12-wc2-dfw1/582145/www.calendarionosotras.com.ar/web/content/php/admin/ck.pem';
        }
        else{
            $apns_url = 'gateway.push.apple.com';
            //$apns_cert = '/mnt/stor12-wc2-dfw1/582145/www.calendarionosotras.com.ar/web/content/php/admin/cert-prod.pem';
            $apns_cert = 'cert-prod.pem';
        }

        $stream_context = stream_context_create();
        stream_context_set_option($stream_context, 'ssl', 'local_cert', $apns_cert);

        $apns = stream_socket_client('ssl://' . $apns_url . ':' . $apns_port, $error, $error_string, 2, STREAM_CLIENT_CONNECT, $stream_context);

        //    You will need to put your device tokens into the $device_tokens array yourself
        $device_tokens = $registrationIdsArray;
        //$device_tokens = array("49e18ed4d533ff7c6a910a1c27735ba9146909797b2cca57cf0066e2db8860b6");

        foreach($device_tokens as $device_token){
            //echo $device_token.' - lero lero<br />';
            $apns_message = chr(0) . chr(0) . chr(32) . pack('H*', str_replace(' ', '', $device_token)) . chr(0) . chr(strlen($payload)) . $payload;
            fwrite($apns, $apns_message);
        }

        @socket_close($apns);
        @fclose($apns);
    }*/
}
?>