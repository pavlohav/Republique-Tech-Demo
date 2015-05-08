using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

[RequireComponent(typeof(ReflectionProbe))]
public class ReflectionProbeUpdater : MonoBehaviour 
{
	[SerializeField]
    private ReflectionProbeTimeSlicingMode _timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;

	private ReflectionProbe _reflectionProbe;

    private void Awake()
    {
		_reflectionProbe = GetComponent<ReflectionProbe>();
    }

    private void Update()
    {
		_reflectionProbe.timeSlicingMode = _timeSlicingMode;
		_reflectionProbe.RenderProbe();
    }
}