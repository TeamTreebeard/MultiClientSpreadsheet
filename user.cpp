#include "user.h"


user::user(std::string username, int socketIn)
{
  name = username;
  socket = socketIn;
}

user::user (const user & other)
{
	this->socket = other.socket;
	this->name = other.name;
}

const user& user::operator= (const user & rhs)
{
	this->socket = rhs.socket;
	this->name = rhs.name;
	return *this;
}


int user::getSocket()
{
  return socket;
}
