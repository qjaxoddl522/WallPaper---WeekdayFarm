using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IdolSync : MonoBehaviour
{
    public float tweenTime;
    public float tweenSize;

    Transform child;

    void Start()
    {
        child = GetComponentInChildren<Transform>();

        float upSize = 1f + tweenSize;
        float downSize = 1f - tweenSize;

        transform.localScale = new Vector3(upSize, downSize, 1f);
        child.localScale = new Vector3(upSize, downSize, 1f);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(new Vector3(downSize, upSize, 1f), tweenTime).SetEase(Ease.Linear))
           .Join(child.DOScale(new Vector3(downSize, upSize, 1f), tweenTime).SetEase(Ease.Linear))
           .Append(transform.DOScale(new Vector3(upSize, downSize, 1f), tweenTime).SetEase(Ease.Linear))
           .Join(child.DOScale(new Vector3(upSize, downSize, 1f), tweenTime).SetEase(Ease.Linear))
           .SetLoops(-1, LoopType.Restart);
        
    }
}
