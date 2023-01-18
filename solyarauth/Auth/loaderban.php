<?php
//error_reporting(0); //Disable ALL ERRORS
include("database_config.php"); //Include Database

if (isset($_GET["hwid"]) && isset($_GET["machinename"]) && isset($_GET["cpuid"]) && isset($_GET["mac"]) && isset($_GET["ip"]) && isset($_GET["mbsn"]) && isset($_GET["reason"]))
{
    //Data
    $UserAgent = "123";
    $EncryptionPassword="4sad45as54d54as";
    $UserHwid = $_GET["hwid"];
    $UserMachine = $_GET["machinename"];
    $banreason = $_GET["reason"];
    $UserCPUId = $_GET["cpuid"];
    $UserMacAddress = $_GET["mac"];
    $UserIPAddress = $_GET["ip"];
    $UserMBSN = $_GET["mbsn"];
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
    	$result = mysqli_query($con, "SELECT * FROM loader_ban WHERE hwid='$UserHwid' AND machinename='$UserMachine' AND reason='$banreason' AND CPUId='$UserCPUId' AND ipaddress='$UserIPAddress' AND MacAddress='$UserMacAddress' AND MotherboardSerialNumber='$UserMBSN'");
    	$num_rows= mysqli_num_rows($result);
    	$time=time();
    	
    	if($num_rows <= 0)
    	{
            mysqli_query($con,"INSERT INTO loader_ban (machinename, hwid, reason, bantime, CPUId, ipaddress, MacAddress, MotherboardSerialNumber) VALUES('$UserMachine', '$UserHwid', '$banreason', '$time', '$UserCPUId', '$UserIPAddress', '$UserMacAddress', '$UserMBSN')");
    	}
        
        echo Encrypt("Hoho", $EncryptionPassword);
    }
    else
    {
        //False | Can't access
    	echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "Agent", $EncryptionPassword);
    }
}
else
{
    echo ("Something went wrong!");
}
?>