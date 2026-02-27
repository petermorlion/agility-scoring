using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace AgilityScoring.Maui.Platforms.Android;

// Uses the reverse client ID URI scheme, which Google supports for native Android OAuth.
// Format: com.googleusercontent.apps.{client-id}:/oauth2redirect
[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "com.googleusercontent.apps.650791542042-6ub4916cfv65tt54566ecedu6qkaqgti",
    DataPath = "/oauth2redirect")]
public class WebAuthCallbackActivity : WebAuthenticatorCallbackActivity
{
}
