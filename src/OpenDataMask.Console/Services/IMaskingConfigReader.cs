using OpenDataMask.Console.Models;

namespace OpenDataMask.Console.Services
{
    public interface IMaskingConfigReader
    {
        MaskingConfig Read(string path);
    }
}
