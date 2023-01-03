using Application.Features.BoardItems.Commands.UpdatePlacement;
using Application.Features.Boards.Queries.GetItemsForBoard;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

[Authorize]
public class BoardHub : Hub
{
    private readonly IMediator _mediator;

    public BoardHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<List<BoardItemDto>> JoinBoard(string boardId)
    {
        var response = await _mediator.Send(new GetItemsForBoardQuery(boardId));

        await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
        return response;
    }

    public async Task LeaveBoard(string boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);
    }

    public async Task<string> UpdatePlacement(UpdatePlacementCommand command)
    {
        var result = await _mediator.Send(command);
        if (result?.BoardId is null) return "Could not update placement!";

        await Clients.Group(result.BoardId).SendAsync("PlacementUpdated", result);
        return "Success!";
    }
}