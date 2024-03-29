﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockmarrdk_api.Common;
using stockmarrdk_api.Dto;
using stockmarrdk_api.Services;
using System.Security.Claims;

namespace stockmarrdk_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDto>))]
        public ActionResult ListUsers()
        {
            return StatusCode(StatusCodes.Status200OK, _userService.ListUsers());
        }


        [HttpPost()]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
        public ActionResult CreateUser([FromForm] NewUserDto newUser)
        {
            try
            {
                string? initialPassword = _userService.CreateUser(newUser);
                if (initialPassword is not null)
                {
                    return StatusCode(StatusCodes.Status200OK, initialPassword);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (AlreadyExistsException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPatch()]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult UpdateUser([FromForm] UserDto user)
        {
            UserDto? newUser = _userService.UpdateUser(user);
            if (newUser is not null)
            {
                return StatusCode(StatusCodes.Status200OK, newUser);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpDelete()]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteUser([FromForm] UserDto user)
        {
            UserDto? deletedUser = _userService.DeleteUser(user);
            if (deletedUser is not null)
            {
                return StatusCode(StatusCodes.Status200OK, deletedUser);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

        [HttpPost("reset-password")]
        [Authorize("Admins")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult ResetPassword([FromForm] string userName)
        {
            string? temporaryPassword = _userService.ResetPassword(userName);
            if (temporaryPassword is not null)
            {
                return StatusCode(StatusCodes.Status200OK, temporaryPassword);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpPatch("me")]
        [Authorize("Users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult ChangePassword([FromForm] string newPassword) 
        {
            Claim? nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdentifier is null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            string username = nameIdentifier.Value;

            UserDto? user = _userService.UpdatePassword(username, newPassword);
            if (user is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, user);
            }
        }
    }
}
