using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGun : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }

    public int damage = 15;
    public float range = 100f;
    public float shootInterval = 0.425f;
    public float bulletSpeed = 40f;
    public float bulletDrop = 10f;

    public int clipSize = 15;
    public int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;

    public float impactForce = 30f;

    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;
    public Transform raycastOrigin;

    Ray ray;
    RaycastHit hitInfo;

    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3f;

    void Start()
    {
        currentAmmo = clipSize;
    }

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
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

    // Update is called once per frame
    void Update()
    {

        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        //if (currentAmmo > 0)
        //{
        //    //nextTimeToFire = Time.time + 1f / fireRate;
        //    Shoot();
        //}
        UpdateBullets(Time.deltaTime);
    }

    public void ShootPlayer(Transform destination)
    {
        currentAmmo--;
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }
        FindObjectOfType<AudioManager>().SFXPlay("KyleGunSound");
        Vector3 velocity = (destination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);
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

            Player p = hitInfo.transform.GetComponent<Player>();
            if(p != null)
            {
                p.gotHit(damage);
            }
            
            if (bullet.tracer != null)
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
        ////muzzleFlash[0].Play();
        

        ////Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        ////ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        //if (Physics.Raycast(hitPoint.position, hitPoint.forward, out hit, range))
        //{
        //    Debug.Log(hit.transform.name);

        //    Target target = hit.transform.GetComponent<Target>();
        //    if(target != null)
        //    {
        //        target.TakeDamage(damage);
        //    }

        //    GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        //    Destroy(impactGo, 2f);
        //}
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = clipSize;

        isReloading = false;
    }
}
