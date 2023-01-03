using Application.Features.Comments.Commands.CreateComment;
using Application.Features.Comments.Commands.DeleteComment;
using Application.Features.Comments.Queries.GetCommentsForItem;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{boardItemId}")]
    public async Task<ActionResult<ICollection<Comment>>> GetComments(string boardItemId)
    {
        var result = await _mediator.Send(new GetCommentsForItemQuery(boardItemId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Comment?>> CreateComment([FromBody] CreateCommentCommand request)
    {
        var result = await _mediator.Send(request);
        if (result is null) return BadRequest("Could not create comment.");
        return Created("", result);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteComment([FromBody] DeleteCommentCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result) return BadRequest("Could not delete comment!");
        return Ok();
    }
}