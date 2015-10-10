namespace BackMeUp.Services.Configuration
{
    public interface IConfigurationReader<out T>
    {
        T Read(string configurationXml);
    }
}