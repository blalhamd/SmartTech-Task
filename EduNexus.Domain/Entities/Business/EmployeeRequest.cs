using EduNexus.Domain.Entities.Base;
using EduNexus.Domain.Enums;
using EduNexus.Domain.Errors;
using EduNexus.Shared.Common;

namespace EduNexus.Domain.Entities.Business
{
    public class EmployeeRequest : BaseEntity
    {
        public Guid? EmployeeId { get; private set; }
        public Employee? Employee { get; private set; }

        public ActionType ActionType { get; private set; } // Create, Update, or Delete
        public RequestStatus Status { get; private set; } = RequestStatus.Pending;

        public string? OldData { get; private set; } 
        public string NewData { get; private set; } = string.Empty;

        public string? RejectionReason { get; private set; }

        private EmployeeRequest() { }

        private EmployeeRequest(string newData)
        {
            EmployeeId = null;
            ActionType = ActionType.Create;
            NewData = newData;
            OldData = null;
        }

        public static ValueResult<EmployeeRequest> Create(string newData)
        {
            if (string.IsNullOrWhiteSpace(newData))
                return ValueResult<EmployeeRequest>.Failure(EmployeeRequestErrors.InValidNewData);

            return ValueResult<EmployeeRequest>.Success(new EmployeeRequest(newData));
        }

        public static ValueResult<EmployeeRequest> CreateUpdateRequest(Guid employeeId, string oldData, string newData)
        {
            if (employeeId == Guid.Empty)
                return ValueResult<EmployeeRequest>.Failure(EmployeeRequestErrors.InvalidEmployeeId);

            if (string.IsNullOrWhiteSpace(newData))
                return ValueResult<EmployeeRequest>.Failure(EmployeeRequestErrors.InValidNewData);

            var request = new EmployeeRequest
            {
                EmployeeId = employeeId,
                ActionType = ActionType.Update,
                Status = RequestStatus.Pending,
                OldData = oldData, 
                NewData = newData  
            };

            return ValueResult<EmployeeRequest>.Success(request);
        }

        public Result Update(Guid employeeId, string newData, string oldData)
        {
            if (employeeId == Guid.Empty)
                return Result.Failure(EmployeeRequestErrors.InvalidEmployeeId);

            if (string.IsNullOrWhiteSpace(oldData))
                return Result.Failure(EmployeeRequestErrors.InavlidOldData);

            if (string.IsNullOrWhiteSpace(newData))
                return Result.Failure(EmployeeRequestErrors.InValidNewData);

            EmployeeId = employeeId;
            ActionType = ActionType.Update;
            NewData = newData;
            OldData = oldData;
            
            return Result.Success();
        }

        public static ValueResult<EmployeeRequest> CreateDeleteRequest(Guid employeeId, string employeeDataSnapshot)
        {
            if (employeeId == Guid.Empty)
                return ValueResult<EmployeeRequest>.Failure(EmployeeRequestErrors.InvalidEmployeeId);

            var request = new EmployeeRequest
            {
                EmployeeId = employeeId,
                ActionType = ActionType.Delete,
                Status = RequestStatus.Pending,
                OldData = employeeDataSnapshot,
                NewData = string.Empty
            };

            return ValueResult<EmployeeRequest>.Success(request);
        }

      
        public Result Approve(Guid reviewerId)
        {
            if (Status != RequestStatus.Pending && Status == RequestStatus.Approved)
                return Result.Failure(EmployeeRequestErrors.AlreadyApproved);

            if (CreatedBy == reviewerId)
                return Result.Failure(EmployeeRequestErrors.MakerCannotBeReviewer);

            Status = RequestStatus.Approved;

            return Result.Success();
        }

        public Result Reject(string? reason, Guid reviewerId)
        {
            if (Status != RequestStatus.Pending)
                return Result.Failure(EmployeeRequestErrors.AlreadyProcessed);

            if (CreatedBy == reviewerId) 
                return Result.Failure(EmployeeRequestErrors.ReviewerCannotBeMaker);

            Status = RequestStatus.Rejected;
            RejectionReason = reason;

            return Result.Success();
        }

        public static ValueResult<EmployeeRequest> CreateDeactivateRequest(Guid employeeId, string newData)
        {
            if (employeeId == Guid.Empty)
                return ValueResult<EmployeeRequest>.Failure(EmployeeRequestErrors.InvalidEmployeeId);

            if (string.IsNullOrEmpty(newData))
                return ValueResult<EmployeeRequest>.Failure(EmployeeRequestErrors.InValidNewData);

            var request = new EmployeeRequest
            {
                ActionType = ActionType.Deactivate,
                EmployeeId = employeeId,
                Status = RequestStatus.Pending,
                OldData = string.Empty,
                NewData = newData
            };

            return ValueResult<EmployeeRequest>.Success(request);
        }

    }
}
