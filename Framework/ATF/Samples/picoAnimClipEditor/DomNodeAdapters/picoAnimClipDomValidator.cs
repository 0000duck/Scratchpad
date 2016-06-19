using System;
using System.Collections.Generic;

using Sce.Atf;
using Sce.Atf.Adaptation;
using Sce.Atf.Dom;
//using Sce.Atf.Controls.Timelines;

using pico.Timeline;

namespace picoAnimClipEditor.DomNodeAdapters
{
	//public static class ITimelineObjectExtensions
	//{
	//	public static void CanInsert( this ITimelineObject timelineObject, object parent )
	//	{
			
	//	}
	//}

    /// <summary>
    /// Validator that checks for special rules related to GroupCamera
	/// Only certain type of tracks and intervals can be added to this group
	/// </summary>
    public class picoAnimClipDomValidator : Validator
    {
		public static bool ParentHasChildOfType<T>( DomNode domNode )
			where T : class
		{
			foreach ( DomNode existingChild in domNode.Children )
			{
				if ( existingChild.Is<T>() )
					return true;
			}

			return false;
		}

		public static int CountChildrenOfType<T>( DomNode domNode )
			where T : class
		{
			int count = 0;
			foreach ( DomNode existingChild in domNode.Children )
			{
				if ( existingChild.Is<T>() )
					++count;
			}

			return count;
		}

		//public static bool ValidateGroupCamera( DomNode parent, DomNode child )
		//{
		//	if ( !parent.Is<Timeline>() )
		//		return false;

		//	if ( ParentHasChildOfType<GroupCamera>( parent ) )
		//		return false;

		//	return true;
		//}

        /// <summary>
        /// Performs custom actions after a child is inserted into the DOM subtree</summary>
        /// <param name="sender">Sender (root DOM node)</param>
        /// <param name="e">Child event args</param>
        protected override void OnChildInserted(object sender, ChildEventArgs e)
        {
            if (Validating)
                m_childChanges.Add(new Pair<Pair<DomNode, DomNode>, ChildInfo>(new Pair<DomNode, DomNode>(e.Parent, e.Child), e.ChildInfo));
        }

		///// <summary>
		///// Performs custom actions after a child is removed from the DOM subtree</summary>
		///// <param name="sender">Sender (root DOM node)</param>
		///// <param name="e">Child event args</param>
		//protected override void OnChildRemoved(object sender, ChildEventArgs e)
		//{
		//	if (Validating)
		//		m_childChanges.Add(new Pair<Pair<DomNode, DomNode>, ChildInfo>(new Pair<DomNode, DomNode>(e.Parent, e.Child), e.ChildInfo));
		//}

        /// <summary>
        /// Performs custom actions after validation finished</summary>
        /// <param name="sender">Sender (root DOM node)</param>
        /// <param name="e">Attribute event args</param>
        protected override void OnEnding(object sender, System.EventArgs e)
        {
			// this functionality must be handled in OnEnding, not OnEnded in order for history (undo/redo) to work correctly
			// (history context adds command in OnEnded even if transaction was cancelled)
			//
            foreach (Pair<Pair<DomNode, DomNode>, ChildInfo> pair in m_childChanges)
            {
                DomNode parent = pair.First.First;
                DomNode child = pair.First.Second;

				if ( ! ValidatePair(parent, child) )
				{
					throw new InvalidTransactionException(
						string.Format("{0} can't be parented as a child of {1}", child, parent )
						);
				}
            }
            m_childChanges.Clear();
        }

        /// <summary>
        /// Performs custom actions when validation cancelled</summary>
        /// <param name="sender">Sender (root DOM node)</param>
        /// <param name="e">Attribute event args</param>
        protected override void OnCancelled(object sender, System.EventArgs e)
        {
            m_childChanges.Clear();
        }

		private bool ValidatePair( DomNode parent, DomNode child )
		{
			ITimelineValidationCallback vi = child.As<ITimelineValidationCallback>();
			if ( vi != null )
			{
				if ( !vi.Validate( parent ) )
					return false;
			}
			return true;
		}

        //pairs of parent and child; todo: use the Tuple in .Net 4.0
        private HashSet<Pair<Pair<DomNode, DomNode>,ChildInfo>> m_childChanges =
            new HashSet<Pair<Pair<DomNode, DomNode>,ChildInfo>>();
    }
}