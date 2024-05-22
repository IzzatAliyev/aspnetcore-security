// Copyright (c) IUA. All rights reserved.

namespace jwt.Models;

using System.ComponentModel.DataAnnotations;
using jwt.Enums;

/// <summary>
/// Registration request model.
/// </summary>
public class RegistrationRequest
{
    /// <summary>
    /// Gets or sets the email of user.
    /// </summary>
    [Required]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the username of user.
    /// </summary>
    [Required]
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password of user.
    /// </summary>
    [Required]
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the role of user.
    /// </summary>
    public Role Role { get; set; }
}
