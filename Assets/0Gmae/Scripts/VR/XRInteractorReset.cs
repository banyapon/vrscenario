using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class XRInteractorReset : MonoBehaviour
{
    public NearFarInteractor[] nearFarInteractors;
    public UIInputModule uiInputModule;
    XRInteractionManager interactionManager;

    void Start()
    {
        interactionManager = GetComponent<XRInteractionManager>();

        if (VRNetworkController.Instance)
            VRNetworkController.Instance.onClientDisconnected += ResetXR;
    }

    public void ResetXR()
    {
        StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        if (uiInputModule && uiInputModule.enabled)
            uiInputModule.enabled = false;

        if (interactionManager && uiInputModule.enabled)
            interactionManager.enabled = false;

        yield return null;
        yield return null;

        foreach (var near in nearFarInteractors)
        {
            if (!near) continue;
            near.enabled = false;
        }

        yield return null;

        foreach (var near in nearFarInteractors)
        {
            if (!near) continue;
            near.enabled = true;
        }

        if (uiInputModule) uiInputModule.enabled = true;
        if (interactionManager) interactionManager.enabled = true;
    }

    void OnDestroy()
    {
        if (VRNetworkController.Instance == null) return;
        VRNetworkController.Instance.onClientDisconnected -= ResetXR;
    }
}
