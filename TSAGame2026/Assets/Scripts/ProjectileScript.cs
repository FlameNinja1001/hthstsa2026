using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
    PlayerInputActions input;

    
    public GameObject slingshotPrefab;
    public GameObject bonePrefab;
    public GameObject javelinPrefab;
    public GameObject bowAndArrowPrefab;
    public GameObject musketPrefab;
    public GameObject gearPrefab;

    public string activeWeaponString;
    public GameObject activeWeapon;
    public bool shootButton;
    public int ammoCount = 1;
    public int ammoCountDisplay;
    public static int currentShots = 0;
    public bool canShoot = true;

    public float launchSpeed;
    public int moveDirection;

    public float launchSpeedSlingshot;
    public float launchSpeedBone;
    public float launchSpeedJavelin;
    public float launchSpeedBow;
    public float launchSpeedMusket;
    public float launchSpeedGear;

    public float currentDistance;
    public float slingShotDistance;
    public float boneDistance;
    public float javelinDistance;
    public float bowDistance;
    public float musketDistance;
    public float gearDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ammoCountDisplay = currentShots;
        if (input.Player.Move.ReadValue<Vector2>().x > 0)
        {
            moveDirection = 1;
        }
        else if (input.Player.Move.ReadValue<Vector2>().x < 0)
        {
            moveDirection = -1;
        }
        shootButton = input.Player.Shoot.IsPressed();
        if (!shootButton)
        {
            canShoot = true;
        }

        if (canShoot && shootButton && currentShots < ammoCount)
        {
            currentShots +=1;
            GameObject bullet = Instantiate(activeWeapon, new Vector3(transform.position.x + (currentDistance * moveDirection), transform.position.y, transform.position.z), activeWeapon.transform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (activeWeaponString != "Javelin" && activeWeaponString != "Bone")
            {
                rb.linearVelocity = Vector3.right * moveDirection * launchSpeed;
            }
            else if (activeWeaponString == "Javelin")
            {
                rb.linearVelocity = new Vector3(moveDirection * launchSpeed, launchSpeed / 2.5f, rb.linearVelocity.z);                
            }
            canShoot = false;
        }

        if (activeWeaponString == "Slingshot")
        {
            activeWeapon = slingshotPrefab;
            ammoCount = 1;
            launchSpeed = launchSpeedSlingshot;
            currentDistance = slingShotDistance;
        }
        if (activeWeaponString == "Bone")
        {
            activeWeapon = bonePrefab;
            ammoCount = 2;
            launchSpeed = launchSpeedBone;
            currentDistance = boneDistance;
        }
        if (activeWeaponString == "Javelin")
        {
            activeWeapon = javelinPrefab;
            ammoCount = 2;
            launchSpeed = launchSpeedJavelin;
            currentDistance = javelinDistance;
        }
        if (activeWeaponString == "BowAndArrow")
        {
            activeWeapon = bowAndArrowPrefab;
            ammoCount = 3;
            launchSpeed = launchSpeedBow;
            currentDistance = bowDistance;
        }
        if (activeWeaponString == "Musket")
        {
            activeWeapon = musketPrefab;
            ammoCount = 4;
            launchSpeed = launchSpeedMusket;
            currentDistance = musketDistance;
        }
        if (activeWeaponString == "Gear")
        {
            activeWeapon = gearPrefab;
            ammoCount = 5;
            launchSpeed = launchSpeedGear;
            currentDistance = gearDistance;
        }
    }

    public void UpdateAmmoMax()
    {
        
    }
}
