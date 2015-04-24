/*
http://www.tutorialspoint.com/cplusplus/cpp_multithreading.htm
http://www.tutorialspoint.com/unix_sockets/socket_server_example.htm
http://codebase.eu/source/code-cplusplus/multithreaded-socket-server/
 */
 
#include <iostream>
#include <cstring>
#include <arpa/inet.h>
#include <netdb.h>
#include <sys/socket.h>
#include <pthread.h>
#include <sstream>
#include <fstream>
#include <cstdio>
#include <cstdlib>
#include "Spreadsheet.h"
#include "CircularException.h"

using namespace std;

vector<Spreadsheet> SpreadsheetList;
vector<string> userList;
pthread_mutex_t serverLock = PTHREAD_MUTEX_INITIALIZER;
/*
Returns Spreadsheet that a given socket/client belongs to.
*/
Spreadsheet& findSS(int client)
{
	for(int i=0; i < SpreadsheetList.size(); i++) // loop over all spreadsheets in vector
	{
		if(SpreadsheetList[i].containsUser(client))//check if the user is in the spreadsheet 
		{
			return SpreadsheetList[i];//if client socket id is found in the spreadsheet then the current spreadsheet is returned 
		}
	}
}

bool hasSS(int client)
{
	for(int i=0; i < SpreadsheetList.size(); i++) // loop over all spreadsheets in vector
	{
		if(SpreadsheetList[i].containsUser(client))//check if the user is in the spreadsheet 
		{
			return true;//if client socket id is found in the spreadsheet then the current spreadsheet is returned 
		}
	}
	return false;
}

bool fileExists(string filename)
{
	string fname = filename+".txt";
	ifstream ifile(fname.c_str());
	return ifile;
}

//Sends messages to the client using their int socket identifier and the supplied message.
int send(int sockt, string message)
{
    int ret;
    ret = send(sockt, message.c_str(), strlen(message.c_str()),0);
    return 0;
}

void sendAll(int client, string message)
{
	vector<int> socketList = findSS(client).getSocketList();
	for(int i = 0; i < socketList.size(); i++)
	{
		cout<<"sending to all "<<socketList[i]<<endl;
		send(socketList[i], message);
	}
}
//receives messages from a client socket, uses sockt pointer to identify which client it came from and then
//parses the message to determine which action to take
 void * receive(void * sockt) 
 {
 
	// set socket
	int client;
	client = *((int*)&sockt);
	
	char buffer[256];
	
	while(1) {
	
		// clear buffer
		bzero(buffer,256);
		
		int numBytes;
		numBytes = read(client,buffer,255);
		
		if (numBytes <= 0)
        {
			if(hasSS(client)
			{
				cout << "Client Disconnected." << endl ;
				
				//Find the spreadsheet and save it
				Spreadsheet * temp = &findSS(client);
				temp->Save();
				temp->removeUser(client);
				if(temp->getSocketList().size() == 0)
				{
					for(int i = 0; i < SpreadsheetList.size(); i++)
					{
						if(SpreadsheetList[i].getName() == temp->getName())
						{
							SpreadsheetList.erase(SpreadsheetList.begin() + i);
						}
					}
				}
				cout << "Done" << endl;
			}
			//close socket and end thread for client
            close(client);
            pthread_exit(0);
        }
		
		// Display message
		printf("Client: %d\nMessage: %s",client, buffer);
		
		// Lock
		pthread_mutex_lock(&serverLock);
		int num;
		//do stuff with message
		string msg = buffer;
		string temp = "";
		string message = "";
		cout<<"Before loop"<<endl;
		while((num = msg.find_first_of("\n")) == -1)
		{
			cout<<"in loop"<<endl;
			numBytes = read(client, buffer, 256);
			temp = buffer;
			msg += temp;
		}	
		cout<<"msg after loop"<<msg<<"something off the end here"<<endl;
		msg = msg.substr(0, msg.find_first_of("\n")+1);
		cout<<"msg is set "<<msg<<endl;
		int index = msg.find_first_of(" ");
		string command = "";
		if(index > 0){
		 command = msg.substr(0, msg.find_first_of(" "));
		}
		if(command == "connect")
		{
			string username = msg.substr(8, msg.find_first_of(" ", 8));
			username = username.substr(0, username.find_first_of(" "));
			bool exists = false;
			if(username != "sysadmin")
			{
				for(int k = 0; k<userList.size(); k++)
				{
					if(userList[k] == username)
					{
						exists = true;
						break;
					}
				}
			}
			else
			{
				exists = true;
			}
			if(exists)
			{
				// open spreadsheet
				//create user and add to spreadsheet
				string ssname = msg.substr(9+username.length(), msg.find("\n"));
				ssname = ssname.substr(0, ssname.length()-1);
				cout<<ssname<< " spreadsheet name"<<endl;
				bool found = false;
				for(int i = 0; i<SpreadsheetList.size(); i++)
				{
					if(SpreadsheetList[i].getName() == ssname)
					{
						found = true;
					}
				}
				if(found){
					user usr(username, client);
					for(int i =0; i<SpreadsheetList.size(); i++)
					{
						if(SpreadsheetList[i].getName() == ssname)
						{
							cout<<"are we not getting here?"<<endl;
							SpreadsheetList[i].addUser(usr);
							SpreadsheetList[i].Save();
							map<string, string> sheet = SpreadsheetList[i].getSheet();

							//convert int to string
							int numberCells = sheet.size();
							stringstream ss;
							ss << numberCells;
							string cells = ss.str();
							
							message = "connected " + cells + " \n";
							cout<<message<< "sheet size"<<endl;
							send(client, message);
							
							//send cells from spreadsheet to client
							for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
							{
								message = "cell " + it->first + " " + it->second + "\n"; 
								cout << message << " == message" << endl;
								send(client, message);
							}
		
						}
					}
				
				}
				else{	
					
					if(fileExists(ssname))
					{
						cout << "The file exists" << endl;
						Spreadsheet SS(ssname);
						user usr(username, client);
						SS.addUser(usr);
						map<string, string> sheet = SS.Open(ssname);
						SpreadsheetList.push_back(SS);
						//convert int to string
						int numberCells = sheet.size();
						stringstream ss;
						ss << numberCells;
						string cells = ss.str();
						
						message = "connected " + cells + " \n";
						cout<<message<<endl;
						send(client, message);
						
						//send cells from spreadsheet to client
						for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
						{
							message = "cell " + it->first + " " + it->second + "\n"; 
							cout << message << " == message" << endl;
							send(client, message);
						}
					}
					else{
						Spreadsheet SS(ssname);
						user usr(username, client);
						SS.addUser(usr);
						SpreadsheetList.push_back(SS);
						message = "connected 0\n";
						send(client, message);
					}
				}
			}
			else
			{
				message = "error 4 " + username + "\n";
				send(client, message);
			}
		}
		else if(command == "register")
		{
			//check if username exists 
			string username = msg.substr(9, msg.find_first_of(" ", 9));
			username = username.substr(0, username.find_first_of("\n"));
			bool used = false;
			for(int j = 0; j<userList.size(); j++)
			{
				if(userList[j] == username)
				{
					used = true;
					break;
				}
			}
			if(used)
			{
				message = "error 4 " + username + " \n";
				send(client, message);
			}
			else
			{
				//add new user registration
				userList.push_back(username);
				
				ofstream stream;
				string filename = "userList.txt";
				stream.open(filename.c_str());
				for(int i = 0; i<userList.size(); i++)
				{
					stream<<userList[i]<<"\n";
				}
				stream.close();
			}
		}
		
		else if(command == string("cell"))
		{
			cout<<"inside of cell"<<endl;
			string cellTemp = msg.substr(5); //cut off command
			cout<<cellTemp<<endl;
			string cellName = cellTemp.substr(0, cellTemp.find_first_of(" "));//get cell name
			cout<<cellName<<endl;
			string cellContents = cellTemp.substr(cellTemp.find_first_of(" ")+1, (cellTemp.find_first_of("\n")-(cellTemp.find_first_of(" ")+1)));//get cell contents
			bool circle = findSS(client).SetContentsOfCell(cellName, cellContents, false);//find spreadsheet and call set cell contents
			cout<<"Message to clients "<<msg<<endl;
			sendAll(client, msg);//send change to all clients once change is verified

			if(circle)//bad cell change
			{
				message = "error 1 Introduced a circular exception\n";//prepare error message
				cout << message << endl;
				send(client,message);//send message to cell change requester 
			}
		}
		else if(command == "undo\n")
		{
			cout<<"undoing "<<msg<<endl;
			if(findSS(client).canUndo()){
				string test = findSS(client).undo();
				message = "cell " + test + "\n";
				cout<<"sending back to client: "<<message<<endl;
				sendAll(client, message);//how to send out change to all clients
			}

		}
		else if(command == "save\n")
		{
			cout<<"in save"<<endl;
			findSS(client).Save();
			cout<<"saved"<<endl;
		}	
		
		else
		{
			message = "error 2 " + msg + "\n";
			send(client, message);
		}
		// Unlock
		pthread_mutex_unlock(&serverLock);
	}
 }
 
/*
 * Server Start and Initialization
 */
int main(int argc, char *argv[])
{

	// Declarations
	pthread_t userThread;
	int ssocket;
	int port;
	socklen_t clilen;
	struct sockaddr_in serv_addr, cli_addr;
	int client;
	
	// get port, defaults to 2000 as per protocol
	if (argc < 2)
		port = 2000;
	else
		port = atoi(argv[1]);
	
	// get socket for server (ssocket)
	ssocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (ssocket < 0) 
		{
		cout << "Error opening socket for server" << endl;
		exit(1);
		}
	
	// Initialize socket structure
    bzero((char *) &serv_addr, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;  
    serv_addr.sin_addr.s_addr = INADDR_ANY;  
    serv_addr.sin_port = htons(port);

	// Bind socket to given port/address
    if (bind(ssocket, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0) {
        cout << "ERROR on binding" << endl;
		exit(1);
	}
	
	ifstream stream;
	string name;
	string filename = "userList.txt";
	stream.open(filename.c_str());
	while(stream>>name)
	{
		userList.push_back(name);
	}
	stream.close();
	
	
	//Now start listening for the clients, here process will go in sleep mode
	//and will wait for the incoming connection
    listen(ssocket,5);
	clilen = sizeof(cli_addr);
		
	while(1) {

		int client;
		/* Accept actual connection from the client */
		client = accept(ssocket, (struct sockaddr *) &cli_addr, &clilen);
		if (client < 0) {
			cout << "ERROR on accept" << endl;
		}		
		// send off thread for this connection
		pthread_create(&userThread, 0, receive, (void *)client);
		pthread_detach(userThread);
	}

	// Destory resources
    close(client);
    close(ssocket);
	
    return 0; 
}