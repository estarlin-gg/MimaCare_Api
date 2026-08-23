namespace MimaCare.Application.Common
{
    public class PasswordResetOptions
    {
        /// <summary>
        /// When true, the forgot-password endpoint returns the reset token in the response
        /// (useful when there is no email provider configured). Only enable in Development.
        /// </summary>
        public bool ExposeResetToken { get; set; }
    }
}
