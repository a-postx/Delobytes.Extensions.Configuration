namespace Delobytes.Extensions.Configuration.Tests;

public class AppSecretsValidator : IValidateOptions<AppSecrets>
{
    public ValidateOptionsResult Validate(string name, AppSecrets options)
    {
        List<string> failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.TestString))
        {
            failures.Add($"{nameof(options.TestString)} secret is not found.");
        }

        if (options.TestInt == null)
        {
            failures.Add($"{nameof(options.TestInt)} secret is not found.");
        }

        if (options.TestBool == null)
        {
            failures.Add($"{nameof(options.TestBool)} secret is not found.");
        }

        if (options.TestGuid == null)
        {
            failures.Add($"{nameof(options.TestGuid)} secret is not found.");
        }

        if (options.TestObject == null)
        {
            failures.Add($"{nameof(options.TestObject)} secret is not found.");
        }

        if (options.TestObject?.TestString == null)
        {
            failures.Add($"{nameof(options.TestObject.TestString)} secret is not found.");
        }

        if (failures.Count > 0)
        {
            return ValidateOptionsResult.Fail(failures);
        }
        else
        {
            return ValidateOptionsResult.Success;
        }
    }
}

