﻿using Azure.Core;
using HotelListingApi.Exceptions;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using KeyNotFoundException = HotelListingApi.Exceptions.KeyNotFoundException;
using NotImplementedException = HotelListingApi.Exceptions.NotImplementedException;
using UnauthorizedAccessException = HotelListingApi.Exceptions.UnauthorizedAccessException;

namespace HotelListingApi.Configurations
{
    public class GlobalExceptionHandlingMiddleWare
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionHandlingMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Inovke(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context , ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode Status;
            var stackTrace = string.Empty;
            var message = string.Empty;

            var exceptionType = ex.GetType();

            if (exceptionType == typeof(NotFoundException)) 
            {
                message = ex.Message;
                Status = HttpStatusCode.NotFound;
                stackTrace = ex.StackTrace;
            }

            else if( exceptionType == typeof(BadRequestException))
            {
                message = ex.Message;
                Status= HttpStatusCode.BadRequest;
                stackTrace = ex.StackTrace;
            }

            else if(exceptionType == typeof(NotImplementedException))
            {
                message = ex.Message;
                Status = HttpStatusCode.NotImplemented;
                stackTrace = ex.StackTrace;
            }
            else if(exceptionType == typeof(KeyNotFoundException))
            {
                message = ex.Message;
                Status = HttpStatusCode.NotFound;
                stackTrace = ex.StackTrace;
            }

            else if(exceptionType == typeof(UnauthorizedAccessException))
            {
                message = ex.Message;
                Status = HttpStatusCode.Unauthorized;
                stackTrace = ex.StackTrace;
            }
            else
            {
                message = ex.Message;
                Status = HttpStatusCode.InternalServerError;
                stackTrace = ex.StackTrace;
            }

            var exceptionResult = JsonSerializer.Serialize(new { error = message, stackTrace, });
            context.Response.ContentType= "application/json";
            context.Response.StatusCode = (int)Status;


            return context.Response.WriteAsync(exceptionResult);




        }
    }
}
