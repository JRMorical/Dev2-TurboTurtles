using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class radialFillManager : MonoBehaviour
{

    [SerializeField] Image radialFillImage;
    [SerializeField] float fillDuration = 1.0f;

    private Coroutine fillCoroutine = null;

    public void UpdateRadialProgressCircle(int valueToFill, int maxFill)
    {
        float targetFillAmount = (float)valueToFill  / (float)maxFill;

        if (fillCoroutine != null )
        {
            StopCoroutine(fillCoroutine);
        }

        fillCoroutine = StartCoroutine(AnimateFill(targetFillAmount));
    }

    private IEnumerator AnimateFill(float targetFillAmount)
    {
        float initialFillAmount = radialFillImage.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float lerpRatio = elapsedTime / fillDuration;
            radialFillImage.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, lerpRatio);
            yield return null;
        }

        radialFillImage.fillAmount = targetFillAmount;
        fillCoroutine = null;
    }
}
