// NoteEndpoints — слой Api. Контроллер (ControllerBase): принимает HTTP-запросы и отдаёт коды.
//
// Этап 0 (без защиты):
//   POST   /notes              — создать черновик → 201 + Location
//   GET    /notes              — список опубликованных → 200
//   GET    /notes/{id}         — одна запись → 200 / 404
//   PUT    /notes/{id}         — изменить → 204 / 404 / 409
//   POST   /notes/{id}/publish — опубликовать → 204 / 404 / 409
//   DELETE /notes/{id}         — удалить → 204 / 404
//   Плохие данные → 400. Ответ сервиса: «не найдено» → 404, «конфликт» → 409.
//
// Позже: [Authorize] на всём контроллере, роль Writer — на изменяющих действиях.
//
// Партнёры: NoteService.

using Microsoft.AspNetCore.Mvc;
using NotesApi.Api.DTOs.Requests;
using NotesApi.Api.DTOs.Responses;
using NotesApi.Application.Common;
using NotesApi.Application.Notes.Interfaces;

namespace NotesApi.Api.Endpoints;

[ApiController]
[Route("notes")]
public class NoteEndpoints(INoteService noteService) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpPost]
    public async Task<ActionResult<ResponseNoteDto>> Create([FromBody] RequestNoteDto request, CancellationToken ct)
    {
        var result = await noteService.AddNoteAsync(request, ct);
        if (!result.IsSuccess)
            return ToProblem(result.Status, result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ResponseNoteDto>>> GetPublished(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > MaxPageSize)
            return ToProblem(ResultStatus.Invalid, $"page must be at least 1, pageSize must be between 1 and {MaxPageSize}.");

        return Ok(await noteService.GetPublishedAsync(page, pageSize, ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResponseNoteDto>> GetById(Guid id, CancellationToken ct)
    {
        var note = await noteService.GetNoteByIdAsync(id, ct);
        if (note is null)
            return ToProblem(ResultStatus.NotFound, null);

        return Ok(note);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Change(Guid id, [FromBody] UpdateNoteDto request, CancellationToken ct)
    {
        var result = await noteService.ChangeNoteAsync(id, request, ct);
        if (!result.IsSuccess)
            return ToProblem(result.Status, result.Error);

        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var result = await noteService.PublishNoteAsync(id, ct);
        if (!result.IsSuccess)
            return ToProblem(result.Status, result.Error);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await noteService.DeleteNoteAsync(id, ct);
        if (!deleted)
            return ToProblem(ResultStatus.NotFound, null);

        return NoContent();
    }

    // Единственное место, где результат сервиса превращается в HTTP-код (ProblemDetails).
    private ObjectResult ToProblem(ResultStatus status, string? error) => status switch
    {
        ResultStatus.NotFound => Problem(
            title: "Note not found.", statusCode: StatusCodes.Status404NotFound),
        ResultStatus.Conflict => Problem(
            title: "Conflict with the current state of the note.", detail: error, statusCode: StatusCodes.Status409Conflict),
        ResultStatus.Invalid => Problem(
            title: "Invalid data.", detail: error, statusCode: StatusCodes.Status400BadRequest),
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Success is not an error.")
    };
}
