using System.Text.Json;
using FluentValidation;
using MimaCare.Application.Common;
using MimaCare.Application.Common.Exceptions;

namespace MimaCare.Presentation.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);

            await HandleStatusCodes(context);
        }

        // =====================================================
        // FLUENT VALIDATION
        // =====================================================

        catch (FluentValidation.ValidationException ex)
        {
            var errors = ex.Errors
                .Select(x => new
                {
                    Property = x.PropertyName,

                    Error = x.ErrorMessage
                });

            await WriteResponse(
                context,
                400,
                false,
                "Validation error.",
                errors);
        }

        // =====================================================
        // BAD REQUEST
        // =====================================================

        catch (BadRequestException ex)
        {
            await WriteResponse(
                context,
                400,
                false,
                ex.Message);
        }

        // =====================================================
        // UNAUTHORIZED
        // =====================================================

        catch (UnauthorizedException ex)
        {
            await WriteResponse(
                context,
                401,
                false,
                ex.Message);
        }

        // =====================================================
        // FORBIDDEN
        // =====================================================

        catch (ForbiddenException ex)
        {
            await WriteResponse(
                context,
                403,
                false,
                ex.Message);
        }

        // =====================================================
        // NOT FOUND
        // =====================================================

        catch (NotFoundException ex)
        {
            await WriteResponse(
                context,
                404,
                false,
                ex.Message);
        }

        // =====================================================
        // CONFLICT
        // =====================================================

        catch (ConflictException ex)
        {
            await WriteResponse(
                context,
                409,
                false,
                ex.Message);
        }

        // =====================================================
        // GENERAL ERROR
        // =====================================================

        catch (Exception ex)
        {
            await WriteResponse(
                context,
                500,
                false,
                ex.Message);
        }
    }

    // =====================================================
    // HANDLE STATUS CODES
    // =====================================================

    private static async Task HandleStatusCodes(
        HttpContext context)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        switch (context.Response.StatusCode)
        {
            case 401:

                await WriteResponse(
                    context,
                    401,
                    false,
                    "Unauthorized.");

                break;

            case 403:

                await WriteResponse(
                    context,
                    403,
                    false,
                    "Forbidden.");

                break;

            case 404:

                await WriteResponse(
                    context,
                    404,
                    false,
                    "Resource not found.");

                break;
        }
    }

    // =====================================================
    // RESPONSE WRITER
    // =====================================================

    private static async Task WriteResponse(
        HttpContext context,
        int statusCode,
        bool success,
        string message,
        object? data = null)
    {
        context.Response.ContentType =
            "application/json";

        context.Response.StatusCode =
            statusCode;

        var response =
            new ApiResponse<object>
            {
                StatusCode = statusCode,

                Success = success,

                Message = message,

                Data = data
            };

        var json =
            JsonSerializer.Serialize(response);

        await context.Response
            .WriteAsync(json);
    }
}