using System.Text.RegularExpressions;

namespace Job_Portal_Website.Helpers
{
    /// <summary>
    /// Shared input validation for US-01 (Job Seeker registration),
    /// US-03 (Employer registration) and US-05 (Login).
    ///
    /// These rules live in one place so that the Job Seeker form, the Employer
    /// form and the Login form cannot drift apart — the Sprint 1 review found
    /// that employer registration was missing checks the job seeker form had.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// The exact wording required by the US-01 / US-03 "Password Constraint"
        /// acceptance tests.
        /// </summary>
        public const string PasswordRequirementMessage =
            "Password must contain at least 8 characters, including uppercase, " +
            "lowercase, number and special character.";

        /// <summary>
        /// The exact wording required by the "Email format validation"
        /// acceptance tests for US-01, US-03 and US-05.
        /// </summary>
        public const string EmailFormatMessage = "Please enter a valid email address.";

        // Requires: at least one character before the @, exactly one @, at least
        // one character after it, then a dot and a top level domain of 2+ letters.
        //
        // This deliberately rejects every example listed in the acceptance tests:
        //   "abc"             - no @ at all
        //   "abcgmail.com"    - no @ at all
        //   "abc@"            - nothing after the @
        //   "@gmail.com"      - nothing before the @
        //   "abc@gmail"       - no dot + TLD in the domain
        //   "abc@@gmail.com"  - the part after the first @ may not contain another @
        //
        // Note: the built-in [EmailAddress] data annotation accepts "abc@gmail",
        // so it cannot be used to satisfy these tests.
        private static readonly Regex EmailPattern =
            new(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return EmailPattern.IsMatch(email.Trim());
        }

        /// <summary>
        /// Password must be at least 8 characters and contain an uppercase
        /// letter, a lowercase letter, a digit and a special character.
        /// </summary>
        public static bool IsValidPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;

            // A special character is anything that is not a letter, not a digit
            // and not whitespace (a space on its own does not count).
            if (!password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))) return false;

            return true;
        }

        /// <summary>
        /// Emails are stored and compared in a single normalised form so that
        /// "ABC@Gmail.com" and "abc@gmail.com" can never become two accounts.
        /// </summary>
        public static string NormaliseEmail(string? email)
        {
            return (email ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}
