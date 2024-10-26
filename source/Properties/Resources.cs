using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Rewst.RemoteAgent.Calvindd2f.Properties
{
    [
        System.CodeDom.Compiler.GeneratedCode(
            "System.Resources.Tools.StronglyTypedResourceBuilder",
            "16.0.0.0"
        ),
        System.Diagnostics.DebuggerNonUserCode,
        System.Runtime.CompilerServices.CompilerGenerated
    ]
    internal class Resources
    {
        internal Resources() { }

        [System.ComponentModel.EditorBrowsable(2)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (Rewst.RemoteAgent.Calvindd2f.Properties.Resources.resourceMan == null)
                {
                    Rewst.RemoteAgent.Calvindd2f.Properties.Resources.resourceMan =
                        new System.Resources.ResourceManager(
                            "rewst.Rewst.RemoteAgent.Calvindd2f.Properties.Resources",
                            typeof(Rewst.RemoteAgent.Calvindd2f.Properties.Resources).Assembly
                        );
                }
                return Rewst.RemoteAgent.Calvindd2f.Properties.Resources.resourceMan;
            }
        }

        [System.ComponentModel.EditorBrowsable(2)]
        internal static System.Globalization.CultureInfo Culture
        {
            get { return Rewst.RemoteAgent.Calvindd2f.Properties.Resources.resourceCulture; }
            set { Rewst.RemoteAgent.Calvindd2f.Properties.Resources.resourceCulture = value; }
        }

        internal static System.Drawing.Bitmap favicon
        {
            get
            {
                return (System.Drawing.Bitmap)
                    Rewst.RemoteAgent.Calvindd2f.Properties.Resources.ResourceManager.GetObject(
                        "favicon",
                        Rewst.RemoteAgent.Calvindd2f.Properties.Resources.resourceCulture
                    );
            }
        }

        internal static byte[] AgentService
        {
            get
            {
                return (byte[])
                    Rewst.RemoteAgent.Calvindd2f.Properties.Resources.ResourceManager.GetObject(
                        " AgentService",
                        Rewst.RemoteAgent.Calvindd2f.Properties.Resources.resourceCulture
                    );
            }
        }

        private static System.Resources.ResourceManager resourceMan;

        private static System.Globalization.CultureInfo resourceCulture;
    }
}
