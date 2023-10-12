using System.Diagnostics.CodeAnalysis;

namespace RebacExperiments.Server.Api.Infrastructure.Services
{
    /// <summary>
    /// Untyped Service Result for calls without data.
    /// </summary>
    public class ServiceResult
    {
        [MemberNotNullWhen(returnValue: false, member: nameof(Error))]
        public virtual bool Succeeded => Error == null;

        public virtual ServiceError? Error { get; set; }

        public ServiceResult(ServiceError error)
        {
            Error = error;
        }

        public ServiceResult() { }

        #region Helper Methods

        public static ServiceResult Failed(ServiceError error)
        {
            return new ServiceResult(error);
        }

        public static ServiceResult<T> Failed<T>(ServiceError error)
        {
            return new ServiceResult<T>(error);
        }

        public static ServiceResult<T> Failed<T>(T data, ServiceError error)
        {
            return new ServiceResult<T>(data, error);
        }

        public static ServiceResult Success()
        {
            return new ServiceResult();
        }

        public static ServiceResult<T> Success<T>(T data)
        {
            return new ServiceResult<T>(data);
        }

        #endregion
    }

    /// <summary>
    /// A standard response for service calls.
    /// </summary>
    /// <typeparam name="T">Return data type</typeparam>
    public class ServiceResult<T> : ServiceResult
    {
        [MemberNotNullWhen(returnValue: true, member: nameof(Data))]
        [MemberNotNullWhen(returnValue: false, member: nameof(Error))]
        public override bool Succeeded => Error == null;

        public override ServiceError? Error { get; set; }

        /// <summary>
        /// Result.
        /// </summary>
        public readonly T? Data;

        public ServiceResult(T? data)
        {
            Data = data;
        }

        public ServiceResult(T? data, ServiceError error) 
        {
            Error = error;
            Data = data;
        }

        public ServiceResult(ServiceError error)
        {
            Error = error;
            Data = default;
        }
    }
}
