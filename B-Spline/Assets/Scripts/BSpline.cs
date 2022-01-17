using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BSpline : MonoBehaviour
{
    [SerializeField] private TextAsset controlPointsTextAsset;
    [SerializeField] private Slider slider;

    private readonly List<Vector3> _controlPoints = new List<Vector3>();
    private Matrix4x4 _splineMatrix;

    private void Start()
    {
        LoadControlPoints();
        InitMatrices();
        InitSlider();
        RecalculateObjectPosition(sliderValue: 0);
    }

    private void LoadControlPoints()
    {
        var controlPointLines = controlPointsTextAsset.text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

        foreach (var controlPointLine in controlPointLines)
        {
            var components = controlPointLine.Split();
            var x = int.Parse(components[0]);
            var y = int.Parse(components[1]);
            var z = int.Parse(components[2]);
            _controlPoints.Add(new Vector3(x, y, z));
        }
    }

    private void InitMatrices()
    {
        var column0 = new Vector4(-1, 3, -3, 1);
        var column1 = new Vector4(3, -6, 0, 4);
        var column2 = new Vector4(-3, 3, 3, 1);
        var column3 = new Vector4(1, 0, 0, 0);
        _splineMatrix = new Matrix4x4(column0, column1, column2, column3);
    }

    private void InitSlider()
    {
        slider.maxValue = _controlPoints.Count - 3;
        slider.onValueChanged.AddListener(RecalculateObjectPosition);
    }

    private void RecalculateObjectPosition(float sliderValue)
    {
        if (Math.Abs(sliderValue - slider.maxValue) < 0.001f) return;

        var i = Mathf.FloorToInt(sliderValue) + 1;
        var t = sliderValue - Mathf.FloorToInt(sliderValue);

        transform.position = CalculatePointOnSpline(i, t);

        var firstDerivative = CalculateTangentOnSpline(i, t);
        var secondDerivative = CalculateSecondDerivativeOnSpline(i, t);
        transform.LookAt(transform.position + firstDerivative, Vector3.Cross(firstDerivative, secondDerivative));
    }

    private Vector3 CalculatePointOnSpline(int i, float t) =>
        CalculatePointOnSplineActual(i, new Vector4(t * t * t, t * t, t, 1));

    private Vector3 CalculateTangentOnSpline(int i, float t) =>
        CalculatePointOnSplineActual(i, new Vector4(3 * t * t, 2 * t, 1, 0));

    private Vector3 CalculateSecondDerivativeOnSpline(int i, float t) =>
        CalculatePointOnSplineActual(i, new Vector4(6 * t, 2, 0, 0));

    private Vector3 CalculatePointOnSplineActual(int i, Vector4 tVector)
    {
        var xVector = new Vector4(_controlPoints[i - 1].x, _controlPoints[i].x, _controlPoints[i + 1].x, _controlPoints[i + 2].x);
        var yVector = new Vector4(_controlPoints[i - 1].y, _controlPoints[i].y, _controlPoints[i + 1].y, _controlPoints[i + 2].y);
        var zVector = new Vector4(_controlPoints[i - 1].z, _controlPoints[i].z, _controlPoints[i + 1].z, _controlPoints[i + 2].z);

        var x = Vector4.Dot(tVector, _splineMatrix * xVector);
        var y = Vector4.Dot(tVector, _splineMatrix * yVector);
        var z = Vector4.Dot(tVector, _splineMatrix * zVector);

        return new Vector3(x, y, z) / 6;
    }

    private void OnDrawGizmos()
    {
        if (_controlPoints == null || _controlPoints.Count == 0) return;

        DrawControlPoints();
        DrawBSpline();
    }

    private void DrawControlPoints()
    {
        Gizmos.color = Color.black;

        foreach (var controlPoint in _controlPoints)
        {
            Gizmos.DrawSphere(controlPoint, 0.3f);
        }
    }

    private void DrawBSpline()
    {
        var segmentCount = _controlPoints.Count - 3;
        var lastPoint = CalculatePointOnSpline(i: 1, t: 0);

        Gizmos.color = new Color(0f, 0.66f, 1f);

        for (var i = 1; i <= segmentCount; i++)
        {
            for (var t = 0f; t <= 1; t += 0.05f)
            {
                var point = CalculatePointOnSpline(i, t);
                Gizmos.DrawLine(lastPoint, point);
                lastPoint = point;
            }
        }
    }
}
