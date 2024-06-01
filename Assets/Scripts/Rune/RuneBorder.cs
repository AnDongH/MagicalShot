using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneBorder : MonoBehaviour
{
    private Collider2D borderCollider;

    private void Awake() {
        borderCollider = GetComponent<Collider2D>();
    }

    public void SetCollider(bool flag) {
        borderCollider.enabled = flag;
    }
}
