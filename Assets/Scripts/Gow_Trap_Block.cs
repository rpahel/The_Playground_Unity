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
    private Transform block;
    public Vector3 block_initial_worldposition;
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

    private void Awake()
    {
        block = transform.GetChild(0);
    }

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
    private void OnDrawGizmos()
    {
        block_initial_worldposition = transform.parent.position + transform.localPosition.x * transform.right + block.localPosition.y * transform.up;
        half_block_height = transform.lossyScale.x * .5f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(block_initial_worldposition, start_position * block.forward);
        Gizmos.DrawWireSphere(block_initial_worldposition + start_position * block.forward, half_block_height);
        Gizmos.DrawSphere(block_initial_worldposition + start_position * block.forward, half_block_height * .25f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(block_initial_worldposition, -end_position * block.forward);
        Gizmos.DrawWireSphere(block_initial_worldposition - end_position * block.forward, half_block_height);
        Gizmos.DrawSphere(block_initial_worldposition - end_position * block.forward, half_block_height * .25f);
    }
#endif
}
