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
        ///   Looks up a localized string similar to &lt;div id=&quot;menu-container&quot; name=&quot;menu-container&quot;&gt;
        ///	&lt;div id=&quot;app-side-menu-border&quot; name=&quot;left-menu-border&quot;&gt;&lt;/div&gt;
        ///	&lt;div id=&quot;app-side-menu&quot; name=&quot;left-menu&quot;&gt;
        ///		&lt;div data-position-index=&quot;0&quot; name=&quot;left-menu-icon&quot; class=&quot;row&quot;&gt;
        ///			&lt;i class=&quot;bi bi-three-dots&quot;&gt;&lt;/i&gt;
        ///		&lt;/div&gt;
        ///		&lt;div data-position-index=&quot;1&quot; name=&quot;left-menu-home&quot; class=&quot;row&quot;&gt;
        ///			&lt;a href=&quot;/home&quot;&gt;
        ///				&lt;i class=&quot;bi bi-house&quot; title=&quot;Home&quot;&gt;&lt;/i&gt;
        ///			&lt;/a&gt;
        ///		&lt;/div&gt;
        ///		&lt;div id=&quot;my-account-parent-option&quot; data-position-index=&quot;10&quot; name=&quot;left-menu-account&quot; c [rest of string was truncated]&quot;;.
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
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;main role=&quot;main&quot; class=&quot;inner cover&quot;&gt;
        ///	&lt;div class=&quot;card&quot;&gt;
        ///		&lt;div class=&quot;card-body&quot;&gt;
        ///			&lt;h5 class=&quot;card-title&quot;&gt;Logout&lt;/h5&gt;
        ///			&lt;p&gt;You have been successfully logged out of application.&lt;/p&gt;
        ///		&lt;/div&gt;
        ///	&lt;/div&gt;
        ///&lt;/main&gt;.
        /// </summary>
        internal static string logout_page {
            get {
                return ResourceManager.GetString("logout_page", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///&lt;table automationId=&quot;search-history-table&quot; name=&quot;search-dt-table&quot;&gt;
        ///	&lt;colgroup&gt;
        ///		&lt;col name=&quot;requested-date&quot; /&gt;
        ///		&lt;col name=&quot;state-abbr&quot; style=&quot;width: 60px&quot; /&gt;
        ///		&lt;col name=&quot;county-name&quot; style=&quot;width: 150px&quot; /&gt;
        ///		&lt;col name=&quot;begin-date&quot; style=&quot;width: 95px&quot; /&gt;
        ///		&lt;col name=&quot;ending-date&quot; style=&quot;width: 95px&quot; /&gt;
        ///		&lt;col name=&quot;search-status&quot; style=&quot;width: 90px&quot; /&gt;
        ///	&lt;/colgroup&gt;
        ///	&lt;thead&gt;
        ///		&lt;tr&gt;
        ///			&lt;th style=&quot;padding-left: 3px;&quot;&gt;Request Date&lt;/th&gt;
        ///			&lt;th style=&quot;padding-left: 3px;&quot;&gt;State&lt;/th&gt;
        ///			&lt;th style=&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string search_table_layout {
            get {
                return ResourceManager.GetString("search_table_layout", resourceCulture);
            }
        }
    }
}
