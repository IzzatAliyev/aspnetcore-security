// Copyright (c) IUA. All rights reserved.

namespace jwt.Models;

/// <summary>
/// Authentication request model.
/// </summary>
public class AuthRequest
{
    /// <summary>
    /// Gets or sets the email of user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password of user.
    /// </summary>
    public string? Password { get; set; }
}
