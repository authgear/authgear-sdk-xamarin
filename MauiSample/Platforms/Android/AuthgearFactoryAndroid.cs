using Android.Content;
using Authgear.Xamarin;
using AuthgearSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiSample
{
    internal class AuthgearFactoryAndroid : IAuthgearFactory
    {
        private readonly Context _context;
        public AuthgearFactoryAndroid(Context context)
        {
            _context = context;
        }

        public AuthgearSdk CreateAuthgear(AuthgearOptions options)
        {
            return new AuthgearSdk(_context, options);
        }
    }
}
