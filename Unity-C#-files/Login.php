<?php
$servername = "localhost";
$username = "root";
$password = "";
$databaseName = "users_database";

#user inputs
$loginUser = $_POST("loginUser");
$loginPassword = $_POST("loginPassword");

// connects to mysqli server
$connection = new mysqli($servername, $username, $password, $databaseName);

// verifies connection to server
if ($connection->connect_error) 
{
  die("Connection failed: " . $connection->connect_error);
}
echo "Connected successfully";

// retrieves password for unique username from server
$sql = "SELECT Password FROM user_logininfo WHERE Username = ". $loginUser;
$result = $connection->query($sql);

// outputs each row of data through iteration
if ($result->num_rows > 0) 
{
  while($row = $result->fetch_assoc()) 
  {
    if ($row["Password" == $loginPassword])
    {
      echo "Login success";
    }
    else
    {
      echo "Username or password is incorrect";

    }
  }
} 
else 
{
  echo "Username does not exist";
}

$connection->close();
?>