// Copyright (c) IUA. All rights reserved.

namespace jwt.Models;

using jwt.Enums;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Application user.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the role of user.
    /// </summary>
    public Role Role { get; set; }
}
