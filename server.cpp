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
#include <cstdio>
#include <cstdlib>

using namespace std;

pthread_mutex_t serverLock = PTHREAD_MUTEX_INITIALIZER;

//Sends messages to the client using their int socket identifier and the supplied message.
int send(int sockt, string message)
{
    int ret;
    ret = send(sockt, message.c_str(), strlen(message.c_str()),0);
    return 0;
}

/*
int sendAll(Spreadsheet ss, string message){
	
}

Spreadsheet findSS(User user){
	
	
}
*/
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
			cout<<"in connect"<<endl;
			//if name is ok
			if(false)
			{
				// open spreadsheet
				//create user and add to spreadsheet
				string username = msg.substr(8, msg.find_first_of(" ", 8));
				username = username.substr(0, username.find_first_of(" "));
				string ssname = msg.substr(9+strlen(username), msg.find("\n"));
				cout<<"ssname == "<< ssname<<endl;
				//check for ss
				//if exists - open existing SSg
				//get cell number and send cells to client
				int numberCells = 0;
				stringstream ss;
				ss << numberCells;
				string cells = ss.str();
				cout<<"Cells == "<<cells<<endl;
				message = "connected " + cells + " \n";
				send(client, message);
				//else - new vv
				//Spreadsheet ss = new Spreadsheet();
				//User user = new User(username, ssname, client);
				message = "connected 0\n";
				send(client, message);
				//sendSSCells(client);
			}
			else
			{
				string username = msg.substr(8, msg.find_first_of(" ", 8));
				username = username.substr(0, username.find_first_of(" "));
				message = "error 4 " + username + "\n";
				cout<<"do we get here???"<<endl;
				send(client, message);
			}
		}
		else if(command.compare("register") == 0)
		{
			//check if username exists 
			//add user to list 
		}
		else if(command.compare("cell") == 0)
		{
			if(false){
				//cell check
			}
			else
			{
				message = "error 1 cell change failed";
				send(client,message);
			}
			cout<<"in cell"<<endl;
		}
		else if(command.compare("undo") == 0)
		{
			cout<<"in undo"<<endl;
		}
		else if(command.compare("save") == 0)
		{
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