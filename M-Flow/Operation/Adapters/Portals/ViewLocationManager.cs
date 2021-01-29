using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace MFlow.Operation.Adapters.Portals
{
    /// <summary>
    /// Manages the view locations.
    /// </summary>
    static class ViewLocationManager
    {
        #region Fields

        static readonly Dictionary<Type, Point> ViewStates = new();

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified view to the management. 
        /// </summary>
        /// <param name="view">The view.</param>
        public static void Add(Window view)
        {
            SetupViewLocation(view);
            view.Closing += View_Closing;
        }

        /// <summary>
        /// Setups the location of the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        static void SetupViewLocation(Window view)
        {
            var defaultLocation = GetDefaultLocation(view);
            var location = DetermineViewLocation(view, defaultLocation);
            ApplyLocation(view, location);
        }

        /// <summary>
        /// Stores the location of the view.
        /// </summary>
        /// <param name="view">The view.</param>
        static void StoreViewLocation(Window view)
        {
            var key = view.GetType();
            var value = new Point(view.Left, view.Top);
            if (ViewStates.ContainsKey(key))
                ViewStates[key] = value;
            else
                ViewStates.Add(key, value);
        }

        /// <summary>
        /// Determines the location of the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="defaultLocation">The default location if no stored location could be found.</param>
        /// <returns>The location.</returns>
        static Point DetermineViewLocation(Window view, Point defaultLocation)
        {
            return ViewStates.TryGetValue(view.GetType(), out var location) 
                ? location 
                : defaultLocation;
        }
        
        /// <summary>
        /// Applies the location to the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="location">The location.</param>
        static void ApplyLocation(Window view, Point location)
        {
            view.Left = location.X;
            view.Top = location.Y;
        }

        /// <summary>
        /// Gets the default location on the right bottom of the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The point.</returns>
        static Point GetDefaultLocation(Window view)
        {
            var (screenWidth, screenHeight) = GetScreenSize();
            var left = screenWidth - view.Width - 5;
            var top = screenHeight - view.Height - 45;
            return new Point(left, top);
        }

        /// <summary>
        /// Gets the screen size.
        /// </summary>
        /// <returns>The width and height of teh screen.</returns>
        static Tuple<double, double> GetScreenSize()
        {
            return Tuple.Create(
                SystemParameters.PrimaryScreenWidth, 
                SystemParameters.PrimaryScreenHeight);
        }

        /// <summary>
        /// Handles the closing event of a view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        static void View_Closing(object sender, CancelEventArgs e)
        {
            if (!(sender is Window view))
                return;
            
            StoreViewLocation(view);
            view.Closing -= View_Closing;
        }

        #endregion
    }
}