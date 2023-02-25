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

    [Header("References")]
    [SerializeField] public Rigidbody body;
    [SerializeField] public CryptidSenses senses;
    [SerializeField] public NavMeshAgent navigator;
    [SerializeField] public Transform defendedZoneCenter;
    [SerializeField] public float defendedZoneRadius;

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
        states["Follow"] = new Follow("Follow", this, followTimeToLose, radius: followDistance);
        states["HuntNormal"] = new Hunt("HuntNormal", this);
        states["Toy"] = new Toy("Toy", this);
        states["Lurk"] = new Lurk("Lurk", this);
        states["HuntAggressive"] = new Hunt("HuntAggressive", this);
        states["Chase"] = new Follow("Chase", this, followTimeToLose, chase: true);
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

}
