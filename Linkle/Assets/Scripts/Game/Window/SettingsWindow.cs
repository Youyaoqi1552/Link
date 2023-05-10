using Common.UI;
using Game.Event;
using Game.Scene;

namespace Game.Window
{
    public class SettingsWindow : UIWindow
    {
        public void OnQuitPressed()
        {
            CloseSelf(true);
            SceneNavigator.Shared.GotoHome();
        }
        
        public void OnBackPressed()
        {
            CloseSelf();
            GlobalEvent.InvokeGamePauseEvent(false);
        }
        
        public void OnMusicPressed()
        {
        }
        
        public void OnSFXPressed()
        {
        }
    }
}
