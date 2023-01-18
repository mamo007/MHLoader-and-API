<?php
error_reporting(0); //Disable ALL ERRORS
include("database_config.php"); //Include Database
?>

<?php
if (isset($_GET["login"]) && isset($_GET["groupid"]) && isset($_GET["password"])){
//Data
$UserAgent = "123";
$UserLogin = $_GET["login"];
$UserGroup = $_GET["groupid"];
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

// Auth Part
if ($_SERVER['HTTP_USER_AGENT'] == $UserAgent)
{
	$con = mysqli_connect($dbhost,$dbusername,$dbpassword);
	mysqli_select_db($con, $dbname);
	$result1 = mysqli_query($con, "SELECT member_group_id, members_pass_hash FROM core_members WHERE name ='{$UserLogin}'");
	$row1=mysqli_fetch_assoc($result1);
	$PasswordHashed = $row1['members_pass_hash'];
	
	if (password_verify($EncryptionPassword, $PasswordHashed))
    {
    	if($row1['member_group_id'] == $F_Member && $UserGroup == "Member")
    	{
    	    $Group_Color="White";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE `plan` ='FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if(($row1['member_group_id'] == $F_Admin && $UserGroup == "Administrator") || ($row1['member_group_id'] == $F_Mod && $UserGroup == "Moderator"))
    	{
    	    $Group_Color="";
    	    if(($row1['member_group_id'] == $F_Admin && $UserGroup == "Administrator"))
    	    {
    	        $Group_Color="Red";
    	    }
    	    else if($row1['member_group_id'] == $F_Mod && $UserGroup == "Moderator")
    	    {
    	        $Group_Color = "Blue";
    	    }
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if(($row1['member_group_id'] == $F_CFW_B && $UserGroup == "Crossfire West Bronze"))
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'Crossfire West Bronze') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if(($row1['member_group_id'] == $F_CFW_S && $UserGroup == "Crossfire West Silver"))
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'Crossfire West Silver') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if(($row1['member_group_id'] == $F_CFW_G && $UserGroup == "Crossfire West Gold"))
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'Crossfire West Gold') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if(($row1['member_group_id'] == $F_CFBR_B && $UserGroup == "Crossfire Brazil Bronze"))
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'Crossfire Brazil Bronze') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if(($row1['member_group_id'] == $F_CFBR_S && $UserGroup == "Crossfire Brazil Silver"))
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'Crossfire Brazil Silver') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if(($row1['member_group_id'] == $F_CFBR_G && $UserGroup == "Crossfire Brazil Gold"))
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'Crossfire Brazil Gold') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if($row1['member_group_id'] == $F_CAClassic && $UserGroup == "CA Classic")
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'CA Classic') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if($row1['member_group_id'] == $F_CAReloaded && $UserGroup == "CA Reloaded")
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'CA Reloaded') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else if($row1['member_group_id'] == $F_PUBGM && $UserGroup == "PUBG Mobile")
    	{
    	    $Group_Color="Orange";
        	$array=[];
        	$result = mysqli_query($con, "SELECT * FROM loader_status WHERE (`plan` ='VIP' AND `name` = 'PUBG Mobile') OR `plan` = 'FREE' group by id ASC");
            
            while($row=mysqli_fetch_assoc($result))
            { 
                $data="$row[name]|$row[status]|$row[dll]|$row[plan]|$row[OS]|$row[ProcessName]|$row[key1]|$row[key2]|$row[key3]|$Group_Color|$row[hostsfile]|";
                array_push($array, $data);
            }
            $data= implode($array);
        
            echo Encrypt($data, $EncryptionPassword);
    	}
    	else
    	{
    	    echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "Group", $EncryptionPassword);
    	}
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