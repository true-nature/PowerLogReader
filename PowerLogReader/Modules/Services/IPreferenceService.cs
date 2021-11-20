namespace PowerLogReader.Modules.Services
{
    public interface IPreferenceService
    {
        Preference Preference { get;set;}
        void Save();
    }
}
