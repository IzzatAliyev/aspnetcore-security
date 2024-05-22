// Copyright (c) IUA. All rights reserved.

namespace jwt.Services;

using jwt.Models;

/// <summary>
/// Interface for token service.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Create token.
    /// </summary>
    /// <param name="user">the user.</param>
    /// <returns>the token.</returns>
    public string CreateToken(ApplicationUser user);
}
