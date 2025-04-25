using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Storage;

namespace SemesterProject.Utilities
{
    public static class NotePreferencesManager
    {
        private const string SortOrderKey = "SortOrder";
        private const string SearchFilterKey = "SearchFilter";
        private const string ThemePreferenceKey = "ThemePreference";

        // Save the sort order preference
        public static void SaveSortOrder(string sortOrder)
        {
            Preferences.Set(SortOrderKey, sortOrder);
        }

        // Retrieve the sort order preference
        public static string GetSortOrder()
        {
            return Preferences.Get(SortOrderKey, "Alphabetical A-Z"); // Default to "Alphabetical A-Z"
        }

        // Save the search filter preference
        public static void SaveSearchFilter(string filter)
        {
            Preferences.Set(SearchFilterKey, filter);
        }

        // Retrieve the search filter preference
        public static string GetSearchFilter()
        {
            return Preferences.Get(SearchFilterKey, "Title"); // Default to "Title"
        }

        // Save the theme preference
        public static void SaveThemePreference(string theme)
        {
            Preferences.Set(ThemePreferenceKey, theme);
        }

        // Retrieve the theme preference
        public static string GetThemePreference()
        {
            return Preferences.Get(ThemePreferenceKey, "Light"); // Default to "Light"
        }
    }
}
