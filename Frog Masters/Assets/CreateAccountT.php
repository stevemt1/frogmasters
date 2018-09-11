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
	$Email = $_REQUEST["Email"]; 
	$Password = $_REQUEST["Password"]; 

	if(!$Email || !$Password) {
		echo "Empty";
	} else {
		
		$SQL = "SELECT * FROM  Accounts where Email ='" . $Email . "'";
		$Result = mysqli_query($conn, $SQL);
		$Total = mysqli_num_rows($Result);
		if($Total == 0) {
			$insert = "INSERT INTO `Accounts` (`Email`, `Password`) VALUES ('" . $Email . "', '" . $Password . "')";
			$SQL1 = mysqli_query($conn, $insert);
			echo "Success";
		} else {
			echo "AlreadyUsed";
		}

	}

	//close mysql
	mysqli_close($conn);
?>