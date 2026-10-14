using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Domain.Errors;

namespace Aftab.App.Domain.Ticketing.Errors;

public static class TicketingErrors
{
    public static readonly Error TitleIsTooShort = new(
        "Ticket.TitleIsTooShort",
        $"عنوان نمی تواند کمتر از {Title.MinLenght} کاراکتر باشد.",
        ErrorType.Validation);
    
    public static readonly Error TitleIsTooLong = new(
        "Ticket.TitleIsTooLong",
        $"عنوان نمی تواند بیشتر از {Title.MaxLenght} کاراکتر باشد.",
        ErrorType.Validation);
    
    public static Error NotFound(string ticketId) => new(
        "Ticket.NotFound",
        $"پاسخی با شناسه {ticketId} یافت نشد.",
        ErrorType.NotFound);

    public static Error ReplyTicketNotFound(string replyTicketId) => new(
        "Ticket.ReplyTicketNotFound",
        $"پاسخی با شناسه {replyTicketId} یافت نشد.",
        ErrorType.NotFound);
}
