using System;

namespace Settings
{
    [Serializable]
    public class SettingsData
    {
        public float lookSensitivity;
        public bool invertY;
        
        public float brightness;
        public bool fullScreen;
        public int resolutionWidth;
        public int resolutionHeight;
        
        public float masterVolume;
        public float effectsVolume;
        public float musicVolume;
    }
}
