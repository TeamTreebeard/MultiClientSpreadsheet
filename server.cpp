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
Spreadsheet findSS(int client)
{
	for(int i=0; i < SpreadsheetList.size(); i++) // loop over all spreadsheets in vector
	{
		if(SpreadsheetList[i].containsUser(client))//check if the user is in the spreadsheet 
		{
			return SpreadsheetList[i];//if client socket id is found in the spreadsheet then the current spreadsheet is returned 
		}
	}
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
            cout << "Client Disconnected." << endl ;
			
            close(client);
            pthread_exit(0);
        }
		
		// Display message
		printf("Client: %d\nMessage: %s",client, buffer);
		
		// Lock
		pthread_mutex_lock(&serverLock);
		
		//do stuff with message
		string msg = buffer;
		string message = "";
		string command = msg.substr(0, msg.find_first_of(" "));
	
		cout<<command<<endl;
	
		if(command.compare("connect")==0)
		{
			string username = msg.substr(8, msg.find_first_of(" ", 8));
			username = username.substr(0, username.find_first_of(" "));
			bool exists = false;
			for(int k = 0; k<userList.size(); k++)
			{
				if(userList[k] == username)
				{
					exists = true;
					break;
				}
			}
			if(exists)
			{
				// open spreadsheet
				//create user and add to spreadsheet
				string ssname = msg.substr(9+username.length(), msg.find("\n"));
				bool found = false;
				for(int i = 0; i<SpreadsheetList.size(); i++)
				{
					if(SpreadsheetList[i].getName() == ssname)
					{
						found = true;
					}
				}
				if(found){
					Spreadsheet SS(ssname);
					map<string, string> sheet = SS.Open(ssname);
					//get cell number and send cells to client
					int numberCells = sheet.size();
					stringstream ss;
					ss << numberCells;
					string cells = ss.str();
					cout<<"Cells == "<<cells<<endl;
					message = "connected " + cells + " \n";
					send(client, message);
					for(map<string, string>::iterator it = sheet.begin(); it != sheet.end(); it++)
					{
						message = "cell " + it->first + " " + it->second + "\n"; 
						send(client, message);
					}
				}
				else{
					Spreadsheet ss(ssname);
					cout<<"hi"<<endl;
					user usr(username, client);
					ss.addUser(usr);
					SpreadsheetList.push_back(ss);
					message = "connected 0\n";
					send(client, message);
				}
			}
			else
			{
				message = "error 4 " + username + "\n";
				send(client, message);
			}
		}
		else if(command.compare("register") == 0)
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
		
		/***************HEMI*********************/
		/*
		*─────────────────────────────▄██▄
		*─────────────────────────────▀███
		*────────────────────────────────█
		*───────────────▄▄▄▄▄────────────█
		*──────────────▀▄────▀▄──────────█
		*──────────▄▀▀▀▄─█▄▄▄▄█▄▄─▄▀▀▀▄──█
		*─────────█──▄──█────────█───▄─█─█
		*─────────▀▄───▄▀────────▀▄───▄▀─█
		*──────────█▀▀▀────────────▀▀▀─█─█
		*──────────█───────────────────█─█
		*▄▀▄▄▀▄────█──▄█▀█▀█▀█▀█▀█▄────█─█
		*█▒▒▒▒█────█──█████████████▄───█─█
		*█▒▒▒▒█────█──██████████████▄──█─█
		*█▒▒▒▒█────█───█████ ███ ████▄─█─█
		*█▒▒▒▒█────█────████ ███ █████─█─█
		*█▒▒▒▒█────█───█████ █ █ ████▀─█─█
		*█▒▒▒▒█───██───██████   █████──█─█
		*▀████▀──██▀█──█████████████▀──█▄█
		*──██───██──▀█──█▄█▄█▄█▄█▄█▀──▄█▀
		*──██──██────▀█─────────────▄▀▓█
		*──██─██──────▀█▀▄▄▄▄▄▄▄▄▄▀▀▓▓▓█
		*──████────────█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
		*──███─────────█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
		*──██──────────█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
		*──██──────────█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
		*──██─────────▐█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
		*──██────────▐█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
		*──██───────▐█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▌
		*──██──────▐█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▌
		*──██─────▐█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▌
		*──██────▐█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▌
		*/
		else if(command.compare("cell") == 0)
		{
			string cellTemp = msg.substr(5); //cut off command
			string cellName = cellTemp.substr(0, cellTemp.find_first_of(" "));//get cell name
			string cellContents = cellTemp.substr(cellTemp.find_first_of(" ")+1, (cellTemp.find_first_of("\n")-(cellTemp.find_first_of(" ")+1)));//get cell contents
			
			try
			{
				//try all the things
				findSS(client).SetContentsOfCell(cellName, cellContents, false);//find spreadsheet and call set cell contents
				cout<<"Message to clients "<<msg<<endl;
				//msg = msg+"\n";
				sendAll(client, msg);//send change to all clients once change is verified
			}
			catch(CircularException e)//bad cell change
			{
				message = "error 1 cell change failed\n";//prepare error message
				send(client,message);//send message to cell change requester 
			}
			cout<<"in cell"<<endl;
		}
		else if(command.compare("undo") == 0)
		{
			message = "cell " + findSS(client).undo() + "\n";
			sendAll(client, message);//how to send out change to all clients
			cout<<"in undo"<<endl;
		}
		else if(command.compare("save") == 0)
		{
			findSS(client).Save();
			cout<<"in save"<<endl;
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