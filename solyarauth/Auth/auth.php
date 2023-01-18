<?php
//error_reporting(0); //Disable ALL ERRORS
include("database_config.php"); //Include Database
?>

<?php
//Data
if (isset($_GET["login"]) && isset($_GET["password"]) && isset($_GET["hwid"]) && isset($_GET["machinename"]) && isset($_GET["osversion"]) && isset($_GET["cid"]) && isset($_GET["mac"]) && isset($_GET["mbsn"])){
$MaintenanceMode = "False";
$UserAgent = "123";
$UserLogin = $_GET["login"];
$UserPassword = $_GET["password"];
$UserHwid = $_GET["hwid"];
$UserMachine = $_GET["machinename"];
$UserOSVersion = $_GET["osversion"];
$UserCPUID = $_GET["cid"];
$UserMacAddress = $_GET["mac"];
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
	//True | Can access
	if ($MaintenanceMode == "True")
	{
		//Maintenance Enabled
		echo Encrypt("Error" . "|" . "Found" . "|" . "Maintenance" . "|".  "Enabled", $UserPassword);
	}
	else
	{
		//$HWID_Ban_List = readfile("banlist.inf"); //Gettings banned hwid's 
		//$HWID_Ban_List_MName = readfile("banlistMName.inf"); //Gettings banned hwid's 
		
		if(strpos(file_get_contents("./banlist.inf"),$UserHwid) !== false || strpos(file_get_contents("./banlistMName.inf"),$UserHwid) !== false)
		//if(strpos($HWID_Ban_List, $UserHwid) !== false || strpos($HWID_Ban_List_MName, $UserMachine) !== false)
		{
			//Banned HWID
			die(Encrypt("Error" . "|" . "Cant" . "|" . "Access" . "|".  "Database", $UserPassword));
		}
		
		//Maintenance Disabled | Can connect to Database
		$con = mysqli_connect($dbhost,$dbusername,$dbpassword);
		if (!$con)
		{
			die(Encrypt("Error" . "|" . "Found" . "|" . "HWID" . "|".  "Suspended", $UserPassword));
		}

		mysqli_select_db($con, $dbname);
		
		if(strpos($UserLogin, "@") !== false)
		{
			//Email login
			$result = mysqli_query($con,"SELECT name,members_pass_hash,member_group_id,pp_thumb_photo,HWID,MachineName,OSVersion,CPUID,Lasttime,MacAddress,MotherboardSerialNumber FROM core_members WHERE email='{$UserLogin}'");
		} 
		else
		{
			//Username Login
			$result = mysqli_query($con,"SELECT name,members_pass_hash,member_group_id,pp_thumb_photo,HWID,MachineName,OSVersion,CPUID,Lasttime,MacAddress,MotherboardSerialNumber FROM core_members WHERE name='{$UserLogin}'");
		}
		
		//Gettings Database rows 
		$row = mysqli_fetch_array($result);
		
			$name = $row['name'];
			$PasswordHashed = $row['members_pass_hash'];
			$Group_ID = $row['member_group_id'];
			$AvatarLocation =  $row['pp_thumb_photo'];
			$DbHWID = $row['HWID'];
			$DbMachine = $row['MachineName'];
			$DbOSVersion = $row['OSVersion'];
			$DbCPUID = $row['CPUID'];
			$Lasttime = $row['Lasttime'];
			$DbMacAddress = $row['MacAddress'];
			$DbMBSN = $row['MotherboardSerialNumber'];
		
		
		if (password_verify($UserPassword, $PasswordHashed)) 
		{
		    $time=time();
		    $HWIDCheck=time()-$Lasttime;
		    
		    if (($DbHWID != $UserHwid || $DbMachine != $UserMachine || $DbOSVersion != $UserOSVersion || $DbCPUID != $UserCPUID /*|| $DbMacAddress != $UserMacAddress*/ || $DbMBSN != $UserMBSN) && $HWIDCheck > "604800") //Reset All Checks
			{
				mysqli_query($con,"UPDATE core_members SET HWID = '',MachineName='',OSVersion='' ,CPUID='', MacAddress='', MotherboardSerialNumber='' WHERE name = '$name'");
			}

			//True Password
			if ($DbHWID != $UserHwid && $DbMachine != $UserMachine && $DbOSVersion != $UserOSVersion && $DbCPUID != $UserCPUID /*&& $DbMacAddress != $UserMacAddress*/ && $DbMBSN != $UserMBSN && $HWIDCheck > "604800") //No HWID | Save it
			{
			    if($Group_ID == $F_Admin || $Group_ID == $F_CAClassic || $Group_ID == $F_CAReloaded || $Group_ID == $F_CFBR || $Group_ID == $F_CFW || $Group_ID == $F_Mod || $Group_ID == $F_PUBGM || $Group_ID == $F_Member)
			    {
				    mysqli_query($con,"UPDATE core_members SET HWID = '$UserHwid',MachineName='$UserMachine',OSVersion='$UserOSVersion' ,CPUID='$UserCPUID',Lasttime='$time',MacAddress='$UserMacAddress',MotherboardSerialNumber='$UserMBSN' WHERE name = '$name'"); //Updating HWID
			    }
			    
				//Auth User Groups
				if ($Group_ID == $F_Admin) //Admin
				{
					echo Encrypt($UserLogin . "|" . "Administrator" . "|" . $AvatarLocation . "|".  $UserHwid . "|".  $UserMachine . "|".  $UserOSVersion . "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_Mod) //Mod
				{
					echo Encrypt($UserLogin . "|" . "Moderator" . "|" . $AvatarLocation . "|".  $UserHwid . "|".  $UserMachine . "|".  $UserOSVersion . "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFW_B) //Crossfire West Bronze
				{
					echo Encrypt($UserLogin . "|" . "Crossfire West Bronze" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFW_S) //Crossfire West Silver
				{
					echo Encrypt($UserLogin . "|" . "Crossfire West Silver" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFW_G) //Crossfire West Gold
				{
					echo Encrypt($UserLogin . "|" . "Crossfire West Gold" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_PUBGM) //PUBG Mobile
				{
					echo Encrypt($UserLogin . "|" . "PUBG Mobile" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CAClassic) //CA Classic
				{
					echo Encrypt($UserLogin . "|" . "CA Classic" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CAReloaded) //CA Reloaded
				{
					echo Encrypt($UserLogin . "|" . "CA Reloaded" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFBR_B) //Crossfire Brazil Bronze
				{
					echo Encrypt($UserLogin . "|" . "Crossfire Brazil Bronze" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFBR_S) //Crossfire Brazil Silver
				{
					echo Encrypt($UserLogin . "|" . "Crossfire Brazil Silver" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFBR_G) //Crossfire Brazil Gold
				{
					echo Encrypt($UserLogin . "|" . "Crossfire Brazil Gold" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				//No Auth Groups
				
				if ($Group_ID == $F_Member) //Member
				{
				    echo Encrypt($UserLogin . "|" . "Member" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				    
					//echo Encrypt("Error" . "|" . "Found" . "|" . "No" . "|".  //"Subscription", $UserPassword);
				}
				
				else if ($Group_ID == $F_Banned) //Banned
				{
					echo Encrypt("Error" . "|" . "Found" . "|" . "Account" . "|".  "Suspended", $UserPassword);
				}
			}
			elseif ($DbHWID == $UserHwid && $DbMachine == $UserMachine && $DbOSVersion == $UserOSVersion && $DbCPUID == $UserCPUID /*&& $DbMacAddress == $UserMacAddress*/ && $DbMBSN == $UserMBSN) //HWID saved and correct | Can Auth
			{
			    //Loader Checker set to 1

			    
				//Auth User Groups
				if ($Group_ID == $F_Admin) //Admin
				{
					echo Encrypt($UserLogin . "|" . "Administrator" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_Mod) //Mod
				{
					echo Encrypt($UserLogin . "|" . "Moderator" . "|" . $AvatarLocation . "|".  $UserHwid . "|".  $UserMachine . "|".  $UserOSVersion . "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFW_B) //Crossfire West Bronze
				{
					echo Encrypt($UserLogin . "|" . "Crossfire West Bronze" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFW_S) //Crossfire West Silver
				{
					echo Encrypt($UserLogin . "|" . "Crossfire West Silver" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFW_G) //Crossfire West Gold
				{
					echo Encrypt($UserLogin . "|" . "Crossfire West Gold" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_PUBGM) //PUBG Mobile
				{
					echo Encrypt($UserLogin . "|" . "PUBG Mobile" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CAClassic) //CA Classic
				{
					echo Encrypt($UserLogin . "|" . "CA Classic" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CAReloaded) //CA Reloaded
				{
					echo Encrypt($UserLogin . "|" . "CA Reloaded" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFBR_B) //Crossfire Brazil Bronze
				{
					echo Encrypt($UserLogin . "|" . "Crossfire Brazil Bronze" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFBR_S) //Crossfire Brazil Silver
				{
					echo Encrypt($UserLogin . "|" . "Crossfire Brazil Silver" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				if ($Group_ID == $F_CFBR_G) //Crossfire Brazil Gold
				{
					echo Encrypt($UserLogin . "|" . "Crossfire Brazil Gold" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				}
				
				//No Auth Groups
				
				if ($Group_ID == $F_Member) //Member
				{
				    echo Encrypt($UserLogin . "|" . "Member" . "|" . $AvatarLocation . "|".  $UserHwid. "|".  $UserMachine . "|".  $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress . "|".  $UserMBSN, $UserPassword);
				    
					//echo Encrypt("Error" . "|" . "Found" . "|" . "No" . "|".  //"Subscription", $UserPassword);
				}
				
				else if ($Group_ID == $F_Banned) //Banned
				{
					echo Encrypt("Error" . "|" . "Found" . "|" . "Account" . "|".  "Suspended", $UserPassword);
				}
			}
			else
			{
				echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "HWID" . "|" . $UserMachine . "|" . $UserOSVersion. "|".  $UserCPUID . "|".  $UserMacAddress, $UserPassword);
			}
		}
		else 
		{
			echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "Password", $UserPassword);
		}
	}
}
else
{
	//False | Can't access
	echo Encrypt("Error" . "|" . "Found" . "|" . "Invalid" . "|".  "Agent", $UserPassword);
}
}
else
{
    echo "Something went wrong!";
}
?>