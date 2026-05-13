using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
    public GameObject uiElement;
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

    public float shotDelay = 0.25f;       // ADD: customizable delay between shots
    private float lastShotTime = -999f;  // ADD: tracks when last shot was fired

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        uiElement.SetActive(true);
    }

    void OnDisable()
    {
        input.Player.Disable();
        uiElement.SetActive(false);
    }
    void Start()
    {
        meleeScript = GetComponent<MeleeScript>();
        controlScript = GetComponent<ControlScript>();
    }

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

        if (canShoot && shootButton && currentShots < ammoCount && !meleeScript.isSlashing && !controlScript.isDashing && Time.time - lastShotTime >= shotDelay)
        {
            currentShots += 1;
            lastShotTime = Time.time;  // ADD: record shot time
            GameObject bullet = Instantiate(activeWeapon, new Vector3(transform.position.x + (currentDistance * moveDirection), transform.position.y + offset, transform.position.z), activeWeapon.transform.rotation);
            bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * moveDirection, bullet.transform.localScale.y, bullet.transform.localScale.z);

            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }
            shootCoroutine = StartCoroutine(ShootAnimCoroutine());

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (activeWeaponString != "Javelin")
            {
                rb.velocity = Vector3.right * moveDirection * launchSpeed;
                if (activeWeaponString == "BowAndArrow")
                {
                    bullet.transform.localScale = new Vector3(bullet.transform.localScale.x, bullet.transform.localScale.y * moveDirection, bullet.transform.localScale.z);   
                }
            }
            else
            {
                rb.velocity = new Vector3(moveDirection * launchSpeed, launchSpeed / 2.5f, rb.velocity.z);             
                bullet.transform.localScale = new Vector3(bullet.transform.localScale.x, bullet.transform.localScale.y * moveDirection, bullet.transform.localScale.z);   
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
        if (activeWeaponString == "Javelin" || activeWeaponString == "Slingshot")
        {
            isShootAnimBoolActive = false;
            yield return null;
            isShootAnimBoolActive = true;

            float timer = 0f;
            float animDuration = 0.3f;

            while (timer < animDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            isShootAnimBoolActive = false;
        }      
        else
        {
            isShootAnimBoolActive = true;

            float timer = 0f;

            while (timer < 1f)
            {
                if (meleeScript.meleeButton)
                {
                    meleeScript.StartSlashFromElsewhere();
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            isShootAnimBoolActive = false;
        }  
    }
}