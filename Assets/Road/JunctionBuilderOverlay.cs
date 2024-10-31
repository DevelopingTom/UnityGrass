using UnityEditor;
using UnityEditor.Overlays;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Splines;
using UnityEngine.Splines;
using UnityEngine;
using System.Collections;
using System.Linq;

[Overlay(typeof(SceneView), "Junction Builder", true)]
public class JunctionBuilderOverlay : Overlay
{
    Label SelectionInfoLabel = new Label();
    Button button = new Button();
    VisualElement root = new VisualElement();
    VisualElement junctionArea = new VisualElement();
    VisualElement sliderArea = new VisualElement();

    public override VisualElement CreatePanelContent()
    { 
        button.clicked += OnBuildJunction;
        button.text = "Create junction";
        junctionArea.Add(SelectionInfoLabel);
        junctionArea.Add(button);
        SplineSelection.changed += () =>
        {
            OnSelectionChanged();
        };
        root.Add(junctionArea);
        root.Add(sliderArea);
        return root;
    }

    void ClearSelectionInfo()
    {
        SelectionInfoLabel.text = "";
        sliderArea.Clear();
    }

    private void OnBuildJunction()
    {
        List<SelectedSplineElementInfo> selection = SplineToolUtility.GetSelection();
        Intersection intersection = new Intersection();
        foreach(SelectedSplineElementInfo item in selection)
        {
            SplineContainer container = (SplineContainer)item.target;
            Spline spline = container.Splines[item.targetIndex];
            intersection.AddJunction(item.targetIndex, item.knotIndex, spline, spline.Knots.ToList().ElementAt(item.knotIndex));
        }
        
        sliderArea.Clear();

        ShowIntersection(intersection);

        Selection.activeTransform.GetComponent<SplineRectangleExtruder>().AddJunction(intersection);
    }

    void OnSelectionChanged()
    {
        UpdateSelectionInfo();
    }

    private void UpdateSelectionInfo()
    {
        ClearSelectionInfo();
        List<SelectedSplineElementInfo> infos = SplineToolUtility.GetSelection();
        List<Intersection> intersections = Selection.activeTransform.GetComponent<SplineRectangleExtruder>().GetJunctions();
        foreach (SelectedSplineElementInfo item in infos)
        {
            SelectionInfoLabel.text += $"Spline {item.targetIndex}, Knots {item.knotIndex} \n";
            foreach(Intersection intersection in intersections)
            {
                if (intersection.HasSplineIndex(item.targetIndex))
                {
                    ShowIntersection(intersection);
                }
            }
        }
    }

    public void OnChangeValueEvent()
    {
        Selection.activeTransform.GetComponent<SplineRectangleExtruder>().BuildMesh();
    }

    public void ShowIntersection(Intersection intersection)
    {
        Label intersectionLabel = new Label();
        intersectionLabel.text = "Selected Intersection:";
        sliderArea.Add(intersectionLabel);
        for (int i = 0; i < intersection.curves.Count; i++)
        {
            int value = i;
            Slider slider = new Slider($"Curve {i}", 0, 1, SliderDirection.Horizontal);
            slider.labelElement.style.minWidth = 60;
            slider.labelElement.style.maxWidth = 80;
            slider.value = intersection.curves[i];
            slider.RegisterValueChangedCallback((x) =>
            {
                intersection.curves[value] = x.newValue;
                OnChangeValueEvent();
            });
            sliderArea.Add(slider);
        }
    }
}