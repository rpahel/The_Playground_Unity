using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gow_Trap_Blocks : MonoBehaviour
{
    [SerializeField] private GameObject block_prefab;
    private float block_width;
    private Transform[] blocks;

    /// <summary>
    /// Creates the given number of blocks for the trap.
    /// </summary>
    /// <param name="n">The number of blocks to be created.</param>
    public void CreateTrapBlocks(int n = 3)
    {
        DestroyTrapBlocks();

        if (n == 0){ Debug.Log("You can't create a blocks trap with 0 blocks."); return; }
        blocks = new Transform[n];

        block_width = block_prefab.transform.GetChild(0).localScale.x;

        for(int i = 0; i < n; i++)
        {
            blocks[i] = Instantiate(block_prefab, new Vector3(block_width * (i - 0.5f * (n-1)), 0, 0), Quaternion.identity, transform).transform;
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
}
