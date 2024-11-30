using AutoMapper;
using BookStore.DTOs.Author;
using BookStore.DTOs.Book;
using BookStore.Models;
using BookStore.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly UnitOfWork unit;
    private readonly IMapper mapper;

    public AuthorsController(UnitOfWork unit, IMapper mapper)
    {
        this.unit = unit;
        this.mapper = mapper;
    }

    [SwaggerOperation(Summary = "Get all authors", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The list of authors", typeof(List<AuthorViewDTO>))]
    [Produces("application/json")]
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Author> authors = unit.Authors.SelectAll();
        List<AuthorViewDTO> views = mapper.Map<List<AuthorViewDTO>>(authors);

        return Ok(views);
    }

    [SwaggerOperation(Summary = "Get an author by Id", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The author was found", typeof(AuthorViewDTO))]
    [SwaggerResponse(404, "The author was not found")]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Author? author = unit.Authors.SelectById(id);
        if (author is null)
            return NotFound();

        AuthorViewDTO view = mapper.Map<AuthorViewDTO>(author);
        return Ok(view);
    }

    [SwaggerOperation(Summary = "Get all books by author Id", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The list of books by author", typeof(List<BookViewDTO>))]
    [SwaggerResponse(404, "The author was not found")]
    [Produces("application/json")]
    [HttpGet("{id}/books")]
    public IActionResult GetBooks(int id)
    {
        Author? author = unit.Authors.SelectById(id);
        if (author is null) return NotFound();

        List<Book> books = unit.Books.SelectAll().Where(e => e.AuthorId == id).ToList();
        List<BookViewDTO> views = mapper.Map<List<BookViewDTO>>(books);

        return Ok(views);
    }

    [SwaggerOperation(Summary = "Add a new author", Description = "Requires Admin role to create a new author")]
    [SwaggerResponse(201, "The author was created", typeof(AuthorViewDTO))]
    [SwaggerResponse(400, "The author data is invalid")]
    [SwaggerResponse(401, "The user is not authorized to create an author, you must be an Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(AddAuthorDTO authorDTO)
    {
        if (authorDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        Author author = mapper.Map<Author>(authorDTO);

        unit.Authors.Add(author);
        unit.Save();

        AuthorViewDTO view = mapper.Map<AuthorViewDTO>(author);
        return CreatedAtAction(nameof(GetById), new { Id = view.Id }, view);
    }

    [SwaggerOperation(Summary = "Edit an author by Id", Description = "Requires Admin role to edit an author")]
    [SwaggerResponse(204, "The author was edited")]
    [SwaggerResponse(400, "The author data is invalid")]
    [SwaggerResponse(404, "The author was not found")]
    [SwaggerResponse(401, "The user is not authorized to edit an author, you must be an Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Edit(int id, EditAuthorDTO authorDTO)
    {
        if (authorDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        Author? author = unit.Authors.SelectById(id);
        if (author is null) return NotFound();

        mapper.Map(authorDTO, author);

        unit.Save();
        return NoContent();
    }

    [SwaggerOperation(Summary = "Delete an author by Id", Description = "Requires Admin role to delete an author")]
    [SwaggerResponse(204, "The author was deleted")]
    [SwaggerResponse(404, "The author was not found")]
    [SwaggerResponse(401, "The user is not authorized to delete an author, you must be an Admin")]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Author? author = unit.Authors.SelectById(id);
        if (author is null) return NotFound();

        unit.Authors.Delete(author);
        unit.Save();
        return NoContent();
    }
}

