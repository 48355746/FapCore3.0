using Ardalis.GuardClauses;
using System;
using System.Runtime.Serialization;

namespace Fap.Core.Exceptions
{
    public class FapException : SystemException
    {
        public FapException()
            : base()
        {

        }

        public FapException(string message) :
            base(message)
        {
        }

        public FapException(string message, Exception innerException) :
            base(message, innerException)
        {

        }    
        protected FapException(
          SerializationInfo info, StreamingContext cxt) :
            base(info, cxt)
        {
        }
    }
    public static class FapGuard
    {
        public static void FapRuntime(this IGuardClause guardClause, string message)
        {
            throw new FapException(message);
        }
        public static void FapRuntime(this IGuardClause guardClause, string message, Exception exception)
        {
            throw new FapException(message, exception);
        }
        public static void FapRuntime(this IGuardClause guardClause,  Exception exception)
        {
            throw new FapException(exception.Message, exception);
        }
        public static void FapBusiness(this IGuardClause guardClause, string message, Exception exception)
        {
            throw new FapBizException(message, exception);
        }
        public static void FapBusiness(this IGuardClause guardClause, string message)
        {
            throw new FapBizException(message);
        }
        public static void FapBusiness(this IGuardClause guardClause, Exception exception)
        {
            throw new FapBizException(exception.Message, exception);
        }
    }
    /// <summary>
    /// 业务异常
    /// </summary>
    public class FapBizException : SystemException
    {
        public FapBizException()
            : base()
        {

        }

        public FapBizException(string message) :
            base(message)
        {
        }

        public FapBizException(string message, Exception innerException) :
            base(message, innerException)
        {

        }
        protected FapBizException(
          SerializationInfo info, StreamingContext cxt) :
            base(info, cxt)
        {
        }
    }
}
