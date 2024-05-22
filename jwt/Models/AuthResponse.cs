// Copyright (c) IUA. All rights reserved.

namespace jwt.Models;

/// <summary>
/// Authentication response.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets the username of user.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the email of user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the token of user.
    /// </summary>
    public string? Token { get; set; }
}
