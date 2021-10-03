namespace MonoGameRPG.Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {
        private readonly MenuEntry controlsMenu;
        private readonly MenuEntry audioMenu;

        public OptionsMenuScreen() : base("Options")
        {
            controlsMenu = new MenuEntry(string.Empty);
            audioMenu= new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            controlsMenu.Selected += ControlsMenuEntrySelected;
            audioMenu.Selected += AudioMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(controlsMenu);
            MenuEntries.Add(audioMenu);
            MenuEntries.Add(back);
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            controlsMenu.Text = "Controls";
            audioMenu.Text = "Audio";
        }

        private void ControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuEntryText();
        }

        private void AudioMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuEntryText();
        }
    }
}