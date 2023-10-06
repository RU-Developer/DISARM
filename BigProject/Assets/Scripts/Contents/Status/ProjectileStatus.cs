using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStatus : MonoBehaviour
{
    public LayerMask target;
    private Collider2D collider;

    protected virtual void OnEnable()
    {
        collider = GetComponent<Collider2D>();
    }
}
