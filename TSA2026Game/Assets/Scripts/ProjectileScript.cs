using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
    PlayerInputActions input;

    
    public GameObject slingshotPrefab;    
    public GameObject javelinPrefab;
    public GameObject bowAndArrowPrefab;
    public GameObject musketPrefab;    

    public string activeWeaponString;
    public GameObject activeWeapon;
    public bool shootButton;
    public MeleeScript meleeScript;
    public int ammoCount = 1;
    public int ammoCountDisplay;
    public static int currentShots = 0;
    public bool canShoot = true;

    public float launchSpeed;
    public int moveDirection;

    public float launchSpeedSlingshot;    
    public float launchSpeedJavelin;
    public float launchSpeedBow;
    public float launchSpeedMusket;    

    public float currentDistance;
    public float slingShotDistance;    
    public float javelinDistance;
    public float bowDistance;
    public float musketDistance;    
    public bool isShootAnimBoolActive;
    public float offset;
    public ControlScript controlScript;
    private Coroutine shootCoroutine;
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
        meleeScript = GetComponent<MeleeScript>();
        controlScript = GetComponent<ControlScript>();
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

        if (canShoot && shootButton && currentShots < ammoCount && !meleeScript.isSlashing && !controlScript.isDashing)
        {
            currentShots +=1;
            GameObject bullet = Instantiate(activeWeapon, new Vector3(transform.position.x + (currentDistance * moveDirection), transform.position.y + offset, transform.position.z), activeWeapon.transform.rotation);
            bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * moveDirection,bullet.transform.localScale.y,bullet.transform.localScale.z);
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }

            shootCoroutine = StartCoroutine(ShootAnimCoroutine());
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (activeWeaponString != "Javelin")
            {
                rb.velocity = Vector3.right * moveDirection * launchSpeed;
            }
            else if (activeWeaponString == "Javelin")
            {
                rb.velocity = new Vector3(moveDirection * launchSpeed, launchSpeed / 2.5f, rb.velocity.z);                
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
    }

    public void UpdateAmmoMax()
    {
        
    }
    public IEnumerator ShootAnimCoroutine()
    {
        isShootAnimBoolActive = true;

        float timer = 0f;

        while (timer < 1f)
        {
            // Break immediately if melee button pressed
            if (meleeScript.meleeButton)
            {
                meleeScript.StartSlashFromElsewhere();
                break;
            }

            timer += Time.deltaTime;
            yield return null; // wait one frame
        }

        isShootAnimBoolActive = false;
    }
}
