using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using Dapper;
using BookmarkManager.Infrastructure;

namespace BookmarkManager.Infrastructure.Queries;

public class UserQueries : IUserQueries
{
    private readonly IConnectionFactory _connectionFactory;
    public UserQueries(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Users WHERE Id = @id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<UserDto>(sql, new { Id = id });
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM Users WHERE Email = @email";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<UserDto>(sql, new { Email = email });
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var sql = "SELECT * FROM Users";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryAsync<UserDto>(sql);
    }

    public async Task<BookmarkManager.Domain.Entities.User?> GetUserEntityByEmailAsync(string email)
    {
        var sql = "SELECT * FROM Users WHERE Email = @email";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<BookmarkManager.Domain.Entities.User>(sql, new { Email = email });
    }

    public async Task<BookmarkManager.Domain.Entities.User?> GetUserEntityByIdAsync(int id)
    {
        var sql = "SELECT * FROM Users WHERE Id = @id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<BookmarkManager.Domain.Entities.User>(sql, new { Id = id });
    }
}
