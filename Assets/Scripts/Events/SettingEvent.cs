using System;

namespace Events
{
    public class SettingEvent
    {
        public event Action<float, bool> LookSettingsChanged;
        
        public void InvokeLookSettingsChanged(float lookSensitivity, bool invertY) 
            => LookSettingsChanged?.Invoke(lookSensitivity, invertY);
    }
}
