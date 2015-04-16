#include "user.h"


user::user(std::string username, int socketIn)
{
  name = username;
  socket = socket;
}

int user::getSocket()
{
  return socket;
}
