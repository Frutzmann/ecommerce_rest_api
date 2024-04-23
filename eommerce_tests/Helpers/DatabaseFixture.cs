using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using quest_web;
using quest_web.Models;

namespace quest_web_tests.Helpers;

public class DatabaseFixture : IDisposable
{
    public readonly APIDbContext Context;
    public DatabaseFixture()
    {
        var dbContextOptions = new DbContextOptionsBuilder<APIDbContext>().UseInMemoryDatabase("quest_web_tests");
        dbContextOptions.EnableSensitiveDataLogging();
        Context = new APIDbContext(dbContextOptions.Options);
        Context.Database.EnsureCreated();

        var mockedUsers = new MockData.MockedUsers();
        var mockedAddresses = new MockData.MockedAddresses();

        foreach (var myUser in mockedUsers.MyUsers)
        {
            myUser.Password = BCrypt.Net.BCrypt.HashPassword(myUser.Password);
            Context.Set<User>().Add(myUser);
        }

        //foreach (var myAddress in mockedAddresses.MyAddresses)
        //{
        //    Context.Set<Address>().Add(myAddress);
        //}
        Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        GC.SuppressFinalize(this);
    }


}