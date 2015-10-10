namespace BackMeUp.Data.Services
{
    public interface IConfigurationReader<out T>
    {
        T Read(string configurationXml);
    }
}