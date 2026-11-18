namespace NotesApi.Application.Common;

// Чем закончилось действие сервиса.
public enum ResultStatus
{
    Success,  // действие выполнено
    NotFound, // записи нет
    Conflict, // действие противоречит состоянию записи (например, она уже опубликована)
    Invalid   // Note отклонила данные (например, пустой заголовок)
}
