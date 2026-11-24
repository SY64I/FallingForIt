using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Vector3 playerSpawnPoint, monsterSpawnPoint;
    Quaternion playerRotation, monsterRotation;

    [SerializeField] GameObject player, monster;
    PlayerControls playerControls;
    [SerializeField] GravityController gravityControl;
    Animator monsterAnim;

    // It'd be nice to have this script as a reference to an entire enemy AI lol
    MietteAnimations monsterAnimationController;

    // This would likely become a static UI instance reference instead of a direct reference.
    [SerializeField] Animator uiAnim;

    public bool respawning;

    public event Action checkpointHit;
    public event Action mietteEjected;
    public event Action cancelEject;
    public bool ejecting;
    [SerializeField] float ejectTimerDuration;
    float ejectTimerCounter;

    public bool victory;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetSpawnPoints(player.transform, monster.transform);
        playerRotation = player.transform.rotation;
        monsterRotation = monster.transform.rotation;

        playerControls = player.GetComponent<PlayerControls>();
        monsterAnim = monster.GetComponentInChildren<Animator>();
        monsterAnimationController = monster.GetComponent<MietteAnimations>();

        ejectTimerCounter = ejectTimerDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (ejecting)
        {
            if (!playerControls.isDead)
            {
                if (ejectTimerCounter > 0)
                {
                    ejectTimerCounter -= Time.deltaTime;
                }

                if (ejectTimerCounter <= 0)
                {
                    if (!victory)
                    {
                        // You win lmao
                        Debug.Log("Jail for Asbro! Jail for Asbro for 10000 years!");
                        victory = true;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }

                }
            }
            else
            {
                cancelEject?.Invoke();
                ejecting = false;
            }
        }
    }


    public void SetSpawnPoints(Transform playerPoint, Transform monsterPoint)
    {
        playerSpawnPoint = playerPoint.position;
        monsterSpawnPoint = monsterPoint.position;
        checkpointHit?.Invoke();
    }


    public IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1.45f);
        if (uiAnim != null) uiAnim.SetBool("close", true); // The check for null would be removed once we have a proper UI management system
        yield return new WaitForSeconds(0.75f);

        Debug.Log("Respawning...");
        respawning = true;
        gravityControl.ResetGravityDirection();
        monsterAnimationController.Respawn();
        //monster.transform.position = monsterSpawnPoint;
        monster.transform.rotation = monsterRotation;
        yield return new WaitForSeconds(0.1f); // Delay to prevent race condition of player spawning in Miette's grab point

        Debug.Log("Moving player to " + playerSpawnPoint);
        playerControls.Respawn();
        uiAnim.SetBool("close", false);
        respawning = false;
    }

    public void CheckpointRespawn()
    {
        respawning = true;
        gravityControl.ResetGravityDirection();
        monsterAnimationController.Respawn();
        //monster.transform.position = monsterSpawnPoint;
        monster.transform.rotation = monsterRotation;

        Debug.Log("Moving player to " + playerSpawnPoint);
        playerControls.Respawn();
        uiAnim.SetBool("close", false);
        respawning = false;
    }

    public IEnumerator TransitionLevel(string nextLevel)
    {
        if (uiAnim != null) uiAnim.SetBool("close", true);
        yield return new WaitForSeconds(0.75f);

        SceneManager.LoadScene(nextLevel);
    }

    public void Eject()
    {
        mietteEjected?.Invoke();
        ejecting = true;
    }
}
