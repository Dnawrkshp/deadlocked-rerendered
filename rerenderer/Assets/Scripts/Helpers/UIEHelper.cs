using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIEHelper
{
    const int HORIZONTAL_MARGINS = 200;
    const int VERTICAL_MARGINS = 0; //15;

    /// <summary>
    /// Returns the true if the current element is a child of the given element.
    /// </summary>
    public static bool IsChildOf(this VisualElement childElement, VisualElement parentElement)
    {
        if (childElement == null)
            return false;

        var parent = childElement.parent;
        while (parent != null && parent != parentElement)
            parent = parent.parent;

        return parent != null;
    }

    /// <summary>
    /// Returns the VisualElement that childElement is a child of, and that the VisualElement is a first child of rootElement.
    /// Returns NULL if no such ancestor exists.
    /// </summary>
    public static T FindFirstAncestorOfType<T>(this VisualElement childElement)
        where T : VisualElement
    {
        if (childElement == null) return null;

        var parent = childElement;
        while (parent != null)
        {
            if (parent is T tInstance) return tInstance;
            parent = parent.parent;
        }

        return null;
    }

    /// <summary>
    /// Returns the VisualElement that childElement is a child of, and that the VisualElement is a first child of rootElement.
    /// Returns NULL if no such ancestor exists.
    /// </summary>
    public static VisualElement FindAncestorThatIsFirstChildOf(VisualElement rootElement, VisualElement childElement)
    {
        if (rootElement == null) return null;
        if (childElement == null) return null;

        var rootChildren = rootElement.Children().ToList();

        var parent = childElement;
        while (parent != null && !rootChildren.Contains(parent))
        {
            parent = parent.parent;
        }

        return parent;
    }

    /// <summary>
    /// Returns the VisualElement that childElement is a child of, and that the VisualElement contains the given class name.
    /// Returns NULL if no such ancestor exists.
    /// </summary>
    public static VisualElement FindAncestorWithClass(VisualElement childElement, string className)
    {
        var parent = childElement;
        while (parent != null && !parent.ClassListContains(className))
        {
            parent = parent.parent;
        }

        return parent;
    }

    /// <summary>
    /// Returns the VisualElement that childElement is a child of, and that the VisualElement is a first child of rootElement.
    /// Returns NULL if no such ancestor exists.
    /// </summary>
    public static VisualElement FindFirstSelectableInHierarchy(this VisualElement element, bool ignoreDisabledElements = true)
    {
        if (element == null)
            return null;

        if (element.resolvedStyle.display != DisplayStyle.Flex)
            return null;

        if (element.focusable && element.canGrabFocus && (!ignoreDisabledElements || element.enabledInHierarchy))
            return element;

        foreach (var child in element.Children())
        {
            var focusableChild = child.FindFirstSelectableInHierarchy();
            if (focusableChild != null)
                return focusableChild;
        }

        return null;
    }

    /// <summary>
    /// Returns the VisualElement that childElement is a child of, and that the VisualElement is a first child of rootElement.
    /// Returns NULL if no such ancestor exists.
    /// </summary>
    public static VisualElement FindClosestSelectableInHierarchy(this VisualElement element, VisualElement toElement, bool ignoreDisabledElements = true)
    {
        float bestScore = float.MaxValue;
        VisualElement bestElement = null;

        if (element == null)
            return null;

        if (element.resolvedStyle.display != DisplayStyle.Flex)
            return null;

        if (element.focusable && element.canGrabFocus && (!ignoreDisabledElements || element.enabledInHierarchy))
        {
            var score = GetClosestDistanceTo(element, toElement);
            if (score < bestScore)
            {
                bestElement = element;
                bestScore = score;
            }
        }

        foreach (var child in element.Children())
        {
            var focusableChild = child.FindClosestSelectableInHierarchy(toElement, ignoreDisabledElements: ignoreDisabledElements);
            if (focusableChild != null)
            {
                var score = GetClosestDistanceTo(focusableChild, toElement);
                if (score < bestScore)
                {
                    bestElement = focusableChild;
                    bestScore = score;
                }
            }
        }

        return bestElement;
    }

    public static VisualElement FindNextFocusableElementVertical(VisualElement rootElement, VisualElement actualElement, int direction, int fromChildIdx, bool loop = true)
    {

        var children = rootElement.Children().ToList();
        var thisElement = children.ElementAtOrDefault(fromChildIdx);
        int loopCount = 0;
        List<(VisualElement element, float score)> candidates = new List<(VisualElement element, float score)>();

        if (thisElement == null || direction == 0)
            return rootElement;

        do
        {
            fromChildIdx += direction;

            if (fromChildIdx < 0)
                fromChildIdx = rootElement.childCount - 1;
            else if (fromChildIdx >= rootElement.childCount)
                fromChildIdx = 0;

            var newTarget = children.ElementAtOrDefault(fromChildIdx)?.FindClosestSelectableInHierarchy(actualElement);

            // compute score
            if (newTarget != null)
            {
                var score = 0f;
                var delta = newTarget.worldTransform.GetPosition() - thisElement.worldTransform.GetPosition();

                // within vertical bounds
                if (delta.x >= -HORIZONTAL_MARGINS && delta.x < (thisElement.contentRect.width + HORIZONTAL_MARGINS) && Mathf.Abs(delta.y) > VERTICAL_MARGINS)
                {
                    if (direction > 0)
                    {
                        // down
                        score = delta.y - thisElement.contentRect.height;
                        score += Mathf.Sign(score) * Mathf.Abs(delta.x / thisElement.contentRect.width);
                    }
                    else
                    {
                        // up
                        score = -delta.y;
                        score += Mathf.Sign(score) * Mathf.Abs(delta.x / thisElement.contentRect.width);
                    }

                    candidates.Add((newTarget, score));
                }
            }

            loopCount++;
        } while (loopCount < (children.Count - 1));

        // best target without looping
        // is lowest positive score
        var nextNoLoop = candidates.Where(x => x.score >= 0).OrderBy(x => x.score).FirstOrDefault().element;
        if (!loop || nextNoLoop != null)
            return nextNoLoop;

        // best target with looping
        // is lowest score
        return candidates.OrderBy(x => x.score).FirstOrDefault().element;
    }

    public static VisualElement FindNextFocusableElementHorizontal(VisualElement rootElement, VisualElement actualElement, int direction, int fromChildIdx, bool loop = true)
    {

        var children = rootElement.Children().ToList();
        var thisElement = children.ElementAtOrDefault(fromChildIdx);
        int loopCount = 0;
        List<(VisualElement element, float score)> candidates = new List<(VisualElement element, float score)>();

        if (thisElement == null || direction == 0)
            return null;

        do
        {
            fromChildIdx += direction;

            if (fromChildIdx < 0)
                fromChildIdx = rootElement.childCount - 1;
            else if (fromChildIdx >= rootElement.childCount)
                fromChildIdx = 0;

            var newTarget = children.ElementAtOrDefault(fromChildIdx)?.FindClosestSelectableInHierarchy(actualElement);

            // compute score
            if (newTarget != null)
            {
                var score = 0f;
                var delta = newTarget.worldTransform.GetPosition() - thisElement.worldTransform.GetPosition();

                // within vertical bounds
                if (delta.y >= -VERTICAL_MARGINS && delta.y < (thisElement.contentRect.height + VERTICAL_MARGINS))
                {
                    if (direction > 0)
                    {
                        // right
                        score = delta.x - thisElement.contentRect.width;
                        score += Mathf.Sign(score) * Mathf.Abs(delta.y);
                    }
                    else
                    {
                        // left
                        score = -delta.x;
                        score += Mathf.Sign(score) * Mathf.Abs(delta.y);
                    }

                    candidates.Add((newTarget, score));
                }
            }

            loopCount++;
        } while (loopCount < (children.Count - 1));

        // best target without looping
        // is lowest positive score
        var nextNoLoop = candidates.Where(x => x.score >= 0).OrderBy(x => x.score).FirstOrDefault().element;
        if (!loop || nextNoLoop != null)
            return nextNoLoop;

        // best target with looping
        // is lowest score
        return candidates.OrderBy(x => x.score).FirstOrDefault().element;
    }

    public static bool IsInView(this VisualElement visualElement)
    {
        var parent = visualElement.parent;
        while (parent != null)
        {
            var inParent = (visualElement.worldBound.xMin > parent.worldBound.xMin && visualElement.worldBound.xMin < parent.worldBound.xMax)
                && (visualElement.worldBound.xMax > parent.worldBound.xMin && visualElement.worldBound.xMax < parent.worldBound.xMax)
                && (visualElement.worldBound.yMin > parent.worldBound.yMin && visualElement.worldBound.yMin < parent.worldBound.yMax)
                && (visualElement.worldBound.yMax > parent.worldBound.yMin && visualElement.worldBound.yMax < parent.worldBound.yMax);

            if (!inParent) return false;

            parent = parent.parent;
        }

        return true;
    }

    public static float GetClosestDistanceTo(VisualElement from, VisualElement to)
    {
        var left = to.worldBound.xMax < from.worldBound.xMin;
        var right = to.worldBound.xMin > from.worldBound.xMax;
        var bottom = to.worldBound.yMin > from.worldBound.yMax;
        var top = to.worldBound.yMax < from.worldBound.yMin;

        Vector2 fromClosestPoint = Vector2.zero;
        Vector2 toClosestPoint = Vector2.zero;

        if (top && left) { fromClosestPoint = new Vector2(from.worldBound.xMin, from.worldBound.yMin); toClosestPoint = new Vector2(to.worldBound.xMax, to.worldBound.yMax); }
        else if (top && right) { fromClosestPoint = new Vector2(from.worldBound.xMax, from.worldBound.yMin); toClosestPoint = new Vector2(to.worldBound.xMin, to.worldBound.yMax); }
        else if (bottom && left) { fromClosestPoint = new Vector2(from.worldBound.xMin, from.worldBound.yMax); toClosestPoint = new Vector2(to.worldBound.xMax, to.worldBound.yMin); }
        else if (bottom && right) { fromClosestPoint = new Vector2(from.worldBound.xMax, from.worldBound.yMax); toClosestPoint = new Vector2(to.worldBound.xMin, to.worldBound.yMin); }
        else if (left) { fromClosestPoint = new Vector2(from.worldBound.xMin, 0); toClosestPoint = new Vector2(to.worldBound.xMax, 0); }
        else if (right) { fromClosestPoint = new Vector2(from.worldBound.xMax, 0); toClosestPoint = new Vector2(to.worldBound.xMin, 0); }
        else if (top) { fromClosestPoint = new Vector2(0, from.worldBound.yMin); toClosestPoint = new Vector2(0, to.worldBound.yMax); }
        else if (bottom) { fromClosestPoint = new Vector2(0, from.worldBound.yMax); toClosestPoint = new Vector2(0, to.worldBound.yMin); }

        // return distance
        return Vector2.Distance(fromClosestPoint, toClosestPoint);
    }

}
