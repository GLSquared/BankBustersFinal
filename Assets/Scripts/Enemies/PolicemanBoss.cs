using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicemanBoss : MonoBehaviour
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

    // Animations values
    private bool _crouching = false;
    private bool _targetting = false;
    private bool _shooting = false;
    private bool _reloading = false;
    private bool _isDead = false;
    private float _walkspeed = 0;

    // Weapon
    [SerializeField]
    private Transform Weapon;

    [SerializeField]
    private Transform Muzzle;

    [SerializeField]
    private GameObject bulletPrefab;

    private float waitTime = 0;

    private int currentAmmo = 31;
    private int TotalAmmo = 31;

    bool enableGizmos = false;

    private AudioSource fireSound;

    void LogicDebugStart()
    {
        // print("Skin Selected: " + skin.ToString());
        print("Spawn Position: " + transform.position.ToString());
    }

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        animator = GetComponent<Animator>();

        enemy.LogicStart();

        enemy.MaxHealth = 666;
        enemy.health = 666;

        enemy.WalkSpeed = 3f;

        if (enemy.DebugEnabled)
            LogicDebugStart();
    }

    private void FixedUpdate()
    {
        if (!enemy.isDead())
        {
            enemy.LogicUpdate();

            if (_shooting)
            {
                Shooting();
            }

            Animate();
        }
        else
        {
            Dead();
        }

    }

    private void Animate()
    {
        // Set the bools
        _targetting = (enemy.currentState == Enemy.State.Following || enemy.currentState == Enemy.State.Attacking) ? true : false;
        _crouching = enemy.isCrouching();
        _walkspeed = enemy.currentWalkSpeed;
        _shooting = enemy.isShooting();

        // Set the values
        animator.SetBool("Targetting", _targetting);
        animator.SetFloat("Walkspeed", _walkspeed);
        animator.SetBool("Crouching", _crouching);
        animator.SetInteger("Magazine", currentAmmo);
        animator.SetBool("Shoot", _shooting);
        animator.SetBool("Reload", _reloading);
        animator.SetInteger("Magazine", currentAmmo);
    }

    private IEnumerator Reload()
    {
        // TODO: Player Animation
        // Load the mag
        _shooting = false;
        _reloading = true;
        yield return new WaitForSeconds(3.3f);
        currentAmmo = TotalAmmo;
        _reloading = false;
    }

    private IEnumerator Die()
    {
        if (!_isDead)
        {
            _isDead = true;
            animator.SetTrigger("Dead");
            yield return new WaitForSeconds(0.6f);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    private void Dead()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().currentLevel=1;
        StartCoroutine(Die());
    }

    private void Shooting()
    {
        if (Mathf.Abs(_walkspeed) > 0.1f)
            return;

        if (currentAmmo == 0)
        {
            StartCoroutine("Reload");
        }
        else
        {
            if (waitTime < 0)
            {
                waitTime = 0.7f;

                GameObject bullet1 = Instantiate(bulletPrefab);
                bullet1.transform.position = Muzzle.transform.position;
                bullet1.transform.rotation = Quaternion.LookRotation((enemy.currentTarget.transform.position + new Vector3(0, 1.5f, 0)) - Muzzle.transform.position, Vector3.up);

                bullet1.GetComponent<InfimaGames.LowPolyShooterPack.Legacy.EnemyProjectile>().bulletDamage = 5*1.5f;

                Rigidbody rb1 = bullet1.GetComponent<Rigidbody>();
                rb1.AddForce(bullet1.transform.forward * 200f, ForceMode.Impulse);

                fireSound = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetComponent<AudioSource>();

                fireSound.PlayOneShot(fireSound.clip);

                currentAmmo--;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

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
