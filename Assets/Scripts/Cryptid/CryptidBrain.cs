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

    [Header("State Settings")]
    [SerializeField] string initialStateKey = "Initial";
    [SerializeField] float idleTime = 5;
    [SerializeField] Transform wanderTargetsParent;
    [SerializeField] float followTimeToLose = 5;
    [SerializeField] float followDistance = 10;
    [SerializeField] float huntSearchRadiusIncrease = 5;
    [SerializeField] float huntTimeBeforeGiveUp = 20;
    [SerializeField] float aggrohuntSearchRadiusIncrease = 5;
    [SerializeField] float aggrohuntTimeBeforeGiveUp = 15;
    [SerializeField] float chaseTimeToLose = 5;
    [SerializeField] float chaseAttackDistance = 3;

    [Header("References")]
    [SerializeField] public Rigidbody body;
    [SerializeField] public CryptidSenses senses;
    [SerializeField] public NavMeshAgent navigator;
    [SerializeField] public Photographable photographable;
    [SerializeField] public Transform defendedZoneCenter;
    [SerializeField] public float defendedZoneRadius;
    [SerializeField] public LayerMask groundLayer;

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
        // its a bit frustrating these can't be set in the editor yet but it works well enough here.

        states["Initial"] = new Idle("Initial", this, idleTime, "Wander");
        states["Wander"] = new Wander("Wander", this, wanderTargetsParent);
        states["Follow"] = new Follow("Follow", this, followTimeToLose, 2, followDistance, 6, 10);
        states["Toy"] = new Toy("Toy", this);
        states["Lurk"] = new Lurk("Lurk", this);
        states["HuntNormal"] = new Hunt("HuntNormal", this, huntSearchRadiusIncrease, 1, 20, 6, false, huntTimeBeforeGiveUp, "Wander");
        states["HuntAggressive"] = new Hunt("HuntAggressive", this, aggrohuntSearchRadiusIncrease, 1, 20, -1, true, aggrohuntTimeBeforeGiveUp, "HuntNormal");
        states["Chase"] = new Chase("Chase", this, chaseTimeToLose, chaseAttackDistance);
        states["Attack"] = new Attack("Attack", this);
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
    }
}
