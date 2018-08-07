namespace Sitecore.Support.sitecore.login
{
  using Sitecore.DependencyInjection;
  using Sitecore.Web;
  using Sitecore.Web.Authentication;
  using System;

  /// <summary>
  /// </summary>
  [AllowDependencyInjection]
  public partial class Default : Sitecore.sitecore.login.Default
  {
    string currentStartUrl;

    new protected void LoginClicked(object sender, EventArgs e)
    {
      if (this.LoggingIn() && this.Login())
      {
        currentStartUrl = String.Empty;
        this.LoggedIn();
        if (!DomainAccessGuard.GetAccess())
        {
          this.LogMaxEditorsExceeded();
          currentStartUrl = "/sitecore/client/Applications/LicenseOptions/StartPage";
        }
        else
        {
          var startUrlField = typeof(Sitecore.sitecore.login.Default).GetField("startUrl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          if (startUrlField != null)
          {
            try
            {
              currentStartUrl = (string)startUrlField.GetValue(this);

              if (!string.IsNullOrEmpty(currentStartUrl))
              {

                var currentHostName = WebUtil.GetHostName();
                var hostNameStart = currentStartUrl.IndexOf(currentHostName);

                if (hostNameStart != -1 && currentHostName.Length > 0)
                {
                  var relativeUrlStart = currentStartUrl.IndexOf("/", hostNameStart + currentHostName.Length);

                  if (relativeUrlStart == -1)
                    relativeUrlStart = hostNameStart + currentHostName.Length;

                  startUrlField.SetValue(this, currentStartUrl.Substring(relativeUrlStart));
                  currentStartUrl = currentStartUrl.Substring(relativeUrlStart);
                }
              }
            }
            catch (Exception ex)
            {
              Log.Warn("Sitecore.Support.225312: ", ex, this);
            }
          }
        }
        WebUtil.Redirect(currentStartUrl);
      }
    }

    private void LogMaxEditorsExceeded()
    {
      string format = "The maximum number of simultaneously active (logged-in) editors exceeded. The User {0} cannot be logged in to the system. The maximum of editors allowed by license is {1}.";
      this.Log.Warn(string.Format(format, WebUtil.HandleFullUserName(this.UserName.Text), DomainAccessGuard.MaximumSessions), this);
    }





  }
}