namespace Toggles.Sources
{
    public abstract class SourceBase : ObservableBase<NewToggleValue>, IToggleSource
    {
        protected void NotifyObservers(string key, bool isSet)
        {
            Observers.ForEach(x => x.OnNext(new NewToggleValue
            {
                SourceType = GetType(),
                Key = key,
                Active = isSet,
            }));
        }

        protected void NotifyCompleted()
        {
            Observers.ForEach(x => x.OnCompleted());
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
