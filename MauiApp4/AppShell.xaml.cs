namespace MauiApp4
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (Application.Current.UserAppTheme == AppTheme.Light)
                Application.Current.UserAppTheme = AppTheme.Dark;

            else
                Application.Current.UserAppTheme = AppTheme.Light;
        }
    }
}
