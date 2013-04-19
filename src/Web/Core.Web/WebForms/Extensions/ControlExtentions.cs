using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Core.Web.WebForms.Extensions
{
    /// <summary>
    /// Provides a set of static methods for querying ASP.NET server control
    /// that implement System.Web.UI.Control
    /// </summary>
    public static class ControlExtentions
    {
        /// <summary>
        /// Gets controls children of a control.
        /// </summary>
        /// <param name="control">The main control.</param>
        /// <returns>Returns a list of controls.</returns>
        private static IEnumerable<Control> GetChild(this Control control)
        {
            var children = control.Controls.Cast<Control>().ToList();
            return children.SelectMany(GetChild).Concat(children);
        }

        /// <summary>
        /// Gets the first parent by type of a control.
        /// </summary>
        /// <typeparam name="T">The type for the first parent.</typeparam>
        /// <param name="control">The main control.</param>
        /// <returns>This first parent control.</returns>
        private static Control GetFirstParentControl<T>(this Control control)
        {
            return control.Parent.GetType() == typeof(T) ? control.Parent : GetFirstParentControl<T>(control.Parent);
        }

        /// <summary>
        /// Gets child controls by type.
        /// </summary>
        /// <typeparam name="T">The type for the child controls.</typeparam>
        /// <param name="control">The main control.</param>
        /// <returns>A list of child controls of T type</returns>
        public static IList<T> FindChildControlsByType<T>(this Control control)
        {
            return GetChild(control).OfType<T>().ToList();
        }

        /// <summary>
        /// Gets the first parent by type of a control.
        /// </summary>
        /// <typeparam name="T">The type for the first parent.</typeparam>
        /// <param name="control">The main control.</param>
        /// <returns>This first parent control.</returns>
        public static T FindFirstParentControlByType<T>(this Control control) where T : Control
        {
            return GetFirstParentControl<T>(control) as T;
        }
    }
}