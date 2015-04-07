extern "C"
{
	#include <stdio.h>
	#include <stdlib.h>
	#include <string.h>
	#include <unistd.h>
	#include <sys/types.h> 
	#include <sys/socket.h>
	#include <netinet/in.h>
}
#include <iostream>

using namespace std;

void error(const char *msg)
{
    perror(msg);
    exit(1);
}

int main(int argc, char *argv[])
{
     int sockfd, newsockfd, portno;
     socklen_t clilen;
     char buffer[256];
     struct sockaddr_in serv_addr, cli_addr;
     int n;
	 
	 cout << "Start" << endl;
	 
     sockfd = socket(AF_INET, SOCK_STREAM, 0);
     if (sockfd < 0) 
        error("ERROR opening socket");
	
     bzero((char *) &serv_addr, sizeof(serv_addr));
     portno = 2120;
     serv_addr.sin_family = AF_INET;
     serv_addr.sin_addr.s_addr = INADDR_ANY;
     serv_addr.sin_port = htons(portno);
	 
	 cout << "Middle" << endl;
	 
     if (bind(sockfd, (struct sockaddr *) &serv_addr,
              sizeof(serv_addr)) < 0) 
              error("ERROR on binding");
	
	 cout<<"Before Listen"<<endl;
     listen(sockfd,5);
	 cout<<"Listeninggggggggg"<<endl;
     clilen = sizeof(cli_addr);
	 cout<<"boop1"<<endl;
     newsockfd = accept(sockfd, 
                 (struct sockaddr *) &cli_addr, 
                 &clilen);
	
	cout<<"boop2"<<endl;
		 
     if (newsockfd < 0) 
          error("ERROR on accept");
     bzero(buffer,256);
	 cout<<"boop3"<<endl;
     n = read(newsockfd,buffer,255);
	 cout<<"boobies"<<endl;
     if (n < 0) error("ERROR reading from socket");
     printf("Here is the message: %s\n",buffer);
     n = write(newsockfd,"I got your message",18);
     if (n < 0) error("ERROR writing to socket");
     close(newsockfd);
     close(sockfd);
     return 0; 
}