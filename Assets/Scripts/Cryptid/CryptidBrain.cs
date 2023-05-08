using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CryptidBrain : MonoBehaviour
{
    // this is a singleton handling all the cryptid's behaviour
    // it being a singleton means its accessible from anywhere without complex getcomponents etc

    private static CryptidBrain _instance;
    public static CryptidBrain Instance { get { return _instance; } }

    // finite state machine to handle the cryptid's behaviour

    [Header("Runtime")]
    [SerializeField] string currentStateKey;
    [SerializeReference] DictionaryOfStringAndBrainState states = new DictionaryOfStringAndBrainState();

    [Header("Emotions")]
    [SerializeField] public float curiosity = 0;
    [SerializeField] public float aggression = 0;

    [Header("Initial Settings")]
    [SerializeField] string initialStateKey = "Initial";
    [SerializeField] float idleTime = 5;

    [Header("Wander Settings")]
    [SerializeField] Transform wanderTargetsParent;
    [SerializeField] float wanderSpeed = 2.5f;

    [Header("Follow Settings")]
    [SerializeField] float followTimeToLose = 5;
    [SerializeField] float followDistance = 10;

    [Header("Toy Settings")]
    [SerializeField] float toyMinimumCuriosity = 6;
    [SerializeField] float toyInterestTime = 15;
    [SerializeField] float toyCameraThreshold = 1500;
    [SerializeField] Transform toyKiteTargetsParent;
    [SerializeField] float toyMaxDistanceToGoal = 45;
    [SerializeField] float toyTimeToSelectNewGoal = 30;
    [SerializeField] float toyDistanceToKiteOutside = 15;
    [SerializeField] float toyKiteOffsetDistance = 10;

    [Header("Lurk Settings")]
    [SerializeField] float lurkMinimumAggression = 10;
    [SerializeField] float lurkDistance = 5;
    [SerializeField] float lurkWatchedDistance = 15;
    [SerializeField] float lurkAvoidance = 2;
    [SerializeField] float lurkLoseInterestTime = 20;
    [SerializeField] float lurkIncreaseAggressionWhenWatchedTimer = 5;

    [Header("Hunt Settings")]
    [SerializeField] float huntSearchRadiusIncrease = 5;
    [SerializeField] float huntTimeBeforeGiveUp = 20;

    [Header("Aggressive Hunt Settings")]
    [SerializeField] float aggrohuntSearchRadiusIncrease = 5;
    [SerializeField] float aggrohuntTimeBeforeGiveUp = 15;

    [Header("Chase Settings")]
    [SerializeField] float chaseMinimumAggression = 20;
    [SerializeField] float chaseTimeToLose = 5;
    [SerializeField] float chaseAttackDistance = 3;
    [SerializeField] float chaseSpeed = 2.5f;
    [SerializeField] float chaseSprintDuration = 5;
    [SerializeField] float chaseRestDuration = 3;

    [Header("Attack Settings")]
    [SerializeField] float attackAnimationDuration = 2;

    [Header("References")]
    [SerializeField] public Rigidbody body;
    [SerializeField] public CryptidSenses senses;
    [SerializeField] public NavMeshAgent navigator;
    [SerializeField] public Photographable photographable;
    [SerializeField] public Transform defendedZoneCenter;
    [SerializeField] public float defendedZoneRadius;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public NavMeshObstacle playerObstacle;

    private void Awake() 
    {
        // singleton setup

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        // declaring all the possible states and their access keys.
        // its a bit frustrating these can't be set in the editor but it works well enough here.
        // prolly would have been best to use scriptableobjects, in retrospect

        states["Initial"] = new Idle("Initial", this, idleTime, "Wander");
        states["Wander"] = new Wander("Wander", this, wanderTargetsParent, wanderSpeed);
        states["Follow"] = new Follow("Follow", this, followTimeToLose, followDistance, toyMinimumCuriosity, lurkMinimumAggression, 15);
        states["Toy"] = new Toy("Toy", this, 5, 1, 10, lurkMinimumAggression, toyMinimumCuriosity - 2, toyInterestTime, toyCameraThreshold, toyKiteTargetsParent,
            toyMaxDistanceToGoal, toyTimeToSelectNewGoal, toyDistanceToKiteOutside, toyKiteOffsetDistance);
        states["Lurk"] = new Lurk("Lurk", this, 5, 2, 10, chaseMinimumAggression, lurkDistance, lurkWatchedDistance, lurkAvoidance,
            lurkIncreaseAggressionWhenWatchedTimer, lurkLoseInterestTime);
        states["HuntNormal"] = new Hunt("HuntNormal", this, huntSearchRadiusIncrease, 1, chaseMinimumAggression, toyMinimumCuriosity, false,
            huntTimeBeforeGiveUp, "Wander", 6);
        states["HuntAggressive"] = new Hunt("HuntAggressive", this, aggrohuntSearchRadiusIncrease, 1, chaseMinimumAggression, -1, true,
            aggrohuntTimeBeforeGiveUp, "HuntNormal", 4);
        states["Chase"] = new Chase("Chase", this, chaseTimeToLose, chaseAttackDistance, chaseSpeed, chaseSprintDuration, chaseRestDuration);
        states["Attack"] = new Attack("Attack", this, attackAnimationDuration);
    }

    void Start()
    {
        if (states.ContainsKey(initialStateKey)) // activating the initial state
        {
            currentStateKey = initialStateKey;
            states[currentStateKey].Enter();
        }
        else
        {
            Debug.Log("CryptidBrain initial state key isn't valid.");
        }
            
    }

    // update triggers the updatelogic of the current state
    void Update()
    {
        if (currentStateKey != null)
            states[currentStateKey].UpdateLogic();
    }

    // change the current state to a new state from the given access key. calls the enter and exit values of the states as they are entered and exited
    public void ChangeState(string key)
    {
        if (states.ContainsKey(key))
        {
            states[currentStateKey].Exit();

            currentStateKey = key;
            states[currentStateKey].Enter();
        }
        else
        {
            Debug.Log("Requested BrainState key not found: " + key);
        }
        
    }

    // when the player takes a photo, trigger the current state's response to that action.
    public void OnCryptidPhotographed()
    {
        states[currentStateKey].CryptidPhotographed();
    }

    public void OnNotCryptidPhotographed()
    {
        states[currentStateKey].NotCryptidPhotographed();
    }

    // check whether the player is near the defended zone
    public bool PlayerNearDefendedZone()
    {
        if (Vector2.Distance(PlayerReference.Instance.transform.position, defendedZoneCenter.position) <= defendedZoneRadius) return true;
        else return false;
    }

    private void OnDrawGizmosSelected() // draws the navigation destination in the editor
    {
        if (navigator.destination != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(navigator.destination, 0.2f);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(defendedZoneCenter.position, defendedZoneRadius);
    }
}
