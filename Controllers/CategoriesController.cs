using AutoMapper;
using BookStore.DTOs.Book;
using BookStore.DTOs.Category;
using BookStore.Models;
using BookStore.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly UnitOfWork unit;
    private readonly IMapper mapper;
    private const string mediaEndPoint = $"api/media";

    public CategoriesController(UnitOfWork unit, IMapper mapper)
    {
        this.unit = unit;
        this.mapper = mapper;
    }

    [SwaggerOperation(Summary = "Get all categories", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The list of categories", typeof(List<CategoryViewDTO>))]
    [Produces("application/json")]
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Category> categories = unit.Categories.SelectAll();
        List<CategoryViewDTO> views = mapper.Map<List<CategoryViewDTO>>(categories);

        return Ok(views);
    }

    [SwaggerOperation(Summary = "Get a category by Id", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The category was found", typeof(CategoryViewDTO))]
    [SwaggerResponse(404, "The category was not found")]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Category? category = unit.Categories.SelectById(id);
        if (category is null)
            return NotFound();

        CategoryViewDTO view = mapper.Map<CategoryViewDTO>(category);
        return Ok(view);
    }

    [SwaggerOperation(Summary = "Get all books by category Id", Description = "Requires no authentication")]
    [SwaggerResponse(200, "The list of books by category", typeof(List<BookViewDTO>))]
    [SwaggerResponse(404, "The category was not found")]
    [Produces("application/json")]
    [HttpGet("{id}/books")]
    public IActionResult GetBooks(int id)
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{mediaEndPoint}";

        Category? category = unit.Categories.SelectById(id);
        if (category is null) return NotFound();

        List<Book> books = unit.Books.SelectAll().Where(e => e.CategoryId == id).ToList();
        List<BookViewDTO> views = mapper.Map<List<BookViewDTO>>(books, opt => opt.Items["BaseUrl"] = baseUrl);

        return Ok(views);
    }

    [SwaggerOperation(Summary = "Add a new category", Description = "Requires Admin role")]
    [SwaggerResponse(201, "The category was created", typeof(CategoryViewDTO))]
    [SwaggerResponse(400, "The category data is invalid")]
    [SwaggerResponse(401, "The user is not authorized to create a category, you must be an Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(AddCategoryDTO categoryDTO)
    {
        if (categoryDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        Category category = mapper.Map<Category>(categoryDTO);

        unit.Categories.Add(category);
        unit.Save();

        CategoryViewDTO view = mapper.Map<CategoryViewDTO>(category);
        return CreatedAtAction(nameof(GetById), new { Id = view.Id }, view);
    }

    [SwaggerOperation(Summary = "Edit a category", Description = "Requires Admin role")]
    [SwaggerResponse(204, "The category was updated")]
    [SwaggerResponse(400, "The category data is invalid")]
    [SwaggerResponse(404, "The category was not found")]
    [SwaggerResponse(401, "The user is not authorized to edit a category, you must be an Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Edit(int id, EditCategoryDTO categoryDTO)
    {
        if (categoryDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        Category? category = unit.Categories.SelectById(id);
        if (category is null) return NotFound();

        mapper.Map(categoryDTO, category);
        unit.Save();

        return NoContent();
    }

    [SwaggerOperation(Summary = "Delete a category", Description = "Requires Admin role")]
    [SwaggerResponse(204, "The category was deleted")]
    [SwaggerResponse(404, "The category was not found")]
    [SwaggerResponse(401, "The user is not authorized to delete a category, you must be an Admin")]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Category? category = unit.Categories.SelectById(id);
        if (category is null) return NotFound();

        unit.Categories.Delete(category);
        unit.Save();

        return NoContent();
    }
}

