<?php
//error_reporting(0); //Disable ALL ERRORS
include("database_config.php"); //Include Database
?>

<?php
//Data
if (isset($_GET["ci"]) && isset($_GET["mn"]) && isset($_GET["h"]) && isset($_GET["hw"]) && isset($_GET["ak"]) && isset($_GET["modid"])){
    $UserAgent = "321";
    $UserMachineName = $_GET["mn"];
    $UserCPUID = $_GET["ci"];
    $UserHWID = $_GET["hw"];
    $UserAPIKEY = $_GET["ak"];
    $UserIP = $_SERVER['REMOTE_ADDR'];
    $CurrTime = time();
    $UserModId = $_GET["modid"];
    
    // Encryption Part
    function encryptDecrypt($input) {
        $key = $_GET['h'];
        $inputLen = strlen($input);
        $keyLen = strlen($key);
    
        if ($inputLen <= $keyLen) {
            return $input ^ $key;
         }
    
        for ($i = 0; $i < $inputLen; ++$i) {
            $input[$i] = $input[$i] ^ $key[$i % $keyLen];
        }
        return $input;
    }
    
    //Decrypt Data
    //$UserMachineName = encryptDecrypt($UserMachineName);
    //$UserCPUID = encryptDecrypt($UserCPUID);
    //$UserHWID = encryptDecrypt($UserHWID);
    
    // Auth Part
    if ($_SERVER['HTTP_USER_AGENT'] == $UserAgent)
    {
    	$con = mysqli_connect($dbhost,$dbusername,$dbpassword);
    	mysqli_select_db($con, $dbname);
    	$result = mysqli_query($con, "SELECT member_group_id,LoaderChecker FROM core_members WHERE MachineName = '$UserMachineName' AND HWID = '$UserHWID' LIMIT 1");
        $row=mysqli_fetch_array($result);
        
        $UserGroupid = $row['member_group_id'];
        $UserLoaderChecker= $row['LoaderChecker'];
        
        $result2 = mysqli_query($con, "SELECT * FROM loader_dll WHERE api_key='$UserAPIKEY'");
        //$row2=mysqli_fetch_array($result2);
        
        $result3 = mysqli_query($con, "SELECT * FROM loader_ban WHERE hwid='$UserHwid' OR machinename='$UserMachineName'");
    	$num_rows= mysqli_num_rows($result3);
        
        if($num_rows <= 0)
        {
            //if(mysqli_num_rows($results2) != 0)
            {
                if(mysqli_num_rows($result) != 0)
                {
                    if($UserGroupid == $UserModId || $UserGroupid == $F_Admin || $UserGroupid == $F_Mod)
                    {
                        if($UserLoaderChecker == '1')
                        {
                            echo encryptDecrypt("[ok]");
                        }
                        else
                        {
                            echo encryptDecrypt("[session_not_active]");
                        }
                    }
                    else
                    {
                        echo encryptDecrypt("[session_invalid]");
                    }
                }
                else
                {
                    //if($num_rows <= 0)
                	{
                        mysqli_query($con,"INSERT INTO loader_ban (machinename, hwid, reason, bantime, ipaddress) VALUES('$UserMachineName', '$UserHWID', 'DLL: User not found', '$CurrTime', '$UserIP')");
                	}
        	
                    echo encryptDecrypt("[user_not_found]");
                }
            }
            //else
            //{
                //if($num_rows <= 0)
            //	{
            //        mysqli_query($con,"INSERT INTO loader_ban (machinename, hwid, reason, bantime, ipaddress) VALUES('$UserMachineName', '$UserHWID', 'DLL: Hack not found', '$CurrTime', '$UserIP'");
            //	}
                
            //    echo encryptDecrypt("[hack_not_found]");
            //}
        }
        else
        {
            echo encryptDecrypt("[banned]");
        }
    }
    else{
    echo ("[no_access]");
    }
}
else{
    echo ("[no_access]");
}
?>