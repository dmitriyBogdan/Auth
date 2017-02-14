namespace Auth.BLL.Interfaces
{
    public interface ICrypto
    {
        string GenerateSalt();

        string ComputeHash(string text, string salt);
    }
}