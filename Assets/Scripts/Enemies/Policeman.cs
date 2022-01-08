using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Policeman : MonoBehaviour
{
    // Enemy constructor module
    Enemy enemy;
    private Animator animator;

    // Enemy Look At
    [SerializeField]
    private Transform EyeLookAt;

    // Skins
    [SerializeField]
    private Transform CharacterModel;
    [SerializeField]
    private Material[] skins;
    private Material skin;

    // Animations values
    private bool _crouching = false;
    private bool _targetting = false;
    private float _walkspeed = 0;

    // Weapon
    [SerializeField]
    private Transform Weapon;

    [SerializeField]
    private Transform Muzzle;
    private int currentAmmo = 31;
    private int TotalAmmo = 31;

    bool enableGizmos = false;

    void LogicDebugStart()
    {
        print("Skin Selected: "+skin.ToString());
        print("Spawn Position: "+transform.position.ToString());
    }
    
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        animator = GetComponent<Animator>();

        skin = skins[Random.Range(0, skins.Length)];

        SkinnedMeshRenderer meshRenderer = CharacterModel.GetComponent<SkinnedMeshRenderer>();
        Material[] materials = new Material[meshRenderer.materials.Length];
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            materials[i] = skin;
        }
        meshRenderer.materials = materials;

        enemy.LogicStart();

        if (enemy.DebugEnabled)
            LogicDebugStart();
    }

    private void FixedUpdate()
    {
        enemy.LogicUpdate();

        Animate();
    }

    private void Animate()
    {
        // Set the bools
        _targetting = (enemy.currentState==Enemy.State.Following)?true:false;
        _crouching  = enemy.isCrouching();
        _walkspeed  = enemy.currentWalkSpeed;

        // Set the values
        animator.SetBool("Targetting", _targetting);
        animator.SetFloat("Walkspeed", _walkspeed);
        animator.SetBool("Crouching", _crouching);
        animator.SetInteger("Magazine", currentAmmo);
    }

    private void Reload()
    {
        // TODO: Player Animation
        // Load the mag
        currentAmmo = TotalAmmo;
    }

    /// <summary>
    /// Display gizmos relating to enemy vision, state, pathfinding, etc...
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (enableGizmos)
        {
            float distance = 4f;

            Gizmos.color = Color.red;
            Vector3 startViewAt = Quaternion.Euler(0, enemy.ViewAngle, 0) * transform.forward * distance;
            Vector3 endViewAt = Quaternion.Euler(0, -enemy.ViewAngle, 0) * transform.forward * distance;

            Gizmos.DrawRay(EyeLookAt.position, startViewAt);
            Gizmos.DrawRay(EyeLookAt.position, endViewAt);
        }
    }
}
