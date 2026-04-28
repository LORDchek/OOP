using System;
using System.Collections;
using System.Collections.Generic;
using OOP.Shapes;

namespace OOP
{
	/// <summary>
	/// Container class for managing a collection of shapes
	/// </summary> 
	public class ShapeList : IEnumerable<IShape>
	{
		private List<IShape> shapes = new List<IShape>();

		/// <summary>
		/// Adds a shape to the list
		/// </summary>
		public void AddShape(IShape shape)
		{
			if (shape != null)
			{
				shapes.Add(shape);
			}
		}

		/// <summary>
		/// Removes a shape from the list
		/// </summary>
		public bool RemoveShape(IShape shape)
		{
			return shapes.Remove(shape);
		}

		/// <summary>
		/// Removes shape at specified index
		/// </summary>
		public void RemoveAt(int index)
		{
			if (index >= 0 && index < shapes.Count)
			{
				shapes.RemoveAt(index);
			}
		}

		/// <summary>
		/// Gets shape at specified index
		/// </summary>
		public IShape GetShape(int index)
		{
			return (index >= 0 && index < shapes.Count) ? shapes[index] : null;
		}

		/// <summary>
		/// Number of shapes in the list
		/// </summary>
		public int Count => shapes.Count;

		/// <summary>
		/// Clears all shapes
		/// </summary>
		public void Clear()
		{
			shapes.Clear();
		}

		/// <summary>
		/// Indexer for accessing shapes
		/// </summary>
		public IShape this[int index]
		{
			get => GetShape(index);
			set
			{
				if (index >= 0 && index < shapes.Count)
				{
					shapes[index] = value;
				}
			}
		}

		public IEnumerator<IShape> GetEnumerator()
		{
			return shapes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}