﻿using stockmarrdk_api.Dto;

namespace stockmarrdk_api.Services
{
    public interface IUserService
    {
        string? CreateUser(NewUserDto newUser);

        UserDto? UpdatePassword(string userName, string newPassword);
        UserDto? UpdateUser(UserDto user);
    }
}
