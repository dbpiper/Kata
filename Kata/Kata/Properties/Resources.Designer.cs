﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kata.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kata.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Cannot select Kata: No Kata Type selected yet. Please select a Kata Type from the dropdown above..
        /// </summary>
        internal static string String_Error_NoKatTypeSelected {
            get {
                return ResourceManager.GetString("String_Error_NoKatTypeSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kata Type.
        /// </summary>
        internal static string String_KataType {
            get {
                return ResourceManager.GetString("String_KataType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only Choose From Specific Lesson.
        /// </summary>
        internal static string String_OnlyOneLesson {
            get {
                return ResourceManager.GetString("String_OnlyOneLesson", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Partial taxonomy selected for species sub-exercies, please enter more information to continue and press &quot;Resume Selection&quot; to continue..
        /// </summary>
        internal static string String_PartialTaxonomySelected {
            get {
                return ResourceManager.GetString("String_PartialTaxonomySelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Selected Kata.
        /// </summary>
        internal static string String_SelectedKata {
            get {
                return ResourceManager.GetString("String_SelectedKata", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: could not update Species.yaml!.
        /// </summary>
        internal static string String_SelectSpeciesError_CouldNotUpdateSpeciesYaml {
            get {
                return ResourceManager.GetString("String_SelectSpeciesError_CouldNotUpdateSpeciesYaml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error, incorrect lesson!.
        /// </summary>
        internal static string String_SelectSpeciesError_IncorrectLesson {
            get {
                return ResourceManager.GetString("String_SelectSpeciesError_IncorrectLesson", resourceCulture);
            }
        }
    }
}
