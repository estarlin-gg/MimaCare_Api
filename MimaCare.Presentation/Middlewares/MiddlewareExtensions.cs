namespace MimaCare.Presentation.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder
      UseExceptionMiddleware(
          this IApplicationBuilder app)
        {
            return app.UseMiddleware<
                ExceptionMiddleware>();
        }
    }
}
