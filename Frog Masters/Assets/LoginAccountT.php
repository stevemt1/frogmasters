<?php

	//PHP Only
	$Hostname = "localhost";
	$DBName = "accounts";
	$User = "root";
	$PasswordP = "";

	$conn = new mysqli($Hostname, $User, $PasswordP, $DBName);
	
	if($conn->connect_error) {
		die("Connection failed: " . $conn->connect_error);
	}

	//Email and password
	$Username = $_REQUEST["Username"]; 
	$Password = $_REQUEST["Password"]; 

	if(!$Username || !$Password) {
		echo "LoginFail";
	} else {
		$SQL = "SELECT * FROM  accounts where Username = '" . $Username . "'";
		$Result = mysqli_query($conn, $SQL);
		$Total = mysqli_num_rows($Result);
		if($Total == 0) {
			echo "DoesNotExist"
		}
		else{
			$data = mysqli_fetch_array($Result);
			if (strcmp($Password, $data["Password"])){
				echo "Success";
			}
			else{
				echo "WrongPassword";
			}
		}
	}

	//close mysql
	mysqli_close($conn);
?>
