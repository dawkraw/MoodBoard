using Application.Features.Boards.Commands.AddMemberToBoard;
using Application.Features.Boards.Commands.CreateBoard;
using Application.Features.Boards.Commands.DeleteBoard;
using Application.Features.Boards.Commands.EditBoard;
using Application.Features.Boards.Commands.RemoveMemberFromBoard;
using Application.Features.Boards.Queries.GetBoard;
using Application.Features.Boards.Queries.GetBoardsForUser;
using Application.Features.Boards.Queries.SearchBoards;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<BoardListItem>> CreateBoard([FromBody] CreateBoardCommand request)
    {
        var result = await _mediator.Send(request);
        return Created($"/board/{result.BoardId}", result);
    }

    [HttpDelete("{boardId}")]
    public async Task<ActionResult> DeleteBoard(string boardId)
    {
        var request = new DeleteBoardCommand(boardId);
        var result = await _mediator.Send(request);
        if (!result) return BadRequest("Could not delete!");
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> EditBoard([FromBody] EditBoardCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result) return BadRequest("Could not edit!");
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoardListItem>>> GetBoards()
    {
        var request = new GetBoardsForUserQuery();
        var result = await _mediator.Send(request);
        if (!result.Any()) return NotFound();
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BoardListItem>>> SearchBoards([FromQuery] SearchBoardsQuery request)
    {
        var result = await _mediator.Send(request);
        if (!result.Any()) return NotFound();
        return Ok(result);
    }

    [HttpGet("{boardId}")]
    public async Task<ActionResult<Board>> GetBoardInfo(string boardId)
    {
        var request = new GetBoardQuery(boardId);
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("Member")]
    public async Task<ActionResult<User>> AddMemberToBoard([FromBody] AddMemberToBoardCommand request)
    {
        var result = await _mediator.Send(request);
        if (result == null) return BadRequest("Could not add member!");
        return Ok(result);
    }

    [HttpDelete("Member")]
    public async Task<ActionResult> RemoveMemberFromBoard([FromBody] RemoveMemberFromBoardCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result) return BadRequest("Could not remove member!");
        return Ok();
    }
}