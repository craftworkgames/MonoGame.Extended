// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Tiled;

#if MONOGAME_385_OR_NEWER && !KNI && !FNA
namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    /// <summary>
    /// Provides helper methods for processing Tiled map content.
    /// </summary>
    public static class TiledMapContentHelper
    {
        /// <summary>
        /// Processes a Tiled map object, including template resolution.
        /// </summary>
        /// <param name="obj">The object to process.</param>
        /// <param name="context">The processor context.</param>
        public static void Process(TiledMapObjectContent obj, ContentProcessorContext context)
        {
            if (string.IsNullOrWhiteSpace(obj.TemplateSource))
                return;

            // Build and load the template
            var template = TiledContentBuildHelpers.BuildAndLoadTemplate(context, obj.TemplateSource);

            if (template?.Object == null)
                return;

            // Process the template's object (templates can reference other templates)
            Process(template.Object, context);

            // Apply template properties to the object if not already set
            ApplyTemplateToObject(obj, template.Object);
        }

        private static void ApplyTemplateToObject(TiledMapObjectContent obj, TiledMapObjectContent templateObj)
        {
            if (!obj._globalIdentifier.HasValue && templateObj._globalIdentifier.HasValue)
                obj.GlobalIdentifier = templateObj.GlobalIdentifier;

            if (!obj._height.HasValue && templateObj._height.HasValue)
                obj.Height = templateObj.Height;

            if (!obj._identifier.HasValue && templateObj._identifier.HasValue)
                obj.Identifier = templateObj.Identifier;

            if (!obj._rotation.HasValue && templateObj._rotation.HasValue)
                obj.Rotation = templateObj.Rotation;

            if (!obj._visible.HasValue && templateObj._visible.HasValue)
                obj.Visible = templateObj.Visible;

            if (!obj._width.HasValue && templateObj._width.HasValue)
                obj.Width = templateObj.Width;

            if (!obj._x.HasValue && templateObj._x.HasValue)
                obj.X = templateObj.X;

            if (!obj._y.HasValue && templateObj._y.HasValue)
                obj.Y = templateObj.Y;

            if (obj.Ellipse == null && templateObj.Ellipse != null)
                obj.Ellipse = templateObj.Ellipse;

            if (string.IsNullOrWhiteSpace(obj.Name) && !string.IsNullOrWhiteSpace(templateObj.Name))
                obj.Name = templateObj.Name;

            if (obj.Polygon == null && templateObj.Polygon != null)
                obj.Polygon = templateObj.Polygon;

            if (obj.Polyline == null && templateObj.Polyline != null)
                obj.Polyline = templateObj.Polyline;

            foreach (var tProperty in templateObj.Properties)
            {
                if (!obj.Properties.Exists(p => p.Name == tProperty.Name))
                    obj.Properties.Add(tProperty);
            }

            if (string.IsNullOrWhiteSpace(obj.Type) && !string.IsNullOrWhiteSpace(templateObj.Type))
                obj.Type = templateObj.Type;

            if (string.IsNullOrWhiteSpace(obj.Class) && !string.IsNullOrWhiteSpace(templateObj.Class))
                obj.Class = templateObj.Class;
        }
    }
}
#endif
