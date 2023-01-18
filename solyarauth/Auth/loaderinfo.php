<?php
//error_reporting(0); //Disable ALL ERRORS
include("database_config.php"); //Include Database
?>

<?php
if (isset($_GET["login"]) && isset($_GET["password"])){
//Data
$UserAgent = "123";
$EncryptionPassword= $_GET["password"];
$UserLogin = $_GET["login"];

function secondsToTime($seconds) 
{
    $dtF = new \DateTime('@0');
    $dtT = new \DateTime("@$seconds");
    return $dtF->diff($dtT)->format('%a day(s) , %h hour(s) , %i minute(s)');
};

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
	$result = mysqli_query($con, "SELECT * FROM Loader_News limit 1");
    $row=mysqli_fetch_assoc($result);
	$result1 = mysqli_query($con, "SELECT ps_expire, ps_name FROM nexus_purchases WHERE `ps_member` = (SELECT member_id FROM core_members WHERE name='$UserLogin' OR email='$UserLogin') group by ps_id DESC");
    $row1=mysqli_fetch_array($result1);
    $product_expire = $row1['ps_expire'];
    $product_name = $row1['ps_name'];
    $date=date("Y-m-d H:i:s", $product_expire);
    
    $result2 = mysqli_query($con,"SELECT Lasttime,MachineName,members_pass_hash FROM core_members WHERE name='$UserLogin'");
	$row2 = mysqli_fetch_array($result2);
	$PasswordHashed = $row2['members_pass_hash'];
	$MachineName = $row2['MachineName'];
    $Lasttime=$row2['Lasttime'];
	$time=time();
	$a7a=$time-$Lasttime;
    $remain=(($Lasttime+604800)-$time);
    $remain=secondsToTime($remain);
    
    if (password_verify($EncryptionPassword, $PasswordHashed))
    {
        $data1="";
        if ($a7a < 604800) 
        {
            $data1 = $remain . "|" . $MachineName;
        }
        else
        {
            $data1 = "Available" . "|" . $MachineName;
        }
        $data2="$row[NEWS]|$row[LoaderVersion]|$row[Loaderexe]|$date|$product_name|$data1";
        
        echo Encrypt($data2, $EncryptionPassword);
    }
    else
    {
        echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "Password", $EncryptionPassword);
    }
}
else
{
    //False | Can't access
	echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "Agent", $EncryptionPassword);
}
}
else
{
     echo "Something went wrong!";   
}
?>