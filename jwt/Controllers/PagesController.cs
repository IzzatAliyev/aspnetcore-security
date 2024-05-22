// Copyright (c) IUA. All rights reserved.

namespace jwt.Controllers;

using jwt.Data;
using jwt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Pages controller.
/// </summary>
[ApiController]
[Route("api/pages")]
public class PagesController(ApplicationDbContext dbContext)
: ControllerBase
{
    private readonly ApplicationDbContext dbContext = dbContext;

    /// <summary>
    /// Create page.
    /// </summary>
    /// <param name="pageDto">the page dto.</param>
    /// <returns>the created page.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("new")]
    public async Task<ActionResult<Page>> CreatePage(PageDto pageDto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var page = new Page
        {
            Id = pageDto.Id,
            Title = pageDto.Title,
            Author = pageDto.Author,
            Body = pageDto.Body,
        };

        this.dbContext.Pages.Add(page);
        await this.dbContext.SaveChangesAsync();

        return this.CreatedAtAction(nameof(this.GetPage), new { id = page.Id }, page);
    }

    /// <summary>
    /// Get page.
    /// </summary>
    /// <param name="id">the id.</param>
    /// <returns>the page.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PageDto>> GetPage(int id)
    {
        var page = await this.dbContext.Pages.FindAsync(id);

        if (page is null)
        {
            return this.NotFound();
        }

        var pageDto = new PageDto
        {
            Id = page.Id,
            Author = page.Author,
            Body = page.Body,
            Title = page.Title,
        };

        return pageDto;
    }

    /// <summary>
    /// Get list pages.
    /// </summary>
    /// <returns>the all pages.</returns>
    [HttpGet]
    public async Task<PagesDto> ListPages()
    {
        var pagesFromDb = await this.dbContext.Pages.ToListAsync();

        var pagesDto = new PagesDto();

        foreach (var page in pagesFromDb)
        {
            var pageDto = new PageDto
            {
                Id = page.Id,
                Author = page.Author,
                Body = page.Body,
                Title = page.Title,
            };

            pagesDto.Pages.Add(pageDto);
        }

        return pagesDto;
    }
}
