using bma.Domain.Entities;

namespace bma.Application.Approvals.Dtos;

public static class ApprovalProfile
{
    public static ApprovalDto ToApprovalDto(Approval approval)
    {
        return new ApprovalDto
        {
            Id = approval.Id,
            Status = approval.Status,
            RequestId = approval.RequestId,
            JoinRequestId = approval.JoinRequestId,
            ApprovedBy = approval.ApprovedBy
        };
    }
}
