using System;

namespace bit.projects.iphone.chromatictuner.model
{
    public interface IUserSettingsService
    {
        bool Configure();
        UserSettings[] RetrieveAll();
        UserSettings RetrieveCurrent();
        int RetrieveCurrentId();
        bool SetCurrent(int id);
        UserSettings Add(string name);
        bool Delete(int id, out int newCurrentSettingId);
        bool Update(UserSettings settings);
        SystemConfig GetSystemConfig();
        bool SetHeadPhoneAlertDisabled(bool value);
		AppFeedback GetAppFeedback();
		bool UpdateAppFeedback(AppFeedback appFeedback);
    }
}

