using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gun : MonoBehaviour
{
    public enum GunOwned
    {
        Single = 1,
        Dual = 2,
        Catapult = 3,
    }

    [System.Serializable]
    public class Weapon
    {
        public GameObject weaponObject;
        public string weaponName;
        public int clipSize;
        public int maxAmmo;
        public int clipAmmo;
        public int currentTotalAmmo;
        public int fireRate; //Primary 15 Secondary 10
        public int bulletSpeed;
        public int bulletDrop;
        public int damage;
    }

    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }

    public int damageMultiplier = 1;
    
    public float range = 100f;


    public Weapon[] weaponList;
    public Weapon currentActiveWeapon, primaryWeapon, secondaryWeapon;

    public float reloadTime = 1f;
    private bool isReloading = false;

    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;
    public Transform raycastOrigin;
    public Transform raycastDestination;

    public TMPro.TextMeshProUGUI weaponText;

    //public GameObject impactEffect;


    Ray ray;
    RaycastHit hitInfo;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3f;

    void Awake()
    {
        currentActiveWeapon = weaponList[0];
        primaryWeapon = getWeapon("primary");
        secondaryWeapon = getWeapon("secondary");
        //Debug.Log(currentActiveWeapon.weaponObject.transform.position);
    }
    void Start()
    {
        foreach (Weapon w in weaponList)
        {
            w.currentTotalAmmo = w.maxAmmo - w.clipSize;
            w.clipAmmo = w.clipSize;
        }
    }

    public void switchWeapon()
    {
        if(currentActiveWeapon == primaryWeapon)
        {
            currentActiveWeapon = secondaryWeapon;
        }
        else
        {
            currentActiveWeapon = primaryWeapon;
        }
    }

    private Weapon getWeapon(string name)
    {
        foreach(Weapon w in weaponList)
        {
            if (w.weaponName.ToLower().Contains(name.ToLower()))
            {
                return w;
            }
        }
        return null;
    }

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * currentActiveWeapon.bulletDrop;
        return bullet.initialPosition + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    private float nextTimeToFire = 0f;

    public string AmmoText()
    {
        return currentActiveWeapon.clipAmmo.ToString() + '|' + currentActiveWeapon.currentTotalAmmo.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        if (isReloading)
            return;

        if (currentActiveWeapon.currentTotalAmmo > 0 && (currentActiveWeapon.clipAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentActiveWeapon.clipAmmo != currentActiveWeapon.clipSize)))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentActiveWeapon.clipAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / currentActiveWeapon.fireRate;
            Shoot();
        }
        UpdateBullets(Time.deltaTime);

        weaponText.text = currentActiveWeapon.weaponName;
    }

    public GameObject getCurrentWeapon()
    {
        return currentActiveWeapon.weaponObject;
    }

    public void IncreaseAmmo(int extraAmmo)
    {
        currentActiveWeapon.currentTotalAmmo += extraAmmo;
    }

    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }
    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = end - start;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            if(hitInfo.transform.tag == "Enemy")
            {
                EnemyAI enemy = hitInfo.transform.GetComponent<EnemyAI>();

                if (enemy != null)
                {
                    enemy.TakeDamage(currentActiveWeapon.damage * damageMultiplier);
                }

            }
            
            if(bullet.tracer != null)
            {
                bullet.tracer.transform.position = hitInfo.point;
            }
                bullet.time = maxLifeTime;
            //GameObject impactGo = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            //Destroy(impactGo, 2f);
        }
        else
        {
            if (bullet.tracer != null)
            {
                bullet.tracer.transform.position = end;
            }
        }
    }
    void Shoot()
    {
        currentActiveWeapon.clipAmmo--;
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }
        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * currentActiveWeapon.bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);
        
        FindObjectOfType<AudioManager>().SFXPlay("PrimaryGunSound");

    }

    IEnumerator Reload()
    {
        isReloading = true;

        CreateMessage cm = FindObjectOfType<CreateMessage>();
        cm.createMessage("Reloading", reloadTime);

        int filledAmmo;

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = currentActiveWeapon.clipSize - currentActiveWeapon.clipAmmo;

        if (ammoNeeded > currentActiveWeapon.currentTotalAmmo)
        {
            filledAmmo = currentActiveWeapon.currentTotalAmmo;
        }
        else
        {
            filledAmmo = ammoNeeded;
        }
        currentActiveWeapon.clipAmmo += filledAmmo;
        currentActiveWeapon.currentTotalAmmo -= filledAmmo;

        isReloading = false;
    }
}
