﻿/*
 * TreeNode.cs - Implementation of the
 *			"System.Windows.Forms.TreeNodeCollection" class.
 *
 * Copyright (C) 2003  Neil Cawse.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace Paril.Windows.Forms
{
	using System;
	using System.Collections;

	public class TreeNodeExCollection : IList
	{
		private TreeNodeEx owner;

		internal TreeNodeExCollection(TreeNodeEx owner)
		{
			this.owner = owner;
		}

		public IEnumerator GetEnumerator()
		{
			return new ArraySubsetEnumerator(owner.children, owner.childCount);
		}

		public int Count
		{
			get
			{
				return owner.childCount;
			}
		}

		public void CopyTo(Array dest, int index)
		{
			if (owner.childCount > 0)
				Array.Copy(owner.children, 0, dest, index, owner.childCount);
		}

		public virtual void RemoveAt(int index)
		{
			this[index].Remove();
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual void Clear()
		{
			owner.Clear();
		}

		void IList.Remove(object node)
		{
			Remove(node as TreeNodeEx);
		}

		void IList.Insert(int index, object node)
		{
			Insert(index, node as TreeNodeEx);
		}

		public virtual void Insert(int index, TreeNodeEx node)
		{
			TreeViewEx treeView = owner.TreeView;
			if (treeView != null && treeView.Sorted)
			{
				owner.AddSorted(node);
			}
			owner.InsertNodeAt(index, node);
		}

		int IList.IndexOf(object node)
		{
			return IndexOf(node as TreeNodeEx);
		}

		bool IList.Contains(object node)
		{
			return Contains(node as TreeNodeEx);
		}

		int IList.Add(object node)
		{
			return Add(node as TreeNodeEx);
		}

		public virtual int Add(TreeNodeEx node)
		{
			if (node.treeView != null && node.treeView.Sorted)
			{
				return owner.AddSorted(node);
			}
			else
			{
				owner.SizeChildrenArray();
				SetNodeOwner(node);
				node.parent = owner;
				node.OnNodeMoved(new TreeNodeExMovedEventArgs(node, null, new TreeNodeExParent(owner)));
				int pos = owner.childCount++;
				node.index = pos;
				owner.children[node.index] = node;
				// Redraw if required.
				if (node.treeView != null && node.treeView.IsHandleCreated)
				{
					node.treeView.Draw(owner);
				}
				return pos;
			}
		}

		public virtual void AddRange(TreeNodeEx[] nodes)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				Add(nodes[i]);
			}
		}

		public virtual TreeNodeEx Add(string text)
		{
			TreeNodeEx node = new TreeNodeEx(text);
			Add(node);
			return node;
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (TreeNodeEx)value;
			}
		}


		public virtual TreeNodeEx this[int index]
		{
			get
			{
				return owner.children[index];
			}
			set
			{
				value.parent = owner;
				value.index = index;
				value.treeView = owner.treeView;
				owner.children[index] = value;
				if (owner.treeView != null)
				{
					owner.treeView.Draw(owner);
				}
			}
		}

		public bool Contains(TreeNodeEx node)
		{
			return IndexOf(node) != -1;
		}

		public int IndexOf(TreeNodeEx node)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i] == node)
				{
					return i;
				}
			}
			return -1;
		}

		public void Remove(TreeNodeEx node)
		{
			node.Remove();
		}

		private void SetNodeOwner(TreeNodeEx node)
		{
			node.treeView = owner.TreeView;
			foreach (TreeNodeEx tn in node.Nodes)
			{
				SetNodeOwner(tn);
			}
		}

		private class ArraySubsetEnumerator : IEnumerator
		{
			private object[] array;
			private int count;
			private int current;

			public ArraySubsetEnumerator(object[] array, int count)
			{
				this.array = array;
				this.count = count;
				current = -1;
			}

			public object Current
			{
				get
				{
					if (current == -1)
					{
						return null;
					}
					return array[current];
				}
			}

			public bool MoveNext()
			{
				if (current < count - 1)
				{
					current++;
					return true;
				}
				else
				{
					return false;
				}
			}

			public void Reset()
			{
				current = -1;
			}

		}

	}
}
