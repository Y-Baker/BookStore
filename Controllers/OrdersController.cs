using AutoMapper;
using BookStore.DTOs.Order;
using BookStore.Models;
using BookStore.UnitOfWorks;
using BookStore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private const string CustomerRole = "Customer";
        private readonly UnitOfWork unit;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public OrdersController(UnitOfWork unit, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            this.unit = unit;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [SwaggerOperation(Summary = "Get all orders", Description = "Requires Admin role")]
        [SwaggerResponse(200, "The list of orders", typeof(List<OrderViewDTO>))]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            List<Order> orders = unit.Orders.SelectAll();
            List<OrderViewDTO> views = mapper.Map<List<OrderViewDTO>>(orders);
            return Ok(views);
        }

        [SwaggerOperation(Summary = "Get an order by Id", Description = "Requires Admin role")]
        [SwaggerResponse(200, "The order was found", typeof(OrderViewDTO))]
        [SwaggerResponse(404, "The order was not found")]
        [Produces("application/json")]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            Order? order = unit.Orders.SelectById(id);
            if (order == null) return NotFound();

            OrderViewDTO view = mapper.Map<OrderViewDTO>(order);
            return Ok(view);
        }

        [SwaggerOperation(Summary = "Get all orders of login customer", Description = "Requires Customer role")]
        [SwaggerResponse(200, "The list of orders", typeof(List<OrderViewDTO>))]
        [SwaggerResponse(401, "The user is not authorized to view orders, you must be a Customer")]
        [Produces("application/json")]
        [HttpGet("my")]
        [Authorize(Roles = CustomerRole)]
        public IActionResult GetOrders()
        {
            if (User.Identity?.Name == null) return Unauthorized();

            Customer? customer = userManager.GetUsersInRoleAsync(CustomerRole).Result.OfType<Customer>().SingleOrDefault(c => c.UserName == User.Identity.Name);
            if (customer == null) return NotFound();

            List<Order> orders = unit.Orders.SelectOrdersByCustomerId(customer.Id);
            List<OrderViewDTO> views = mapper.Map<List<OrderViewDTO>>(orders);
            return Ok(views);
        }

        [SwaggerOperation(Summary = "Create a new order", Description = "Requires Customer role")]
        [SwaggerResponse(201, "The order was created")]
        [SwaggerResponse(400, "The order data is invalid")]
        [SwaggerResponse(401, "The user is not authorized to create an order, you must be a Customer")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Authorize(Roles = CustomerRole)]
        [HttpPost]
        public IActionResult CreateOrder(List<OrderDetailDTO> orderDetails)
        {
            if (orderDetails == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (User.Identity?.Name == null) return Unauthorized();

            Customer? customer = userManager.GetUsersInRoleAsync(CustomerRole).Result.OfType<Customer>().SingleOrDefault(c => c.UserName == User.Identity.Name);
            if (customer == null) return NotFound();

            Order order = new Order()
            {
                CustomerId = customer.Id,
                OrderDate = DateOnly.FromDateTime(DateTime.Today),
                TotalPrice = 0,
                Status = Status.Pending,
            };

            decimal totalPrice = 0;

            foreach (OrderDetailDTO detail in orderDetails)
            {
                Book? book = unit.Books.SelectById(detail.BookId);
                if (book == null) return NotFound();

                OrderDetails orderDetail = new OrderDetails()
                {
                    OrderId = order.Id,
                    BookId = book.Id,
                    Quantity = detail.Quantity,
                    UnitPrice = book.Price,
                };

                if (book.Stock < detail.Quantity)
                    return BadRequest("Not enough stock");

                totalPrice += book.Price * detail.Quantity;
                order.Details.Add(orderDetail);
                book.Stock -= detail.Quantity;
                unit.Books.Update(book);

            }

            order.TotalPrice = totalPrice;
            unit.Orders.Add(order);
            unit.Save();

            OrderViewDTO view = mapper.Map<OrderViewDTO>(order);

            return CreatedAtAction(nameof(GetOrderById), new {id = order.Id}, view);
        }

        [SwaggerOperation(Summary = "Set the status of an order", Description = "Requires Customer role")]
        [SwaggerResponse(204, "The order status was updated")]
        [SwaggerResponse(400, "The order status data is invalid")]
        [SwaggerResponse(401, "The user is not authorized to update the order status, you must be a Customer")]
        [SwaggerResponse(404, "The order was not found")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Authorize(Roles = CustomerRole)]
        [HttpPut("{id}")]
        public IActionResult SetStatus(int id, Status status)
        {
            if (User.Identity?.Name == null) return Unauthorized();

            Customer? customer = userManager.GetUsersInRoleAsync(CustomerRole).Result.OfType<Customer>().SingleOrDefault(c => c.UserName == User.Identity.Name);
            if (customer == null) return NotFound();

            Order? order = unit.Orders.SelectById(id);
            if (order == null) return NotFound();
            if (order.CustomerId != customer.Id) return Unauthorized();

            order.Status = status;
            unit.Orders.Update(order);
            unit.Save();

            return NoContent();
        }

        [SwaggerOperation(Summary = "Delete an order by Id", Description = "Requires Customer role")]
        [SwaggerResponse(204, "The order was deleted")]
        [SwaggerResponse(401, "The user is not authorized to delete the order, you must be a Customer")]
        [SwaggerResponse(404, "The order was not found")]
        [Produces("application/json")]
        [Authorize(Roles = CustomerRole)]
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            if (User.Identity?.Name == null) return Unauthorized();

            Customer? customer = userManager.GetUsersInRoleAsync(CustomerRole).Result.OfType<Customer>().SingleOrDefault(c => c.UserName == User.Identity.Name);
            if (customer == null) return NotFound();

            Order? order = unit.Orders.SelectById(id);
            if (order == null) return NotFound();
            if (order.CustomerId != customer.Id) return Unauthorized();

            unit.Orders.Delete(order);
            unit.Save();

            return NoContent();
        }
    }
}
