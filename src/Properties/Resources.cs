using System.ComponentModel;

namespace Rewst.RemoteAgent.Properties
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

        [System.ComponentModel.EditorBrowsable((EditorBrowsableState)2)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (Resources.resourceMan == null)
                {
                    Resources.resourceMan =
                        new System.Resources.ResourceManager(
                            "rewst.Resources",
                            typeof(Resources).Assembly
                        );
                }
                return Resources.resourceMan;
            }
        }

        [System.ComponentModel.EditorBrowsable((EditorBrowsableState)2)]
        internal static System.Globalization.CultureInfo Culture
        {
            get { return Resources.resourceCulture; }
            set { Resources.resourceCulture = value; }
        }

        internal static System.Drawing.Bitmap favicon
        {
            get
            {
                return (System.Drawing.Bitmap)
                    Resources.ResourceManager.GetObject(
                        "favicon",
                        Resources.resourceCulture
                    );
            }
        }

        internal static byte[] AgentService
        {
            get
            {
                return (byte[])
                Resources.ResourceManager.GetObject("Rewst AgentService",Resources.resourceCulture);
            }
        }

        private static System.Resources.ResourceManager? resourceMan;

        private static System.Globalization.CultureInfo? resourceCulture;
    }
}
