using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rb;
    bool hasLanded;
    bool thrown;

    Vector3 initPossition;
    int diceValue;

    [SerializeField] DiceSide[] diceSides;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPossition = transform.position;
        rb.useGravity = false;
        rb.isKinematic = true;

    }

    void Update()
    {
        if (rb.IsSleeping() && !hasLanded && thrown)
        {
            hasLanded = true;
            rb.useGravity = false;
            rb.isKinematic = true;
            SideValueCheck();
        }
        else if (rb.IsSleeping() && hasLanded && diceValue == 0)
        {
            ReRollDice();
        }
    }

    public void RollDice()
    {
        Reset();
        if (!thrown && !hasLanded)
        {
            thrown = true;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        }
        // else if (thrown && hasLanded)
        // {
        //     //reset dice
        //     Reset();
        // }
    }

    void Reset()
    {
        transform.position = initPossition;
        thrown = false;
        hasLanded = false;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void ReRollDice()
    {
        Reset();
        thrown = true;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));

    }

    void SideValueCheck()
    {
        diceValue = 0;
        foreach (var side in diceSides)
        {
            if (side.OnGround)
            {
                diceValue = side.SideValue();
                //Debug.Log("Numero sacado = " + diceValue);
                break;
            }
        }
        GameManager.instance.ReportDiceRolled(diceValue);
    }
}
