﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL3_Server.Models;
using PBL3_Server.Services.AssetService;
using PBL3_Server.Services.RoomService;
using PBL3_Server.Services.UserService;
using System.Data;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;

        public UserController(IUserService UserService)
        {
            _UserService = UserService;
        }

        [Authorize(Roles = "Quản trị viên")]
        [HttpGet]
        // Hàm trả về danh sách user 
        public async Task<ActionResult<List<User>>> GetAllUsers(int pageNumber = 1, int pageSize = 10, string searchQuery = "", string permission = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }

            var users = await _UserService.GetAllUsers();

            if (!string.IsNullOrEmpty(permission))
            {
                users = users.Where(a => a.UserRole.ToLower() == permission.ToLower()).ToList();
            }

            // tìm kiếm tài sản
            if (!string.IsNullOrEmpty(searchQuery))
            {
                users = users.Where(a =>
                    a.Username.ToLower().Contains(searchQuery.ToLower()) ||
                    a.FullName.ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }

            var pagedUsers = users.ToPagedList(pageNumber, pageSize);

            //Tạo đối tượng paginationInfo để lưu thông tin phân trang
            var paginationInfo = new PaginationInfo
            {
                TotalPages = pagedUsers.PageCount,
                CurrentPage = pagedUsers.PageNumber,
                HasPreviousPage = pagedUsers.HasPreviousPage,
                HasNextPage = pagedUsers.HasNextPage,
                PageSize = pagedUsers.PageSize
            };
            return Ok(new { status = "success", data = pagedUsers, meta = paginationInfo });
        }

        [Authorize(Roles = "Quản trị viên")]
        [HttpGet("{id}")]
        // Hàm trả về thông tin của user qua ID
        public async Task<ActionResult<User>> GetSingleUser(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _UserService.GetSingleUser(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "User not found!" });
            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "Quản trị viên")]
        [HttpPost]
        // Hàm thêm user
        public async Task<ActionResult<List<User>>> AddUser(User user)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;
            user.UserID = Guid.NewGuid().ToString();
            var result = await _UserService.AddUser(user);
            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "Quản trị viên")]
        [HttpPut("{id}")]
        //Hàm cập nhật user
        public async Task<ActionResult<List<User>>> UpdateUser(string id, User request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            request.Password = hashedPassword;
            var result = await _UserService.UpdateUser(id, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "User not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "Quản trị viên")]
        [HttpDelete("{id}")]
        // Hàm xóa tài sản theo ID
        public async Task<ActionResult<List<User>>> DeleteUser(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _UserService.DeleteUser(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "User found!" });

            return Ok(new { status = "success" });
        }
    }
}
