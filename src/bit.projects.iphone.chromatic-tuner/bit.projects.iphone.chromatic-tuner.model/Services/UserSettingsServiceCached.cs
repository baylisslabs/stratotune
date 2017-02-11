using System;

namespace bit.projects.iphone.chromatictuner.model
{
    public class UserSettingsServiceCached : IUserSettingsService
    {
        IUserSettingsService _service;
        UserSettings[] _retrieveAllData;
        UserSettings _retrieveCurrentData;
        int? _retrieveCurrentId;
        SystemConfig _getSystemConfigData;

        public UserSettingsServiceCached (IUserSettingsService service)
        {
            _service = service;
        }

        #region IUserSettingsService implementation

        public bool Configure ()
        {
            invalidate();
            return _service.Configure();
        }

        public UserSettings[] RetrieveAll ()
        {
            return getOrLoad(ref _retrieveAllData,()=>_service.RetrieveAll());
        }

        public UserSettings RetrieveCurrent ()
        {
            return getOrLoad(ref _retrieveCurrentData,()=> _service.RetrieveCurrent());
        }

        public int RetrieveCurrentId ()
        {
            return getOrLoad(ref _retrieveCurrentId,()=> _service.RetrieveCurrentId()).Value;
        }

        public bool SetCurrent (int id)
        {
            invalidate();
            return _service.SetCurrent(id);
        }

        public UserSettings Add (string name)
        {
            invalidate();
            return _service.Add(name);
        }

        public bool Delete (int id, out int newCurrentSettingId)
        {
            invalidate();
            return _service.Delete(id, out newCurrentSettingId);
        }

        public bool Update (UserSettings settings)
        {
            invalidate();
            return _service.Update(settings);
        }

        public SystemConfig GetSystemConfig ()
        {
            return getOrLoad(ref _getSystemConfigData,()=>_service.GetSystemConfig());
        }

        public bool SetHeadPhoneAlertDisabled(bool value)
        {
            invalidate();
            return _service.SetHeadPhoneAlertDisabled(value);
        }

		public AppFeedback GetAppFeedback()
		{
			return _service.GetAppFeedback ();
		}

		public bool UpdateAppFeedback(AppFeedback appFeedback)
		{
			return _service.UpdateAppFeedback (appFeedback);
		}

        #endregion

        private void invalidate()
        {
            _retrieveAllData = null;
            _retrieveCurrentData = null;
            _retrieveCurrentId = null;
            _getSystemConfigData = null;
        }

        private static T getOrLoad<T> (ref T cached, Func<T> serviceCall)
        {
            if (cached == null) {
                cached = serviceCall();
            }
            return cached;
        }
    }
}

