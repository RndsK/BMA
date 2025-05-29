namespace bma.Domain.Exceptions;

public class DuplicateJoinRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateJoinRequestException"/> class with a predefined message.
    /// </summary>
    /// <param name="userId">The user ID associated with the duplicate request.</param>
    /// <param name="companyId">The company ID associated with the duplicate request.</param>
    public DuplicateJoinRequestException(string userId, int companyId)
        : base($"A join request already exists for UserId: {userId} and CompanyId: {companyId}.")
    {
    }
}
