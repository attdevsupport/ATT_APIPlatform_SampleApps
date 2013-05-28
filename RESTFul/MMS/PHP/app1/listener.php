<?php
require_once __DIR__ . '/lib/Util/FileUtil.php';
require_once __DIR__ . '/lib/Util/Util.php';

$path = __DIR__ . '/MMSImages';
if(!is_dir($path)) {
    mkdir($path);
}
$db_filename = $path . '/mmslistener.db';
$post_body = file_get_contents('php://input');

if (file_exists($db_filename)) {
    $messages = FileUtil::loadArray($db_filename); 
} else{
    $messages = null;
}

$local_post_body = $post_body;
$ini = strpos($local_post_body,"<SenderAddress>tel:+");
if ($ini == 0 )
{
    exit();
}else{
    preg_match("@<SenderAddress>tel:(.*)</SenderAddress>@i",$local_post_body,$matches);
    $message["address"] = $matches[1];
    preg_match("@<subject>(.*)</subject>@i",$local_post_body,$matches);
    $message["subject"] = $matches[1];
    $message["date"]= Util::getServerTime();
}

if( $messages !=null ){
    $last=end($messages);
    $message['id']=$last['id']+1;
}else{
    $message['id'] = 0;
}

mkdir($path.'/'.$message['id']);

$boundaries_parts = explode("--Nokia-mm-messageHandler-BoUnDaRy",$local_post_body);

foreach ( $boundaries_parts as $mime_part ){
    if ( preg_match( "@BASE64@",$mime_part )){
        $mm_part = explode("BASE64", $mime_part );
        $filename = null;
        $content_type =null;
        if ( preg_match("@Filename=([^;^\n]+)@i",$mm_part[0],$matches)){
            $filename = trim($matches[1]);
        }
        if ( preg_match("@Content-Type:([^;^\n]+)@i",$mm_part[0],$matches)){
            $content_type = trim($matches[1]);
        }
        if ( $content_type != null ){
            if ( $filename == null ){
                preg_match("@Content-ID: ([^;^\n]+)@i",$mm_part[0],$matches);
                $filename = trim($matches[1]);    
            }
            if ( $filename != null ){
                //Save file 
                $base64_data = base64_decode($mm_part[1]);
                $full_filename = $path.'/'.$message['id'].'/'.$filename;
                if (!$file_handle = fopen($full_filename, 'w')) {
                    echo "Cannot open file ($full_filename)";
                    exit;
                }
                fwrite($file_handle, $base64_data);
                fclose($file_handle);

                if ( preg_match( "@image@",$content_type ) && ( !isset($message["image"]))){
                    $message["image"]=$message['id'].'/'.$filename;
                }
                if ( preg_match( "@text@",$content_type ) && ( !isset($message["text"]))){
                    $message["text"]=$message['id'].'/'.$filename;
                }
            }
        }
    }
}

if( $messages !=null ){
    $messages_stored=array_push($messages,$message);
    if ( $messages_stored > 10 ){
        $old_message = array_shift($messages);
        // remove old message folder 
    }
}else{
    $messages = array($message);
}

FileUtil::saveArray($messages, $db_filename);
?>

