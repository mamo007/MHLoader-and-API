<?php
error_reporting(0); //Disable ALL ERRORS
include("database_config.php"); //Include Database
?>

<?php
//Data
$UserAgent = "123";
$UserLogin = $_GET["login"];
$EncryptionPassword= "ds54bd5asf4d4a5sd";

// Encryption Part
function Encrypt($Data, $EncryptionKey)
{
	$EncryptionSalt = substr(time(), 0, -1);
	
	$EncryptedHash = hash_pbkdf2("sha1", $EncryptionKey, mb_convert_encoding($EncryptionSalt, 'UTF-16LE'), 1000, 32, true); 
	$key = substr($EncryptedHash, 0, 24);
	$iv = substr($EncryptedHash, 24, 8);
	
	$result = openssl_encrypt(iconv('UTF-8', 'UTF-16', $Data), 'des-ede3-cbc', $key, 0, $iv);
	return $result;
}

// Auth Part
if ($_SERVER['HTTP_USER_AGENT'] == $UserAgent)
{
	$con = mysqli_connect($dbhost,$dbusername,$dbpassword);
	mysqli_select_db($con, $dbname);
	
	$array=[];
	$result = mysqli_query($con, "SELECT * FROM loader_strings");
    
    while($row=mysqli_fetch_assoc($result))
    { 
        $data="$row[detected_string]|";
        array_push($array, $data);
    }
    $data= implode($array);

    echo Encrypt($data, $EncryptionPassword);
}
else
{
    //False | Can't access
	echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "Agent", $EncryptionPassword);
}
?>