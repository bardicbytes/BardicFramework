using System.Threading.Tasks;
using UnityEngine;

public class SpecialEffectObject : MonoBehaviour
{
    public float lifespan = 1;
    public bool useUnscaledTime = false;

    public bool IsAlive => this != null && isActiveAndEnabled && MyTime <= EndTime;
    public float StartTime { get; private set; }
    public float EndTime { get; private set; }
    private float MyTime => (useUnscaledTime ? Time.unscaledTime : Time.time);

    [SerializeField]
    private bool doScaleOverTime = false;
    [SerializeField]
    private AnimationCurve scaleOverTime;
    [SerializeField]
    private Transform scaleTarget;


    public void OnEnable()
    {
        StartTime = MyTime;
        EndTime = MyTime + lifespan;
        if (doScaleOverTime) PlayScaleAnimation();
    }

    private async void PlayScaleAnimation()
    {
        while(MyTime < EndTime)
        {
            await Task.Yield();
            if (!IsAlive) return;
            float t = (MyTime - StartTime) / lifespan;
            scaleTarget.localScale = Vector3.one * scaleOverTime.Evaluate(t);
        }
    }

    public void Kill()
    {
        EndTime = MyTime;
    }
}