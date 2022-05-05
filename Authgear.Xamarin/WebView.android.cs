using Android.Content;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using AndroidX.Browser.CustomTabs;
using AndroidX.Fragment.App;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Authgear.Xamarin
{
    internal class WebView : IWebView
    {
        private const string HookFragmentTagFormat = "hookFragment.{0}";
        private class HookFragment : Fragment
        {
            public WeakReference<WebView> Owner { get; set; }
            public TaskCompletionSource<object> TaskSource { get; set; }

            public bool IsWaiting { get; set; }

            public override void OnPause()
            {
                base.OnPause();
                if (!IsAlive()) { return; }
            }

            public override void OnResume()
            {
                base.OnResume();
                if (!IsAlive()) { return; }
                if (!IsWaiting) { return; }
                IsWaiting = false;
                if (TaskSource == null)
                {
                    throw new ArgumentNullException(nameof(TaskSource));
                }
                TaskSource.SetResult(null);
            }
            /// <summary>
            /// Check whether the owner web view (i.e. authgear) is still alive, and remove self if not.
            /// This is needed becoz finalizer is not recommended in C# so we couldn't afford to remove the fragment
            /// in this class's (i.e. authgear's) finalizer.
            /// </summary>
            /// <returns>Whether the fragment should continue to do work.</returns>
            private bool IsAlive()
            {
                if (Owner == null)
                {
                    throw new ArgumentException(nameof(Owner));
                }
                if (!Owner.TryGetTarget(out _))
                {
                    if (!(Activity is AppCompatActivity activity)) { return false; }
                    activity.SupportFragmentManager.BeginTransaction().Remove(this).CommitAllowingStateLoss();
                    return false;
                }
                return true;
            }
        }
        private readonly Guid TagGuid;
        public WebView()
        {
            TagGuid = Guid.NewGuid();
            _ = FindOrCreateFragment();
        }
        private HookFragment FindOrCreateFragment()
        {
            if (!(Platform.CurrentActivity is AppCompatActivity activity))
            {
                Debug.WriteLine("Calling ShowAsync or initiating authgear without a valid activity in use, fragment setup ignored.");
                return null;
            }
            var fragment = activity.SupportFragmentManager.FindFragmentByTag(string.Format(HookFragmentTagFormat, TagGuid)) as HookFragment;
            if (fragment != null) { return fragment; }
            fragment = new HookFragment
            {
                Owner = new WeakReference<WebView>(this)
            };
            activity.SupportFragmentManager.BeginTransaction().Add(fragment, string.Format(HookFragmentTagFormat, TagGuid)).CommitNow();
            return fragment;
        }
        public async Task ShowAsync(string url)
        {
            var taskSource = new TaskCompletionSource<object>();
            var fragment = FindOrCreateFragment();
            if (fragment == null) { return; }
            if (fragment.IsWaiting)
            {
                Debug.WriteLine("Calling ShowAsync when a pending ShowAsync is in progress, ignored.");
                return;
            }
            fragment.TaskSource = taskSource;
            fragment.IsWaiting = true;
            var intent = new CustomTabsIntent.Builder()
                .Build();
            intent.LaunchUrl(Platform.CurrentActivity, Android.Net.Uri.Parse(url));
            await taskSource.Task;
        }
    }
}
