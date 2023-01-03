using Application.Features.BoardItems.Commands.CreateBoardItem;
using Application.Features.BoardItems.Commands.DeleteBoardItem;
using Application.Features.BoardItems.Commands.EditBoardItem;
using Application.Features.BoardItems.Queries.GetBoardItem;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BoardItemsController : ControllerBase
{
    private readonly IHubContext<BoardHub> _boardHubContext;
    private readonly IMediator _mediator;

    public BoardItemsController(IMediator mediator, IHubContext<BoardHub> boardHubContext)
    {
        _mediator = mediator;
        _boardHubContext = boardHubContext;
    }

    [HttpGet("{itemId}")]
    public async Task<ActionResult<BoardItemDto>> GetItem(string itemId)
    {
        var request = new GetBoardItemQuery(itemId);
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BoardItemDto>> CreateItem([FromBody] CreateBoardItemCommand request)
    {
        var result = await _mediator.Send(request);
        if (result is null) return BadRequest("Could not create item.");

        await _boardHubContext.Clients.Group(result.BoardId).SendAsync("ItemCreated", result);
        return Created($"/boardItem/{result.BoardItemId}", result);
    }

    [HttpPut]
    public async Task<ActionResult> EditItem([FromBody] EditBoardItemCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result) return BadRequest("Could not edit item!");
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteItem([FromBody] DeleteBoardItemCommand request)
    {
        var result = await _mediator.Send(request);
        if (result?.BoardId is null) return BadRequest("Could not delete item!");
        await _boardHubContext.Clients.Group(result.BoardId).SendAsync("ItemDeleted", result);
        return Ok();
    }
}