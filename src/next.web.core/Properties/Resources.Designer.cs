﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace next.web.core.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("next.web.core.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div name=&quot;left-menu&quot;&gt;
        ///	&lt;div name=&quot;left-menu-icon&quot; class=&quot;row&quot;&gt;
        ///		&lt;i class=&quot;bi bi-three-dots&quot;&gt;&lt;/i&gt;
        ///	&lt;/div&gt;
        ///&lt;/div&gt;.
        /// </summary>
        internal static string base_menu {
            get {
                return ResourceManager.GetString("base_menu", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;Permissions_API&quot;: &quot;http://api.legallead.co/&quot;,
        ///  &quot;api.permissions&quot;: {
        ///    &quot;destination&quot;: &quot;remote&quot;,
        ///    &quot;remote&quot;: &quot;http://api.legallead.co/&quot;,
        ///    &quot;local&quot;: &quot;https://localhost:44345/&quot;
        ///  },
        ///  &quot;stripe.payment&quot;: {
        ///    &quot;key&quot;: &quot;test&quot;,
        ///    &quot;names&quot;: {
        ///      &quot;test&quot;: &quot;sk_test_51LCZucDhgP60CL9xtS9RSEC5IhGsKkZwnNxwFwpHbDHLLBLvB87uICAJB5fVZNyqEHNpI9ZbLYwFFKXPNnbgucs200OOT3UvbK&quot;,
        ///      &quot;prod&quot;: &quot;sk_test_51LCZucDhgP60CL9xtS9RSEC5IhGsKkZwnNxwFwpHbDHLLBLvB87uICAJB5fVZNyqEHNpI9ZbLYwFFKXPNnbgucs200OOT3UvbK&quot;
        ///    } [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string core_configuration {
            get {
                return ResourceManager.GetString("core_configuration", resourceCulture);
            }
        }
    }
}
