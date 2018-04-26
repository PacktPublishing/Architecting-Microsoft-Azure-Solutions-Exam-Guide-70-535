using Owin;
using System;
using System.Collections.Generic;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(PacktB2CWebApp.Startup))]

namespace PacktB2CWebApp
{
    public partial class Startup
    {
        // The OWIN middleware will invoke this method when the app starts
        public void Configuration(IAppBuilder app)
        {
            // ConfigureAuth defined in other part of the class
            ConfigureAuth(app);
        }
    }
}