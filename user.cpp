/*
Filename: user.cpp
Authors: Kevin Faustino, Christyn Phillippi, Brady Mathews, Brandon Hilton
Last Modified: 4/25/2015

C++ File containing the definitions for the user class. A user contains a username and a int identifier for the user's socket.
 */

#include "user.h"

//constructor for user class. SocketIn is the int identifier for the socket. username is the given username.
user::user(std::string username, int socketIn)
{
  name = username;
  socket = socketIn;
}

//copy constructor for user
user::user (const user & other)
{
	this->socket = other.socket;
	this->name = other.name;
}

//operator= overload for user
const user& user::operator= (const user & rhs)
{
	this->socket = rhs.socket;
	this->name = rhs.name;
	return *this;
}

//returns an int identifier for the socket given to the user when they connected to the server
int user::getSocket()
{
  return socket;
}
