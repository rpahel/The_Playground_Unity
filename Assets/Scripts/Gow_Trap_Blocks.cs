using System;
using UnityEngine;

[ExecuteInEditMode]
public class Gow_Trap_Blocks : MonoBehaviour
{
    [Header("Blocks move")]
    [SerializeField] private float move_duration;
    [SerializeField] private AnimationCurve move_ease;

    [Space(10)]
    [Header("General block properties")]
    public float start_position;
    public float end_position;
    public float time_to_start;
    public float time_between_moves;

    [Space(10)]
    [Header("Block")]
    [SerializeField] private GameObject block_prefab;
    private float block_width;
    private Gow_Trap_Block[] blocks;

    [Space(10)]
    [Header("Block Properties")]
    public BlockSettings[] block_settings;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            CreateTrapBlocks(block_settings.Length);
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            CreateTrapBlocks(block_settings.Length);
        }
    }

    /// <summary>
    /// Creates the given number of blocks for the trap.
    /// </summary>
    /// <param name="n">The number of blocks to be created.</param>
    private void CreateTrapBlocks(int n = 3)
    {
        DestroyTrapBlocks();

        if (n == 0){ Debug.Log("You can't create a blocks trap with 0 blocks."); return; }
        blocks = new Gow_Trap_Block[n];

        block_width = block_prefab.transform.GetChild(0).localScale.x;

        for(int i = 0; i < n; i++)
        {
            blocks[i] = Instantiate(block_prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Gow_Trap_Block>();
            blocks[i].transform.localPosition = new Vector3(block_width * (i - 0.5f * (n - 1)), 0, 0);
            blocks[i].InitialLocalPosition = blocks[i].transform.localPosition;
            blocks[i].gameObject.name = "Block " + i.ToString("00");
            blocks[i].ApplyProperties(block_settings[i]);
            blocks[i].MoveDuration = move_duration;
            blocks[i].Curve = move_ease;
        }
    }

    /// <summary>
    /// Destroys the children blocks of the trap.
    /// </summary>
    private void DestroyTrapBlocks()
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        blocks = null;
    }

    [ContextMenu("Apply all general settings.")]
    public void ApplyAllGeneralProperties()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            block_settings[i].start_position = start_position;
            block_settings[i].end_position = end_position;
            block_settings[i].time_to_start = time_to_start;
            block_settings[i].time_between_moves = time_between_moves;
        }
    }
}

[Serializable]
public struct BlockSettings
{
    public float start_position;
    public float end_position;
    public float time_to_start;
    public float time_between_moves;
}