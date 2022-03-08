using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms.View.Controls
{
    public abstract class PausableContentPage : ContentPage
    {
        public abstract void Pause();
        public abstract void Resume();
    }
}
