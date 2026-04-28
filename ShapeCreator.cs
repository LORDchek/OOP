using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OOP.Factories;
using OOP.Shapes;

namespace OOP
{
    /// <summary>
    /// Handles interactive shape creation using mouse input
    /// Uses Factory pattern to create shapes without switch statements
    /// </summary>
    public class ShapeCreator
    {
        private Dictionary<string, IShapeFactory> factories = new Dictionary<string, IShapeFactory>();
        private IShapeFactory currentFactory;
        private Point? startPoint = null;
        private bool isCreating = false;
        private Color currentColor = Color.Black;

        public event Action<IShape> ShapeCreated;

        public ShapeCreator()
        {
            // Automatically register all available factories
            // New shape factories can be added here without modifying other code
            RegisterFactory(new LineFactory());
            RegisterFactory(new RectangleFactory());
            RegisterFactory(new EllipseFactory());
            RegisterFactory(new TriangleFactory());
            RegisterFactory(new SquareFactory());
            RegisterFactory(new CircleFactory());
        }

        /// <summary>
        /// Registers a shape factory
        /// </summary>
        public void RegisterFactory(IShapeFactory factory)
        {
            if (!factories.ContainsKey(factory.ShapeName))
            {
                factories.Add(factory.ShapeName, factory);
            }
        }

        /// <summary>
        /// Gets list of available shape names
        /// </summary>
        public List<string> GetAvailableShapes()
        {
            return new List<string>(factories.Keys);
        }

        /// <summary>
        /// Sets the current shape type to create
        /// </summary>
        public bool SetCurrentShape(string shapeName)
        {
            if (factories.TryGetValue(shapeName, out var factory))
            {
                currentFactory = factory;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles mouse down event for shape creation
        /// </summary>
        public void HandleMouseDown(Point location)
        {
            if (currentFactory != null)
            {
                startPoint = location;
                isCreating = true;
            }
        }

        /// <summary>
        /// Handles mouse up event for shape creation
        /// </summary>
        public void HandleMouseUp(Point location)
        {
            if (isCreating && startPoint.HasValue && currentFactory != null)
            {
                // Create the shape using the factory
                IShape shape = currentFactory.CreateShape(startPoint.Value, location, currentColor);
                OnShapeCreated(shape);
                isCreating = false;
                startPoint = null;
            }
        }

        /// <summary>
        /// Cancels current shape creation
        /// </summary>
        public void CancelCreation()
        {
            isCreating = false;
            startPoint = null;
        }

        /// <summary>
        /// Sets the current color for new shapes
        /// </summary>
        public void SetCurrentColor(Color color)
        {
            currentColor = color;
        }

        /// <summary>
        /// Creates a default shape at the specified location
        /// </summary>
        public IShape CreateDefaultShape(string shapeName, Point location)
        {
            if (factories.TryGetValue(shapeName, out var factory))
            {
                return factory.CreateDefaultShape(location);
            }
            return null;
        }

        private void OnShapeCreated(IShape shape)
        {
            ShapeCreated?.Invoke(shape);
        }
    }
}