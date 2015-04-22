#include "user.h"


user::user(std::string username, int socketIn)
{
  name = username;
  socket = socketIn;
}

int user::getSocket()
{
  return socket;
}
