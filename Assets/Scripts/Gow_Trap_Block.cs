using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Gow_Trap_Block : MonoBehaviour
{
    public float start_position;
    public float end_position;
    public float time_to_start;
    public float time_between_moves;

    //================================================================================
    private Vector3 initial_localposition;
    public Vector3 InitialLocalPosition { set { initial_localposition = value; } }
    private float move_duration;
    public float MoveDuration {  set { move_duration = value; } }
    private float half_block_height;
    private AnimationCurve curve;
    public AnimationCurve Curve { set { curve = value;} }
    private bool start_to_end;
    private bool frozen;

    //================================================================================
    private Vector3 start_localposition_v3;
    private Vector3 end_localposition_v3;

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        start_localposition_v3 = initial_localposition + start_position * Vector3.forward;
        end_localposition_v3 = initial_localposition - end_position * Vector3.forward;
        transform.localPosition = start_localposition_v3;
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine(bool go_back = false)
    {
        yield return new WaitForSeconds(time_to_start);

        float t = 0;
        while (true)
        {
            if (!go_back)
            {
                start_to_end = true;
                while(t < 1)
                {
                    if (frozen)
                        break;

                    transform.localPosition = Vector3.LerpUnclamped(start_localposition_v3, end_localposition_v3, curve.Evaluate(t));
                    t += Time.fixedDeltaTime / move_duration;
                    yield return new WaitForFixedUpdate();
                }

                if (frozen)
                    break;

                transform.localPosition = end_localposition_v3;
                go_back = true;
            }
            else
            {
                start_to_end = false;
                while (t < 1)
                {
                    if (frozen)
                        break;

                    transform.localPosition = Vector3.LerpUnclamped(end_localposition_v3, start_localposition_v3, curve.Evaluate(t));
                    t += Time.fixedDeltaTime / move_duration;
                    yield return new WaitForFixedUpdate();
                }

                if (frozen)
                    break;

                transform.localPosition = start_localposition_v3;
                go_back = false;
            }

            t = 0;
            while(t < time_between_moves)
            {
                if (frozen)
                    break;

                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            t = 0;
        }

        //if (frozen) Initiate slowing down coroutine;
    }

    public void ApplyProperties(BlockSettings block_settings)
    {
        start_position = block_settings.start_position;
        end_position = block_settings.end_position;
        time_to_start = block_settings.time_to_start;
        time_between_moves = block_settings.time_between_moves;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            transform.localPosition = initial_localposition + start_position * Vector3.forward;
            return;
        }
    }

    private void OnDrawGizmos()
    {
        half_block_height = transform.localScale.y * .5f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine((transform.parent.position + initial_localposition) + half_block_height * Vector3.up,
                        (transform.parent.position + initial_localposition) + half_block_height * Vector3.up + start_position * Vector3.forward);
        Gizmos.DrawWireCube((transform.parent.position + initial_localposition) + half_block_height * Vector3.up + start_position * Vector3.forward, transform.localScale);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine((transform.parent.position + initial_localposition) + half_block_height * Vector3.up,
                        (transform.parent.position + initial_localposition) + half_block_height * Vector3.up - end_position * Vector3.forward);
        Gizmos.DrawWireCube((transform.parent.position + initial_localposition) + half_block_height * Vector3.up - end_position * Vector3.forward, transform.localScale);
    }
#endif
}
