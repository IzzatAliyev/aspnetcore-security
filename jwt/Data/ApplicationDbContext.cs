// Copyright (c) IUA. All rights reserved.

namespace jwt.Data;

using jwt.Enums;
using jwt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Application Db context.
/// </summary>
public class ApplicationDbContext : IdentityUserContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">the db context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        this.Database.EnsureCreated();
    }

    /// <summary>
    /// Gets the set of pages.
    /// </summary>
    public DbSet<Page> Pages => this.Set<Page>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var hasher = new PasswordHasher<ApplicationUser>();

        var adminEmail = "admin@gmail.com";
        var adminPassword = "admin12345";

        modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "1",
                UserName = "admin",
                NormalizedUserName = adminEmail.ToUpper(),
                PasswordHash = hasher.HashPassword(null!, adminPassword),
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                Role = Role.Admin,
            });
    }
}
