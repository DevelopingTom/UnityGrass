using UnityEngine;
using UnityEditor.Splines;
using System.Collections.Generic;
namespace UnityEditor.Splines
{
    public struct SelectedSplineElementInfo
    {
        public Object target;
        public int targetIndex;
        public int knotIndex;
        public SelectedSplineElementInfo(Object Object, int Index, int knot)
        {
            target = Object;
            targetIndex = Index;
            knotIndex = knot;
        }
    }

    public static class SplineToolUtility
    {
        public static bool HasSelection()
        {
            return SplineSelection.HasActiveSplineSelection(); 
        }

        public static List<SelectedSplineElementInfo> GetSelection()
        {
            List<SelectableSplineElement> elements = SplineSelection.selection;
            List<SelectedSplineElementInfo> infos = new List<SelectedSplineElementInfo>();
            foreach(SelectableSplineElement element in elements)
            {
                infos.Add(new SelectedSplineElementInfo(element.target, element.targetIndex, element.knotIndex));
            }
            return infos;
        }
    }
}