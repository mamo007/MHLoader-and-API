<?php
//error_reporting(0); //Disable ALL ERRORS
include("database_config.php"); //Include Database
?>

<?php
if (isset($_GET["login"]) && isset($_GET["token"]) && isset($_GET["password"])){
//Data
$UserAgent = "123";
$UserLogin = $_GET["login"];
$UserToken = $_GET["token"];
$EncryptionPassword= $_GET["password"];

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

function Decrypt($Data, $EncryptionKey)
{
	$EncryptionSalt = substr(time(), 0, -1);
	
	$EncryptedHash = hash_pbkdf2("sha1", $EncryptionKey, mb_convert_encoding($EncryptionSalt, 'UTF-16LE'), 1000, 32, true); 
	$key = substr($EncryptedHash, 0, 24);
	$iv = substr($EncryptedHash, 24, 8);
	
	$result = iconv(openssl_decrypt('UTF-8', 'UTF-16', $Data), 'des-ede3-cbc', $key, 0, $iv);
	return $result;
}

// Auth Part
if ($_SERVER['HTTP_USER_AGENT'] == $UserAgent)
{
	$con = mysqli_connect($dbhost,$dbusername,$dbpassword);
	mysqli_select_db($con, $dbname);
	$result = mysqli_query($con, "SELECT member_group_id, members_pass_hash FROM core_members WHERE `name` = '{$UserLogin}'");
    $row=mysqli_fetch_assoc($result);
    $UserGroupid = $row['member_group_id'];
    $PasswordHashed = $row['members_pass_hash'];
    
    if (password_verify($EncryptionPassword, $PasswordHashed))
    {
        $UserGroup = "";
        if($UserGroupid == $F_Member)
        {
            $UserGroup = "Member";
        }
        else if($UserGroupid == $F_Admin)
        {
            $UserGroup = "Administrator";
        }
        else if($UserGroupid == $F_Mod)
        {
            $UserGroup = "Moderator";
        }
        else if($UserGroupid == $F_CAClassic)
        {
            $UserGroup = "CA Classic";
        }
        else if($UserGroupid == $F_CAReloaded)
        {
            $UserGroup = "CA Reloaded";
        }
        else if($UserGroupid == $F_CFW_B)
        {
            $UserGroup = "Crossfire West Bronze";
        }
        else if($UserGroupid == $F_CFW_S)
        {
            $UserGroup = "Crossfire West Silver";
        }
        else if($UserGroupid == $F_CFW_G)
        {
            $UserGroup = "Crossfire West Gold";
        }
        else if($UserGroupid == $F_CFBR_B)
        {
            $UserGroup = "Crossfire Brazil Bronze";
        }
        else if($UserGroupid == $F_CFBR_S)
        {
            $UserGroup = "Crossfire Brazil Silver";
        }
        else if($UserGroupid == $F_CFBR_G)
        {
            $UserGroup = "Crossfire Brazil Gold";
        }
        else if($UserGroupid == $F_PUBGM)
        {
            $UserGroup = "PUBG Mobile";
        }
        
    	$result1 = mysqli_query($con, "SELECT ps_name FROM nexus_purchases WHERE `ps_member` = (SELECT member_id FROM core_members WHERE name='$UserLogin') group by ps_id DESC");
        $row1=mysqli_fetch_array($result1);
        $product_name = $row1['ps_name'];
        $TimeOnServer = "";
        
        $ServerToken = ($UserLogin + $product_name + $UserGroup);
        $UserToken = Decrypt($UserToken, $EncryptionPassword);
        
        if(($UserGroupid == $F_Admin || $UserGroupid == $F_CAClassic || $UserGroupid == $F_CAReloaded || $UserGroupid == $F_CFBR_B || $UserGroupid == $F_CFBR_S || $UserGroupid == $F_CFBR_G || $UserGroupid == $F_CFW_B || $UserGroupid == $F_CFW_S || $UserGroupid == $F_CFW_G || $UserGroupid == $F_Mod || $UserGroupid == $F_PUBGM || $UserGroupid == $F_Member) && $ServerToken == $UserToken)
    	{
    	    mysqli_query($con,"UPDATE core_members SET LoaderChecker = '1' WHERE `name` = '{$UserLogin}'");
    	    
    	    mysqli_query($con,"UPDATE (SELECT SLEEP(45)) dummy, core_members SET LoaderChecker = '0' WHERE `name` = '{$UserLogin}'");
    			    
    	    echo Encrypt("True", $EncryptionPassword);
    	}
    	else
    	{
    	    mysqli_query($con,"UPDATE core_members SET LoaderChecker = '0' WHERE name = '$UserLogin'");
    			    
    	    echo Encrypt("False", $EncryptionPassword);
    	}
    }
    else
    {
        echo Encrypt("False", $EncryptionPassword);
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