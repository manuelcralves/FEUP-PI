using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Web;

namespace fbc_webapi.ErrorHandling
{
    public class FlorestasBemCuidadaWebApiException : Exception
    {
        public bool UserFriendlyMessage { get; set; }
        public EnumErrorCode ErrorCode { get; set; }
        public HttpStatusCode? StatusCode { get; set; }

        public FlorestasBemCuidadaWebApiException()
        {

        }

        public FlorestasBemCuidadaWebApiException(string message) : base(message)
        {

        }

        public FlorestasBemCuidadaWebApiException(EnumErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public FlorestasBemCuidadaWebApiException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage) : base(message)
        {
            UserFriendlyMessage = userFriendlyMessage;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage, EnumErrorCode errorCode) : base(message)
        {
            UserFriendlyMessage = userFriendlyMessage;
            ErrorCode = errorCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage, HttpStatusCode statusCode) : base(message)
        {
            UserFriendlyMessage = userFriendlyMessage;
            StatusCode = statusCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage, Exception innerException) : base(message, innerException)
        {
            UserFriendlyMessage = userFriendlyMessage;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage, EnumErrorCode errorCode, HttpStatusCode statusCode) : base(message)
        {
            UserFriendlyMessage = userFriendlyMessage;
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage, EnumErrorCode errorCode, Exception innerException) : base(message, innerException)
        {
            UserFriendlyMessage = userFriendlyMessage;
            ErrorCode = errorCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
        {
            UserFriendlyMessage = userFriendlyMessage;
            StatusCode = statusCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, bool userFriendlyMessage, EnumErrorCode errorCode, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
        {
            UserFriendlyMessage = userFriendlyMessage;
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, EnumErrorCode errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, EnumErrorCode errorCode, HttpStatusCode statusCode) : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, EnumErrorCode errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, EnumErrorCode errorCode, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public FlorestasBemCuidadaWebApiException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public FlorestasBemCuidadaWebApiException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        protected FlorestasBemCuidadaWebApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}