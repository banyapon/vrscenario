using UnityEngine;

public class ResetToDefault : MonoBehaviour
{
    Vector3 _defaultLocalPosition;
    Vector3 _defaultLocalEuler;
    Vector3 _defaultLocalScale;

    void Awake()
    {
        _defaultLocalPosition = transform.localPosition;
        _defaultLocalEuler = transform.localEulerAngles;
        _defaultLocalScale = transform.localScale;
    }

    public void ResetTransform()
    {
        transform.localPosition = _defaultLocalPosition;
        transform.localEulerAngles = _defaultLocalEuler;
        transform.localScale = _defaultLocalScale;
    }
}
