using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeScript : MonoBehaviour
{
    public GameObject uiElement;
    PlayerInputActions input;
    ProjectileScript projectileScript;
    public bool meleeButton;    
    public bool hasReleased;
    public bool isSlashing = false;
    public float duration;
    public GameObject trigger;
    public MeleeDamage meleeDamage;
    public ControlScript controlScript;
    
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
        projectileScript = GetComponent<ProjectileScript>();
        meleeDamage = trigger.GetComponent<MeleeDamage>();
        controlScript = GetComponent<ControlScript>();
    }

    // Update is called once per frame
    void Update()
    {        
        meleeDamage.damageAmount = projectileScript.ammoCount * 2;
        meleeButton = input.Player.Slash.IsPressed();    
        if (!meleeButton)
        {
            hasReleased = true;
        }   
        if (hasReleased && meleeButton && !isSlashing && !projectileScript.isShootAnimBoolActive && !controlScript.isDashing)
        {
            hasReleased = false;
            StartCoroutine(SlashCoroutine());
        }
    }
    public IEnumerator SlashCoroutine()
    {
        isSlashing = true;
        trigger.SetActive(true);
        yield return new WaitForSeconds(duration);
        isSlashing = false;
        trigger.SetActive(false);
    }

    public void StartSlashFromElsewhere()
    {
        StartCoroutine(SlashCoroutine());
    }
}
