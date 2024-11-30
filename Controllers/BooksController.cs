using BookStore.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.DTOs.Book;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[Authorize (Roles = "Admin")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly UnitOfWork unit;
    private readonly IMapper mapper;

    public BooksController(UnitOfWork unit, IMapper mapper)
    {
        this.unit = unit;
        this.mapper = mapper;
    }

    [SwaggerOperation(Summary = "Get all books", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The list of books", typeof(List<BookViewDTO>))]
    [Produces("application/json")]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Book> books = unit.Books.SelectAll();
        List<BookViewDTO> views = mapper.Map<List<BookViewDTO>>(books);
        return Ok(views);
    }

    [SwaggerOperation(Summary = "Get a book by Id", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The book was found", typeof(BookViewDTO))]
    [SwaggerResponse(404, "The book was not found")]
    [Produces("application/json")]
    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Book? book = unit.Books.SelectById(id);
        if (book is null)
            return NotFound();

        BookViewDTO view = mapper.Map<BookViewDTO>(book);
        return Ok(view);
    }

    [SwaggerOperation(Summary = "Add a new book", Description = "Requires Admin role")]
    [SwaggerResponse(201, "The book was created", typeof(BookViewDTO))]
    [SwaggerResponse(400, "The book data is invalid")]
    [SwaggerResponse(401, "The user is not authorized to create a book, you must be an Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost]
    public IActionResult Add(AddBookDTO bookDTO)
    {
        if (bookDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        Book book = mapper.Map<Book>(bookDTO);

        unit.Books.Add(book);
        unit.Save();

        BookViewDTO view = mapper.Map<BookViewDTO>(book);
        return CreatedAtAction(nameof(GetById), new { Id = view.Id }, view);
    }

    [SwaggerOperation(Summary = "Edit a book", Description = "Requires Admin role")]
    [SwaggerResponse(204, "The book was updated")]
    [SwaggerResponse(400, "The book data is invalid")]
    [SwaggerResponse(404, "The book was not found")]
    [SwaggerResponse(401, "The user is not authorized to edit a book, you must be an Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("{id}")]
    public IActionResult Edit(int id, EditBookDTO bookDTO)
    {
        if (bookDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        Book? book = unit.Books.SelectById(id);
        if (book is null) return NotFound();

        mapper.Map(bookDTO, book);
        unit.Save();

        return NoContent();
    }

    [SwaggerOperation(Summary = "Delete a book", Description = "Requires Admin role")]
    [SwaggerResponse(204, "The book was deleted")]
    [SwaggerResponse(404, "The book was not found")]
    [SwaggerResponse(401, "The user is not authorized to delete a book, you must be an Admin")]
    [Produces("application/json")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Book? book = unit.Books.SelectById(id);
        if (book is null) return NotFound();

        unit.Books.Delete(book);
        unit.Save();

        return NoContent();
    }
}
