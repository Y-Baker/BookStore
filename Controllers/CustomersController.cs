using AutoMapper;
using BookStore.DTOs.User;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private const string CustomerRoleName = "Customer";
    private const string AdminRoleName = "Admin";
    private readonly UserManager<IdentityUser> userManager;
    private readonly IMapper mapper;

    public CustomersController(UserManager<IdentityUser> userManager, IMapper _mapper)
    {
        this.userManager = userManager;
        mapper = _mapper;
    }

    [SwaggerOperation(Summary = "Register a new Customer", Description = "no authentication required")]
    [SwaggerResponse(201, "The Customer was created", typeof(CustomerViewDTO))]
    [SwaggerResponse(400, "The Customer data is invalid")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost("register")]
    public IActionResult Register(AddCustomerDTO? customerDTO)
    {
        if (customerDTO is null)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest();

        Customer customer = mapper.Map<Customer>(customerDTO); 

        IdentityResult res = userManager.CreateAsync(customer, customerDTO.Password).Result;
        if (!res.Succeeded)
            return BadRequest(res.Errors);

        res = userManager.AddToRoleAsync(customer, CustomerRoleName).Result;
        if (!res.Succeeded)
            return BadRequest(res.Errors);

        CustomerViewDTO view = mapper.Map<CustomerViewDTO>(customer);

        return CreatedAtAction(nameof(GetById), new { Id = view.Id }, view);
    }

    [SwaggerOperation(Summary = "Edit a Customer", Description = "Requires Admin role or the Customer Id must match the logged in user")]
    [SwaggerResponse(204, "The Customer was updated")]
    [SwaggerResponse(400, "The Customer data is invalid")]
    [SwaggerResponse(401, "The user is not authorized to edit a Customer")]
    [SwaggerResponse(404, "The Customer was not found")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    [HttpPut("profile")]
    public IActionResult Edit(EditCustomerDTO customerDTO)
    {
        if (customerDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ClaimsIdentity? identity = User.Identity as ClaimsIdentity;
        if (identity == null)
            return Unauthorized();
        List<Claim>? claims = identity.Claims.ToList();

        if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == AdminRoleName))
        {
            Claim? claimId = claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claimId == null || claimId.Value != customerDTO.Id)
                return Unauthorized();
        }
        
        Customer? customer = userManager.GetUsersInRoleAsync(CustomerRoleName).Result.OfType<Customer>().SingleOrDefault(e => e.Id == customerDTO.Id);
        if (customer == null) return NotFound();

        mapper.Map(customerDTO, customer);

        IdentityResult res = userManager.UpdateAsync(customer).Result;
        if (!res.Succeeded) return BadRequest(res.Errors);

        return NoContent();
    }


    [SwaggerOperation(Summary = "Get all Customers", Description = "no authentication required")]
    [SwaggerResponse(200, "The list of Customers", typeof(List<CustomerViewDTO>))]
    [Produces("application/json")]
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Customer> customers = userManager.GetUsersInRoleAsync("Customer").Result.OfType<Customer>().ToList();
        List<CustomerViewDTO>? customersDTO = mapper.Map<List<CustomerViewDTO>>(customers);

        return Ok(customersDTO);
    }

    [SwaggerOperation(Summary = "Get Customer by Id", Description = "no authentication required")]
    [SwaggerResponse(200, "The Customer was found", typeof(CustomerViewDTO))]
    [SwaggerResponse(404, "The Customer was not found")]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        Customer? customer = (Customer?)userManager.GetUsersInRoleAsync("Customer").Result.Where(e => e.Id == id).SingleOrDefault();

        if (customer == null)
            return NotFound();

        CustomerViewDTO view = mapper.Map<CustomerViewDTO>(customer);
        return Ok(view);
    }
}
