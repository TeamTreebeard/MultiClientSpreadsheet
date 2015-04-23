#include "user.h"
#include<iostream>

user::user(std::string username, int socketIn)
{
  name = username;
  socket = socketIn;
  std::cout<<"USER CONSTRUCTOR"<<std::endl;
}

user::user (const user & other)
{
	std::cout<<"USER COPY CONSTRUCTOR"<<std::endl;
	this->socket = other.socket;
	this->name = other.name;
}

const user& user::operator= (const user & rhs)
{
	this->socket = rhs.socket;
	this->name = rhs.name;
	std::cout<<"USER OP="<<std::endl;
	return *this;
}


int user::getSocket()
{
  return socket;
}
