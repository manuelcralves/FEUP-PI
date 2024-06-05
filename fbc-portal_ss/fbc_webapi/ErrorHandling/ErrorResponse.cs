using Primavera.Platform.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace fbc_webapi.ErrorHandling
{
    public class ErrorResponse
    {
        public EnumErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string TechnicalErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }

        internal HttpStatusCode? StatusCode { get; set; }

        public ErrorResponse()
        {
            ErrorCode = EnumErrorCode.Geral;
            Timestamp = DateTime.Now;
        }

        public ErrorResponse(EnumErrorCode errorCode) : this()
        {
            ErrorCode = errorCode;
        }

        public ErrorResponse(EnumErrorCode errorCode, string errorMessage) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public ErrorResponse(EnumErrorCode errorCode, string errorMessage, string technicalErrorMessage) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
        }

        public ErrorResponse(EnumErrorCode errorCode, string errorMessage, DateTime timestamp) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Timestamp = timestamp;
        }

        internal ErrorResponse(EnumErrorCode errorCode, string errorMessage, HttpStatusCode statusCode) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public ErrorResponse(EnumErrorCode errorCode, string errorMessage, string technicalErrorMessage, DateTime timestamp) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
            Timestamp = timestamp;
        }

        internal ErrorResponse(EnumErrorCode errorCode, string errorMessage, string technicalErrorMessage, HttpStatusCode statusCode) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
            StatusCode = statusCode;
        }

        internal ErrorResponse(EnumErrorCode errorCode, string errorMessage, HttpStatusCode statusCode, DateTime timestamp) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            Timestamp = timestamp;
        }

        internal ErrorResponse(EnumErrorCode errorCode, string errorMessage, string technicalErrorMessage, HttpStatusCode statusCode, DateTime timestamp) : this()
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
            StatusCode = statusCode;
            Timestamp = timestamp;
        }

        public ErrorResponse(string errorMessage) : this()
        {
            ErrorMessage = errorMessage;
        }

        public ErrorResponse(string errorMessage, string technicalErrorMessage) : this()
        {
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
        }

        internal ErrorResponse(string errorMessage, HttpStatusCode statusCode) : this()
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public ErrorResponse(string errorMessage, DateTime timestamp) : this()
        {
            ErrorMessage = errorMessage;
            Timestamp = timestamp;
        }

        internal ErrorResponse(string errorMessage, string technicalErrorMessage, HttpStatusCode statusCode) : this()
        {
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
            StatusCode = statusCode;
        }

        public ErrorResponse(string errorMessage, string technicalErrorMessage, DateTime timestamp) : this()
        {
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
            Timestamp = timestamp;
        }

        internal ErrorResponse(string errorMessage, HttpStatusCode statusCode, DateTime timestamp) : this()
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            Timestamp = timestamp;
        }

        internal ErrorResponse(string errorMessage, string technicalErrorMessage, HttpStatusCode statusCode, DateTime timestamp) : this()
        {
            ErrorMessage = errorMessage;
            TechnicalErrorMessage = technicalErrorMessage;
            StatusCode = statusCode;
            Timestamp = timestamp;
        }

        internal ErrorResponse(HttpStatusCode statusCode) : this()
        {
            StatusCode = statusCode;
        }

        internal ErrorResponse(HttpStatusCode statusCode, DateTime timestamp) : this()
        {
            StatusCode = statusCode;
            Timestamp = timestamp;
        }

        public HttpResponseMessage ConstruirHttpResponseMessage(HttpRequestMessage request)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            if (StatusCode != null)
                statusCode = StatusCode.Value;

            HttpResponseMessage httpResponse = request.CreateResponse(statusCode, this);

            return httpResponse;
        }

        public static ErrorResponse ConstruirRespostaDeExcecao(Exception exception)
        {
            ErrorResponse errorResponse = new ErrorResponse(EnumErrorCode.Geral, "Ocorreu um erro inesperado no servidor. Se o problema continuar a acontecer contacte um técnico.", exception?.Message, DateTime.Now);

            if (exception is FlorestasBemCuidadaWebApiException apiException)
            {
                errorResponse.StatusCode = apiException.StatusCode;

                if (apiException.ErrorCode != EnumErrorCode.NA)
                    errorResponse.ErrorCode = apiException.ErrorCode;

                if (apiException.UserFriendlyMessage)
                {
                    errorResponse.ErrorMessage = apiException.Message;
                    errorResponse.TechnicalErrorMessage = null;
                }
            }
            else if (exception is ExpectedException primaveraException)
            {
                errorResponse.StatusCode = HttpStatusCode.BadRequest;

                errorResponse.ErrorMessage = primaveraException.Message;
                errorResponse.TechnicalErrorMessage = null;
            }

            return errorResponse;
        }

        public static HttpResponseMessage ConstruirHttpResponseMessageDeExcecao(Exception exception, HttpRequestMessage request)
        {
            ErrorResponse errorResponse = ConstruirRespostaDeExcecao(exception);
            
            return errorResponse.ConstruirHttpResponseMessage(request);
        }
    }
}