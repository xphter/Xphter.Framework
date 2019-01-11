using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Web.WebPages;

namespace Xphter.Framework.Web.Mvc {
    internal abstract class ModularViewEngine : BuildManagerViewEngine {
        protected ModularViewEngine()
            : base() {
        }

        protected ModularViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator) {
        }

        // format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{areaName}:{moduleName}:"
        private const string CacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}:";
        private const string CacheKeyPrefixMaster = "Master";
        private const string CacheKeyPrefixPartial = "Partial";
        private const string CacheKeyPrefixView = "View";
        private static readonly string[] g_emptyLocations = new string[0];

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ModuleAreaMasterLocationFormats {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ModuleAreaPartialViewLocationFormats {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ModuleAreaViewLocationFormats {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ModuleMasterLocationFormats {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ModulePartialViewLocationFormats {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ModuleViewLocationFormats {
            get;
            set;
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            if(controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }
            if(String.IsNullOrEmpty(partialViewName)) {
                throw new ArgumentException("Value cannot be null or empty.", "partialViewName");
            }

            string[] searched;
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string partialPath = this.GetPath(controllerContext, this.PartialViewLocationFormats, this.AreaPartialViewLocationFormats, this.ModulePartialViewLocationFormats, this.ModuleAreaPartialViewLocationFormats, "PartialViewLocationFormats", partialViewName, controllerName, CacheKeyPrefixPartial, useCache, out searched);

            if(String.IsNullOrEmpty(partialPath)) {
                return new ViewEngineResult(searched);
            }

            return new ViewEngineResult(this.CreatePartialView(controllerContext, partialPath), this);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            if(controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }
            if(String.IsNullOrEmpty(viewName)) {
                throw new ArgumentException("Value cannot be null or empty.", "viewName");
            }

            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string viewPath = this.GetPath(controllerContext, this.ViewLocationFormats, this.AreaViewLocationFormats, this.ModuleViewLocationFormats, this.ModuleAreaMasterLocationFormats, "ViewLocationFormats", viewName, controllerName, CacheKeyPrefixView, useCache, out viewLocationsSearched);
            string masterPath = this.GetPath(controllerContext, this.MasterLocationFormats, this.AreaMasterLocationFormats, this.ModuleMasterLocationFormats, this.ModuleAreaMasterLocationFormats, "MasterLocationFormats", masterName, controllerName, CacheKeyPrefixMaster, useCache, out masterLocationsSearched);

            if(String.IsNullOrEmpty(viewPath) || (String.IsNullOrEmpty(masterPath) && !String.IsNullOrEmpty(masterName))) {
                return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
            }

            return new ViewEngineResult(this.CreateView(controllerContext, viewPath, masterPath), this);
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations, string[] moduleLocations, string[] moduleAreaLocations, string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations) {
            searchedLocations = g_emptyLocations;

            if(String.IsNullOrEmpty(name)) {
                return String.Empty;
            }

            string moduleName = AreaHelpers.GetModuleName(controllerContext.RouteData);
            string areaName = AreaHelpers.GetAreaName(controllerContext.RouteData);
            bool usingModules = !string.IsNullOrEmpty(moduleName);
            bool usingAreas = !String.IsNullOrEmpty(areaName);
            List<ViewLocation> viewLocations = GetViewLocations(
                locations,
                usingModules || !usingAreas ? null : areaLocations,
                !usingModules ? null : moduleLocations,
                !usingModules || !usingAreas ? null : moduleAreaLocations);

            if(viewLocations.Count == 0) {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "The property '{0}' cannot be null or empty.", locationsPropertyName));
            }

            bool nameRepresentsPath = IsSpecificPath(name);
            string cacheKey = this.CreateCacheKey(cacheKeyPrefix, name, (nameRepresentsPath) ? String.Empty : controllerName, areaName, moduleName);

            if(useCache) {
                // Only look at cached display modes that can handle the context.
                string cachedLocation = null;
                IEnumerable<IDisplayMode> possibleDisplayModes = DisplayModeProvider.GetAvailableDisplayModesForContext(controllerContext.HttpContext, controllerContext.DisplayMode);
                foreach(IDisplayMode displayMode in possibleDisplayModes) {
                    cachedLocation = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, this.AppendDisplayModeToCacheKey(cacheKey, displayMode.DisplayModeId));

                    if(cachedLocation != null) {
                        if(controllerContext.DisplayMode == null) {
                            controllerContext.DisplayMode = displayMode;
                        }
                        break;
                    }
                }

                // if cachedLocation is null GetPath will be called again without using the cache.
                return cachedLocation;
            } else {
                return (nameRepresentsPath) ?
                    this.GetPathFromSpecificName(controllerContext, name, cacheKey, ref searchedLocations) :
                    this.GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName, moduleName, cacheKey, ref searchedLocations);
            }


        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string areaName, string moduleName) {
            return String.Format(CultureInfo.InvariantCulture, CacheKeyFormat,
                GetType().AssemblyQualifiedName, prefix, name, controllerName, areaName, moduleName);
        }

        private string AppendDisplayModeToCacheKey(string cacheKey, string displayMode) {
            // key format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{areaName}:{moduleName}:"
            // so append "{displayMode}:" to the key
            return cacheKey + displayMode + ":";
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations, string name, string controllerName, string areaName, string moduleName, string cacheKey, ref string[] searchedLocations) {
            string result = String.Empty;
            searchedLocations = new string[locations.Count];

            string virtualPath = null;
            ViewLocation location = null;
            string resolvedVirtualPath = null;
            DisplayInfo virtualPathDisplayInfo = null;
            for(int i = 0; i < locations.Count; i++) {
                location = locations[i];
                virtualPath = location.Format(name, controllerName, areaName, moduleName);
                virtualPathDisplayInfo = this.DisplayModeProvider.GetDisplayInfoForVirtualPath(virtualPath, controllerContext.HttpContext, path => FileExists(controllerContext, path), controllerContext.DisplayMode);

                if(virtualPathDisplayInfo != null) {
                    resolvedVirtualPath = virtualPathDisplayInfo.FilePath;

                    // save to view location cache
                    searchedLocations = g_emptyLocations;
                    result = resolvedVirtualPath;
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, this.AppendDisplayModeToCacheKey(cacheKey, virtualPathDisplayInfo.DisplayMode.DisplayModeId), result);

                    if(controllerContext.DisplayMode == null) {
                        controllerContext.DisplayMode = virtualPathDisplayInfo.DisplayMode;
                    }

                    // Populate the cache with the existing paths returned by all display modes.
                    // Since we currently don't keep track of cache misses, if we cache view.aspx on a request from a standard browser
                    // we don't want a cache hit for view.aspx from a mobile browser so we populate the cache with view.Mobile.aspx.
                    DisplayInfo displayInfoToCache = null;
                    IEnumerable<IDisplayMode> allDisplayModes = DisplayModeProvider.Modes;
                    foreach(IDisplayMode displayMode in allDisplayModes) {
                        if(displayMode.DisplayModeId != virtualPathDisplayInfo.DisplayMode.DisplayModeId) {
                            displayInfoToCache = displayMode.GetDisplayInfo(controllerContext.HttpContext, virtualPath, virtualPathExists: path => FileExists(controllerContext, path));

                            if(displayInfoToCache != null && displayInfoToCache.FilePath != null) {
                                ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, this.AppendDisplayModeToCacheKey(cacheKey, displayInfoToCache.DisplayMode.DisplayModeId), displayInfoToCache.FilePath);
                            }
                        }
                    }
                    break;
                }

                searchedLocations[i] = virtualPath;
            }

            return result;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations) {
            string result = name;

            if(!(this.FilePathIsSupported(name) && this.FileExists(controllerContext, name))) {
                result = String.Empty;
                searchedLocations = new[] { name };
            }

            this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
            return result;
        }

        private bool FilePathIsSupported(string virtualPath) {
            if(this.FileExtensions == null) {
                // legacy behavior for custom ViewEngine that might not set the FileExtensions property
                return true;
            } else {
                // get rid of the '.' because the FileExtensions property expects extensions withouth a dot.
                string extension = VirtualPathUtility.GetExtension(virtualPath).TrimStart('.');
                return this.FileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
            }
        }

        private static List<ViewLocation> GetViewLocations(string[] viewLocationFormats, string[] areaViewLocationFormats, string[] moduleViewLocationFormats, string[] moduleAreaViewLocationFormats) {
            List<ViewLocation> allLocations = new List<ViewLocation>();

            if(moduleAreaViewLocationFormats != null) {
                allLocations.AddRange(moduleAreaViewLocationFormats.Select((item) => new ModuleAreaAwareViewLocation(item)));
            }

            if(moduleViewLocationFormats != null) {
                allLocations.AddRange(moduleViewLocationFormats.Select((item) => new ModuleViewLocation(item)));
            }

            if(areaViewLocationFormats != null) {
                allLocations.AddRange(areaViewLocationFormats.Select((item) => new AreaAwareViewLocation(item)));
            }

            if(viewLocationFormats != null) {
                allLocations.AddRange(viewLocationFormats.Select((item) => new ViewLocation(item)));
            }

            return allLocations;
        }

        private static bool IsSpecificPath(string name) {
            char c = name[0];
            return (c == '~' || c == '/');
        }

        private class ViewLocation {
            public ViewLocation(string virtualPathFormatString) {
                this.m_virtualPathFormatString = virtualPathFormatString;
            }

            protected string m_virtualPathFormatString;

            public virtual string Format(string viewName, string controllerName, string areaName, string moduleName) {
                return string.Format(CultureInfo.InvariantCulture, this.m_virtualPathFormatString, viewName, controllerName);
            }

            public override string ToString() {
                return this.m_virtualPathFormatString;
            }
        }

        private class AreaAwareViewLocation : ViewLocation {
            public AreaAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString) {
            }

            public override string Format(string viewName, string controllerName, string areaName, string moduleName) {
                return string.Format(CultureInfo.InvariantCulture, this.m_virtualPathFormatString, viewName, controllerName, areaName);
            }
        }

        private class ModuleViewLocation : ViewLocation {
            public ModuleViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString) {
            }

            public override string Format(string viewName, string controllerName, string areaName, string moduleName) {
                return string.Format(CultureInfo.InvariantCulture, this.m_virtualPathFormatString, viewName, controllerName, moduleName);
            }
        }

        private class ModuleAreaAwareViewLocation : ViewLocation {
            public ModuleAreaAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString) {
            }

            public override string Format(string viewName, string controllerName, string areaName, string moduleName) {
                return string.Format(CultureInfo.InvariantCulture, this.m_virtualPathFormatString, viewName, controllerName, areaName, moduleName);
            }
        }
    }
}
