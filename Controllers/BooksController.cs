using BookStore.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.DTOs.Book;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using BookStore.Utils;
using System.Data.Common;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[Authorize (Roles = "Admin")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly UnitOfWork unit;
    private readonly IMapper mapper;
    private readonly FileService fileService;
    private string uploadPath;
    private string basePath;
    private const string mediaEndPoint = $"api/media";
    private const string mimeType = "image/jpeg";
    public BooksController(UnitOfWork unit, IMapper mapper, FileService _fileService, IConfiguration configuration)
    {
        this.unit = unit;
        this.mapper = mapper;
        fileService = _fileService;

        string upload = configuration.GetSection("Upload-Path").Get<string>() ?? throw new Exception("Upload Path Doesn't Exists in appsettings.json");
        basePath = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? throw new Exception("Error in Find Base Directory");
        uploadPath = Path.Combine(basePath, upload);
    }

    [SwaggerOperation(Summary = "Get all books", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The list of books", typeof(List<BookViewDTO>))]
    [Produces("application/json")]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll()
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{mediaEndPoint}";

        List<Book> books = unit.Books.SelectAll();
        List<BookViewDTO> views = mapper.Map<List<BookViewDTO>>(books, opt => opt.Items["BaseUrl"] = baseUrl);
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
        string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{mediaEndPoint}";

        Book? book = unit.Books.SelectById(id);
        if (book is null)
            return NotFound();

        BookViewDTO view = mapper.Map<BookViewDTO>(book, opt => opt.Items["BaseUrl"] = baseUrl);
        return Ok(view);
    }

    [SwaggerOperation("View Image Of Product", "Return a Image Based on Product Id")]
    [SwaggerResponse(200, "Successfully", typeof(PhysicalFileResult))]
    [SwaggerResponse(404, "Failed, Product Not Found or File Not Found.")]
    [AllowAnonymous]
    [HttpGet("{id}/image")]
    public IActionResult GetImgByProductId(int id)
    {
        Book? book = unit.Books.SelectById(id);

        if (book == null || book.PhotoId is null)
            return NotFound();

        string filePath = Path.Combine(uploadPath, book.PhotoId);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found.");
        }
        return PhysicalFile(filePath, mimeType);
    }

    [SwaggerOperation(Summary = "Add a new book", Description = "Requires Admin role")]
    [SwaggerResponse(201, "The book was created", typeof(BookViewDTO))]
    [SwaggerResponse(400, "The book data is invalid")]
    [SwaggerResponse(401, "The user is not authorized to create a book, you must be an Admin")]
    [Produces("application/json")]
    [HttpPost]
    public IActionResult Add(AddBookDTO bookDTO)
    {
        if (bookDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (unit.Authors.SelectById(bookDTO.AuthorId) is null) return BadRequest("Author not found");
        if (bookDTO.CategoryId.HasValue && unit.Categories.SelectById(bookDTO.CategoryId.Value) is null) return BadRequest("Category not found");

        string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{mediaEndPoint}";
        string? PhotoId = null;
        if (bookDTO.Photo != null)
        {
            PhotoId = fileService.AddPhoto(bookDTO.Photo, uploadPath);
        }

        Book book = mapper.Map<Book>(bookDTO);
        book.PhotoId = PhotoId;

        unit.Books.Add(book);
        unit.Save();

        BookViewDTO view = mapper.Map<BookViewDTO>(book, opt => opt.Items["BaseUrl"] = baseUrl);
        return CreatedAtAction(nameof(GetById), new { Id = view.Id }, view);
    }

    [SwaggerOperation(Summary = "Edit a book", Description = "Requires Admin role")]
    [SwaggerResponse(204, "The book was updated")]
    [SwaggerResponse(400, "The book data is invalid")]
    [SwaggerResponse(404, "The book was not found")]
    [SwaggerResponse(401, "The user is not authorized to edit a book, you must be an Admin")]
    [Produces("application/json")]
    [HttpPut("{id}")]
    public IActionResult Edit(int id, EditBookDTO bookDTO)
    {
        if (bookDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (unit.Authors.SelectById(bookDTO.AuthorId) is null) return BadRequest("Author not found");
        if (bookDTO.CategoryId.HasValue && unit.Categories.SelectById(bookDTO.CategoryId.Value) is null) return BadRequest("Category not found");

        string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{mediaEndPoint}";

        Book? book = unit.Books.SelectById(id);
        if (book is null) return NotFound();
        string? PhotoId = book.PhotoId;
        string? newPhotoId = null;

        if (bookDTO.Photo != null)
        {
            if (PhotoId != null)
                fileService.RemovePhoto(PhotoId, uploadPath);
            newPhotoId = fileService.AddPhoto(bookDTO.Photo, uploadPath);
        }

        mapper.Map(bookDTO, book);
        book.PhotoId = newPhotoId ?? PhotoId;

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

        if (book.PhotoId != null)
            fileService.RemovePhoto(book.PhotoId, uploadPath);
        unit.Books.Delete(book);
        unit.Save();

        return NoContent();
    }
}
