namespace ApiAuth.Endpoints;

public record MyAccountResult(Guid Id, string Name, string Email, string? PhoneNumber, bool EmailConfirmed, bool PhoneNumberConfirmed);